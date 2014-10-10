using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace zvs.Entities
{
    public abstract class BaseValue : INotifyPropertyChanged
    {
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

        private DataType _valueType;
        [Required(ErrorMessage = "Value type cannot be empty")]
        public DataType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                if (value == _valueType) return;
                _valueType = value;
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
