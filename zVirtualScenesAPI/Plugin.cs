using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;

namespace zVirtualScenesAPI
{
    public abstract class Plugin
    {
        protected string PluginName;
        protected API API;

        public bool IsRunning { get; private set; }
        public bool IsReady { get; protected set; }
        public bool ListenForStateChanges { get; protected set; }

        protected Plugin(string apiName)
        {
            API = new API(apiName);
        }

        public void Start()
        {
            if(Enabled)
                IsRunning = StartPlugin();
        }

        public void Stop()
        {
            IsRunning = !StopPlugin();
        }

        public bool Enabled
        {
            get
            {
                return DatabaseControl.GetPluginEnabled(API.GetPluginName());                 
            }
            set
            {
                DatabaseControl.SetPluginEnabled(API.GetPluginName(), value);
            }
        }

        public override string ToString() 
        { 
            return PluginName; 
        }               

        public string GetAPIName()
        {
            return API.GetPluginName();
        }
        
        public void SetSetting(string settingName, string settingValue)
        {
            if (API != null)
            {
                DatabaseControl.SetPluginSetting(API.GetPluginName(), settingName, settingValue);
                SettingChanged(settingName, settingValue);
            }
        }

        public string GetSetting(string settingName)
        {
            if (API == null)
                return null; 

            return DatabaseControl.GetPluginSetting(API.GetPluginName(), settingName);            
        }

        // Abstract functions
        protected abstract bool StartPlugin();
        protected abstract bool StopPlugin();
        protected abstract void SettingChanged(string settingName, string settingValue);
        public abstract void Initialize();
        public abstract void ProcessCommand(QuedCommand cmd);
        public abstract void Repoll(string ObjId);
        public abstract void ActivateGroup(string GroupName);
        public abstract void DeactivateGroup(string GroupName);        

    }
}
