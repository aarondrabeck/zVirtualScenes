using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    [Table("ScheduledTasks", Schema = "ZVS")]
    public class ScheduledTask : INotifyPropertyChanged, IIdentity
    {
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //No actual navigational property here
        private StoredCommand _storedCommand;
        public virtual StoredCommand StoredCommand
        {
            get
            {
                return _storedCommand;
            }
            set
            {
                if (value != _storedCommand)
                {
                    _storedCommand = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        private TaskFrequency _frequency;
        public TaskFrequency Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (value != _frequency)
                {
                    _frequency = value;
                    NotifyPropertyChanged();                   
                }
            }
        }
       
        private string _name;
        [StringLength(255)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isEnabled;
        public bool isEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (value != _startTime)
                {
                    _startTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [NotMapped]
        public DateTimeOffset? EdmStartTime
        {
            // Assume the CreateOn property stores UTC time.
            get
            {
                return StartTime.HasValue ? new DateTimeOffset(StartTime.Value, TimeSpan.FromHours(0)) : (DateTimeOffset?)null;
            }
            set
            {
                StartTime = value.HasValue ? value.Value.UtcDateTime : (DateTime?)null;
            }
        }

        private int? _sortOrder;
        public int? SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                if (value != _sortOrder)
                {
                    _sortOrder = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private bool? _recurMonday;
        public bool? RecurMonday
        {
            get
            {
                return _recurMonday;
            }
            set
            {
                if (value != _recurMonday)
                {
                    _recurMonday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _recurTuesday;
        public bool? RecurTuesday
        {
            get
            {
                return _recurTuesday;
            }
            set
            {
                if (value != _recurTuesday)
                {
                    _recurTuesday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurWednesday;
        public bool? RecurWednesday
        {
            get
            {
                return _RecurWednesday;
            }
            set
            {
                if (value != _RecurWednesday)
                {
                    _RecurWednesday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurThursday;
        public bool? RecurThursday
        {
            get
            {
                return _RecurThursday;
            }
            set
            {
                if (value != _RecurThursday)
                {
                    _RecurThursday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurFriday;
        public bool? RecurFriday
        {
            get
            {
                return _RecurFriday;
            }
            set
            {
                if (value != _RecurFriday)
                {
                    _RecurFriday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurSaturday;
        public bool? RecurSaturday
        {
            get
            {
                return _RecurSaturday;
            }
            set
            {
                if (value != _RecurSaturday)
                {
                    _RecurSaturday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurSunday;
        public bool? RecurSunday
        {
            get
            {
                return _RecurSunday;
            }
            set
            {
                if (value != _RecurSunday)
                {
                    _RecurSunday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _RecurDays;
        public int? RecurDays
        {
            get
            {
                return _RecurDays;
            }
            set
            {
                if (value != _RecurDays)
                {
                    _RecurDays = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _RecurWeeks;
        public int? RecurWeeks
        {
            get
            {
                return _RecurWeeks;
            }
            set
            {
                if (value != _RecurWeeks)
                {
                    _RecurWeeks = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _RecurMonth;
        public int? RecurMonth
        {
            get
            {
                return _RecurMonth;
            }
            set
            {
                if (value != _RecurMonth)
                {
                    _RecurMonth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _RecurDayofMonth;
        public int? RecurDayofMonth
        {
            get
            {
                return _RecurDayofMonth;
            }
            set
            {
                if (value != _RecurDayofMonth)
                {
                    _RecurDayofMonth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _RecurSeconds;
        public int? RecurSeconds
        {
            get
            {
                return _RecurSeconds;
            }
            set
            {
                if (value != _RecurSeconds)
                {
                    _RecurSeconds = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurEven;
        public bool? RecurEven
        {
            get
            {
                return _RecurEven;
            }
            set
            {
                if (value != _RecurEven)
                {
                    _RecurEven = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay01;
        public bool? RecurDay01
        {
            get
            {
                return _RecurDay01;
            }
            set
            {
                if (value != _RecurDay01)
                {
                    _RecurDay01 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay02;
        public bool? RecurDay02
        {
            get
            {
                return _RecurDay02;
            }
            set
            {
                if (value != _RecurDay02)
                {
                    _RecurDay02 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay03;
        public bool? RecurDay03
        {
            get
            {
                return _RecurDay03;
            }
            set
            {
                if (value != _RecurDay03)
                {
                    _RecurDay03 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay04;
        public bool? RecurDay04
        {
            get
            {
                return _RecurDay04;
            }
            set
            {
                if (value != _RecurDay04)
                {
                    _RecurDay04 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay05;
        public bool? RecurDay05
        {
            get
            {
                return _RecurDay05;
            }
            set
            {
                if (value != _RecurDay05)
                {
                    _RecurDay05 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay06;
        public bool? RecurDay06
        {
            get
            {
                return _RecurDay06;
            }
            set
            {
                if (value != _RecurDay06)
                {
                    _RecurDay06 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay07;
        public bool? RecurDay07
        {
            get
            {
                return _RecurDay07;
            }
            set
            {
                if (value != _RecurDay07)
                {
                    _RecurDay07 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay08;
        public bool? RecurDay08
        {
            get
            {
                return _RecurDay08;
            }
            set
            {
                if (value != _RecurDay08)
                {
                    _RecurDay08 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay09;
        public bool? RecurDay09
        {
            get
            {
                return _RecurDay09;
            }
            set
            {
                if (value != _RecurDay09)
                {
                    _RecurDay09 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay10;
        public bool? RecurDay10
        {
            get
            {
                return _RecurDay10;
            }
            set
            {
                if (value != _RecurDay10)
                {
                    _RecurDay10 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay11;
        public bool? RecurDay11
        {
            get
            {
                return _RecurDay11;
            }
            set
            {
                if (value != _RecurDay11)
                {
                    _RecurDay11 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay12;
        public bool? RecurDay12
        {
            get
            {
                return _RecurDay12;
            }
            set
            {
                if (value != _RecurDay12)
                {
                    _RecurDay12 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay13;
        public bool? RecurDay13
        {
            get
            {
                return _RecurDay13;
            }
            set
            {
                if (value != _RecurDay13)
                {
                    _RecurDay13 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay14;
        public bool? RecurDay14
        {
            get
            {
                return _RecurDay14;
            }
            set
            {
                if (value != _RecurDay14)
                {
                    _RecurDay14 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay15;
        public bool? RecurDay15
        {
            get
            {
                return _RecurDay15;
            }
            set
            {
                if (value != _RecurDay15)
                {
                    _RecurDay15 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay16;
        public bool? RecurDay16
        {
            get
            {
                return _RecurDay16;
            }
            set
            {
                if (value != _RecurDay16)
                {
                    _RecurDay16 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay17;
        public bool? RecurDay17
        {
            get
            {
                return _RecurDay17;
            }
            set
            {
                if (value != _RecurDay17)
                {
                    _RecurDay17 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay18;
        public bool? RecurDay18
        {
            get
            {
                return _RecurDay18;
            }
            set
            {
                if (value != _RecurDay18)
                {
                    _RecurDay18 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay19;
        public bool? RecurDay19
        {
            get
            {
                return _RecurDay19;
            }
            set
            {
                if (value != _RecurDay19)
                {
                    _RecurDay19 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay20;
        public bool? RecurDay20
        {
            get
            {
                return _RecurDay20;
            }
            set
            {
                if (value != _RecurDay20)
                {
                    _RecurDay20 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay21;
        public bool? RecurDay21
        {
            get
            {
                return _RecurDay21;
            }
            set
            {
                if (value != _RecurDay21)
                {
                    _RecurDay21 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay22;
        public bool? RecurDay22
        {
            get
            {
                return _RecurDay22;
            }
            set
            {
                if (value != _RecurDay22)
                {
                    _RecurDay22 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay23;
        public bool? RecurDay23
        {
            get
            {
                return _RecurDay23;
            }
            set
            {
                if (value != _RecurDay23)
                {
                    _RecurDay23 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay24;
        public bool? RecurDay24
        {
            get
            {
                return _RecurDay24;
            }
            set
            {
                if (value != _RecurDay24)
                {
                    _RecurDay24 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay25;
        public bool? RecurDay25
        {
            get
            {
                return _RecurDay25;
            }
            set
            {
                if (value != _RecurDay25)
                {
                    _RecurDay25 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay26;
        public bool? RecurDay26
        {
            get
            {
                return _RecurDay26;
            }
            set
            {
                if (value != _RecurDay26)
                {
                    _RecurDay26 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay27;
        public bool? RecurDay27
        {
            get
            {
                return _RecurDay27;
            }
            set
            {
                if (value != _RecurDay27)
                {
                    _RecurDay27 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay28;
        public bool? RecurDay28
        {
            get
            {
                return _RecurDay28;
            }
            set
            {
                if (value != _RecurDay28)
                {
                    _RecurDay28 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private bool? _RecurDay29;
        public bool? RecurDay29
        {
            get
            {
                return _RecurDay29;
            }
            set
            {
                if (value != _RecurDay29)
                {
                    _RecurDay29 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay30;
        public bool? RecurDay30
        {
            get
            {
                return _RecurDay30;
            }
            set
            {
                if (value != _RecurDay30)
                {
                    _RecurDay30 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool? _RecurDay31;
        public bool? RecurDay31
        {
            get
            {
                return _RecurDay31;
            }
            set
            {
                if (value != _RecurDay31)
                {
                    _RecurDay31 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
