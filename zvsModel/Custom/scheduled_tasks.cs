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

        public string Run(zvsLocalDBEntities context)
        {
            scene scene = context.scenes.FirstOrDefault(s => s.id == this.Scene_id);
            if (scene != null)
                return string.Format("Scheduled task '{0}' {1}", this.friendly_name, scene.RunScene());
            else
                return string.Format("Scheduled task '{0}' Failed to find scene ID '{1}'.", this.friendly_name, this.Scene_id);
        }        
    }
}
