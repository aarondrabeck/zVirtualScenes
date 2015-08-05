using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        public static void SetDescription(this IStoredCommand storedCommand)
        {
            if (storedCommand.Command is BuiltinCommand)
            {
                // Built-in Command
                var bc = storedCommand.Command as BuiltinCommand;
                storedCommand.Description = bc.Name;
            }
            else if (storedCommand.Command is DeviceCommand)
            {
                //DeviceCommand
                var bc = storedCommand.Command as DeviceCommand;
                switch (bc.ArgumentType)
                {
                    case DataType.NONE:
                        storedCommand.Description = bc.Name;
                        break;
                    default:
                        storedCommand.Description = $"{bc.Name} to {storedCommand.Argument} on";
                        break;
                }

            }
            else if (storedCommand.Command is DeviceTypeCommand)
            {
                //DeviceTypeCommand
                var bc = storedCommand.Command as DeviceTypeCommand;
                switch (bc.ArgumentType)
                {
                    case DataType.NONE:
                        storedCommand.Description = bc.Name;
                        break;
                    default:
                        storedCommand.Description = $"{bc.Name} to {storedCommand.Argument} on";
                        break;
                }
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
                var bc = storedCommand.Command as BuiltinCommand;

                switch (bc.UniqueIdentifier)
                {
                    case "REPOLL_ME":
                        {
                            var dId = 0;
                            int.TryParse(storedCommand.Argument, out dId);

                            var deviceToRepoll = await context.Devices.FirstOrDefaultAsync(d => d.Id == dId);
                            if (deviceToRepoll != null)
                                storedCommand.TargetObjectName = $"{deviceToRepoll.Location} {deviceToRepoll.Name}";

                            break;
                        }
                    case "GROUP_ON":
                    case "GROUP_OFF":
                        {
                            int gId;
                            int.TryParse(storedCommand.Argument, out gId);
                            var g = await context.Groups.FirstOrDefaultAsync(gr => gr.Id == gId);
                            if (g != null)
                                storedCommand.TargetObjectName = g.Name;

                            break;
                        }
                    case "RUN_SCENE":
                        {
                            int sceneId;
                            int.TryParse(storedCommand.Argument, out sceneId);

                            var scene = await context.Scenes.FirstOrDefaultAsync(d => d.Id == sceneId);
                            if (scene != null)
                                storedCommand.TargetObjectName = scene.Name;
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
                var device = await context.Devices.FirstOrDefaultAsync(o => o.Commands.Any(p => p.Id == storedCommand.Command.Id));
                storedCommand.TargetObjectName = $"{device.Location} {device.Name}";
            }
            else if (storedCommand.Command is DeviceTypeCommand)
            {
                int dId = int.TryParse(storedCommand.Argument2, out dId) ? dId : 0;
                var d = await context.Devices.FirstOrDefaultAsync(o => o.Id == dId);

                storedCommand.TargetObjectName = d != null ? $"{d.Location} {d.Name}" : "Unknown Device";
            }
            else if (storedCommand.Command is JavaScriptCommand)
            {
                var jsCmd = storedCommand.Command as JavaScriptCommand;
                storedCommand.TargetObjectName = jsCmd.Name;
            }

            sw.Stop();
            Debug.WriteLine("SetTargetObjectNameAsync took " + sw.Elapsed.ToString());
        }
    }
}
