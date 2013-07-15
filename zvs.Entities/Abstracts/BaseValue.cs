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
    public abstract class BaseValue : INotifyPropertyChanged
    {
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

        private DataType _ValueType;
        [Required(ErrorMessage = "Value type cannot be empty")]
        public DataType ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                if (value != _ValueType)
                {
                    _ValueType = value;
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
