using GalaSoft.MvvmLight.Messaging;
using WorkSummarizerGUI.Models;

namespace WorkSummarizerGUI.ViewModels
{
    public class ToastNotificationWidgetViewModel : ViewModelBase
    {
        private bool m_isOpen;
        private string m_message;
        
        public ToastNotificationWidgetViewModel()
        {
            Messenger.Default.Register<Notification>(this, OnNotificationReceived);
        }

        public bool IsOpen
        {
            get { return m_isOpen; }
            private set { m_isOpen = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get { return m_message; }
            private set { m_message = value; OnPropertyChanged(); }
        }

        private void OnNotificationReceived(Notification notification)
        {
            Message = notification.Message;
            IsOpen = notification.Progress < 100;
        }
    }
}
