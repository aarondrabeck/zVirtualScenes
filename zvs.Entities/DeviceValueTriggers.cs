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
    [Table("DeviceValueTriggers", Schema = "ZVS")]
    public partial class DeviceValueTrigger : INotifyPropertyChanged
    {
        public int DeviceValueTriggerId { get; set; }

        private Scene _Scene;
        public virtual Scene Scene
        {
            get
            {
                return _Scene;
            }
            set
            {
                if (value != _Scene)
                {
                    _Scene = value;
                    NotifyPropertyChanged("Scene");
                    NotifyPropertyChanged("TriggerDescription");
                }
            }
        }

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
                    NotifyPropertyChanged("DeviceValue");
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
                    NotifyPropertyChanged("Operator");
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
                    NotifyPropertyChanged("isEnabled");
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
                    NotifyPropertyChanged("Name");
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
                    NotifyPropertyChanged("Value");
                    NotifyPropertyChanged("TriggerDescription");
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

        public string TriggerDescription
        {
            get
            {
                string trigger_op_name = Operator.ToString();

                if (this.Scene == null || this.DeviceValue == null || this.DeviceValue.Device == null)
                    return "UNKNOWN";

                return string.Format("When '{0}' {1} is {2} {3} activate scene '{4}'", this.DeviceValue.Device.Name,
                                                                this.DeviceValue.Name,
                                                                trigger_op_name,
                                                                this.Value,
                                                                this.Scene.Name);
            }
        }
    }
}
