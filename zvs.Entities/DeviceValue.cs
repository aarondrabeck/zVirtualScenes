using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceValues", Schema = "ZVS")]
    public class DeviceValue : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }

        private ObservableCollection<DeviceValueHistory> _history = new ObservableCollection<DeviceValueHistory>();
        public virtual ObservableCollection<DeviceValueHistory> History
        {
            get { return _history; }
            set { _history = value; }
        }

        private ObservableCollection<DeviceValueTrigger> _triggers = new ObservableCollection<DeviceValueTrigger>();
        public virtual ObservableCollection<DeviceValueTrigger> Triggers
        {
            get { return _triggers; }
            set { _triggers = value; }
        }

        private string _genre;
        [StringLength(255)]
        public string Genre
        {
            get
            {
                return _genre;
            }
            set
            {
                if (value != _genre)
                {
                    _genre = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _index;
        [StringLength(255)]
        public string Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value != _index)
                {
                    _index = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _commandClass;
        [StringLength(255)]
        public string CommandClass
        {
            get
            {
                return _commandClass;
            }
            set
            {
                if (value != _commandClass)
                {
                    _commandClass = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isReadOnly;
        public bool isReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                if (value != _isReadOnly)
                {
                    _isReadOnly = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _customData1;
        [StringLength(255)]
        public string CustomData1
        {
            get
            {
                return _customData1;
            }
            set
            {
                if (value != _customData1)
                {
                    _customData1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _customData2;
        [StringLength(255)]
        public string CustomData2
        {
            get
            {
                return _customData2;
            }
            set
            {
                if (value != _customData2)
                {
                    _customData2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Events
        /// <summary>
        /// Called after the Value has been changed in the database
        /// </summary>
        public static event ValueDataChangedEventHandler DeviceValueDataChangedEvent = delegate { };
        public delegate void ValueDataChangedEventHandler(object sender, ValueDataChangedEventArgs e);

        public void DeviceValueDataChanged(ValueDataChangedEventArgs args)
        {
            DeviceValueDataChangedEvent(this, args);
        }

        public class ValueDataChangedEventArgs : System.EventArgs
        {
            public int DeviceValueId { get; private set; }
            public string NewValue { get; private set; }
            public string OldValue { get; private set; }

            public ValueDataChangedEventArgs(int deviceValueId, string newValue, string oldValue)
            {
                DeviceValueId = deviceValueId;
                NewValue = newValue;
                OldValue = oldValue;
            }
        }
    }
}
