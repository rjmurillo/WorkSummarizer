using System;

namespace WorkSummarizerGUI.ViewModels
{
    public class UserWidgetViewModel : ViewModelBase
    {
        public string Name
        {
            get
            {
                return Environment.UserName;
            }
        }
    }
}
