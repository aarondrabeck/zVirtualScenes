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

        partial void onAfterPropertyChanged(string name)
        {
            if(name =="Frequency")
                NotifyPropertyChanged("FrequencyEnum");
        }

        private frequencys _FrequencyEnum;
        public frequencys FrequencyEnum
        {
            get
            {
                if (!Frequency.HasValue)
                    return frequencys.Daily;

                frequencys freq = frequencys.Daily;
                Enum.TryParse(Frequency.Value.ToString(), out freq);
                return freq;
            }
            set
            {
                _FrequencyEnum = value;
                Frequency = (int)value;
            }
        }
    }
}
