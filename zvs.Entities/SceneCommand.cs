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
    [Table("SceneCommands", Schema = "ZVS")]
    public partial class SceneCommand : INotifyPropertyChanged
    {
        public int SceneCommandId { get; set; }

        public SceneCommand()
        {

        }
        
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
                    NotifyPropertyChanged("ActionDescription");
                    NotifyPropertyChanged("ActionableObject");
                    NotifyPropertyChanged("Scene");
                }
            }
        }

        private Device _Device;
        public virtual Device Device
        {
            get
            {
                return _Device;
            }
            set
            {
                if (value != _Device)
                {
                    _Device = value;
                    NotifyPropertyChanged("ActionDescription");
                    NotifyPropertyChanged("ActionableObject");
                    NotifyPropertyChanged("Device");
                }
            }
        }

        private Command _Command;
        public virtual Command Command
        {
            get
            {
                return _Command;
            }
            set
            {
                if (value != _Command)
                {
                    _Command = value;
                    NotifyPropertyChanged("ActionDescription");
                    NotifyPropertyChanged("ActionableObject");
                    NotifyPropertyChanged("Command");
                }
            }
        }

        private string _Argument;
        [StringLength(512)]
        public string Argument
        {
            get
            {
                return _Argument;
            }
            set
            {
                if (value != _Argument)
                {
                    _Argument = value;
                    NotifyPropertyChanged("Argument");
                    NotifyPropertyChanged("ActionDescription");
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
                    NotifyPropertyChanged("SortOrder");
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

        public string ActionableObject
        {
            get
            {
                using (zvsContext context = new zvsContext())
                {
                    if (this.Command is BuiltinCommand)
                    {
                        return this.Command.Name;
                    }
                    else if (this.Command is DeviceCommand || 
                             this.Command is DeviceTypeCommand)
                    {
                       return Device.Name;
                    }
                    return string.Empty;
                }
            }
        }

        public string ActionDescription
        {
            get
            {
                using (zvsContext context = new zvsContext())
                {
                    if (this.Command is BuiltinCommand)
                    {
                        BuiltinCommand bc = (BuiltinCommand)this.Command;
                        if (bc != null)
                        {
                            switch (bc.UniqueIdentifier)
                            {
                                case "REPOLL_ME":
                                    {
                                        int d_id = 0;
                                        int.TryParse(this.Argument, out d_id);

                                        Device device_to_repoll = context.Devices.FirstOrDefault(d => d.DeviceId == d_id);
                                        if (device_to_repoll != null && d_id > 0)
                                        {
                                            return device_to_repoll.Name;
                                        }
                                        else
                                        {
                                            return this.Argument;
                                        }
                                    }
                                case "GROUP_ON":
                                case "GROUP_OFF":
                                    {
                                        int g_id = 0;
                                        int.TryParse(this.Argument, out g_id);
                                        Group g = context.Groups.FirstOrDefault(gr => gr.GroupId == g_id);

                                        if (g != null)
                                            return g.Name;
                                        break;
                                    }
                                default:
                                    return this.Argument;
                            }
                        }
                    }
                    else if (this.Command is DeviceCommand)
                    {
                        DeviceCommand bc = (DeviceCommand)this.Command;
                        if (bc != null)
                        {
                            switch (bc.ArgumentType)
                            {
                                case DataType.NONE:
                                    return bc.Name;
                                case DataType.SHORT:
                                case DataType.STRING:
                                case DataType.LIST:
                                case DataType.INTEGER:
                                case DataType.DECIMAL:
                                case DataType.BYTE:
                                case DataType.BOOL:
                                    return string.Format("{0} to '{1}'", bc.Name, this.Argument);
                            }
                        }

                    }
                    else if (this.Command is DeviceTypeCommand)
                    {
                        DeviceTypeCommand bc = (DeviceTypeCommand)this.Command;
                        if (bc != null)
                        {
                            switch (bc.ArgumentType)
                            {
                                case DataType.NONE:
                                    return bc.Name;
                                case DataType.SHORT:
                                case DataType.STRING:
                                case DataType.LIST:
                                case DataType.INTEGER:
                                case DataType.DECIMAL:
                                case DataType.BYTE:
                                case DataType.BOOL:
                                    return string.Format("{0} to '{1}'", bc.Name, this.Argument);
                            }
                        }
                    }
                    return string.Empty;
                }
            }
        }

    }
}
