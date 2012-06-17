using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using zVirtualScenes;

namespace zVirtualScenes_WPF
{
    public class ZVSTaskbarIcon
    {
        private System.Windows.Forms.NotifyIcon Notify;
        private App app = (App)Application.Current;       
        private System.Windows.Forms.MenuItem ShowMainWindow;       

        public ZVSTaskbarIcon()
        {
            ShowMainWindow = new System.Windows.Forms.MenuItem("Show " + Utils.ApplicationName, (o, e) =>
            {
                app.ShowMainWindow();
            });           

            Notify = new System.Windows.Forms.NotifyIcon();
            Notify.DoubleClick += new EventHandler(Notify_Click);
            Notify.Text = Utils.ApplicationName;
            Notify.Icon = zVirtualScenes_WPF.Properties.Resources.zvs32;
            Notify.Visible = true;
            Notify.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                ShowMainWindow,
                new System.Windows.Forms.MenuItem("-"), 
                new System.Windows.Forms.MenuItem("Exit " + Utils.ApplicationName, (object o,EventArgs e) =>  {
                    app.ShutdownZVS(); 
                })
            });

            Notify.BalloonTipClicked += Notify_BalloonTipClicked;            
        }

        void Notify_Click(object sender, EventArgs e)
        {
            app.ShowMainWindow();
        }

        private void Notify_BalloonTipClicked(object sender, EventArgs e)
        {
            app.ShowMainWindow();
        }

        public void Dispose()
        {            
            Notify.Dispose();
            Notify = null;
        }

        public void ShowBalloonTip(string Title, string TipText, int timeout, System.Windows.Forms.ToolTipIcon icon)
        {
            if (Notify != null)
            {
                Notify.ShowBalloonTip(timeout,
                                     Title,
                                     TipText,
                                     icon);
            }
        }
    }
}
