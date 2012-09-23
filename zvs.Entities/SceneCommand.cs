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
    [Table("SceneCommands", Schema = "ZVS")]
    public partial class SceneCommand
    {
        public int SceneCommandId { get; set; }

        private StoredCommand _StoredCommand;
        [Required]
        public virtual StoredCommand StoredCommand
        {
            get
            {
                return _StoredCommand;
            }
            set
            {
                if (value != _StoredCommand)
                {
                    _StoredCommand = value;
                    NotifyPropertyChanged("StoredCommand");
                }
            }
        }

        private Scene _Scene;
        [Required]
        public virtual Scene Scene
        {
            get
            {
                return _Scene;
            }
            set
            {
                if (value != _Scene)
                {
                    _Scene = value;
                    NotifyPropertyChanged("Scene");
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
