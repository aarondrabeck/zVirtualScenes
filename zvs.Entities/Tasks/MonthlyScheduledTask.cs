using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zvs.DataModel.Tasks
{
    public class MonthlyScheduledTask : ScheduledTask
    {
        private int _repeatIntervalInMonths;
        public int RepeatIntervalInMonths
        {
            get
            {
                return _repeatIntervalInMonths;
            }
            set
            {
                if (value == _repeatIntervalInMonths) return;
                _repeatIntervalInMonths = value;
                NotifyPropertyChanged();
            }
        }

        private DaysOfMonth _daysOfMonthToActivate;
        public DaysOfMonth DaysOfMonthToActivate
        {
            get
            {
                return _daysOfMonthToActivate;
            }
            set
            {
                if (value == _daysOfMonthToActivate) return;
                _daysOfMonthToActivate = value;
                NotifyPropertyChanged();
            }
        }
    }
}
