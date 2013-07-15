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
    [Table("Groups", Schema = "ZVS")]
    public partial class Group : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        private ObservableCollection<Device> _Devices = new ObservableCollection<Device>();
        public virtual ObservableCollection<Device> Devices
        {
            get { return _Devices; }
            set { _Devices = value; }
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
