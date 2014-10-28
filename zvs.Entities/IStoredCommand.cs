using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;

namespace zvs.DataModel
{
    public interface IStoredCommand
    {
        int? CommandId { get; set; }
        Command Command { get; set; }

        [StringLength(512)]
        string Argument { get; set; }

        [StringLength(512)]
        string Argument2 { get; set; }

        string TargetObjectName { get; set; }

        string Description { get; set; }
    }

    public static class StoredCommandExtensionMethods
    {
        public static void SetDescription(this IStoredCommand storedCommand, ZvsContext context)
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

        public static async Task SetTargetObjectNameAsync(this IStoredCommand storedCommand, ZvsContext context)
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
