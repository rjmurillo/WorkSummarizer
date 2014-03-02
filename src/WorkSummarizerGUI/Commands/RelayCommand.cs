using System;
using System.Windows.Input;

namespace WorkSummarizerGUI.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action m_handler;
        private bool m_isEnabled;

        public RelayCommand(Action handler)
        {
            m_handler = handler;
            m_isEnabled = true;
        }

        public event EventHandler CanExecuteChanged;
        
        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                if (value != m_isEnabled)
                {
                    m_isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }
                
        public void Execute(object parameter)
        {
            m_handler();
        }
    }
}
