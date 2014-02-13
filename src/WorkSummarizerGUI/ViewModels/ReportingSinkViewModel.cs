
namespace WorkSummarizerGUI.ViewModels
{
    public class ReportingSinkViewModel : ViewModelBase
    {
        private readonly string m_name;

        public ReportingSinkViewModel()
        {
            m_name = "Edkotows";
        }

        public string Name
        {
            get { return m_name; }
        }
    }
}
