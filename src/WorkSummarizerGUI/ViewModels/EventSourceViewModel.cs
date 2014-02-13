
namespace WorkSummarizerGUI.ViewModels
{
    public class EventSourceViewModel : ViewModelBase
    {
        private readonly string m_name;

        public EventSourceViewModel()
        {
            m_name = "Jocarrie";
        }

        public string Name
        {
            get { return m_name; }
        }
    }
}
