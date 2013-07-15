using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("Scenes", Schema = "ZVS")]
    public partial class Scene : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<SceneCommand> _Commands = new ObservableCollection<SceneCommand>();
        public virtual ObservableCollection<SceneCommand> Commands
        {
            get { return _Commands; }
            set { _Commands = value; }
        }

        private ObservableCollection<SceneSettingValue> _SettingValues = new ObservableCollection<SceneSettingValue>();
        public virtual ObservableCollection<SceneSettingValue> SettingValues
        {
            get { return _SettingValues; }
            set { _SettingValues = value; }
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

        private bool _isRunning;
        public bool isRunning
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

        private int? _SortOrder;
        public int? SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                if (value != _SortOrder)
                {
                    _SortOrder = value;
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
