//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;
//using System.Data;
//using zVirtualScenesAPI;
//using zVirtualScenesAPI.Structs;
//

//namespace zVirtualScenesApplication.Structs
//{   
//    public class Scene
//    {
//        public int id { get; set; }
//        public string txt_name { get; set; }        
//        public BindingList<SceneCommands> scene_commands = new BindingList<SceneCommands>();
//        public bool isRunning = false; 
//        BackgroundWorker ExecuteScene = new BackgroundWorker();

//        public override string ToString()
//        {
//            return txt_name;
//        }

//        public Scene()
//        {
//            ExecuteScene.DoWork += new DoWorkEventHandler(ExecuteScene_DoWork);
//            ExecuteScene.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExecuteScene_RunWorkerCompleted);            
//        }

//        public string DeviceIcon()
//        {
//            if (!isRunning)
//                return "Scene";
//            else
//                return "SceneRun";
//        }

//        public void Remove()
//        {
//            API.Scenes.Delete(id);
//        }

//        public void Add()
//        {
//            API.Scenes.AddBlankScene();
//        }

//        public string RunScene()
//        {
//            if (API.Scenes.GetIsRunning(id) || ExecuteScene.IsBusy)           
//                return "Cannot start scene '" + txt_name + "' because it is already running!";            
//            else
//            {
//                if(scene_commands.Count < 1)
//                    return "Cannot start scene '" + txt_name + "' because it has no commands!";

//                API.Scenes.SetIsRunning(id, true);
//                zVirtualSceneEvents.SceneChanged(this.id);
//                ExecuteScene.RunWorkerAsync(); 
//                return "Started scene '" + txt_name + "'.";  
//            }
//        }

//        void ExecuteScene_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            API.Scenes.SetIsRunning(id, false); 
//            zVirtualSceneEvents.SceneChanged(this.id);

//            zVirtualSceneEvents.SceneRunComplete(id, errorCount);
//        }

//        public static bool Waiting; 
//        public static int CmdWaitingForProcessingQueID;
//        public static int errorCount; 
//        void ExecuteScene_DoWork(object sender, DoWorkEventArgs e)
//        {
//            zVirtualSceneEvents.CommandRunCompleteEvent += new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
//            errorCount = 0;
//            foreach(SceneCommands sCMD in scene_commands)
//            {                
//                Command CommandInfo = API.Commands.GetCommand(sCMD.CommandId, sCMD.cmdtype);

//                if (CommandInfo.Name.Equals("TIMEDELAY"))
//                {
//                    int delay = 0;
//                    int.TryParse(sCMD.Argument, out delay);
//                    if (delay > 0)
//                        System.Threading.Thread.Sleep(delay * 1000);
//                }
//                else
//                {
//                    Waiting = true;
//                    CmdWaitingForProcessingQueID = sCMD.Run();
//                    zVirtualSceneEvents.QueCommandAdded(CmdWaitingForProcessingQueID);

//                    while (Waiting)
//                        System.Threading.Thread.Sleep(300);
//                }
//            }
//            zVirtualSceneEvents.CommandRunCompleteEvent -= new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
//        }

//        void zVirtualSceneEvents_CommandRunCompleteEvent(int QueID, bool withErrors, string txtError)
//        {
//            if (QueID == CmdWaitingForProcessingQueID)
//            {
//                Waiting = false;

//                if (withErrors)
//                    errorCount++;
//            }
//        } 
//    }    

    
//}
