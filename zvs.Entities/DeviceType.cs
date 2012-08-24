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
    [Table("DeviceTypes", Schema = "ZVS")]
    public partial class DeviceType : INotifyPropertyChanged
    {
        public int DeviceTypeId { get; set; }

        public DeviceType()
        {
            this.Devices = new ObservableCollection<Device>();
            this.Commands = new ObservableCollection<DeviceTypeCommand>();
        }

        public virtual Plugin Plugin { get; set; }
        public virtual ObservableCollection<Device> Devices { get; set; }
        public virtual ObservableCollection<DeviceTypeCommand> Commands { get; set; }
        
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
                    NotifyPropertyChanged("Name");
                }
            }
        }
        
        private string _UniqueIdentifier;
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(50)]
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
                    NotifyPropertyChanged("UniqueIdentifier");
                }
            }
        }

        private bool _ShowInList;
        public bool ShowInList
        {
            get
            {
                return _ShowInList;
            }
            set
            {
                if (value != _ShowInList)
                {
                    _ShowInList = value;
                    NotifyPropertyChanged("ShowInList");
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
