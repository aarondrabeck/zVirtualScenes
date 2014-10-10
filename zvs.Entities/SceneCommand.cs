using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    [Table("SceneCommands", Schema = "ZVS")]
    public partial class SceneCommand : IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //No actual navigational property here
        private StoredCommand _storedCommand;
        public virtual StoredCommand StoredCommand
        {
            get
            {
                return _storedCommand;
            }
            set
            {
                if (value == _storedCommand) return;
                _storedCommand = value;
                NotifyPropertyChanged();
            }
        }

        public int SceneId { get; set; }
        private Scene _scene;
        public virtual Scene Scene
        {
            get
            {
                return _scene;
            }
            set
            {
                if (value == _scene) return;
                _scene = value;
                NotifyPropertyChanged();
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
                if (value == _sortOrder) return;
                _sortOrder = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
