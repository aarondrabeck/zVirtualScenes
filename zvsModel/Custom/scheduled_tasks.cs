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
        private const string _FriendlyName = "TASK";

        public string IconName { get { return "Task"; } }
        public string isEnabledString { get { return (this.Enabled ? "Yes" : "No"); } }
        public string FrequencyString { get { return Enum.GetName(typeof(frequencys), this.Frequency); } }

        public enum frequencys
        {
            Daily = 0,
            Weekly = 1,
            Once = 2,
            Seconds = 3,
            Monthly = 4
        }
    }
}
