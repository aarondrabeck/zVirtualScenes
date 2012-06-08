using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace zVirtualScenesModel
{
    public partial class scene
    {
        private zvsLocalDBEntities context;

        //Events
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event SceneRunStartedEventHandler SceneRunStartedEvent;
        public delegate void SceneRunStartedEventHandler(scene s, string result);

        public static void SceneRunStarted(scene s, string result)
        {
            if (SceneRunStartedEvent != null)
                SceneRunStartedEvent(s, result);
        }

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public static event SceneRunCompleteEventHandler SceneRunCompleteEvent;
        public delegate void SceneRunCompleteEventHandler(int scene_id, int ErrorCount);

        public static void SceneRunComplete(int scene_id, int ErrorCount)
        {
            if (SceneRunCompleteEvent != null)
                SceneRunCompleteEvent(scene_id, ErrorCount);
        }

        public delegate void SceneModifiedEventHandler(object sender, int? SceneID);
        public static event SceneModifiedEventHandler SceneModified;
        public static void CallSceneModified(object sender, int? SceneID)
        {
            if (SceneModified != null)
                SceneModified(sender, SceneID);
        }

        int ExecutionErrors = 0;
        int ExecutedCommands = 0;
        List<scene_commands> CommandsToExecute = new List<scene_commands>();

        //Methods
        public string RunScene()
        {
            if (this.is_running)
                return "Failed to start scene '" + this.friendly_name + "' because it is already running!";
            else
            {
                context = new zvsLocalDBEntities();

                if (this.scene_commands.Count < 1)
                    return "Failed to start scene '" + this.friendly_name + "' because it has no commands!";

                this.is_running = true;
                context.SaveChanges();

                string result = "Scene '" + this.friendly_name + "' started.";
                SceneRunStarted(this, result);

                ExecutionErrors = 0;
                ExecutedCommands = 0;
                CommandsToExecute = this.scene_commands.OrderBy(o => o.sort_order).ToList();

                ProcessNextCommand();

                return result;
            }
        }

        private void ProcessNextCommand()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                scene_commands sceneCommand = CommandsToExecute[ExecutedCommands];
                switch ((Command_Types)sceneCommand.command_type_id)
                {
                    case Command_Types.builtin:
                        {
                            builtin_commands cmd = context.builtin_commands.FirstOrDefault(c => c.id == sceneCommand.command_id);
                            if (cmd != null)
                            {
                                if (cmd.name.Equals("TIMEDELAY"))
                                {
                                    int delay = 0;
                                    if (int.TryParse(sceneCommand.arg, out delay) && delay > 0)
                                    {
                                        Timer delayTimer = new Timer((state) =>
                                        {
                                            ExecutedCommands++;

                                            //on to the next one
                                            ProcessNextCommand();

                                        }, null, delay * 1000, Timeout.Infinite);
                                    }
                                }
                                else
                                {
                                    //Add the command to the queue    
                                    builtin_command_que b_cmd = new builtin_command_que()
                                    {
                                        arg = sceneCommand.arg,
                                        builtin_command_id = sceneCommand.command_id
                                    };
                                    context.builtin_command_que.Add(b_cmd);
                                    context.SaveChanges();


                                    builtin_command_que.BuiltinCommandRunCompleteEventHandler handler = null;
                                    handler = (command, withErrors, txtError) =>
                                    {
                                        if (command.id == b_cmd.id)
                                        {
                                            builtin_command_que.BuiltinCommandRunCompleteEvent -= handler;

                                            if (withErrors)
                                                ExecutionErrors++;

                                            ExecutedCommands++;

                                            //on to the next one
                                            ProcessNextCommand();
                                        }
                                    };

                                    builtin_command_que.BuiltinCommandRunCompleteEvent += handler;
                                    builtin_command_que.BuiltinCommandAddedToQue(b_cmd.id);
                                }
                            }
                            break;
                        }
                    case Command_Types.device_command:
                        {
                            //Add the command to the queue    
                            device_command_que d_cmd = new device_command_que()
                            {
                                device_id = sceneCommand.device_id.Value,
                                device_command_id = sceneCommand.command_id,
                                arg = sceneCommand.arg
                            };
                            context.device_command_que.Add(d_cmd);
                            context.SaveChanges();


                            device_command_que.DeviceCommandRunCompleteEventHandler handler = null;
                            handler = (command, withErrors, txtError) =>
                            {
                                if (command.id == d_cmd.id)
                                {
                                    device_command_que.DeviceCommandRunCompleteEvent -= handler;

                                    if (withErrors)
                                        ExecutionErrors++;

                                    ExecutedCommands++;

                                    //on to the next one
                                    ProcessNextCommand();
                                }
                            };

                            device_command_que.DeviceCommandRunCompleteEvent += handler;
                            device_command_que.DeviceCommandAddedToQue(d_cmd.id);

                            break;
                        }
                    case Command_Types.device_type_command:
                        {
                            //Add the command to the queue    
                            device_type_command_que dt_cmd = new device_type_command_que()
                            {
                                device_type_command_id = sceneCommand.command_id,
                                device_id = sceneCommand.device_id.Value,
                                arg = sceneCommand.arg
                            };
                            context.device_type_command_que.Add(dt_cmd);
                            context.SaveChanges();



                            device_type_command_que.DeviceTypeCommandRunCompleteEventHandler handler = null;
                            handler = (command, withErrors, txtError) =>
                            {
                                if (command.id == dt_cmd.id)
                                {
                                    device_type_command_que.DeviceTypeCommandRunCompleteEvent -= handler;

                                    if (withErrors)
                                        ExecutionErrors++;

                                    ExecutedCommands++;

                                    //on to the next one
                                    ProcessNextCommand();
                                }
                            };

                            device_type_command_que.DeviceTypeCommandRunCompleteEvent += handler;
                            device_type_command_que.DeviceTypeCommandAddedToQue(dt_cmd.id);
                            break;
                        }
                }
            }
            else
            {
                this.is_running = false;
                context.SaveChanges();

                SceneRunComplete(this.id, ExecutionErrors);
            }
        }
    }
}
