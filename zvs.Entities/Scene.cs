using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("Scenes", Schema = "ZVS")]
    public class Scene : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<SceneStoredCommand> _commands = new ObservableCollection<SceneStoredCommand>();
        public virtual ObservableCollection<SceneStoredCommand> Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }

        private ObservableCollection<SceneSettingValue> _settingValues = new ObservableCollection<SceneSettingValue>();
        public virtual ObservableCollection<SceneSettingValue> SettingValues
        {
            get { return _settingValues; }
            set { _settingValues = value; }
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

        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int? _sortOrder;
        public int? SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                if (value != _sortOrder)
                {
                    _sortOrder = value;
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
