using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using ControlThink.ZWave;

namespace zVirtualScenesApplication 
{
    public class Scene : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        public Scene()
        {
            _Name = Name;
            this.ID = ID;
            this.Actions = new BindingList<Action>();
            this.GlobalHotKey = 0; 
        }


        public SceneResult Run(ZWaveController ControlThinkController)
        {

            string errors = "";
            if (this.Actions.Count > 0)
            {
                foreach (Action sceneAction in this.Actions)
                {
                    Action.ActionResult actionresult = sceneAction.Run(ControlThinkController);

                    if (actionresult.SuccessLevel == 2)
                        errors += actionresult.Description + ", ";
                }
            }
            else
                return new SceneResult { SuccessLevel = 2, Description = "Scene '" + this.Name + "' has no actions."};

            if(errors == "")
                return new SceneResult { SuccessLevel = 1, Description = "Ran scene '" + this.Name + "' with " + this.Actions.Count() + " action(s)." };
            else
                return new SceneResult { SuccessLevel = 2, Description = "Ran scene '" + this.Name + "' with the following errors: " + errors};
            
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

        /// <summary>
        /// SuccessLevel: 1 = Good, 2 = Error
        /// </summary>
        public class SceneResult
        {
             public int SuccessLevel { get; set; }
             public string Description { get; set; }
        }
        
    }
}
