using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlThink.ZWave;
using ControlThink.ZWave.Devices;

namespace ThinkStickHIDPlugin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ControllerDialogWindow : Window
    {
        private readonly ZWaveController CTController = new ZWaveController();
        private string cmdName = string.Empty;
        private Action BtnClickAction;

        public ControllerDialogWindow(ZWaveController CTController, string cmdName)
        {
            this.CTController = CTController;
            this.cmdName = cmdName;
            InitializeComponent();
        }

        private void ControllerDialogWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            //public IAsyncResult BeginAddController(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginAddController(ZWaveController.ReplicationGroup[] replicationGroups, ZWaveController.ReplicationScene[] replicationScenes, AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginAddDevice(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginCreateNewPrimaryController(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginReceiveConfiguration(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginRemoveController(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginRemoveDevice(AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginReplaceFailedDevice(ZWaveDevice device, AsyncCallback requestCallback, object stateObject);
            //public IAsyncResult BeginTransferPrimaryRole(AsyncCallback requestCallback, object stateObject);
            switch (cmdName)
            {
                case "BeginAddController":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Listening for new controller.";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortAddController();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if(args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginAddController((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndAddController(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Add controller failed.";
                                    else
                                        StatusTxtBl.Text = "Added controller, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Add controller failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }
                case "BeginAddDevice":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Listening for new device.";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortAddDevice();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginAddDevice((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndAddDevice(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Add device failed.";
                                    else
                                        StatusTxtBl.Text = "Added device, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Add device failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }
                case "BeginCreateNewPrimaryController":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Creating New Primary Controller";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortCreateNewPrimaryController();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginCreateNewPrimaryController((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndCreateNewPrimaryController(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Create New Primary Controller failed.";
                                    else
                                        StatusTxtBl.Text = "Created New Primary Controller, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Create New Primary Controller failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }

                case "BeginReceiveConfiguration":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Listening for new configuration";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortReceiveConfiguration();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginReceiveConfiguration((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    bool result = CTController.EndReceiveConfiguration(cb);
                                    if (!result)
                                        StatusTxtBl.Text = "Listening for new configuration failed.";
                                    else
                                        StatusTxtBl.Text = "Configuration Received.";
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Listening for new configuration failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }

                case "BeginRemoveController":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Listening for controller to be removed.";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortRemoveController();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginRemoveController((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndRemoveController(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Remove controller failed.";
                                    else
                                        StatusTxtBl.Text = "Removed Controller, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Remove controller failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }
                case "BeginRemoveDevice":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Listening for device to be removed.";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortRemoveDevice();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginRemoveDevice((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndRemoveDevice(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Remove device failed.";
                                    else
                                        StatusTxtBl.Text = "Removed device, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Remove device failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }
                case "BeginTransferPrimaryRole":
                    {
                        #region Controller Command
                        StatusTxtBl.Text = "Transfering primary role";
                        DynamicBtn.Content = "Abort";
                        BtnClickAction = () =>
                        {
                            DynamicBtn.IsEnabled = false;
                            DynamicBtn.Content = "Aborting...";
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += (send, args) => CTController.AbortTransferPrimaryRole();
                            bw.RunWorkerCompleted += (send, args) =>
                            {
                                DynamicBtn.IsEnabled = true;
                                if (args.Error == null)
                                    StatusTxtBl.Text = "Aborted.";
                                else
                                    StatusTxtBl.Text = "Abort failed. " + args.Error;
                            };
                        };

                        CTController.BeginTransferPrimaryRole((cb) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                DynamicBtn.IsEnabled = true;
                                DynamicBtn.Content = "Close";
                                BtnClickAction = () => this.Close();

                                try
                                {
                                    ZWaveDevice device = CTController.EndTransferPrimaryRole(cb);
                                    if (device != null)
                                        StatusTxtBl.Text = "Transfering primary role failed.";
                                    else
                                        StatusTxtBl.Text = "Transfered primary role, node id " + device.NodeID;
                                }
                                catch (Exception ex)
                                {
                                    StatusTxtBl.Text = "Transfering primary role failed. " + ex.Message;
                                }
                            }));
                        }, null);
                        #endregion
                        break;
                    }
            }
        }

        private void DynamicBtn_Click(object sender, RoutedEventArgs e)
        {
            BtnClickAction.DynamicInvoke();
        }
    }
}
