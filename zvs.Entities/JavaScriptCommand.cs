using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("JavaScriptCommands", Schema = "ZVS")]
    public partial class JavaScriptCommand : Command
    {  
        private string _Script;
        [StringLength(4000)]
        public string Script
        {
            get
            {
                return _Script;
            }
            set
            {
                if (value != _Script)
                {
                    _Script = value;
                    NotifyPropertyChanged("Script");
                    NotifyPropertyChanged("ActionDescription");
                    NotifyPropertyChanged("ActionableObject");
                }
            }
        }

        

        public void Run(zvsContext context, string argument = "")
        {
            QueuedJavaScriptCommand cmd = new QueuedJavaScriptCommand { Command = this };
            cmd.Run(context);
        }
    }
}
