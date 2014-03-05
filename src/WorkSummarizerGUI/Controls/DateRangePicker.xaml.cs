using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WorkSummarizerGUI.Controls
{
    /// <summary>
    /// Interaction logic for DateRangePicker.xaml
    /// </summary>
    public partial class DateRangePicker : INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedStartDateProperty = DependencyProperty.Register(
            "SelectedStartDate", 
            typeof(DateTime), 
            typeof(DateRangePicker),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedDateChanged), null));

        public static readonly DependencyProperty SelectedEndDateProperty = DependencyProperty.Register(
            "SelectedEndDate", 
            typeof(DateTime),
            typeof(DateRangePicker),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedDateChanged), null));

        private string m_reportingDuration;

        public DateRangePicker()
        {
            InitializeComponent();
        }        

        public event PropertyChangedEventHandler PropertyChanged;

        public string ReportingDuration
        {
            get { return m_reportingDuration; }
            private set
            {
                m_reportingDuration = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndAbsoluteTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime SelectedStartDate
        {
            get { return (DateTime)GetValue(SelectedStartDateProperty); }
            set { SetValue(SelectedStartDateProperty, value); UpdateReportingDuration(); }
        }

        public DateTime SelectedEndDate
        {
            get { return (DateTime)GetValue(SelectedEndDateProperty); }
            set { SetValue(SelectedEndDateProperty, value); UpdateReportingDuration(); }
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker picker = d as DateRangePicker;
            picker.UpdateReportingDuration();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateReportingDuration()
        {
            var duration = SelectedEndDate - SelectedStartDate;
            var upperWeeks = (int)Math.Ceiling(Math.Ceiling(duration.TotalDays * 5 / 7) / 5);
            ReportingDuration = String.Format("About {0} work weeks", upperWeeks);
        }
    }
}
