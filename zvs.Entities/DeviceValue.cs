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
    [Table("DeviceValues", Schema = "ZVS")]
    public partial class DeviceValue : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }

        private ObservableCollection<DeviceValueTrigger> _Triggers = new ObservableCollection<DeviceValueTrigger>();
        public virtual ObservableCollection<DeviceValueTrigger> Triggers
        {
            get { return _Triggers; }
            set { _Triggers = value; }
        }

        private string _Genre;
        [StringLength(255)]
        public string Genre
        {
            get
            {
                return _Genre;
            }
            set
            {
                if (value != _Genre)
                {
                    _Genre = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Index;
        [StringLength(255)]
        public string Index
        {
            get
            {
                return _Index;
            }
            set
            {
                if (value != _Index)
                {
                    _Index = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CommandClass;
        [StringLength(255)]
        public string CommandClass
        {
            get
            {
                return _CommandClass;
            }
            set
            {
                if (value != _CommandClass)
                {
                    _CommandClass = value;
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

        private string _CustomData1;
        [StringLength(255)]
        public string CustomData1
        {
            get
            {
                return _CustomData1;
            }
            set
            {
                if (value != _CustomData1)
                {
                    _CustomData1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CustomData2;
        [StringLength(255)]
        public string CustomData2
        {
            get
            {
                return _CustomData2;
            }
            set
            {
                if (value != _CustomData2)
                {
                    _CustomData2 = value;
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
            public string newValue { get; private set; }
            public string oldValue { get; private set; }

            public ValueDataChangedEventArgs(int DeviceValueId, string newValue, string oldValue)
            {
                this.DeviceValueId = DeviceValueId;
                this.newValue = newValue;
                this.oldValue = oldValue;
            }
        }

        public static event DeviceValueAddedEventHandler DeviceValueAddedEvent = delegate { };
        public delegate void DeviceValueAddedEventHandler(object sender, EventArgs e);

        public void DeviceValueAdded(EventArgs e)
        {
            DeviceValueAddedEvent(this, e);
        }
    }
}
