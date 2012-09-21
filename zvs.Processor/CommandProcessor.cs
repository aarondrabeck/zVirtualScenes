using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace zvs.Processor
{
    public class CommandProcessor
    {
        private Core Core;

        //Constructor
        public CommandProcessor(Core core)
        {
            if (core == null)
                throw new ArgumentException("Core");

            this.Core = core;
        }

        //Events
        public delegate void onProcessingCommandEventHandler(object sender, onProcessingCommandEventArgs args);
        public class onProcessingCommandEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int QueueCommandId { get; private set; }

            public onProcessingCommandEventArgs(bool Errors, string Details, int CommandQueueID)
            {
                this.QueueCommandId = CommandQueueID;
                this.Errors = Errors;
                this.Details = Details;
            }
        }
        public event onProcessingCommandEventHandler onProcessingCommandBegin;
        public event onProcessingCommandEventHandler onProcessingCommandEnd;

        private void ProcessingCommandBegin(onProcessingCommandEventArgs args)
        {
            string msg = string.Format("{0}, QueueCommandId:{1}", args.Details, args.QueueCommandId);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

            if (onProcessingCommandBegin != null)
                onProcessingCommandBegin(this, args);
        }

        private void ProcessingCommandEnd(onProcessingCommandEventArgs args)
        {
            string msg = string.Format("{0}, QueueCommandId:{1}", args.Details, args.QueueCommandId);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

            if (onProcessingCommandEnd != null)
                onProcessingCommandEnd(this, args);
        }



        //public Methods 
        public void RunDeviceCommand(zvsContext context, DeviceCommand deviceCommand, string argument = "")
        {
            QueuedDeviceCommand qdc = new QueuedDeviceCommand
            {
                Command = deviceCommand,
                Argument = argument,
                Device = deviceCommand.Device
            };
            context.QueuedCommands.Add(qdc);
            context.SaveChanges();
            ProcessCommand(qdc);
        }

        public void RunDeviceTypeCommand(zvsContext context, DeviceTypeCommand deviceTypeCommand, Device device, string argument = "")
        {
            QueuedDeviceTypeCommand qdtc = new QueuedDeviceTypeCommand
            {
                Command = deviceTypeCommand,
                Argument = argument,
                Device = device
            };
            context.QueuedCommands.Add(qdtc);
            context.SaveChanges();
            ProcessCommand(qdtc);
        }

        public void RunBuiltinCommand(zvsContext context, BuiltinCommand builtinCommand, string argument = "")
        {
            QueuedBuiltinCommand qbc = new QueuedBuiltinCommand
            {
                Command = builtinCommand,
                Argument = argument,
            };
            context.QueuedCommands.Add(qbc);
            context.SaveChanges();
            ProcessCommand(qbc);
        }

        public void RunJavaScriptCommand(zvsContext context, JavaScriptCommand javaScriptCommand, string argument = "")
        {
            QueuedJavaScriptCommand qjsc = new QueuedJavaScriptCommand
            {
                Command = javaScriptCommand,
                Argument = argument,
            };
            context.QueuedCommands.Add(qjsc);
            context.SaveChanges();
            ProcessCommand(qjsc);
        }

        //private Methods
        private void ProcessCommand(QueuedCommand queuedCommand)
        {
            if (queuedCommand == null)
                throw new ArgumentException("QueuedCommand");

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                zvsContext context = new zvsContext();
                if (queuedCommand is QueuedDeviceCommand)
                {
                    QueuedDeviceCommand cmd = context.QueuedCommands.OfType<QueuedDeviceCommand>().FirstOrDefault(o => o.QueuedCommandId == queuedCommand.QueuedCommandId);
                    ProcessDeviceCmd(context, cmd);
                }
                else if (queuedCommand is QueuedDeviceTypeCommand)
                {
                    QueuedDeviceTypeCommand cmd = context.QueuedCommands.OfType<QueuedDeviceTypeCommand>().FirstOrDefault(o => o.QueuedCommandId == queuedCommand.QueuedCommandId);
                    ProcessDeviceTypeCmd(context, cmd);
                }
                else if (queuedCommand is QueuedBuiltinCommand)
                {
                    QueuedBuiltinCommand cmd = context.QueuedCommands.OfType<QueuedBuiltinCommand>().FirstOrDefault(o => o.QueuedCommandId == queuedCommand.QueuedCommandId);
                    ProcessBuiltinCmd(context, cmd);
                }
                else if (queuedCommand is QueuedJavaScriptCommand)
                {
                    QueuedJavaScriptCommand cmd = context.QueuedCommands.OfType<QueuedJavaScriptCommand>().FirstOrDefault(o => o.QueuedCommandId == queuedCommand.QueuedCommandId);
                    ProcessJavaScriptCmd(context, cmd);
                }
            };
            bw.RunWorkerAsync();
        }

        private void ProcessDeviceCmd(zvsContext context, QueuedDeviceCommand cmd)
        {
            if (cmd == null && cmd.Device == null)
            {
                ProcessingCommandBegin(new onProcessingCommandEventArgs(false, string.Format("Processing device command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));

                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process device command id {0}. Could not locate device in the database.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.Dispose();
                return;
            }

            string StartDetails = string.Format("Processing queued device command #{0} ({1} with arg '{2}' on {3})",
                                                     cmd.QueuedCommandId,
                                                     cmd.Command.Name,
                                                     cmd.Argument,
                                                     cmd.Device.Name);

            //We found the command so continue
            ProcessingCommandBegin(new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
            if (p == null)
            {
                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process device command id {0}. Could not locate queued commands plug-in.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();

                return;
            }

            if (p.Enabled && p.IsReady)
            {
                string details = string.Format("Finished processing command #{0} ({1} with arg '{2}' on {3} to plug-in '{4}')",
                                                      cmd.QueuedCommandId,
                                                      cmd.Command.Name,
                                                      cmd.Argument,
                                                      cmd.Device.Name,
                                                      p.Name);

                ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                p.ProcessDeviceCommand(cmd);
                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;

            }
            else
            {
                string err_str = string.Format("Failed to process command #{0} '{1}' on '{2}' because the '{3}' plug-in is {4}. Removing command from queue...",
                    cmd.QueuedCommandId,
                    cmd.Command.Name,
                    cmd.Device.Name,
                    cmd.Device.Type.Plugin.Name,
                    p.Enabled ? "not ready" : "disabled");

                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, err_str, cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;
            }
        }

        private void ProcessDeviceTypeCmd(zvsContext context, QueuedDeviceTypeCommand cmd)
        {
            if (cmd == null || cmd.Device == null)
            {
                ProcessingCommandBegin(new onProcessingCommandEventArgs(false, string.Format("Processing device type command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));

                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process device type command id {0}. Could not locate queued command in database.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.Dispose();

                return;
            }

            string StartDetails = string.Format("Processing queued device type command #{0} ({1} with arg '{2}' on {3})",
                                                     cmd.QueuedCommandId,
                                                     cmd.Command.Name,
                                                     cmd.Argument,
                                                     cmd.Device.Name);

            //We found the command so continue
            ProcessingCommandBegin(new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
            if (p == null)
            {
                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process device type command id {0}. Could not locate queued commands plug-in.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();

                return;
            }


            if (p.Enabled && p.IsReady)
            {
                string details = string.Format("Finished processing command #{0} ({1} with arg '{2}' on {3} to plug-in '{4}')",
                                                    cmd.QueuedCommandId,
                                                    cmd.Command.Name,
                                                     cmd.Argument,
                                                    cmd.Device.Name,
                                                    p.Name);

                ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                p.ProcessDeviceTypeCommand(cmd);
                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;

            }
            else
            {
                string err_str = string.Format("Failed to process command #{0} '{1}' on '{2}' because the '{3}' plug-in is {4}. Removing command from queue...",
                cmd.QueuedCommandId,
                cmd.Command.Name,
                cmd.Device.Name,
                cmd.Device.Type.Plugin.Name,
                p.Enabled ? "not ready" : "disabled"
                );

                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, err_str, cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;
            }

        }

        private void ProcessBuiltinCmd(zvsContext context, QueuedBuiltinCommand cmd)
        {
            if (cmd == null)
            {
                ProcessingCommandBegin(new onProcessingCommandEventArgs(false, string.Format("Processing built-in command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));
                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process built-in command id {0}. Could not locate queued command in database.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.Dispose();
                return;

            }

            string StartDetails = string.Format("Processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

            //We found the command so continue
            ProcessingCommandBegin(new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            switch (cmd.Command.UniqueIdentifier)
            {
                case "TIMEDELAY":
                    {
                        int delay = 0;
                        if (int.TryParse(cmd.Argument, out delay) && delay > 0)
                        {
                            System.Timers.Timer timer = new System.Timers.Timer();
                            timer.Elapsed += (sender, args) =>
                            {
                                timer.Stop();

                                string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                                ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();

                            };
                            timer.Interval = delay * 1000;
                            timer.Start();
                        }
                        break;
                    }
                case "REPOLL_ME":
                    {
                        int d_id = 0;
                        int.TryParse(cmd.Argument, out d_id);
                        Device d = Device.GetAllDevices(context, false).FirstOrDefault(o => o.DeviceId == d_id);

                        if (d.Type.Plugin.isEnabled)
                            Core.pluginManager.GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "REPOLL_ALL":
                    {
                        foreach (Device d in Device.GetAllDevices(context, false))
                        {
                            if (d.Type.Plugin.isEnabled)
                            {
                                Core.pluginManager.GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1}))",
                                                          cmd.QueuedCommandId,
                                                         cmd.Command.Name);

                        ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "GROUP_ON":
                    {
                        int g_id = 0;
                        int.TryParse(cmd.Argument, out g_id);
                        //EXECUTE ON ALL API's
                        if (g_id > 0)
                        {
                            foreach (zvsPlugin p in Core.pluginManager.GetPlugins())
                            {
                                if (p.Enabled)
                                    p.ActivateGroup(g_id);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "GROUP_OFF":
                    {
                        int g_id = 0;
                        int.TryParse(cmd.Argument, out g_id);
                        //EXECUTE ON ALL API's
                        if (g_id > 0)
                        {
                            foreach (zvsPlugin p in Core.pluginManager.GetPlugins())
                            {
                                if (p.Enabled)
                                    p.DeactivateGroup(g_id);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                       cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        break;
                    }
                case "RUN_SCENE":
                    {
                        int id = 0;
                        //EXECUTE ON ALL API's
                        if (int.TryParse(cmd.Argument, out id) && id > 0)
                        {
                            SceneRunner sr = new SceneRunner(Core);
                            sr.onRunEnd += (s, a) =>
                            {
                                string details = string.Format("Finished processing queued built-in command #{0} ({1} '{2}')",
                                                       cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                                ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            };
                            sr.RunScene(id);
                        }
                        break;
                    }
                default:
                    {
                        string details = string.Format("Error processing command '{0}':'{1}'. Command {2} not recognized.",
                                                         cmd.Command.Name,
                                                         cmd.Argument,
                                                         cmd.Command.UniqueIdentifier);

                        ProcessingCommandEnd(new onProcessingCommandEventArgs(true, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        break;
                    }
            }
        }

        private void ProcessJavaScriptCmd(zvsContext context, QueuedJavaScriptCommand cmd)
        {
            if (cmd == null)
            {
                ProcessingCommandBegin(new onProcessingCommandEventArgs(false, string.Format("Processing device type command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));
                ProcessingCommandEnd(new onProcessingCommandEventArgs(true, string.Format("Failed to process device type command id {0}. Could not locate queued command in database.", cmd.QueuedCommandId), cmd.QueuedCommandId));
                context.Dispose();
                return;
            }

            string StartDetails = string.Format("Processing queued JavaScript command #{0} ({1} with arg '{2}' )",
                                                     cmd.QueuedCommandId,
                                                     cmd.Command.Name,
                                                     cmd.Argument);

            //We found the command so continue
            ProcessingCommandBegin(new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            JavaScriptCommand js = (JavaScriptCommand)cmd.Command;
            JavaScriptExecuter je = new JavaScriptExecuter(Core);

            je.onExecuteScriptEnd += (s, a) =>
            {
                if (a.Errors)
                {
                    string details = string.Format("Finished processing JavaScript command #{0} ({1} with arg '{2}')",
                                                       cmd.QueuedCommandId,
                                                       cmd.Command.Name,
                                                        cmd.Argument);

                    ProcessingCommandEnd(new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                    context.QueuedCommands.Remove(cmd);
                    context.SaveChanges();
                    context.Dispose();
                    return;
                }
            };
            je.ExecuteScript(js.Script, context);
        }
    }
}
