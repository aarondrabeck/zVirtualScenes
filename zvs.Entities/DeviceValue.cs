using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceValues", Schema = "ZVS")]
    public class DeviceValue : BaseValue, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
                if (value == _genre) return;
                _genre = value;
                NotifyPropertyChanged();
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
                if (value == _index) return;
                _index = value;
                NotifyPropertyChanged();
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
                if (value == _commandClass) return;
                _commandClass = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                if (value == _isReadOnly) return;
                _isReadOnly = value;
                NotifyPropertyChanged();
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
                if (value == _customData1) return;
                _customData1 = value;
                NotifyPropertyChanged();
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
                if (value == _customData2) return;
                _customData2 = value;
                NotifyPropertyChanged();
            }
        }
    }
}
