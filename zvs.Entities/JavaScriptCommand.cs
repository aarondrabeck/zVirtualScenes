using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("JavaScriptCommands", Schema = "ZVS")]
    public class JavaScriptCommand : Command
    {  
        private string _script;
        public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                if (value != _script)
                {
                    _script = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
