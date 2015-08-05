using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("Plugins", Schema = "ZVS")]
    public class Plugin : INotifyPropertyChanged, IIdentity
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
                if (value == _name) return;
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private Guid _pluginGuid;
        public Guid PluginGuid
        {
            get { return _pluginGuid; }
            set
            {
                if (value == _pluginGuid) return;
                _pluginGuid = value;
                NotifyPropertyChanged();
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
                if (value == _isEnabled) return;
                _isEnabled = value;
                NotifyPropertyChanged();
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

        private ObservableCollection<PluginSetting> _settings = new ObservableCollection<PluginSetting>();
        public virtual ObservableCollection<PluginSetting> Settings
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
