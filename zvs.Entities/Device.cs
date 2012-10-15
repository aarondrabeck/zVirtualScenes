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
    [Table("Devices", Schema = "ZVS")]
    public partial class Device : INotifyPropertyChanged
    {
        public int DeviceId { get; set; }

        public Device()
        {
            this.Commands = new ObservableCollection<DeviceCommand>();
            this.PropertyValues = new ObservableCollection<DevicePropertyValue>();
            this.Values = new ObservableCollection<DeviceValue>();
            this.Groups = new ObservableCollection<Group>();
            this.QueuedDeviceCommands = new ObservableCollection<QueuedDeviceCommand>();
            this.QueuedDeviceTypeCommands = new ObservableCollection<QueuedDeviceTypeCommand>();
            this.SceneCommands = new ObservableCollection<SceneCommand>();
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
                    NotifyPropertyChanged("NodeNumber");
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
                    NotifyPropertyChanged("LastHeardFrom");
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
                    NotifyPropertyChanged("CurrentLevelText");
                }
            }
        }

        private Nullable<double> _CurrentLevelInt;
        public Nullable<double> CurrentLevelInt
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
                    NotifyPropertyChanged("CurrentLevelInt");
                }
            }
        }

        public virtual DeviceType Type { get; set; }
        public virtual ObservableCollection<QueuedDeviceCommand> QueuedDeviceCommands { get; set; }
        public virtual ObservableCollection<QueuedDeviceTypeCommand> QueuedDeviceTypeCommands { get; set; }
        public virtual ObservableCollection<DeviceCommand> Commands { get; set; }
        public virtual ObservableCollection<DevicePropertyValue> PropertyValues { get; set; }
        public virtual ObservableCollection<DeviceValue> Values { get; set; }
        public virtual ObservableCollection<Group> Groups { get; set; }
        public virtual ObservableCollection<SceneCommand> SceneCommands { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// This is called when a device or device value 
        /// </summary> 
        public static IQueryable<Device> GetAllDevices(zvsContext context, bool forList)
        {
            var query = context.Devices.Where(o => o.Type.Plugin.UniqueIdentifier != "BUILTIN");

            if (forList)
                return query.Where(o => o.Type.ShowInList == true);

            return query;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
