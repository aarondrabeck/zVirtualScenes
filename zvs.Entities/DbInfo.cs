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
    [Table("DbInfo", Schema = "ZVS")]
    public partial class DbInfo : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
