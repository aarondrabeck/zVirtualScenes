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
    [Table("Scenes", Schema = "ZVS")]
    public partial class Scene : INotifyPropertyChanged
    {
        public int SceneId { get; set; }

        public virtual ObservableCollection<SceneCommand> Commands { get; set; }
        public virtual ObservableCollection<ScenePropertyValue> PropertyValues { get; set; }

        public Scene()
        {
            Commands = new ObservableCollection<SceneCommand>();
            PropertyValues = new ObservableCollection<ScenePropertyValue>();
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
                    NotifyPropertyChanged("Name");
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
                    NotifyPropertyChanged("isRunning");
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
                    NotifyPropertyChanged("SortOrder");
                }
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
