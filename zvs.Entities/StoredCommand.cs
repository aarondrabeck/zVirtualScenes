using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;

namespace zvs.DataModel
{
    [Table("StoredCommands", Schema = "ZVS")]
    public class StoredCommand : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual DeviceValueTrigger DeviceValueTrigger { get; set; }

        public virtual SceneCommand SceneCommand { get; set; }

        public virtual CommandScheduledTask CommandScheduledTask { get; set; }

        public int CommandId { get; set; }
        private Command _command;
        public virtual Command Command
        {
            get
            {
                return _command;
            }
            set
            {
                if (value != _command)
                {
                    _command = value;
                    //NotifyPropertyChanged("ActionDescription");
                    //NotifyPropertyChanged("ActionableObject");
                    NotifyPropertyChanged("Command");
                    NotifyPropertyChanged("Summary");
                }
            }
        }

        private string _argument;
        [StringLength(512)]
        public string Argument
        {
            get
            {
                return _argument;
            }
            set
            {
                if (value != _argument)
                {
                    _argument = value;
                    NotifyPropertyChanged("Argument");
                    NotifyPropertyChanged("ActionDescription");
                    NotifyPropertyChanged("Summary");

                }
            }
        }

        private string _argument2;
        [StringLength(512)]
        public string Argument2
        {
            get
            {
                return _argument2;
            }
            set
            {
                if (value != _argument2)
                {
                    _argument2 = value;
                    NotifyPropertyChanged("Argument2");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _targetObjectName;
        public string TargetObjectName
        {
            get { return _targetObjectName; }
            set
            {
                if (value == _targetObjectName) return;
                _targetObjectName = value;
                NotifyPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyPropertyChanged();
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
        public static async Task<Result> TryRemoveDependenciesAsync(this StoredCommand storedCommand, ZvsContext context, CancellationToken cancellationToken)
        {
            if (storedCommand.CommandScheduledTask != null)
            {
                storedCommand.CommandScheduledTask.IsEnabled = false;
                storedCommand.CommandScheduledTask = null;
            }

            if (storedCommand.DeviceValueTrigger != null)
            {
                storedCommand.DeviceValueTrigger.isEnabled = false;
                storedCommand.DeviceValueTrigger = null;
            }

            // if (sc.SceneCommand != null)
            //     context.SceneCommands.Local.Remove(sc.SceneCommand);

            context.SceneCommands.RemoveRange(await context.SceneCommands.Where(o => o.StoredCommand.Id == storedCommand.Id).ToListAsync());
            return await context.TrySaveChangesAsync(cancellationToken);
        }

        public static void SetDescription(this StoredCommand storedCommand, ZvsContext context)
        {
            if (storedCommand.Command is BuiltinCommand)
            {
                #region Built-in Command
                BuiltinCommand bc = storedCommand.Command as BuiltinCommand;
                storedCommand.Description = bc.Name;
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
                        storedCommand.Description = string.Format("{0} to {1} on", bc.Name, storedCommand.Argument);
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
                        storedCommand.Description = string.Format("{0} to {1} on", bc.Name, storedCommand.Argument);
                        break;
                }
                #endregion
            }
            else if (storedCommand.Command is JavaScriptCommand)
            {
                storedCommand.Description = "Execute JavaScript";
            }
        }

        public static async Task SetTargetObjectNameAsync(this StoredCommand storedCommand, ZvsContext context)
        {
            var sw = new Stopwatch();
            sw.Start();

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
                                storedCommand.TargetObjectName = device_to_repoll.Name;

                            break;
                        }
                    case "GROUP_ON":
                    case "GROUP_OFF":
                        {
                            int g_id = 0;
                            int.TryParse(storedCommand.Argument, out g_id);
                            Group g = await context.Groups.FirstOrDefaultAsync(gr => gr.Id == g_id);
                            if (g != null)
                                storedCommand.TargetObjectName = g.Name;

                            break;
                        }
                    case "RUN_SCENE":
                        {
                            int SceneId = 0;
                            int.TryParse(storedCommand.Argument, out SceneId);

                            Scene Scene = await context.Scenes.FirstOrDefaultAsync(d => d.Id == SceneId);
                            if (Scene != null)
                                storedCommand.TargetObjectName = Scene.Name;
                            break;
                        }
                    default:
                        storedCommand.TargetObjectName = storedCommand.Argument;
                        break;
                }
                #endregion
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
                JavaScriptCommand JSCmd = storedCommand.Command as JavaScriptCommand;
                storedCommand.TargetObjectName = JSCmd.Name;
            }

            sw.Stop();
            Debug.WriteLine("SetTargetObjectNameAsync took " + sw.Elapsed.ToString());
        }
    }
}
