using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("DeviceValueTriggers", Schema = "ZVS")]
    public class DeviceValueTrigger : INotifyPropertyChanged, IIdentity, IStoredCommand
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? DeviceValueId { get; set; }
        private DeviceValue _deviceValue;
        public virtual DeviceValue DeviceValue
        {
            get
            {
                return _deviceValue;
            }
            set
            {
                if (value != _deviceValue)
                {
                    _deviceValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private TriggerOperator _operator;
        public TriggerOperator Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                if (value != _operator)
                {
                    _operator = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
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

        private string _value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int? CommandId { get; set; }
        private Command _command;
        public virtual Command Command
        {
            get
            {
                return _command;
            }
            set
            {
                if (value == _command) return;
                _command = value;
                NotifyPropertyChanged();
            }
        }

        private string _argument;
        [StringLength(512)]
        public string Argument
        {
            get
            {
                return _argument;
            }
            set
            {
                if (value == _argument) return;
                _argument = value;
                NotifyPropertyChanged();
            }
        }

        private string _argument2;
        [StringLength(512)]
        public string Argument2
        {
            get
            {
                return _argument2;
            }
            set
            {
                if (value == _argument2) return;
                _argument2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _targetObjectName;
        public string TargetObjectName
        {
            get { return _targetObjectName; }
            set
            {
                if (value == _targetObjectName) return;
                _targetObjectName = value;
                NotifyPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
       
        //public void SetDescription()
        //{
        //    var triggerOpName = Operator.ToString();

        //    if (DeviceValueTriggerStoredCommand == null || DeviceValue == null || DeviceValue.Device == null)
        //        Description = "Incomplete ScheduledTask";
        //    else
        //        Description = string.Format("{0} {1} is {2} {3}", DeviceValue.Device.Name,
        //                                                    DeviceValue.Name,
        //                                                    triggerOpName,
        //                                                    Value);
        //}
    }

  
}
