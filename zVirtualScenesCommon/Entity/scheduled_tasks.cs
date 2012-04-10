using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using System.ComponentModel;
using zVirtualScenesCommon.Util;

namespace zVirtualScenesCommon.Entity
{
    public partial class scheduled_tasks : EntityObject
    {
        private const string _FriendlyName = "TASK";

        public string IconName { get { return "Task"; } }
        public string isEnabledString { get  { return (this.Enabled ? "Yes" : "No");} }
        public string FrequencyString { get { return Enum.GetName(typeof(frequencys), this.Frequency); }  }

        public enum frequencys
        {
            Daily = 0,
            Weekly = 1,
            Once = 2,
            Seconds = 3,
            Monthly = 4
        }

        public void Run()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                scene scene = db.scenes.FirstOrDefault(s => s.id == this.Scene_id);
                if (scene != null)
                {
                    string result = scene.RunScene(db);
                    Logger.WriteToLog(Urgency.INFO, string.Format("Scheduled task '{0}' {1}", this.friendly_name, result), _FriendlyName);
                }
                else
                    Logger.WriteToLog(Urgency.WARNING, "Scheduled task '" + this.friendly_name + "' Failed to find scene ID '" + this.Scene_id + "'.", _FriendlyName);
            }
        }
    }
}
