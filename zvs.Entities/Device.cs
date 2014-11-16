using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    [Table("Devices", Schema = "ZVS")]
    public class Device : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Location, Name);
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

        private int _nodeNumber;
        public int NodeNumber
        {
            get
            {
                return _nodeNumber;
            }
            set
            {
                if (value != _nodeNumber)
                {
                    _nodeNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime? _lastHeardFrom;
        public DateTime? LastHeardFrom
        {
            get
            {
                return _lastHeardFrom;
            }
            set
            {
                if (value != _lastHeardFrom)
                {
                    _lastHeardFrom = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _currentLevelText;
        [StringLength(255)]
        public string CurrentLevelText
        {
            get
            {
                return _currentLevelText;
            }
            set
            {
                if (value != _currentLevelText)
                {
                    _currentLevelText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _currentLevelInt;
        public double CurrentLevelInt
        {
            get
            {
                return _currentLevelInt;
            }
            set
            {
                if (value == _currentLevelInt) return;
                _currentLevelInt = value;
                NotifyPropertyChanged();
            }
        }

        private int _deviceTypeId;
        public int DeviceTypeId
        {
            get { return _deviceTypeId; }
            set
            {
                if (value != _deviceTypeId)
                {
                    _deviceTypeId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DeviceType _type;
        public virtual DeviceType Type
        {
            get { return _type; }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set
            {
                if (value == _location) return;
                _location = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<DeviceCommand> _commands = new ObservableCollection<DeviceCommand>();
        [ConfidentialData]
        public virtual ObservableCollection<DeviceCommand> Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }

        private ObservableCollection<DeviceSettingValue> _deviceSettingValues = new ObservableCollection<DeviceSettingValue>();
        public virtual ObservableCollection<DeviceSettingValue> DeviceSettingValues
        {
            get { return _deviceSettingValues; }
            set { _deviceSettingValues = value; }
        }

        private ObservableCollection<DeviceTypeSettingValue> _deviceTypeSettingValues = new ObservableCollection<DeviceTypeSettingValue>();
        public virtual ObservableCollection<DeviceTypeSettingValue> DeviceTypeSettingValues
        {
            get { return _deviceTypeSettingValues; }
            set { _deviceTypeSettingValues = value; }
        }

        private ObservableCollection<DeviceValue> _values = new ObservableCollection<DeviceValue>();
        public virtual ObservableCollection<DeviceValue> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        public virtual ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotMapped]
        public DateTimeOffset? EdmLastHeardFrom
        {
            // Assume the CreateOn property stores UTC time.
            get
            {
                return LastHeardFrom.HasValue ? new DateTimeOffset(LastHeardFrom.Value, TimeSpan.FromHours(0)) : (DateTimeOffset?)null;
            }
            set
            {
                LastHeardFrom = value.HasValue ? value.Value.UtcDateTime : (DateTime?)null;
            }
        }

        public async Task<string> GetDeviceTypeValueAsync(string deviceTypeSettingUniqueIdentifier, ZvsContext context)
        {
            //check if the value has been defined by the user for this device
            var definedSetting = await context.DeviceTypeSettingValues.FirstOrDefaultAsync(o =>
                                        o.DeviceTypeSetting.UniqueIdentifier == deviceTypeSettingUniqueIdentifier &&
                                        o.Device.Id == Id);

            if (definedSetting != null)
                return definedSetting.Value;

            //otherwise get default for the command
            var defaultSetting = await context.DeviceTypeSettings.FirstOrDefaultAsync(o =>
                o.UniqueIdentifier == deviceTypeSettingUniqueIdentifier &&
                 o.DeviceTypeId == DeviceTypeId);

            return defaultSetting != null ? defaultSetting.Value : null;
        }

        public async Task<string> GetDeviceSettingAsync(string deviceSettingUniqueIdentifier, ZvsContext context)
        {
            //check if the value has been defined by the user for this device
            var definedSetting = await context.DeviceSettingValues.FirstOrDefaultAsync(o =>
                                        o.DeviceSetting.UniqueIdentifier == deviceSettingUniqueIdentifier &&
                                        o.Device.Id == Id);

            if (definedSetting != null)
                return definedSetting.Value;

            //otherwise get default for the command
            var defaultSetting = await context.DeviceTypeSettings.FirstOrDefaultAsync(o =>
                o.UniqueIdentifier == deviceSettingUniqueIdentifier);

            return defaultSetting != null ? defaultSetting.Value : null;
        }


    }
}
