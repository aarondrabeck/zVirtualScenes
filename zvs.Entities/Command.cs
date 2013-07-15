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
    [Table("Commands", Schema = "ZVS")]
    public class Command : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<CommandOption> _Options = new ObservableCollection<CommandOption>();
        public virtual ObservableCollection<CommandOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
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
                    NotifyPropertyChanged("Command");
                }
            }
        }

        private string _UniqueIdentifier;
        [StringLength(255)]
        public string UniqueIdentifier
        {
            get
            {
                return _UniqueIdentifier;
            }
            set
            {
                if (value != _UniqueIdentifier)
                {
                    _UniqueIdentifier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DataType _ArgumentType;
        [Required(ErrorMessage = "Argument type cannot be empty")]
        public DataType ArgumentType
        {
            get
            {
                return _ArgumentType;
            }
            set
            {
                if (value != _ArgumentType)
                {
                    _ArgumentType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Description;
        [StringLength(1024)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CustomData1;
        [StringLength(255)]
        public string CustomData1
        {
            get
            {
                return _CustomData1;
            }
            set
            {
                if (value != _CustomData1)
                {
                    _CustomData1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CustomData2;
        [StringLength(255)]
        public string CustomData2
        {
            get
            {
                return _CustomData2;
            }
            set
            {
                if (value != _CustomData2)
                {
                    _CustomData2 = value;
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

        private string _Help;
        [StringLength(1024)]
        public string Help
        {
            get
            {
                return _Help;
            }
            set
            {
                if (value != _Help)
                {
                    _Help = value;
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
