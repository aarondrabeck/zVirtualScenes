using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("JavaScriptCommands", Schema = "ZVS")]
    public partial class JavaScriptCommand : Command
    {  
        private string _Script;
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
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
