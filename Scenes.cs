using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

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

        public Scene()
        {
            _Name = Name;
            this.ID = ID;
            this.Actions = new BindingList<Action>();
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
}
