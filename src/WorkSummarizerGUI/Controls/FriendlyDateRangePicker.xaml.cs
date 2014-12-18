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

        private IEnumerable<DateSegmentViewModel> m_years;
        private IEnumerable<DateSegmentViewModel> m_days;
        private IEnumerable<DateSegmentViewModel> m_months;
        private DateSegmentViewModel m_selectedMonth;
        private DateSegmentViewModel m_selectedDay;
        private DateSegmentViewModel m_selectedYear;

        private string m_reportingDuration;

        public FriendlyDateRangePicker()
        {
            var currentDate = DateTime.Now;
            
            Years = DateSegmentViewModel.GetYears(currentDate.Year - 3);
            SelectedYear = Years.First(p => p.Value == currentDate.Year);
            SelectedMonth = Months.First(p => p.Value == currentDate.Month);
            SelectedDay = Days.First(p => p.Value == currentDate.Day);
            MonthDuration = 1;

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
                var selectedDay = SelectedDay != null ? SelectedDay.Value : value.First().Value;
                OnPropertyChanged();
                SelectedDay = value.FirstOrDefault(p => p.Value == selectedDay) ?? value.First();
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
                var selectedMonth = SelectedMonth != null ? SelectedMonth.Value : value.First().Value;
                OnPropertyChanged();
                SelectedMonth = value.FirstOrDefault(p => p.Value == selectedMonth) ?? value.First();
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

        public DateSegmentViewModel SelectedMonth
        {
            get
            {
                return m_selectedMonth;
            }
            set
            {
                m_selectedMonth = value ?? Months.First();
                Days = DateSegmentViewModel.GetDays(SelectedYear.Value, m_selectedMonth.Value);
                this.OnPropertyChanged();
                SelectedEndDate = new DateTime(SelectedYear.Value, SelectedMonth.Value, SelectedDay.Value);
            }
        }

        public DateSegmentViewModel SelectedDay
        {
            get
            {
                return m_selectedDay;
            }
            set
            {
                m_selectedDay = value ?? Days.First();
                this.OnPropertyChanged();
                SelectedEndDate = new DateTime(SelectedYear.Value, SelectedMonth.Value, SelectedDay.Value);
            }
        }

        public DateSegmentViewModel SelectedYear
        {
            get
            {
                return m_selectedYear;
            }
            set
            {
                m_selectedYear = value ?? Years.First();
                Months = DateSegmentViewModel.GetMonths(m_selectedYear.Value);
                this.OnPropertyChanged();
                SelectedEndDate = new DateTime(SelectedYear.Value, SelectedMonth.Value, SelectedDay.Value);
            }
        }

        private int m_monthDuration;

        public int MonthDuration
        {
            get
            {
                return m_monthDuration;
            }
            set
            {
                m_monthDuration = value;
                this.OnPropertyChanged();

                SelectedStartDate =
                    SelectedEndDate.AddYears(-YearDuration).AddMonths(-MonthDuration).AddDays(-DayDuration);
            }
        }

        private int m_dayDuration;

        public int DayDuration
        {
            get
            {
                return m_dayDuration;
            }
            set
            {
                m_dayDuration = value;
                this.OnPropertyChanged();
                SelectedStartDate =
                    SelectedEndDate.AddYears(-YearDuration).AddMonths(-MonthDuration).AddDays(-DayDuration);
            }
        }

        private int m_yearDuration;

        public int YearDuration
        {
            get
            {
                return m_yearDuration;
            }
            set
            {
                m_yearDuration = value;
                this.OnPropertyChanged();
                SelectedStartDate =
                    SelectedEndDate.AddYears(-YearDuration).AddMonths(-MonthDuration).AddDays(-DayDuration);
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
            picker.SelectedStartDate =
                    picker.SelectedEndDate.AddYears(-picker.YearDuration).AddMonths(-picker.MonthDuration).AddDays(-picker.DayDuration);
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
