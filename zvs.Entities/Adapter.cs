using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("Adapters", Schema = "ZVS")]
    public class Adapter : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private Guid _adapterGuid;
        public Guid AdapterGuid
        {
            get { return _adapterGuid; }
            set
            {
                if (value != _adapterGuid)
                {
                    _adapterGuid = value;
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

        private string _description;
        [StringLength(1024)]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<DeviceType> _deviceTypes = new ObservableCollection<DeviceType>();
        public virtual ObservableCollection<DeviceType> DeviceTypes
        {
            get { return _deviceTypes; }
            set { _deviceTypes = value; }
        }

        private ObservableCollection<AdapterSetting> _settings = new ObservableCollection<AdapterSetting>();
        public virtual ObservableCollection<AdapterSetting> Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
