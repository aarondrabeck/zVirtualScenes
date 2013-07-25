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
                    //NotifyPropertyChanged("ActionDescription");
                    //NotifyPropertyChanged("ActionableObject");
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

        private string _TargetObjectName;
        public string TargetObjectName
        {
            get { return _TargetObjectName; }
            set
            {
                if (value != _TargetObjectName)
                {
                    _TargetObjectName = value;
                    NotifyPropertyChanged();
                }
            }
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

    public static class StoredCommandExtensionMethods
    {
        /// <summary>
        /// Helper to find where the stored command is used and remove its use.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storedCommand"></param>
        public static async Task<Result> TryRemoveDependenciesAsync(this StoredCommand storedCommand, zvsContext context)
        {
            if (storedCommand.ScheduledTask != null)
            {
                storedCommand.ScheduledTask.isEnabled = false;
                storedCommand.ScheduledTask = null;
            }

            if (storedCommand.DeviceValueTrigger != null)
            {
                storedCommand.DeviceValueTrigger.isEnabled = false;
                storedCommand.DeviceValueTrigger = null;
            }

            // if (sc.SceneCommand != null)
            //     context.SceneCommands.Local.Remove(sc.SceneCommand);

            context.SceneCommands.RemoveRange(await context.SceneCommands.Where(o => o.StoredCommand.Id == storedCommand.Id).ToListAsync());

            return await context.TrySaveChangesAsync();
        }

        public static async Task SetDescriptionAsync(this StoredCommand storedCommand, zvsContext context)
        {
            if (storedCommand.Command is BuiltinCommand)
            {
                #region Built-in Command
                BuiltinCommand bc = storedCommand.Command as BuiltinCommand;

                switch (bc.UniqueIdentifier)
                {
                    case "REPOLL_ME":
                        {
                            int d_id = 0;
                            int.TryParse(storedCommand.Argument, out d_id);

                            Device device_to_repoll = await context.Devices.FirstOrDefaultAsync(d => d.Id == d_id);
                            if (device_to_repoll != null)
                                storedCommand.Description = device_to_repoll.Name;

                            break;
                        }
                    case "GROUP_ON":
                    case "GROUP_OFF":
                        {
                            int g_id = 0;
                            int.TryParse(storedCommand.Argument, out g_id);
                            Group g = await context.Groups.FirstOrDefaultAsync(gr => gr.Id == g_id);
                            if (g != null)
                                storedCommand.Description = g.Name;

                            break;
                        }
                    case "RUN_SCENE":
                        {
                            int SceneId = 0;
                            int.TryParse(storedCommand.Argument, out SceneId);

                            Scene Scene = await context.Scenes.FirstOrDefaultAsync(d => d.Id == SceneId);
                            if (Scene != null)
                                storedCommand.Description = Scene.Name;
                            break;
                        }
                    default:
                        storedCommand.Description = storedCommand.Argument;
                        break;
                }
                #endregion
            }
            else if (storedCommand.Command is DeviceCommand)
            {
                #region DeviceCommand
                DeviceCommand bc = storedCommand.Command as DeviceCommand;
                switch (bc.ArgumentType)
                {
                    case DataType.NONE:
                        storedCommand.Description = bc.Name;
                        break;
                    default:
                        storedCommand.Description = string.Format("{0} to '{1}'", bc.Name, storedCommand.Argument);
                        break;
                }
                #endregion
            }
            else if (storedCommand.Command is DeviceTypeCommand)
            {
                #region DeviceTypeCommand
                DeviceTypeCommand bc = storedCommand.Command as DeviceTypeCommand;
                switch (bc.ArgumentType)
                {
                    case DataType.NONE:
                        storedCommand.Description = bc.Name;
                        break;
                    default:
                        storedCommand.Description = string.Format("{0} to '{1}'", bc.Name, storedCommand.Argument);
                        break;
                }
                #endregion
            }
            else if (storedCommand.Command is JavaScriptCommand)
            {
                JavaScriptCommand JSCmd = storedCommand.Command as JavaScriptCommand;
                storedCommand.Description = string.Format("{0} '{1}'", JSCmd.Name, JSCmd.Script);
            }
        }

        public static async Task SetTargetObjectNameAsync(this StoredCommand storedCommand, zvsContext context)
        {
            storedCommand.TargetObjectName = string.Empty;

            if (storedCommand.Command is BuiltinCommand)
            {
                storedCommand.TargetObjectName = storedCommand.Command.Name;
            }
            else if (storedCommand.Command is DeviceCommand)
            {
                var dc = storedCommand.Command as DeviceCommand;
                var device = await context.Devices.FirstOrDefaultAsync(o => o.Commands.Any(p => p.Id == storedCommand.Command.Id));
                storedCommand.TargetObjectName = device.Name;
            }
            else if (storedCommand.Command is DeviceTypeCommand)
            {
                int d_id = int.TryParse(storedCommand.Argument2, out d_id) ? d_id : 0;
                Device d = await context.Devices.FirstOrDefaultAsync(o => o.Id == d_id);

                if (d != null)
                    storedCommand.TargetObjectName = d.Name;
                else
                    storedCommand.TargetObjectName = "Unknown Device";
            }
            else if (storedCommand.Command is JavaScriptCommand)
            {
                storedCommand.TargetObjectName = "Execute JavaScript";
            }
        }
    }
}
