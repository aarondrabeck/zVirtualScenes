using System;
using System.Windows.Forms;
using zvs.Processor;
using zvs.WPF.Properties;
using Application = System.Windows.Application;

namespace zvs.WPF
{
    public class ZVSTaskbarIcon : IDisposable
    {
        private NotifyIcon Notify;
        private App app = (App)Application.Current;
        private MenuItem ShowMainWindow;

        public ZVSTaskbarIcon()
        {
            ShowMainWindow = new MenuItem("Show " + Utils.ApplicationName, (o, e) =>
            {
                app.ShowzvsWindow();
            });

            Notify = new NotifyIcon();
            Notify.DoubleClick += Notify_Click;
            Notify.Text = Utils.ApplicationName;
            Notify.Icon = Resources.zvs_icon;
            Notify.Visible = true;
            Notify.ContextMenu = new ContextMenu(new MenuItem[] 
            { 
                ShowMainWindow,
                new MenuItem("-"), 
                new MenuItem("Exit " + Utils.ApplicationName, (o, e) =>  {
                   app.ShutdownZvs(); 
                })
            });

            Notify.BalloonTipClicked += Notify_BalloonTipClicked;
        }

        void Notify_Click(object sender, EventArgs e)
        {
            app.ShowzvsWindow();
        }

        private void Notify_BalloonTipClicked(object sender, EventArgs e)
        {
            // app.ShowzvsWindow();
        }

        public void ShowBalloonTip(string title, string tipText, int timeout, ToolTipIcon icon)
        {
            if (Notify != null)
            {
                Notify.ShowBalloonTip(timeout,
                                     title,
                                     tipText,
                                     icon);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Notify != null)
            {
                Notify.Dispose();
                Notify = null;
            }

            if (ShowMainWindow != null)
                ShowMainWindow.Dispose();
        }
    }
}
