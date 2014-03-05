using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using DataSources.Who;
using Events;
using Extensibility;
using FUSE.Weld.Base;
using GalaSoft.MvvmLight.Messaging;
using Processing.Text;
using Renders;
using WorkSummarizer;
using WorkSummarizerGUI.Commands;
using WorkSummarizerGUI.Models;

namespace WorkSummarizerGUI.ViewModels
{
    public class CreateReportViewModel : ViewModelBase
    {
        private readonly IEnumerable<ServiceViewModel> m_eventSources;
        private readonly IEnumerable<ServiceViewModel> m_reportingSinks;
        private readonly IMessenger m_messenger = Messenger.Default;

        private readonly IDictionary<ServiceRegistration, IEventQueryService> m_eventQueryServices;
        private readonly IDictionary<ServiceRegistration, IRenderEvents> m_renderServices;

        private DateTime m_endLocalTime;
        private bool m_isBusy;
        private bool m_isGeneratePerSourceEnabled;
        private bool m_isGenerateSummaryEnabled;
        private DateTime m_startLocalTime;
        private int m_progressPercentage;
        private string m_progressStatus;
        private string m_reportingDuration;

        public CreateReportViewModel(
            IDictionary<ServiceRegistration, IEventQueryService> eventQueryServices, 
            IDictionary<ServiceRegistration, IRenderEvents> renderServices)
        {
            m_eventQueryServices = eventQueryServices;
            m_renderServices = renderServices;

            m_eventSources =
                eventQueryServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p =>
                                 {
                                     return new ServiceViewModel(
                                         p.Key,
                                         p.Select(q => q.Key.Id).ToList(),
                                         new RelayCommand(() => { m_messenger.Send(new ServiceConfigurationRequest { Name = p.Key, Ids = p.Select(q => q.Key.Id).ToList() }); }) { IsEnabled = p.Any(q => q.Key.IsConfigurable) })
                                        {
                                            HelpText = String.Join(", ", p.Select(pair => pair.Key.Name))
                                        };
                                 })
                             .ToList();

            EndLocalTime = DateTime.Now;

            m_reportingSinks = renderServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p => 
                                 {
                                     return new ServiceViewModel(
                                         p.Key,
                                         p.Select(q => q.Key.Id).ToList(),
                                         new RelayCommand(() => { m_messenger.Send(new ServiceConfigurationRequest { Name = p.Key, Ids = p.Select(q => q.Key.Id).ToList() }); }) { IsEnabled = p.Any(q => q.Key.IsConfigurable) })
                                         {
                                             HelpText = String.Join(", ", p.Select(pair => pair.Key.Name))
                                         }; 
                                 })
                             .ToList();

            m_reportingSinks.Last().IsSelected = true;

            StartLocalTime = DateTime.Now.AddMonths(-1);
            m_isGenerateSummaryEnabled = true;
            GenerateReportCommand = new RelayCommand(() => GenerateAsync());
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

        public ICommand GenerateReportCommand
        {
            get; private set;
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

        public string ProgressStatus
        {
            get { return m_progressStatus; }
            private set
            {
                m_progressStatus = value;
                Messenger.Default.Send<Notification>(new Notification(ProgressPercentage, ProgressStatus));
                OnPropertyChanged();
            }
        }

        public int ProgressPercentage
        {
            get { return m_progressPercentage; }
            private set 
            { 
                m_progressPercentage = value;
                Messenger.Default.Send<Notification>(new Notification(ProgressPercentage, ProgressStatus));
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

            ProgressPercentage = 0;
            if (selectedEventSourceIds.Any() && selectedReportingSinkTypes.Any() && (selectedIsGeneratePerSourceEnabled || selectedIsGeneratePerSummaryEnabled))
            {
                IsBusy = true;
                ProgressPercentage = 1;
                ProgressStatus = String.Empty;

                var uiDispatcher = Dispatcher.CurrentDispatcher;
                await Task.Run(() =>
                {
                    try
                    {
                        var eventQueryServiceRegistrations =
                            m_eventQueryServices
                                .Where(p => selectedEventSourceIds.Contains(p.Key.Id));

                        var renderServiceRegistrations =
                            m_renderServices
                                .Where(p => selectedReportingSinkTypes.Contains(p.Key.Id));

                        var summaryEvents = new List<Event>();
                        var summaryWeightedTags = new ConcurrentDictionary<string, int>();
                        var summaryWeightedPeople = new ConcurrentDictionary<string, int>();
                        var summaryImportantSentences = new List<string>();

                        var eventQueryServiceRegistrationsCount = eventQueryServiceRegistrations.Count();
                        var totalProgressSteps = eventQueryServiceRegistrationsCount;

                        if (selectedIsGeneratePerSourceEnabled)
                        {
                            totalProgressSteps += eventQueryServiceRegistrationsCount;
                        }

                        if (selectedIsGeneratePerSummaryEnabled)
                        {
                            totalProgressSteps += eventQueryServiceRegistrationsCount; // uhh nice padding for summary generation
                        }

                        var progressIncrement = 100 / Math.Max(totalProgressSteps, 1);
                        foreach (var eventQueryServiceRegistration in eventQueryServiceRegistrations)
                        {
                            KeyValuePair<ServiceRegistration, IEventQueryService> registration1 = eventQueryServiceRegistration;
                            uiDispatcher.Invoke(() => { ProgressStatus = String.Format("Pulling data for {0} - {1}...", registration1.Key.Family, registration1.Key.Name); });

                            IEnumerable<Event> evts = Enumerable.Empty<Event>();
                            Action pullEventsDelegate = () =>
                            {
                                evts = registration1.Value.PullEvents(selectedStartLocalTime, (selectedEndLocalTime.AddDays(1).AddTicks(-1)), Environment.UserName);
                            };

                            if (eventQueryServiceRegistration.Key.InvokeOnShellDispatcher)
                            {
                                uiDispatcher.Invoke(pullEventsDelegate);
                            }
                            else
                            {
                                pullEventsDelegate();
                            }

                            uiDispatcher.Invoke(() => { ProgressPercentage += progressIncrement; ProgressStatus = String.Format("Summarizing data for {0} - {1}...", registration1.Key.Family, registration1.Key.Name); });

                            var textProc = new TextProcessor();
                            var peopleProc = new PeopleProcessor();

                            var sb = new StringBuilder();

                            foreach (var evt in evts)
                            {
                                sb.Append(evt.Text.Replace("\n", String.Empty).Replace("\r", String.Empty));
                            }


                            IDictionary<string, int> weightedPeople = peopleProc.GetTeam(evts);

                            var customStopList = IdentityUtility.GetIdentityAttributes();

                            var splitCustomStopList = new List<string>();

                            foreach (var token in customStopList)
                            {
                                splitCustomStopList.AddRange(token.Split(' '));
                            }

                            textProc.AddStopWords(splitCustomStopList);

                            var nouns = textProc.GetNouns(sb.ToString());
                            IDictionary<string, int> weightedTags = textProc.GetNouns(sb.ToString());
                            IEnumerable<string> importantSentences = textProc.GetImportantEvents(evts.Select(x => x.Text), nouns);

                            if (selectedIsGeneratePerSourceEnabled)
                            {
                                foreach (var render in renderServiceRegistrations)
                                {
                                    KeyValuePair<ServiceRegistration, IRenderEvents> render1 = render;
                                    uiDispatcher.Invoke(() => { ProgressStatus = String.Format("Rendering data for {0} - {1} with {2} - {3}...", registration1.Key.Family, registration1.Key.Name, render1.Key.Family, render1.Key.Name); });

                                    KeyValuePair<ServiceRegistration, IEventQueryService> registration = eventQueryServiceRegistration;
                                    Action renderEventsDelegate = () => render1.Value.Render(registration.Key.Id, selectedStartLocalTime, (selectedEndLocalTime.AddDays(1).AddTicks(-1)), evts, weightedTags, weightedPeople, importantSentences);
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
                                Action renderEventsDelegate = () => render1.Value.Render("Summary", selectedStartLocalTime, (selectedEndLocalTime.AddDays(1).AddTicks(-1)), summaryEvents, summaryWeightedTags, summaryWeightedPeople, summaryImportantSentences);
                                uiDispatcher.Invoke(() => { ProgressStatus = String.Format("Rendering summary data with {0} - {1}...", render1.Key.Family, render1.Key.Name); });

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
                    catch (AggregateException ex)
                    {
                        Trace.WriteLine("Aggregate inner exception: " + ex.InnerException);
                        m_messenger.Send(ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        m_messenger.Send(ex);
                    }
                });

                IsBusy = false;
                ProgressStatus = "...done.";
                ProgressPercentage = 100;
            }
        }
        
        private void UpdateReportingDuration()
        {
            var duration = m_endLocalTime - m_startLocalTime;
            var upperWeeks = (int)Math.Ceiling(Math.Ceiling(duration.TotalDays * 5/7) / 5);
            ReportingDuration = String.Format("About {0} work weeks", upperWeeks);
        }
    }
}
