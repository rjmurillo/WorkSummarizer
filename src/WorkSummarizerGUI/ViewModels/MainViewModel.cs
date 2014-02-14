﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Events;
using Events.CodeFlow;
using Events.Connect;
using Events.Kudos;
using Events.ManicTime;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Extensibility;
using Processing.Text;
using Renders;
using Renders.Console;
using Renders.Excel;
using Renders.HTML;
using WorkSummarizer;

namespace WorkSummarizerGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEnumerable<ServiceViewModel> m_eventSources;
        private readonly IPluginRuntime m_pluginRuntime;
        private readonly IEnumerable<ServiceViewModel> m_reportingSinks;

        private DateTime m_endLocalTime;
        private bool m_isBusy;
        private bool m_isGeneratePerSourceEnabled;
        private bool m_isGenerateSummaryEnabled;
        private DateTime m_startLocalTime;
        private int m_progressPercentage;
        private string m_reportingDuration;

        public MainViewModel()
        {
            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(new[]
            {
                typeof(CodeFlowPlugin),
                typeof(ConnectPlugin),
                typeof(KudosPlugin),
                typeof(ManicTimePlugin),
                typeof(OutlookPlugin),
                typeof(TeamFoundationServerPlugin),
                typeof(YammerPlugin),

                typeof(ExcelRenderPlugin),
                typeof(HtmlRenderPlugin),
            });

            m_pluginRuntime = pluginRuntime;

            m_eventSources =
                pluginRuntime.EventQueryServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p => new ServiceViewModel(p.Key, p.Select(q => q.Key.Id).ToList()))
                             .ToList();

            EndLocalTime = DateTime.Now;
            
            m_reportingSinks = pluginRuntime.RenderEventServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p => new ServiceViewModel(p.Key, p.Select(q => q.Key.Id).ToList()))
                             .ToList();

            m_reportingSinks.Last().IsSelected = true;

            StartLocalTime = DateTime.Now.AddMonths(-1);
            m_isGenerateSummaryEnabled = true;
        }
        
        public DateTime EndLocalTime
        {
            get { return m_endLocalTime; }
            set 
            {
                m_endLocalTime = value;
                UpdateReportingDuration();
                OnPropertyChanged();
            }
        }
        
        public IEnumerable<ServiceViewModel> EventSources
        {
            get { return m_eventSources; }
        }
        
        public bool IsBusy
        {
            get { return m_isBusy; }
            private set
            {
                m_isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool IsGeneratePerSourceEnabled
        {
            get { return m_isGeneratePerSourceEnabled; }
            private set
            {
                m_isGeneratePerSourceEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsGenerateSummaryEnabled
        {
            get { return m_isGenerateSummaryEnabled; }
            private set
            {
                m_isGenerateSummaryEnabled = value;
                OnPropertyChanged();
            }
        }

        public int ProgressPercentage
        {
            get { return m_progressPercentage; }
            private set 
            { 
                m_progressPercentage = value;
                OnPropertyChanged();
            }
        }

        public string ReportingDuration
        {
            get { return m_reportingDuration; }
            private set 
            { 
                m_reportingDuration = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ServiceViewModel> ReportingSinks
        {
            get { return m_reportingSinks; }
        }

        public DateTime StartLocalTime
        {
            get { return m_startLocalTime; }
            set 
            {
                m_startLocalTime = value;
                UpdateReportingDuration();
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get { return "v" + Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public async Task GenerateAsync()
        {
            IsBusy = true;
            ProgressPercentage = 0;

            var selectedEventSourceIds = EventSources.Where(p => p.IsSelected)
                                                     .SelectMany(p => p.ServiceIds)
                                                     .ToList();

            var selectedReportingSinkTypes = ReportingSinks.Where(p => p.IsSelected)
                                                           .SelectMany(p => p.ServiceIds)
                                                           .ToList();

            var selectedStartLocalTime = m_startLocalTime;
            var selectedEndLocalTime = m_endLocalTime;

            var selectedIsGeneratePerSourceEnabled = m_isGeneratePerSourceEnabled;
            var selectedIsGeneratePerSummaryEnabled = m_isGenerateSummaryEnabled;

            if (selectedEventSourceIds.Any() && selectedReportingSinkTypes.Any() && (selectedIsGeneratePerSourceEnabled || selectedIsGeneratePerSummaryEnabled))
            {
                var uiDispatcher = Dispatcher.CurrentDispatcher;
                await Task.Run(() =>
                {
                    try
                    {
                        var eventQueryServiceRegistrations =
                            m_pluginRuntime
                                .EventQueryServices
                                .Where(p => selectedEventSourceIds.Contains(p.Key.Id));

                        var renderServiceRegistrations =
                            m_pluginRuntime
                                .RenderEventServices
                                .Where(p => selectedReportingSinkTypes.Contains(p.Key.Id));

                        var summaryEvents = new List<Event>();
                        var summaryWeightedTags = new ConcurrentDictionary<string, int>();
                        var summaryWeightedPeople = new ConcurrentDictionary<string, int>();
                        var summaryImportantSentences = new List<string>();

                        var totalProgressSteps = eventQueryServiceRegistrations.Count();

                        if (selectedIsGeneratePerSourceEnabled)
                        {
                            totalProgressSteps += eventQueryServiceRegistrations.Count();
                        }

                        if (selectedIsGeneratePerSummaryEnabled)
                        {
                            totalProgressSteps += 1;
                        }

                        var progressIncrement = 100 / Math.Max(totalProgressSteps, 1);
                        foreach (var eventQueryServiceRegistration in eventQueryServiceRegistrations)
                        {
                            IEnumerable<Event> evts = Enumerable.Empty<Event>();
                            KeyValuePair<ServiceRegistration, IEventQueryService> registration1 = eventQueryServiceRegistration;
                            Action pullEventsDelegate = () =>
                            {
                                evts = registration1.Value.PullEvents(selectedStartLocalTime, selectedEndLocalTime);
                            };

                            if (eventQueryServiceRegistration.Key.InvokeOnShellDispatcher)
                            {
                                uiDispatcher.Invoke(pullEventsDelegate);
                            }
                            else
                            {
                                pullEventsDelegate();
                            }
                            
                            uiDispatcher.Invoke(() => { ProgressPercentage += progressIncrement; });

                            var textProc = new TextProcessor();
                            var peopleProc = new PeopleProcessor();

                            var sb = new StringBuilder();

                            foreach (var evt in evts)
                            {
                                sb.Append(String.Format(" {0} {1} ", evt.Subject.Text.Replace("\n", String.Empty).Replace("\r", String.Empty), evt.Text.Replace("\n", String.Empty).Replace("\r", String.Empty)));
                            }

                            IDictionary<string, int> weightedTags = textProc.GetNouns(sb.ToString());
                            IEnumerable<string> importantSentences = textProc.GetImportantSentences(sb.ToString());
                            IDictionary<string, int> weightedPeople = peopleProc.GetTeam(evts);

                            if (selectedIsGeneratePerSourceEnabled)
                            {
                                foreach (var render in renderServiceRegistrations)
                                {
                                    KeyValuePair<ServiceRegistration, IEventQueryService> registration = eventQueryServiceRegistration;
                                    KeyValuePair<ServiceRegistration, IRenderEvents> render1 = render;
                                    Action renderEventsDelegate = () => render1.Value.Render(registration.Key.Id, evts, weightedTags, weightedPeople, importantSentences);
                                    if (render.Key.InvokeOnShellDispatcher)
                                    {
                                        uiDispatcher.Invoke(renderEventsDelegate);
                                    }
                                    else
                                    {
                                        renderEventsDelegate();
                                    }
                                }
                            }

                            summaryEvents.AddRange(evts);
                            foreach (var weightedTagEntry in weightedTags)
                            {
                                summaryWeightedTags.AddOrUpdate(weightedTagEntry.Key, weightedTagEntry.Value,
                                    (s, i) => i + weightedTagEntry.Value);
                            }

                            foreach (var weightedPersonEntry in weightedPeople)
                            {
                                summaryWeightedPeople.AddOrUpdate(weightedPersonEntry.Key, weightedPersonEntry.Value,
                                    (s, i) => i + weightedPersonEntry.Value);
                            }

                            summaryImportantSentences.AddRange(importantSentences);

                            uiDispatcher.Invoke(() => { ProgressPercentage += progressIncrement; });
                        }

                        if (selectedIsGeneratePerSummaryEnabled)
                        {
                            foreach (var render in renderServiceRegistrations)
                            {
                                KeyValuePair<ServiceRegistration, IRenderEvents> render1 = render;
                                Action renderEventsDelegate = () => render1.Value.Render("Summary", summaryEvents, summaryWeightedTags, summaryWeightedPeople, summaryImportantSentences);
                                if (render.Key.InvokeOnShellDispatcher)
                                {
                                    uiDispatcher.Invoke(renderEventsDelegate);
                                }
                                else
                                {
                                    renderEventsDelegate();
                                }
                            }
                        }

                        uiDispatcher.Invoke(() => { ProgressPercentage = 100; });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Oh noes!" + Environment.NewLine + Environment.NewLine + ex);
                    }
                });
            }

            IsBusy = false;
        }
        
        private void UpdateReportingDuration()
        {
            var duration = m_endLocalTime - m_startLocalTime;
            var upperWeeks = (int)(Math.Ceiling(duration.TotalDays * 5/7) / 7);
            ReportingDuration = String.Format("About {0} work weeks", upperWeeks);
        }
    }
}
