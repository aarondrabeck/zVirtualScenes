using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    [Table("DeviceTypes", Schema = "ZVS")]
    public partial class DeviceType : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterId { get; set; }
        public virtual Adapter Adapter { get; set; }

        private ObservableCollection<Device> _Devices = new ObservableCollection<Device>();
        public virtual ObservableCollection<Device> Devices
        {
            get { return _Devices; }
            set { _Devices = value; }
        }

        private ObservableCollection<DeviceTypeSetting> _DeviceTypeSettings = new ObservableCollection<DeviceTypeSetting>();
        public virtual ObservableCollection<DeviceTypeSetting> Settings
        {
            get { return _DeviceTypeSettings; }
            set { _DeviceTypeSettings = value; }
        }

        private ObservableCollection<DeviceTypeCommand> _Commands = new ObservableCollection<DeviceTypeCommand>();
        public virtual ObservableCollection<DeviceTypeCommand> Commands
        {
            get { return _Commands; }
            set { _Commands = value; }
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

        private string _UniqueIdentifier;
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(255)]
        public string UniqueIdentifier
        {
            get
            {
                return _UniqueIdentifier;
            }
            set
            {
                if (value != _UniqueIdentifier)
                {
                    _UniqueIdentifier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _ShowInList;
        public bool ShowInList
        {
            get
            {
                return _ShowInList;
            }
            set
            {
                if (value != _ShowInList)
                {
                    _ShowInList = value;
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
