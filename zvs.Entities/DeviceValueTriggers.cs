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
using System.Data.Entity;

namespace zvs.Entities
{
    [Table("DeviceValueTriggers", Schema = "ZVS")]
    public partial class DeviceValueTrigger : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //No actual navigational property here
        private StoredCommand _StoredCommand;
        public virtual StoredCommand StoredCommand
        {
            get
            {
                return _StoredCommand;
            }
            set
            {
                if (value != _StoredCommand)
                {
                    _StoredCommand = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        public int? DeviceValueId { get; set; }
        private DeviceValue _DeviceValue;
        public virtual DeviceValue DeviceValue
        {
            get
            {
                return _DeviceValue;
            }
            set
            {
                if (value != _DeviceValue)
                {
                    _DeviceValue = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        private TriggerOperator _Operator;
        public TriggerOperator Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                if (value != _Operator)
                {
                    _Operator = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        private bool _isEnabled;
        public bool isEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
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
                    NotifyPropertyChanged("TriggerDescription");
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
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }

    public static class DeviceValueTriggerExtensionMethods
    {
        public static void SetDescription(this DeviceValueTrigger trigger, zvsContext context)
        {
            string trigger_op_name = trigger.Operator.ToString();

            if (trigger.StoredCommand == null || trigger.DeviceValue == null || trigger.DeviceValue.Device == null)
                trigger.Description = "Incomplete Trigger";

            trigger.Description = string.Format("'{0}' {1} is {2} {3}", trigger.DeviceValue.Device.Name,
                                                        trigger.DeviceValue.Name,
                                                        trigger_op_name,
                                                        trigger.Value
                                                        );
        }
    }
}
