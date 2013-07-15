using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("Devices", Schema = "ZVS")]
    public partial class Device : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public override string ToString()
        {
            return this.Name;
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
                    NotifyPropertyChanged();
                }
            }
        }

        private int _NodeNumber;
        public int NodeNumber
        {
            get
            {
                return _NodeNumber;
            }
            set
            {
                if (value != _NodeNumber)
                {
                    _NodeNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime? _LastHeardFrom;
        public DateTime? LastHeardFrom
        {
            get
            {
                return _LastHeardFrom;
            }
            set
            {
                if (value != _LastHeardFrom)
                {
                    _LastHeardFrom = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CurrentLevelText;
        [StringLength(255)]
        public string CurrentLevelText
        {
            get
            {
                return _CurrentLevelText;
            }
            set
            {
                if (value != _CurrentLevelText)
                {
                    _CurrentLevelText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double? _CurrentLevelInt;
        public double? CurrentLevelInt
        {
            get
            {
                return _CurrentLevelInt;
            }
            set
            {
                if (value != _CurrentLevelInt)
                {
                    _CurrentLevelInt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int DeviceTypeId { get; set; }
        public virtual DeviceType Type { get; set; }

        private ObservableCollection<DeviceCommand> _Commands = new ObservableCollection<DeviceCommand>();
        [ConfidentialData]
        public virtual ObservableCollection<DeviceCommand> Commands
        {
            get { return _Commands; }
            set { _Commands = value; }
        }

        private ObservableCollection<DeviceSettingValue> _DeviceSettingValues = new ObservableCollection<DeviceSettingValue>();
        public virtual ObservableCollection<DeviceSettingValue> DeviceSettingValues
        {
            get { return _DeviceSettingValues; }
            set { _DeviceSettingValues = value; }
        }

        private ObservableCollection<DeviceTypeSettingValue> _DeviceTypeSettingValues = new ObservableCollection<DeviceTypeSettingValue>();
        public virtual ObservableCollection<DeviceTypeSettingValue> DeviceTypeSettingValues
        {
            get { return _DeviceTypeSettingValues; }
            set { _DeviceTypeSettingValues = value; }
        }

        private ObservableCollection<DeviceValue> _Values = new ObservableCollection<DeviceValue>();
        public virtual ObservableCollection<DeviceValue> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        private ObservableCollection<Group> _Groups = new ObservableCollection<Group>();
        public virtual ObservableCollection<Group> Groups
        {
            get { return _Groups; }
            set { _Groups = value; }
        }

        private ObservableCollection<StoredCommand> _StoredCommands = new ObservableCollection<StoredCommand>();
        public virtual ObservableCollection<StoredCommand> StoredCommands
        {
            get { return _StoredCommands; }
            set { _StoredCommands = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
