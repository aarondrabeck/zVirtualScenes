using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using ControlThink.ZWave;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Serialization;

namespace zVirtualScenesApplication 
{
    public delegate SceneResult RunWorkerDelegate(ZWaveController ControlThinkController);

    public class Scene : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void  SceneExecutionFinished(SceneResult sceneresult);
        public event SceneExecutionFinished SceneExecutionFinishedEvent;

        //Properties that require PropertyChangedEvent to fire to sync GUI
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { GlobalFunctions.Set(this, "Name", ref _Name, value, PropertyChanged); }
        }
        public int ID { get; set; }
        public BindingList<Action> Actions { get; set; }
        public int GlobalHotKey { get; set; }

        [XmlIgnore]  //Dont serialize isRunning.
        public bool isRunning { get; set; } 

        public Scene()
        {
            _Name = Name;
            this.ID = ID;
            this.Actions = new BindingList<Action>();
            this.GlobalHotKey = 0;
            this.isRunning = false;
        }

        /// <summary>
        /// Actual Scene Run Worker to be run in a seperate threads.  
        /// </summary>
        /// <param name="ControlThinkController"></param>
        /// <returns></returns>
        public SceneResult RunWorker(ZWaveController ControlThinkController)
        {
            string errors = "";
            
            foreach (Action sceneAction in this.Actions)
            {
                ActionResult actionresult = sceneAction.Run(ControlThinkController);

                if (actionresult.ResultType == ActionResult.ResultTypes.Error)
                    errors += actionresult.Description + ", ";
            }

            if (errors == "")
                return new SceneResult { ResultType = SceneResult.ResultTypes.Success, Description = "Scene '" + this.Name + "' successfully finished executing.  " + this.Actions.Count() + " action(s) executed." };
            else
                return new SceneResult { ResultType = SceneResult.ResultTypes.Error, Description = "Scene '" + this.Name + "' finished with the following errors: " + errors };
        }

        /// <summary>
        /// Handles Scene Run Workers
        /// </summary>
        /// <param name="ControlThinkController"></param>
        /// <returns></returns>
        public SceneResult Run(ZWaveController ControlThinkController)
        {
            if(this.isRunning)
                return new SceneResult { ResultType = SceneResult.ResultTypes.Error, Description = "Scene '" + this.Name + "' is already running." }; 

            if (this.Actions.Count < 1)
                return new SceneResult { ResultType = SceneResult.ResultTypes.Error, Description = "Scene '" + this.Name + "' has no actions." };            
            
            RunWorkerDelegate RunWorkerDel = new RunWorkerDelegate(RunWorker);
            this.isRunning = true; 
            //Start worker in thread
            RunWorkerDel.BeginInvoke(ControlThinkController, new AsyncCallback(RunSceneCallback), null);
            return new SceneResult { ResultType = SceneResult.ResultTypes.Success, Description = "Scene '" + this.Name + "' began."};                
        }

        /// <summary>
        /// Gets called with a scene worker thread is finished. 
        /// Then it will fire public SceneExecutionFinishedEvent.
        /// </summary>
        /// <param name="ar"></param>
        private void RunSceneCallback(IAsyncResult ar)
        {
            // first case IAsyncResult to an AsyncResult object, so we can get the delegate that was used to call the function.            
            AsyncResult result = (AsyncResult)ar;

            // grab the delegate
            RunWorkerDelegate RunWorkerDel = (RunWorkerDelegate)result.AsyncDelegate;

            SceneResult Sceneresult = RunWorkerDel.EndInvoke(ar);

            //mark not running
            this.isRunning = false; 

            if (SceneExecutionFinishedEvent != null)  //Call Event ONLY IF someone is subscribed.
                 this.SceneExecutionFinishedEvent(Sceneresult);             
        }       

        public override string ToString()
        {
            return _Name;
        }

        public string ToStringForHTTP()
        {
            return _Name + " - ID:" + ID; ;
        }

        //Light Switch Socket Format
        public string ToLightSwitchSocketString()
        {
            return "SCENE~" + _Name + "~" + this.ID;
        }        
        
    }
    /// <summary>
    /// SuccessLevel: 1 = Good, 2 = Error
    /// </summary>
    public class SceneResult
    {
        public enum ResultTypes
        {
            Success = 1,
            Error = 2
        }

        public ResultTypes ResultType { get; set; }
        public string Description { get; set; }
    }
}
