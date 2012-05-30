using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using zVirtualScenesModel;
using zVirtualScenes;
using System.ComponentModel;

namespace zVirtualScenes_WPF
{
    /// <summary>
    /// interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        public Core zvsCore;

        protected override void OnStartup(StartupEventArgs e)
        {  
            //Initilize the core
            zvsCore = new Core(this.Dispatcher);


                        
           base.OnStartup(e);
        }

        private void processorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }
    }
}
