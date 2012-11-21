using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("ScheduledTasks", Schema = "ZVS")]
    public partial class ScheduledTask : INotifyPropertyChanged, IIdentity
    {
        public int Id { get; set; }

        public int StoredCommandId { get; set; }
        private StoredCommand _StoredCommand;
        public virtual StoredCommand StoredCommand
        {
            get
            {
                return _StoredCommand;
            }
            set
            {
                if (value != _StoredCommand)
                {
                    _StoredCommand = value;
                    NotifyPropertyChanged("StoredCommand");
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        private TaskFrequency _Frequency;
        public TaskFrequency Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                if (value != _Frequency)
                {
                    _Frequency = value;
                    NotifyPropertyChanged("Frequency");                   
                }
            }
        }
       
        private string _Name;
        [StringLength(255)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
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
                    NotifyPropertyChanged("isEnabled");
                }
            }
        }
        
        private DateTime? _StartTime;
        public DateTime? StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                if (value != _StartTime)
                {
                    _StartTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        private int? _SortOrder;
        public int? SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                if (value != _SortOrder)
                {
                    _SortOrder = value;
                    NotifyPropertyChanged("SortOrder");
                }
            }
        }
        
        private bool? _RecurMonday;
        public bool? RecurMonday
        {
            get
            {
                return _RecurMonday;
            }
            set
            {
                if (value != _RecurMonday)
                {
                    _RecurMonday = value;
                    NotifyPropertyChanged("RecurMonday");
                }
            }
        }

        private bool? _RecurTuesday;
        public bool? RecurTuesday
        {
            get
            {
                return _RecurTuesday;
            }
            set
            {
                if (value != _RecurTuesday)
                {
                    _RecurTuesday = value;
                    NotifyPropertyChanged("RecurTuesday");
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
                    NotifyPropertyChanged("RecurWednesday");
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
                    NotifyPropertyChanged("RecurThursday");
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
                    NotifyPropertyChanged("RecurFriday");
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
                    NotifyPropertyChanged("RecurSaturday");
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
                    NotifyPropertyChanged("RecurSunday");
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
                    NotifyPropertyChanged("RecurDays");
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
                    NotifyPropertyChanged("RecurWeeks");
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
                    NotifyPropertyChanged("RecurMonth");
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
                    NotifyPropertyChanged("RecurDayofMonth");
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
                    NotifyPropertyChanged("RecurSeconds");
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
                    NotifyPropertyChanged("RecurEven");
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
                    NotifyPropertyChanged("RecurDay01");
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
                    NotifyPropertyChanged("RecurDay02");
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
                    NotifyPropertyChanged("RecurDay03");
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
                    NotifyPropertyChanged("RecurDay04");
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
                    NotifyPropertyChanged("RecurDay05");
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
                    NotifyPropertyChanged("RecurDay06");
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
                    NotifyPropertyChanged("RecurDay07");
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
                    NotifyPropertyChanged("RecurDay08");
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
                    NotifyPropertyChanged("RecurDay09");
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
                    NotifyPropertyChanged("RecurDay10");
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
                    NotifyPropertyChanged("RecurDay11");
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
                    NotifyPropertyChanged("RecurDay12");
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
                    NotifyPropertyChanged("RecurDay13");
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
                    NotifyPropertyChanged("RecurDay14");
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
                    NotifyPropertyChanged("RecurDay15");
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
                    NotifyPropertyChanged("RecurDay16");
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
                    NotifyPropertyChanged("RecurDay17");
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
                    NotifyPropertyChanged("RecurDay18");
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
                    NotifyPropertyChanged("RecurDay19");
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
                    NotifyPropertyChanged("RecurDay20");
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
                    NotifyPropertyChanged("RecurDay21");
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
                    NotifyPropertyChanged("RecurDay22");
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
                    NotifyPropertyChanged("RecurDay23");
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
                    NotifyPropertyChanged("RecurDay24");
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
                    NotifyPropertyChanged("RecurDay25");
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
                    NotifyPropertyChanged("RecurDay26");
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
                    NotifyPropertyChanged("RecurDay27");
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
                    NotifyPropertyChanged("RecurDay28");
                }
            }
        }

        partial void BeforeRecurDay28Change(bool? oldValue, bool? newValue);
        partial void AfterRecurDay28Change(bool? oldValue, bool? newValue);

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
                    NotifyPropertyChanged("RecurDay29");
                }
            }
        }

        partial void BeforeRecurDay29Change(bool? oldValue, bool? newValue);
        partial void AfterRecurDay29Change(bool? oldValue, bool? newValue);

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
                    NotifyPropertyChanged("RecurDay30");
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
                    NotifyPropertyChanged("RecurDay31");
                }
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
