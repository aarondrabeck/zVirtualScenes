using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace zVirtualScenesApplication
{
    public class Scene
    {
        //Default constructor string name = "", int ID = 0
        public Scene()
        {
            this.name = name;
            this.ID = ID;
            this.Actions = new List<Action>();
        }

        public string name { get; set; } 
        public int ID { get; set; }
        public List<Action> Actions { get; set; }        

        public override string ToString()
        {
            return this.name;
        }

    }
}
