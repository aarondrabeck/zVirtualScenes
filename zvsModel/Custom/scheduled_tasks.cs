using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace zVirtualScenesModel
{      
    public partial class scheduled_tasks
    {
        public enum frequencys
        {
            Once = 2,
            Seconds = 3,
            Daily = 0,
            Weekly = 1,
            Monthly = 4
        }        
    }
}
