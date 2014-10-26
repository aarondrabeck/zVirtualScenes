using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel.Tasks
{
    [Table("ScheduledTasks", Schema = "ZVS")]
    public abstract class ScheduledTask : INotifyPropertyChanged, IIdentity
    {
        public int Id { get; set; }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (value == _startTime) return;
                _startTime = value;
                NotifyPropertyChanged();
            }
        }

        public virtual CommandScheduledTask CommandScheduledTask { get; set; }

        [NotMapped]
        public DateTimeOffset StartTimeOffset
        {
            // Assume the CreateOn property stores UTC time.
            get { return new DateTimeOffset(StartTime, TimeSpan.FromHours(0)); }
            set { StartTime = value.UtcDateTime; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
