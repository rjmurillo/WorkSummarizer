using System.Globalization;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using WorkSummarizerGUI.Commands;
using WorkSummarizerGUI.Services;

namespace WorkSummarizerGUI.ViewModels
{
    public class ThemeSwitchWidgetViewModel : ViewModelBase
    {
        private readonly IThemeSwitchService m_themeSwitchService;
        private string m_accentText;
        private string m_backgroundText;

        public ThemeSwitchWidgetViewModel()
        {
            m_themeSwitchService = SimpleIoc.Default.GetInstance<IThemeSwitchService>();
            AccentText = string.Format(CultureInfo.CurrentCulture, "Theme accent: {0}", m_themeSwitchService.AccentName);
            BackgroundText = string.Format(CultureInfo.CurrentCulture, "Theme background: {0}", m_themeSwitchService.BackgroundName);

            CycleAccentCommand = new RelayCommand(() => 
            {
                m_themeSwitchService.CycleAccent();
                AccentText = string.Format(CultureInfo.CurrentCulture, "Theme accent: {0}", m_themeSwitchService.AccentName);
            });

            CycleBackgroundCommand = new RelayCommand(() => 
            {
                m_themeSwitchService.CycleBackground();
                BackgroundText = string.Format(CultureInfo.CurrentCulture, "Theme background: {0}", m_themeSwitchService.BackgroundName);
            });
        }

        public string AccentText 
        { 
            get { return m_accentText; } 
            private set { m_accentText = value; OnPropertyChanged(); } 
        }

        public string BackgroundText
        {
            get { return m_backgroundText; }
            private set { m_backgroundText = value; OnPropertyChanged(); }
        }

        public ICommand CycleAccentCommand { get; private set; }

        public ICommand CycleBackgroundCommand { get; private set; }
    }
}
