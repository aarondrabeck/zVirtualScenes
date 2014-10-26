using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using zvs.DataModel.Tasks;

namespace zvs.DataModel
{
    [Table("CommandScheduledTasks", Schema = "ZVS")]
    public class CommandScheduledTask : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StoredCommandId { get; set; }
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

        public virtual ScheduledTask ScheduledTask { get; set; }

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
