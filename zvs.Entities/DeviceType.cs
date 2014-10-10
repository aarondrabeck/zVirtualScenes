using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    [Table("DeviceTypes", Schema = "ZVS")]
    public class DeviceType : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterId { get; set; }
        public virtual Adapter Adapter { get; set; }

        private ObservableCollection<Device> _devices = new ObservableCollection<Device>();
        public virtual ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }

        private ObservableCollection<DeviceTypeSetting> _deviceTypeSettings = new ObservableCollection<DeviceTypeSetting>();
        public virtual ObservableCollection<DeviceTypeSetting> Settings
        {
            get { return _deviceTypeSettings; }
            set { _deviceTypeSettings = value; }
        }

        private ObservableCollection<DeviceTypeCommand> _commands = new ObservableCollection<DeviceTypeCommand>();
        public virtual ObservableCollection<DeviceTypeCommand> Commands
        {
            get { return _commands; }
            set { _commands = value; }
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

        private string _uniqueIdentifier;
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(255)]
        public string UniqueIdentifier
        {
            get
            {
                return _uniqueIdentifier;
            }
            set
            {
                if (value != _uniqueIdentifier)
                {
                    _uniqueIdentifier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _showInList;
        public bool ShowInList
        {
            get
            {
                return _showInList;
            }
            set
            {
                if (value != _showInList)
                {
                    _showInList = value;
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
