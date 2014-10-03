using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    [Table("Adapters", Schema = "ZVS")]
    public partial class Adapter : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        private Guid _AdapterGuid;
        public Guid AdapterGuid
        {
            get { return _AdapterGuid; }
            set
            {
                if (value != _AdapterGuid)
                {
                    _AdapterGuid = value;
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

        private string _Description;
        [StringLength(1024)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<DeviceType> _DeviceTypes = new ObservableCollection<DeviceType>();
        public virtual ObservableCollection<DeviceType> DeviceTypes
        {
            get { return _DeviceTypes; }
            set { _DeviceTypes = value; }
        }

        private ObservableCollection<AdapterSetting> _Settings = new ObservableCollection<AdapterSetting>();
        public virtual ObservableCollection<AdapterSetting> Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
