using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesAPI.Structs
{
    public class Task
    {
        public string IconName { get { return "Task"; } }

        public int ID    { get; set; }
        public string Name { get; set; }
        public frequencys Frequency { get; set; }
        public bool Enabled { get; set; }
        public int SceneID { get; set; }
       
        public bool RecurMonday { get; set; }
        public bool RecurTuesday { get; set; }
        public bool RecurWednesday { get; set; }
        public bool RecurThursday { get; set; }
        public bool RecurFriday { get; set; }
        public bool RecurSaturday { get; set; }
        public bool RecurSunday { get; set; }

        public int RecurDays { get; set; }
        public int RecurWeeks { get; set; }
        public int RecurMonth { get; set; }
        public int RecurDayofMonth { get; set; }
        public int RecurSeconds { get; set; }
        public DateTime StartTime { get; set; }

        public int Order { get; set; }

        public enum frequencys
        {
            Daily = 0,
            Weekly = 1,
            Once = 2,
            Seconds = 3
        }
        
        public Task()
        {
            this.Name = "New Scheduled Event";
            this.Frequency = frequencys.Daily;
            this.StartTime = DateTime.Now;
            this.Enabled = false;
            this.RecurWeeks = 1;
            this.RecurDays = 1; 
        }

        public override string ToString()
        {
            return this.Name;
        }      

        public string isEnabled()
        {
            return (this.Enabled ? "Yes" : "No");
        }

        public string FrequencyString()
        {
            return Enum.GetName(typeof(frequencys), this.Frequency);
        }

        public void Add()
        {
            API.ScheduledTasks.Add(this);
        }

        public void Update()
        {
            API.ScheduledTasks.Update(this);
        }

        public void Remove()
        {
            API.ScheduledTasks.Remove(this);
        }

    }
}
