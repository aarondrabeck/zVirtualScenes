using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using zvs.DataModel.Tasks;

namespace zvs.DataModel
{
    [Table("ZvsScheduledTasks", Schema = "ZVS")]
    public class ZvsScheduledTask : INotifyPropertyChanged, IIdentity, IStoredCommand
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public int? CommandId { get; set; }
        private Command _command;
        public virtual Command Command
        {
            get
            {
                return _command;
            }
            set
            {
                if (value == _command) return;
                _command = value;
                NotifyPropertyChanged();
            }
        }

        private string _argument;
        [StringLength(512)]
        public string Argument
        {
            get
            {
                return _argument;
            }
            set
            {
                if (value == _argument) return;
                _argument = value;
                NotifyPropertyChanged();
            }
        }

        private string _argument2;
        [StringLength(512)]
        public string Argument2
        {
            get
            {
                return _argument2;
            }
            set
            {
                if (value == _argument2) return;
                _argument2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _targetObjectName;
        public string TargetObjectName
        {
            get { return _targetObjectName; }
            set
            {
                if (value == _targetObjectName) return;
                _targetObjectName = value;
                NotifyPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
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
