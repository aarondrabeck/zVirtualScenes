using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using zVirtualScenesService.PluginSystem;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon;
using zVirtualScenesAPI;

namespace zVirtualScenesService
{
    public partial class CoreService : ServiceBase
    {
        private const string _FriendlyName = "ServerCore";
        private PluginManager pm;
        public CoreService()
        {
            InitializeComponent();
            pm = null;
            Logger.LogFileName = "zVirtualScenesService.log";
        }

        protected override void OnStart(string[] args)
        {
            if (pm != null)
            {
                OnStop();
            }
            Logger.WriteToLog(Urgency.INFO, "STARTED", _FriendlyName);
            pm = new PluginManager();
        }

        protected override void OnStop()
        {
            foreach (Plugin plugin in pm.GetPlugins())
            {
                try
                {
                    plugin.Stop();
                }
                catch (Exception e)
                {
                    Logger.WriteToLog(Urgency.ERROR, string.Format("Could not stop plugin {0} : {1}", plugin.Name, e.Message), _FriendlyName);
                }
            }
            Logger.WriteToLog(Urgency.INFO, "STOPPED", _FriendlyName);
            Logger.SaveLogToFile();   
        }

        internal void Start()
        {
            OnStart(null);
        }
    }
}
