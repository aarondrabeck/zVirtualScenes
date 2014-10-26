using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("DeviceValueTriggers", Schema = "ZVS")]
    public class DeviceValueTrigger : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StoredCommandId { get; set; }
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
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public void SetDescription()
        {
            var triggerOpName = Operator.ToString();

            if (StoredCommand == null || DeviceValue == null || DeviceValue.Device == null)
                Description = "Incomplete ScheduledTask";
            else
                Description = string.Format("{0} {1} is {2} {3}", DeviceValue.Device.Name,
                                                            DeviceValue.Name,
                                                            triggerOpName,
                                                            Value);
        }
    }

  
}
