using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using zVirtualScenesModel;

namespace zVirtualScenes
{
    public class SceneRunner : IDisposable
    {
        private zvsLocalDBEntities context;
        private scene _scene;
        private int ExecutionErrors = 0;
        private int ExecutedCommands = 0;
        private List<scene_commands> CommandsToExecute = new List<scene_commands>();
        private BackgroundWorker worker = new BackgroundWorker();
        private bool hasExecuted = false;

        private Guid _SceneRunnerGUID = Guid.NewGuid();
        public Guid SceneRunnerGUID
        {
            get
            {
                return _SceneRunnerGUID;
            }
        }

        #region Events
        public delegate void onSceneRunEventHandler(object sender, onSceneRunEventArgs args);
        public class onSceneRunEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int SceneID { get; private set; }
            public Guid SceneRunnerGUID { get; private set; }

            public onSceneRunEventArgs(int SceneID, bool Errors, string Details, Guid SceneRunnerGUID)
            {
                this.SceneID = SceneID;
                this.Errors = Errors;
                this.Details = Details;
                this.SceneRunnerGUID = SceneRunnerGUID;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunBegin;

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunComplete;
        #endregion

        public SceneRunner()
        {
            context = new zvsLocalDBEntities();
        }

        //Methods
        public void RunScene(int SceneID)
        {
            if (hasExecuted)
                throw new Exception("Scene runners cannot be reused.");

            hasExecuted = true;
            worker.DoWork += (sender, args) =>
            {
                _scene = context.scenes.FirstOrDefault(o => o.id == SceneID);

                if (_scene == null)
                {
                    if (onSceneRunBegin != null)
                        onSceneRunBegin(this, new onSceneRunEventArgs(SceneID, false, "Scene '" + SceneID + "' started.", SceneRunnerGUID));

                    if (onSceneRunComplete != null)
                        onSceneRunComplete(this, new onSceneRunEventArgs(SceneID, true, "Failed to run scene '" + SceneID + "' because it was not found in the database!", SceneRunnerGUID));
                }


                if (onSceneRunBegin != null)
                    onSceneRunBegin(this, new onSceneRunEventArgs(_scene.id, false, "Scene '" + _scene.friendly_name + "' started.", SceneRunnerGUID));


                if (_scene.is_running)
                {
                    if (onSceneRunComplete != null)
                        onSceneRunComplete(this, new onSceneRunEventArgs(_scene.id, true, "Failed to run scene '" + _scene.friendly_name + "' because it is already running!", SceneRunnerGUID));
                }
                else
                {
                    if (_scene.scene_commands.Count < 1)
                    {
                        if (onSceneRunComplete != null)
                            onSceneRunComplete(this, new onSceneRunEventArgs(_scene.id, true, "Failed to run scene '" + _scene.friendly_name + "' because it has no commands!", SceneRunnerGUID));

                        return;
                    }

                    _scene.is_running = true;
                    context.SaveChanges();

                    ExecutionErrors = 0;
                    ExecutedCommands = 0;
                    CommandsToExecute = _scene.scene_commands.OrderBy(o => o.sort_order).ToList();

                    ProcessNextCommand();
                }
            };
            worker.RunWorkerAsync();
        }

        private void ProcessNextCommand()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                scene_commands sceneCommand = CommandsToExecute[ExecutedCommands];
                switch ((Command_Types)sceneCommand.command_type_id)
                {
                    #region Process Commands
                    case Command_Types.builtin:
                        {
                            builtin_commands cmd = context.builtin_commands.FirstOrDefault(c => c.id == sceneCommand.command_id);
                            if (cmd != null)
                            {
                                //Add the command to the queue    
                                builtin_command_que b_cmd = new builtin_command_que()
                                {
                                    arg = sceneCommand.arg,
                                    builtin_command_id = sceneCommand.command_id
                                };
                                context.builtin_command_que.Add(b_cmd);
                                context.SaveChanges();

                                PluginManager.onProcessingCommandEventHandler handler = null;
                                handler = (sender, args) =>
                                {
                                    if (args.CommandType == scene_commands.command_types.builtin && args.CommandQueueID == b_cmd.id)
                                    {
                                        PluginManager.onProcessingCommandEnd -= handler;

                                        if (args.hasErrors)
                                            ExecutionErrors++;

                                        ExecutedCommands++;

                                        //on to the next one
                                        ProcessNextCommand();
                                    }
                                };
                                PluginManager.onProcessingCommandEnd += handler;

                                builtin_command_que.BuiltinCommandAddedToQue(b_cmd.id);
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


                            PluginManager.onProcessingCommandEventHandler handler = null;
                            handler = (sender, args) =>
                            {
                                if (args.CommandType == scene_commands.command_types.device_command && args.CommandQueueID == d_cmd.id)
                                {
                                    PluginManager.onProcessingCommandEnd -= handler;

                                    if (args.hasErrors)
                                        ExecutionErrors++;

                                    ExecutedCommands++;

                                    //on to the next one
                                    ProcessNextCommand();
                                }
                            };
                            PluginManager.onProcessingCommandEnd += handler;

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

                            PluginManager.onProcessingCommandEventHandler handler = null;
                            handler = (sender, args) =>
                            {
                                if (args.CommandType == scene_commands.command_types.device_type_command && args.CommandQueueID == dt_cmd.id)
                                {
                                    PluginManager.onProcessingCommandEnd -= handler;

                                    if (args.hasErrors)
                                        ExecutionErrors++;

                                    ExecutedCommands++;

                                    //on to the next one
                                    ProcessNextCommand();
                                }
                            };

                            PluginManager.onProcessingCommandEnd += handler;
                            device_type_command_que.DeviceTypeCommandAddedToQue(dt_cmd.id);
                            break;
                        }
                    #endregion
                }
            }
            else
            {
                _scene.is_running = false;
                context.SaveChanges();

                if (onSceneRunComplete != null)
                    onSceneRunComplete(this, new onSceneRunEventArgs(_scene.id, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.friendly_name, ExecutionErrors), SceneRunnerGUID));

            }
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
