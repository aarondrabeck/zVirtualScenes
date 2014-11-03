namespace zvs.DataModel
{
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
                if (value == _script) return;
                _script = value;
                NotifyPropertyChanged();
            }
        }
    }
}
