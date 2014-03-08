
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Threading;

namespace WorkSummarizerGUI.ViewModels
{
    public class LocationStatusWidgetViewModel : ViewModelBase
    {
        private readonly Dispatcher m_dispatcher;
        private bool m_isLocationLocked;
        private string m_location;

        public LocationStatusWidgetViewModel()
        {
            m_dispatcher = Dispatcher.CurrentDispatcher;
            Location = "off";
            
            ThreadPool.QueueUserWorkItem(state => UpdateGatewayMacBackground());
        }

        public bool IsLocationLocked 
        {
            get { return m_isLocationLocked; }
            private set 
            { 
                m_isLocationLocked = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get { return m_location; }
            private set 
            {
                m_location = value;
                OnPropertyChanged();
            }
        }

        // REVIEW dangerous background thread interaction with dispatching
        private void UpdateGatewayMacBackground()
        {
            m_dispatcher.Invoke(() => { IsLocationLocked = false; Location = "searching..."; });

            // completely for special effects
            Thread.Sleep(TimeSpan.FromSeconds(5)); 

            try
            {
                foreach (
                    var gatewayIp4Address in
                        NetworkInterface.GetAllNetworkInterfaces()
                            .SelectMany(ni => ni.GetIPProperties().GatewayAddresses
                                .Select(ga => ga.Address.ToString())))
                {

                    try
                    {
                        using (var p = Process.Start(new ProcessStartInfo("arp", "-a " + gatewayIp4Address)
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        }))
                        {
                            var output = p.StandardOutput.ReadToEnd();

                            foreach (var outputLine in output.Split(new char[] { '\r', '\n' }))
                            {
                                var outputColumns =
                                    outputLine.Split(new char[] { ' ', '\t' })
                                        .Where(q => !string.IsNullOrWhiteSpace(q))
                                        .ToList();

                                if (outputColumns.Count != 3)
                                {
                                    continue;
                                }

                                m_dispatcher.Invoke(() => { IsLocationLocked = true; Location = "Gateway " + outputColumns[1].Replace('-', ':').ToUpperInvariant() + " (" + gatewayIp4Address + ")"; });
                                return;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // if this interface fails then oh wells.
                    }
                }
            }
            catch (Exception)
            {
                // if all interfaces fail then oh wells.
                m_dispatcher.Invoke(() => { IsLocationLocked = false; Location = "unknown"; });
            }
        }
    }
}
