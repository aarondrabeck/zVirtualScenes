using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using System.ComponentModel;

namespace zVirtualScenesCommon.Entity
{
    public partial class scene : EntityObject
    {        

        private BackgroundWorker ExecuteScene = new BackgroundWorker();        

        public string DeviceIcon()
        {
            if (!this.is_running)
                return "Scene";
            else
                return "SceneRun";
        }

        public string RunScene()
        {
            if (this.is_running || ExecuteScene.IsBusy)
                return "Failed to start scene '" + this.friendly_name + "' because it is already running!";
            else
            {
                if (scene_commands.Count < 1)
                    return "Failed to start scene '" + this.friendly_name + "' because it has no commands!";

                ExecuteScene.DoWork += new DoWorkEventHandler(ExecuteScene_DoWork);
                ExecuteScene.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(ExecuteScene_RunWorkerCompleted);

                this.is_running = true;
                zvsEntityControl.zvsContext.SaveChanges();

                ExecuteScene.RunWorkerAsync(this.id);

                string result = "Scene '" + this.friendly_name + "' started."; 
                zvsEntityControl.SceneRunStarted(this, result);
                return result;
            }
        }

        void ExecuteScene_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (zvsEntities2 context = new zvsEntities2())
            {
                scene _scene = context.scenes.SingleOrDefault(s => s.id == (long)e.Result);
                if (_scene != null)
                {
                    _scene.is_running = false;
                    context.SaveChanges();
                }
            }
            zvsEntityControl.SceneRunComplete((long)e.Result, errorCount);
            ExecuteScene.DoWork -= new DoWorkEventHandler(ExecuteScene_DoWork);
            ExecuteScene.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(ExecuteScene_RunWorkerCompleted);
        }

        public static bool Waiting;
        public static long CmdWaitingForProcessingQueID;
        public static int errorCount;
        void ExecuteScene_DoWork(object sender, DoWorkEventArgs e)
        {
            //Use a new context here to thread safety
            using (zvsEntities2 context = new zvsEntities2())
            {
                //Find Scene
                scene _scene = context.scenes.SingleOrDefault(s => s.id == (long)e.Argument);
                if(_scene != null)
                {
                    e.Result = (long)e.Argument;
                    errorCount = 0;
                    foreach (scene_commands sCMD in _scene.scene_commands)
                    {
                        switch ((command_types)sCMD.command_type_id)
                        {
                            case command_types.builtin:
                                {
                                    builtin_commands cmd = context.builtin_commands.SingleOrDefault(c => c.id == sCMD.command_id);
                                    if (cmd != null)
                                    {
                                        if (cmd.name.Equals("TIMEDELAY"))
                                        {
                                            int delay = 0;
                                            int.TryParse(sCMD.arg, out delay);
                                            if (delay > 0)
                                                System.Threading.Thread.Sleep(delay * 1000);
                                        }
                                        else
                                        {
                                            Waiting = true;
                                            builtin_command_que.BuiltinCommandRunCompleteEvent += new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);

                                            builtin_command_que b_cmd = builtin_command_que.Createbuiltin_command_que(0, sCMD.arg, sCMD.command_id);
                                            context.builtin_command_que.AddObject(b_cmd);
                                            context.SaveChanges();
                                            CmdWaitingForProcessingQueID = b_cmd.id;
                                            //trigger the event
                                            builtin_command_que.BuiltinCommandAddedToQue(b_cmd.id);


                                            while (Waiting)
                                                System.Threading.Thread.Sleep(300);

                                            builtin_command_que.BuiltinCommandRunCompleteEvent -= new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);
                                        }
                                    }
                                    break;
                                }
                            case command_types.device_command:
                                {
                                    Waiting = true;
                                    device_command_que.DeviceCommandRunCompleteEvent += new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);

                                    device_command_que d_cmd = device_command_que.Createdevice_command_que(0, sCMD.device_id.Value, sCMD.command_id, sCMD.arg);
                                    context.device_command_que.AddObject(d_cmd);
                                    context.SaveChanges();
                                    CmdWaitingForProcessingQueID = d_cmd.id;
                                    device_command_que.DeviceCommandAddedToQue(d_cmd.id);

                                    while (Waiting)
                                        System.Threading.Thread.Sleep(300);

                                    device_command_que.DeviceCommandRunCompleteEvent -= new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);

                                    break;
                                }
                            case command_types.device_type_command:
                                {
                                    Waiting = true;
                                    device_type_command_que.DeviceTypeCommandRunCompleteEvent += new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);

                                    device_type_command_que dt_cmd = device_type_command_que.Createdevice_type_command_que(0, sCMD.command_id, sCMD.device_id.Value, sCMD.arg);
                                    context.device_type_command_que.AddObject(dt_cmd);
                                    context.SaveChanges();
                                    CmdWaitingForProcessingQueID = dt_cmd.id;
                                    device_type_command_que.DeviceTypeCommandAddedToQue(dt_cmd.id);

                                    while (Waiting)
                                        System.Threading.Thread.Sleep(300);

                                    device_type_command_que.DeviceTypeCommandRunCompleteEvent -= new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);

                                    break;
                                }
                        }
                    }
                }
            }
        }

        void device_type_command_que_DeviceTypeCommandRunCompleteEvent(device_type_command_que cmd, bool withErrors, string txtError)
        {
            if (cmd.id == CmdWaitingForProcessingQueID)
            {
                Waiting = false;

                if (withErrors)
                    errorCount++;
            }
        }

        void device_command_que_DeviceCommandRunCompleteEvent(device_command_que cmd, bool withErrors, string txtError)
        {
            if (cmd.id == CmdWaitingForProcessingQueID)
            {
                Waiting = false;

                if (withErrors)
                    errorCount++;
            }
        }

        void builtin_command_que_BuiltinCommandRunCompleteEvent(builtin_command_que cmd, bool withErrors, string txtError)
        {
            if (cmd.id == CmdWaitingForProcessingQueID)
            {
                Waiting = false;

                if (withErrors)
                    errorCount++;
            }
        }
    }
}
