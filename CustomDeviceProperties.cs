using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class CustomDeviceProperties
    {
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public string Name { get; set; }

        public CustomDeviceProperties()
        {
            this.HomeID = 0;
            this.NodeID = 0;
            this.Name = "Default Device";           
        }

        public string GlbUniqueID()
        {
            return this.HomeID.ToString() + this.NodeID.ToString();
        }
    }
}
