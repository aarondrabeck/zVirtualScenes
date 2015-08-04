using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace zvs.DataModel
{
    [Table("Commands", Schema = "ZVS")]
    public class Command : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<CommandOption> _options = new ObservableCollection<CommandOption>();
        public virtual ObservableCollection<CommandOption> Options
        {
            get { return _options; }
            set { _options = value; }
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
                    NotifyPropertyChanged("Command");
                }
            }
        }

        private string _uniqueIdentifier;
        [StringLength(255)]
        public string UniqueIdentifier
        {
            get
            {
                return _uniqueIdentifier;
            }
            set
            {
                if (value == _uniqueIdentifier) return;
                _uniqueIdentifier = value;
                NotifyPropertyChanged();
            }
        }

        private string _value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == _value) return;
                _value = value;
                NotifyPropertyChanged();
            }
        }

        private DataType _argumentType;
        [Required(ErrorMessage = "Argument type cannot be empty")]
        public DataType ArgumentType
        {
            get
            {
                return _argumentType;
            }
            set
            {
                if (value == _argumentType) return;
                _argumentType = value;
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
                if (value == _description) return;
                _description = value;
                NotifyPropertyChanged();
            }
        }

        private string _customData1;
        [StringLength(255)]
        public string CustomData1
        {
            get
            {
                return _customData1;
            }
            set
            {
                if (value == _customData1) return;
                _customData1 = value;
                NotifyPropertyChanged();
            }
        }

        private string _customData2;
        [StringLength(255)]
        public string CustomData2
        {
            get
            {
                return _customData2;
            }
            set
            {
                if (value == _customData2) return;
                _customData2 = value;
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

        private string _help;
        [StringLength(1024)]
        public string Help
        {
            get
            {
                return _help;
            }
            set
            {
                if (value == _help) return;
                _help = value;
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
