using System;
using System.Windows;

namespace WorkSummarizerGUI.Controls
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Interaction logic for FriendlyDateRangePicker.xaml
    /// </summary>
    public partial class FriendlyDateRangePicker : INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedStartDateProperty = DependencyProperty.Register(
            "SelectedStartDate", 
            typeof(DateTime), 
            typeof(FriendlyDateRangePicker),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged, null));

        public static readonly DependencyProperty SelectedEndDateProperty = DependencyProperty.Register(
            "SelectedEndDate", 
            typeof(DateTime),
            typeof(FriendlyDateRangePicker),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged, null));

        private IEnumerable<DateSegmentViewModel> m_days;
        private IEnumerable<DateSegmentViewModel> m_months;
        private IEnumerable<DateSegmentViewModel> m_years;

        private string m_reportingDuration;

        public FriendlyDateRangePicker()
        {
            var currentDate = DateTime.Now;
            Years = DateSegmentViewModel.GetYears(currentDate.Year - 3);
            Months = DateSegmentViewModel.GetMonths(currentDate.Year);
            Days = DateSegmentViewModel.GetDays(currentDate.Year, currentDate.Month);

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

        public IEnumerable<DateSegmentViewModel> Days
        {
            get
            {
                return m_days;
            }
            private set
            {
                m_days = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<DateSegmentViewModel> Months 
        {
            get
            {
                return m_months;
            }
            private set
            {
                m_months = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<DateSegmentViewModel> Years
        {
            get
            {
                return m_years;
            }
            private set
            {
                m_years = value;
                OnPropertyChanged();
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
            var picker = d as FriendlyDateRangePicker;
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


        public class DayViewModel
        {
        }

        public class DateSegmentViewModel
        {
            private DateSegmentViewModel(int value, string description)
            {
                Value = value;
                Description = description;
            }

            public static IEnumerable<DateSegmentViewModel> GetDays(int year, int month)
            {
                var days = new List<DateSegmentViewModel>();
                for (var i = 1; i <= DateTime.DaysInMonth(year, month); i++)
                {
                    var date = new DateTime(year, month, i);
                    days.Add(new DateSegmentViewModel(i, DateTimeFormatInfo.CurrentInfo.GetDayName(date.DayOfWeek)));
                }

                return days;
            }

            public static IEnumerable<DateSegmentViewModel> GetMonths(int year)
            {
                var months = new List<DateSegmentViewModel>();
                for (var i = 1; i < DateTimeFormatInfo.CurrentInfo.MonthNames.Count(); i++)
                {
                    months.Add(new DateSegmentViewModel(i, DateTimeFormatInfo.CurrentInfo.MonthNames[i - 1]));
                }

                return months;
            }

            public static IEnumerable<DateSegmentViewModel> GetYears(int firstYear)
            {
                var years = new List<DateSegmentViewModel>();
                for (var i = firstYear; i <= DateTime.Now.Year; i++)
                {
                    years.Add(new DateSegmentViewModel(i, DateTime.IsLeapYear(i) ? "Leap year" : string.Empty));
                }

                return years;
            }

            public int Value { get; private set; }

            public string Description { get; private set; }
        }
    }
}
