//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.ComponentModel.Composition.Hosting;
//using System.Data;
//using System.Linq;
//using System.Threading;
//using System;
//using System.ComponentModel;

//using System.Windows.Threading;
//using zvs.Entities;

//namespace zvs.Processor
//{
//    public class PluginManager
//    {
//        private const int verbose = 10;
//        private const string _Name = "Plug-in Manager";
//        private Core Core;

//        public delegate void onPluginInitializedEventHandler(object sender, onPluginInitializedEventArgs args);
//        public class onPluginInitializedEventArgs : EventArgs
//        {
//            public string Details = string.Empty;

//            public onPluginInitializedEventArgs(string Details)
//            {
//                this.Details = Details;
//            }
//        }
//        public static event onPluginInitializedEventHandler onPluginInitialized;

//        [ImportMany]
//        #pragma warning disable 649
//        private IEnumerable<zvsPlugin> _plugins;
//        #pragma warning restore 649

//        public PluginManager(Core Core)
//        {
//            this.Core = Core;
//            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
//            CompositionContainer compositionContainer = new CompositionContainer(catalog);
//            compositionContainer.ComposeParts(this);

//            using (zvsContext context = new zvsContext())
//            {
//                // Iterate the plug-in
//                foreach (zvsPlugin p in _plugins)
//                {
//                    //keeps this plug-in in scope 
//                    var p2 = p;

//                    //Plug-in need access to the core in order to use the Logger
//                    p2.Core = this.Core;

//                    //initialize each plug-in async.
//                    BackgroundWorker pluginInitializer = new BackgroundWorker();
//                    pluginInitializer.DoWork += (object sender, DoWorkEventArgs e) =>
//                    {
//                        string msg = string.Format("Initializing '{0}'", p2.Name);
//                        Core.log.Info(msg);
//                        if (onPluginInitialized != null)
//                            onPluginInitialized(this, new onPluginInitializedEventArgs(msg));

//                        p2.Initialize();
//                        p2.Start();
//                    };
//                    pluginInitializer.RunWorkerAsync();
//                }
//            }
            
//        }

//        public IEnumerable<zvsPlugin> GetPlugins()
//        {
//            return _plugins;
//        }

//        public zvsPlugin GetPlugin(string uniqueIdentifier)
//        {
//            return _plugins.FirstOrDefault(p => p.UniqueIdentifier == uniqueIdentifier);
//        }

//        public void NotifyPluginSettingsChanged(AdapterSetting ps)
//        {
//            zvsPlugin p = GetPlugin(ps.Plugin.UniqueIdentifier);
//            if (p != null)
//                p.SettingsChange(ps);
//        }
//    }
//}

