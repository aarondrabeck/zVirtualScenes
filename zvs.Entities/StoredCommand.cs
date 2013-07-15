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
    [Table("StoredCommands", Schema = "ZVS")]
    public partial class StoredCommand : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //public int? DeviceValueTriggerId { get; set; }
        //not sure how or if you can expose the ID here
        public virtual DeviceValueTrigger DeviceValueTrigger { get; set; }

        public int? SceneCommandId
        {
            get
            {
                if (SceneCommand != null)
                    return SceneCommand.Id;
                else
                    return null;
            }
        }
        public virtual SceneCommand SceneCommand { get; set; }

        //public int? ScheduledTaskId { get; set; }
        public virtual ScheduledTask ScheduledTask { get; set; }

        public int CommandId { get; set; }
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
                    NotifyPropertyChanged("Summary");
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
                    NotifyPropertyChanged("Summary");

                }
            }
        }

        private string _Argument2;
        [StringLength(512)]
        public string Argument2
        {
            get
            {
                return _Argument2;
            }
            set
            {
                if (value != _Argument2)
                {
                    _Argument2 = value;
                    NotifyPropertyChanged("Argument2");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Summary
        {
            get
            {
                return string.Format("{0} '{1}'", this.ActionableObject, this.ActionDescription);
            }
        }

        //TODO: MOVE TO ASYNC METHOD
        public string ActionableObject
        {
            get
            {
                if (this.Command is BuiltinCommand)
                {
                    return this.Command.Name;
                }
                else if (this.Command is DeviceCommand)
                {
                    using (zvsContext context = new zvsContext())
                    {
                        DeviceCommand dc = context.DeviceCommands.Find(this.Command.Id);
                        return dc.Device.Name;
                    }
                }
                else if (this.Command is DeviceTypeCommand)
                {
                    using (zvsContext context = new zvsContext())
                    {
                        int d_id = int.TryParse(this.Argument2, out d_id) ? d_id : 0;
                        Device d = context.Devices.FirstOrDefault(o => o.Id == d_id);

                        if(d!= null)
                            return d.Name;
                        else
                            return "Unknown Device";
                    }
                }
                else if (this.Command is JavaScriptCommand)
                {
                    return "Execute JavaScript";
                }
                return string.Empty;
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

                                        Device device_to_repoll = context.Devices.FirstOrDefault(d => d.Id == d_id);
                                        if (device_to_repoll != null)
                                            return device_to_repoll.Name;

                                        break;
                                    }
                                case "GROUP_ON":
                                case "GROUP_OFF":
                                    {
                                        int g_id = 0;
                                        int.TryParse(this.Argument, out g_id);
                                        Group g = context.Groups.FirstOrDefault(gr => gr.Id == g_id);

                                        if (g != null)
                                            return g.Name;
                                        break;
                                    }
                                case "RUN_SCENE":
                                    {
                                        int SceneId = 0;
                                        int.TryParse(this.Argument, out SceneId);

                                        Scene Scene = context.Scenes.FirstOrDefault(d => d.Id == SceneId);
                                        if (Scene != null)
                                            return Scene.Name;
                                        break;
                                    }
                            }
                        }
                        return this.Argument;
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
                    else if (this.Command is JavaScriptCommand)
                    {
                        JavaScriptCommand JSCmd = (JavaScriptCommand)this.Command;
                        return string.Format("{0} '{1}'", JSCmd.Name, JSCmd.Script);
                    }
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Helper to find where the stored command is used and remove its use.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sc"></param>
        public static async Task<Result> TryRemoveDependenciesAsync(zvsContext context, StoredCommand sc)
        {
            if (sc.ScheduledTask != null)
            {
                sc.ScheduledTask.isEnabled = false;
                sc.ScheduledTask = null;
            }

            if (sc.DeviceValueTrigger != null)
            {
                sc.DeviceValueTrigger.isEnabled = false;
                sc.DeviceValueTrigger = null;
            }

            // if (sc.SceneCommand != null)
            //     context.SceneCommands.Local.Remove(sc.SceneCommand);

            foreach (SceneCommand sceneCommand in context.SceneCommands.Where(o => o.StoredCommand.Id == sc.Id))
            {
                context.SceneCommands.Local.Remove(sceneCommand);
            }

            return await context.TrySaveChangesAsync();
        }
    }
}
