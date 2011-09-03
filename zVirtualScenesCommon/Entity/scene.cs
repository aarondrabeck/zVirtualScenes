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
                return "Cannot start scene '" + this.friendly_name + "' because it is already running!";
            else
            {
                if (scene_commands.Count < 1)
                    return "Cannot start scene '" + this.friendly_name + "' because it has no commands!";

                ExecuteScene.DoWork += new DoWorkEventHandler(ExecuteScene_DoWork);
                ExecuteScene.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(ExecuteScene_RunWorkerCompleted);

                this.is_running = true;
                zvsEntityControl.zvsContext.SaveChanges();

                ExecuteScene.RunWorkerAsync();

                string result = "Started scene '" + this.friendly_name + "'."; 
                zvsEntityControl.SceneRunStarted(this, result);
                return result;
            }
        }

        void ExecuteScene_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.is_running = false;
            zvsEntityControl.zvsContext.SaveChanges();
            zvsEntityControl.SceneRunComplete(this, errorCount);

            ExecuteScene.DoWork -= new DoWorkEventHandler(ExecuteScene_DoWork);
            ExecuteScene.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(ExecuteScene_RunWorkerCompleted);
        }

        public static bool Waiting;
        public static long CmdWaitingForProcessingQueID;
        public static int errorCount;
        void ExecuteScene_DoWork(object sender, DoWorkEventArgs e)
        {
            errorCount = 0;
            foreach (scene_commands sCMD in this.scene_commands)
            {
                switch ((command_types)sCMD.command_type_id)
                {
                    case command_types.builtin:
                        {
                            builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.id == sCMD.command_id);
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
                                    zvsEntityControl.zvsContext.builtin_command_que.AddObject(b_cmd);
                                    zvsEntityControl.zvsContext.SaveChanges();
                                    CmdWaitingForProcessingQueID = b_cmd.id;
                                    builtin_command_que.BuiltinCommandAddedToQue(b_cmd);
                                    

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
                            
                            device_command_que d_cmd = device_command_que.Createdevice_command_que(0, sCMD.device_id, sCMD.command_id, sCMD.arg); 
                            zvsEntityControl.zvsContext.device_command_que.AddObject(d_cmd);
                            zvsEntityControl.zvsContext.SaveChanges();
                            CmdWaitingForProcessingQueID = d_cmd.id;
                            device_command_que.DeviceCommandAddedToQue(d_cmd);                            

                            while (Waiting)
                                System.Threading.Thread.Sleep(300);

                            device_command_que.DeviceCommandRunCompleteEvent -= new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);
                            
                            break;
                        }
                    case command_types.device_type_command:
                        {
                            Waiting = true;
                            device_type_command_que.DeviceTypeCommandRunCompleteEvent += new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);

                            device_type_command_que dt_cmd = device_type_command_que.Createdevice_type_command_que(0, sCMD.command_id, sCMD.device_id, sCMD.arg);
                            zvsEntityControl.zvsContext.device_type_command_que.AddObject(dt_cmd);
                            zvsEntityControl.zvsContext.SaveChanges();
                            CmdWaitingForProcessingQueID = dt_cmd.id;
                            device_type_command_que.DeviceTypeCommandAddedToQue(dt_cmd);

                            while (Waiting)
                                System.Threading.Thread.Sleep(300);

                            device_type_command_que.DeviceTypeCommandRunCompleteEvent -= new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);

                            break;
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
