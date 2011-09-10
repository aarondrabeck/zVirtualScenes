using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using zVirtualScenesAPI;

using zVirtualScenesApplication.Forms;
using zVirtualScenesApplication.PluginSystem;
using zVirtualScenesCommon.Util;
using zVirtualScenesApplication.Structs;
using zVirtualScenesCommon;
using System.Data.Objects;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication
{
    public partial class MainForm : Form
    {
       
        public string ProgramName = zvsEntityControl.zvsNameAndVersion;
        private IBindingList deviceList;
        private ObjectQuery<device> deviceListQuery;
        private IBindingList sceneList;
        private ObjectQuery<scene> sceneQuery1;
        private IBindingList sceneCMDsList;
        private IBindingList taskList;

        //Forms
        private GroupEditor grpEditorForm;
        private ActivateGroup grpActivateForm;
        private formPropertiesScene formSceneProperties;               

        //Delegates
        public delegate void anonymousEventDelegate(object sender, EventArgs e);
        public delegate void anonymousDelegate();

        //CORE OBJECTS
        private BindingList<LogItem> _masterlog = new BindingList<LogItem>();

        //TODO: Make an object and finish events
        //private DataTable _masterEvents;

        // Plugin Stuff
        public PluginManager pm;

        public static bool UpdateScripts;
        void HandlerMethod(object sender, UnhandledExceptionEventArgs e)
        {
            if ((e.ExceptionObject is ThreadAbortException) != true)
            {
                var exception = e.ExceptionObject as Exception;
                MessageBox.Show(exception.ToString());
            }
        }
        public MainForm()
        {
            InitializeComponent();

             AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandlerMethod);



            zvsEntityControl.zvsContext.Connection.Open();

            //Load form size
            GeometryFromString(Properties.Settings.Default.WindowGeometry, this);
            FormClosing += zVirtualScenes_FormClosing;

            this.MeterCol.Renderer = new BarRenderer(0, 99);
            dataListViewDevices.CellRightClick += dataListViewDevices_CellRightClick;

            Logger.LogItemPostAdd += Logger_LogItemPostAdd;

            //Events
            builtin_command_que.BuiltinCommandRunCompleteEvent += new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);
            device_type_command_que.DeviceTypeCommandRunCompleteEvent += new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);
            device_command_que.DeviceCommandRunCompleteEvent += new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);
            zvsEntityControl.SceneRunStartedEvent += new zvsEntityControl.SceneRunStartedEventHandler(zvsEntityControl_SceneRunStartedEvent);
            zvsEntityControl.SceneRunCompleteEvent += new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent);
            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            zvsEntityControl.DeviceAddedEvent += new zvsEntityControl.DeviceAddedEventHandler(zvsEntityControl_DeviceAddedEvent);
        }

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {            
            this.Text = ProgramName;            
            dataListViewLog.DataSource = _masterlog;
            Logger.WriteToLog(Urgency.INFO, "STARTED", "MainForm");

            pm = new PluginManager();          
            
            //Bind data to GUI elements
            deviceListQuery = zvsEntityControl.zvsContext.devices;
            deviceListQuery.MergeOption = MergeOption.AppendOnly;
            
            deviceList = ((IListSource)deviceListQuery).GetList() as IBindingList;
            dataListViewDevices.DataSource = deviceList;
            dataListViewDeviceSmallList.DataSource = deviceList;
          
            // Scenes            
            sceneQuery1 = zvsEntityControl.zvsContext.scenes;
            sceneQuery1.MergeOption = MergeOption.AppendOnly;
            sceneList = ((IListSource)sceneQuery1).GetList() as IBindingList;
            dataListViewScenes.DataSource = sceneList;

            //SORT
            dataListViewScenes.Sort(this.SceneSortCol, SortOrder.Ascending);

            // Events
            //_masterEvents = DatabaseControl.GetEventScripts();
            //dataListEvents.DataSource = _masterEvents;

            //Scenes (allow rearrage but not drag and drop from other sources)
            dataListViewScenes.DropSink = new SceneDropSink();            
            if(dataListViewScenes.Items.Count >0)
                dataListViewScenes.SelectedIndex = 0;

            // Scene Commands       
            dataListViewSceneCMDs.DropSink = new SceneCommandDropSink();          

            #region Task Scheduler

            //Load Tasks            
            ObjectQuery<scheduled_tasks> taskQuery1 = zvsEntityControl.zvsContext.scheduled_tasks;
            taskList = ((IListSource)taskQuery1).GetList() as IBindingList;
            dataListTasks.DataSource = taskList;

            comboBox_FrequencyTask.DataSource = Enum.GetNames(typeof(scheduled_tasks.frequencys));       
            comboBox_ActionsTask.DataSource = sceneList;
            comboBox_ActionsTask.DisplayMember = "friendly_name";

            //Add default timer item if list is empty
            if (taskList.Count < 1)
                AddNewTask();
            else
                dataListTasks.SelectedIndex = 0;
            #endregion           
        }        

        private void zVirtualScenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowGeometry = GeometryToString(this);
            Properties.Settings.Default.Save();

            dataListViewLog.DataSource = null;

            Logger.SaveLogToFile();  
            Application.Exit();
        }

        #region Subcribed API Events

        void zvsEntityControl_DeviceAddedEvent(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(zvsEntityControl_DeviceAddedEvent), new object[] { sender, e });
            else
            {
                //REFRESH THE LIST SEE WE CAN ACTUALLY SEE THE NEW DEVICE
                deviceList = ((IListSource)deviceListQuery).GetList() as IBindingList;
                dataListViewDevices.DataSource = deviceList;

                int index = dataListViewDevices.SelectedIndex;
                if (index > 0)
                    dataListViewDevices.EnsureVisible(index);

                dataListViewDeviceSmallList.DataSource = deviceList;                
            }
        }              

        void Logger_LogItemPostAdd(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(Logger_LogItemPostAdd), new object[] { sender, e });
            else
            {
                LogItem logitem = Logger.GetLastEntry();
                _masterlog.Insert(0, logitem);
                toolStripStatusLabel1.Text = logitem.Source + " - " + logitem.Description.Replace(Environment.NewLine, " ");
            }
        }

        void zvsEntityControl_SceneRunStartedEvent(scene s, string result)
        {
            if (this.InvokeRequired)
                this.Invoke(new zvsEntityControl.SceneRunStartedEventHandler(zvsEntityControl_SceneRunStartedEvent), new object[] { s, result });
            else
            {
                if (s != null)
                {
                    Logger.WriteToLog(Urgency.INFO, result, "EVENT");
                }
            }
        }

        void zvsEntityControl_SceneRunCompleteEvent(long scene_id, int ErrorCount)
        {
            if (this.InvokeRequired)
                this.Invoke(new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent), new object[] { scene_id, ErrorCount });
            else
            {
                using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene _scene = context.scenes.SingleOrDefault(s => s.id == scene_id);
                    if (_scene != null)
                    {
                        Logger.WriteToLog(Urgency.INFO, "Scene '" + _scene.friendly_name + "' has completed with " + ErrorCount + " errors.", "EVENT");
                    }
                    else
                        Logger.WriteToLog(Urgency.INFO, "Scene #'" + scene_id + "' has completed with " + ErrorCount + " errors.", "EVENT");                    

                }

                //Since the scene wasn't updated on the the same context, refresh GUI's context. 
                zvsEntityControl.zvsContext.Refresh(RefreshMode.StoreWins, zvsEntityControl.zvsContext.scenes);
                //sceneList = ((IListSource)sceneQuery1).GetList() as IBindingList;
                //dataListViewScenes.DataSource = sceneList;

                //int index = dataListViewScenes.SelectedIndex;
                //if (index > 0)
                //    dataListViewScenes.EnsureVisible(index);
            }
        }

        void device_values_DeviceValueDataChangedEvent(object sender, string PreviousValue)
        {
            if (this.InvokeRequired)
                this.Invoke(new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent), new object[] { sender, PreviousValue });
            else
            {
                device_values dv = (device_values)sender;
                if (dv != null)
                {
                    string device_name;
                    if (String.IsNullOrEmpty(dv.device.friendly_name))
                        device_name = "Device #" + dv.device_id;
                    else
                        device_name = dv.device.friendly_name;

                    if (!String.IsNullOrEmpty(PreviousValue))
                        Logger.WriteToLog(Urgency.INFO, device_name + " " + dv.label_name + " changed to " + dv.value + " from " + PreviousValue + ".", "EVENT");
                    else
                        Logger.WriteToLog(Urgency.INFO, device_name + " " + dv.label_name + " changed to " + dv.value + ".", "EVENT");           
                }

            }
        }

        void builtin_command_que_BuiltinCommandRunCompleteEvent(builtin_command_que cmd, bool withErrors, string txtError)
        {
            if (this.InvokeRequired)
                this.Invoke(new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent), new object[] { cmd, withErrors, txtError });
            else
            {
                if (withErrors)
                    Logger.WriteToLog(Urgency.INFO, "Qued builtin command #'" + cmd.id + "' has completed with errors.", "EVENT");
            }
        }

        void device_command_que_DeviceCommandRunCompleteEvent(device_command_que cmd, bool withErrors, string txtError)
        {
            if (this.InvokeRequired)
                this.Invoke(new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent), new object[] { cmd, withErrors, txtError });
            else
            {
                if (withErrors)
                    Logger.WriteToLog(Urgency.INFO, "Qued device command #'" + cmd.id + "' has completed with errors.", "EVENT");
            }
        }

        void device_type_command_que_DeviceTypeCommandRunCompleteEvent(device_type_command_que cmd, bool withErrors, string txtError)
        {
            if (this.InvokeRequired)
                this.Invoke(new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent), new object[] { cmd, withErrors, txtError });
            else
            {
                if (withErrors)
                    Logger.WriteToLog(Urgency.INFO, "Qued device type command #'" + cmd.id + "' has completed with errors.", "EVENT");
            }
        }

        #endregion
      
        #region Saving and Resorting MAIN FORM Sizing

        public static void GeometryFromString(string thisWindowGeometry, Form formIn)
        {
            if (string.IsNullOrEmpty(thisWindowGeometry) == true)
            {
                return;
            }
            string[] numbers = thisWindowGeometry.Split('|');
            string windowString = numbers[4];
            if (windowString == "Normal")
            {
                Point windowPoint = new Point(int.Parse(numbers[0]),
                    int.Parse(numbers[1]));
                Size windowSize = new Size(int.Parse(numbers[2]),
                    int.Parse(numbers[3]));

                bool locOkay = GeometryIsBizarreLocation(windowPoint, windowSize);
                bool sizeOkay = GeometryIsBizarreSize(windowSize);

                if (locOkay == true && sizeOkay == true)
                {
                    formIn.Location = windowPoint;
                    formIn.Size = windowSize;
                    formIn.StartPosition = FormStartPosition.Manual;
                    formIn.WindowState = FormWindowState.Normal;
                }
                else if (sizeOkay == true)
                {
                    formIn.Size = windowSize;
                }
            }
            else if (windowString == "Maximized")
            {
                formIn.Location = new Point(100, 100);
                formIn.StartPosition = FormStartPosition.Manual;
                formIn.WindowState = FormWindowState.Maximized;
            }
        }

        private static bool GeometryIsBizarreLocation(Point loc, Size size)
        {
            bool locOkay;
            if (loc.X < 0 || loc.Y < 0)
            {
                locOkay = false;
            }
            else if (loc.X + size.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                locOkay = false;
            }
            else if (loc.Y + size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                locOkay = false;
            }
            else
            {
                locOkay = true;
            }
            return locOkay;
        }

        private static bool GeometryIsBizarreSize(Size size)
        {
            return (size.Height <= Screen.PrimaryScreen.WorkingArea.Height &&
                size.Width <= Screen.PrimaryScreen.WorkingArea.Width);
        }

        public static string GeometryToString(Form mainForm)
        {
            return mainForm.Location.X.ToString() + "|" +
                mainForm.Location.Y.ToString() + "|" +
                mainForm.Size.Width.ToString() + "|" +
                mainForm.Size.Height.ToString() + "|" +
                mainForm.WindowState.ToString();
        }

        #endregion     
                       
        #region GUI Events

        #region Scene List Box Handeling     
   
        private void dataListViewScenes_CellRightClick_1(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripScenesNull;
            else
                e.MenuStrip = contextMenuStripScenes;
        }

        private void dataListViewScenes_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            scene selectedscene = (scene)dataListViewScenes.SelectedObject;
            if(selectedscene != null)
            {
                dataListViewSceneCMDs.Visible = true;                
                sceneCMDsList = ((IListSource)selectedscene.scene_commands).GetList() as IBindingList;
                dataListViewSceneCMDs.DataSource = sceneCMDsList;
                dataListViewSceneCMDs.Sort(this.SceneCMDOrderCol, SortOrder.Ascending);
                lbl_sceneActions.Text = "Scene " + selectedscene.id.ToString() + " '" + selectedscene.friendly_name + "' Actions";
            }
            else
            {
                dataListViewSceneCMDs.Visible = false;
                lbl_sceneActions.Text = "No Scene Selected";
            }
        }

        private void duplicateSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                scene selected_scene = (scene)dataListViewScenes.SelectedObject;

                if (selected_scene != null)
                {
                    scene new_scene = new scene { friendly_name = selected_scene.friendly_name + " Copy", sort_order = dataListViewScenes.GetItemCount() + 1 };
                        
                    foreach(scene_commands sc in selected_scene.scene_commands)
                    {
                        new_scene.scene_commands.Add(new scene_commands { arg = sc.arg,
                                                                          command_id = sc.command_id,
                                                                          command_type_id = sc.command_type_id,
                                                                          sort_order = sc.sort_order,
                                                                          device_id = sc.device_id});                                                                           

                    }
                    sceneList.Add(new_scene);
                    zvsEntityControl.zvsContext.SaveChanges();
                }
            }
        }

        private void deleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteScene();
        }

        private void dataListViewScenes_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteScene();
            }
        }

        private void deleteScene()
        {
            scene selected_scene = (scene)dataListViewScenes.SelectedObject;

            if (selected_scene != null)
            {
                if (MessageBox.Show("Are you sure you want to delete this scene?", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    zvsEntityControl.zvsContext.scenes.DeleteObject(selected_scene);
                                           
                    zvsEntityControl.zvsContext.SaveChanges();

                    foreach (scene s in sceneList)
                    {
                        s.sort_order = sceneList.IndexOf(s) + 1;     
                    }
                    zvsEntityControl.zvsContext.SaveChanges();
                }
            }           
            else
                MessageBox.Show("You must have at least one scene.", ProgramName);
        }

        private void addSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScene();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddScene();
        }

        private void AddScene()
        {
            scene new_s = new scene { friendly_name = "New Scene", sort_order = dataListViewScenes.GetItemCount() + 1 };
            sceneList.Add(new_s);
            dataListViewScenes.SelectedObject = new_s;
            zvsEntityControl.zvsContext.SaveChanges();
        }

        public class SceneDropSink : SimpleDropSink
        {
            public SceneDropSink()
            {
                this.CanDropBetween = false;
                this.CanDropOnBackground = false;
                this.CanDropOnItem = true;
            }

            protected override void OnModelCanDrop(ModelDropEventArgs args)
            {
                base.OnModelCanDrop(args);

                if (args.Handled)
                    return;

                args.Effect = DragDropEffects.Move;

                if (args.SourceListView != this.ListView)
                {
                    args.Effect = DragDropEffects.None;
                    args.DropTargetLocation = DropTargetLocation.None;
                    args.InfoMessage = "Cannot drop here.";
                }

                // If we are rearranging a list, don't allow drops on the background
                if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView)
                {
                    args.Effect = DragDropEffects.None;
                    args.DropTargetLocation = DropTargetLocation.None;
                }
            }

            protected override void OnModelDropped(ModelDropEventArgs args)
            {
                base.OnModelDropped(args);
            }
        }

        private void dataListViewScenes_ModelCanDrop_1(object sender, ModelDropEventArgs e)
        {
            if (e.SourceModels[0].GetType().Equals(typeof(scene)))
            {                   
                e.Effect = DragDropEffects.Move;
                e.InfoMessage = "Rearrage Order";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                e.InfoMessage = "Can not drop this here.";
            }
        }

        private void dataListViewScenes_ModelDropped_1(object sender, ModelDropEventArgs e)
        {
            int TargetIndex = 0;

            //Handle if dropped into empty Action box
            if (e.TargetModel == null)
                TargetIndex = 0;
            else
                TargetIndex = e.DropTargetIndex;

            //Handle Device Drop
            //REARRAGE
            if (e.SourceModels[0].GetType().Equals(typeof(scene)))
            {
                scene SourceScene = (scene)e.SourceModels[0];
                int SourceIndex = dataListViewScenes.IndexOf(SourceScene);

                if (SourceIndex != TargetIndex)
                {
                    SourceScene.sort_order = TargetIndex;

                    foreach (scene s in sceneList)
                    {
                        //moving up
                        if (SourceIndex > TargetIndex)
                        {
                            if (dataListViewScenes.IndexOf(s) >= TargetIndex && dataListViewScenes.IndexOf(s) <= SourceIndex && s != SourceScene)
                                s.sort_order++;
                        }
                        //moving down
                        else
                        {
                            if (dataListViewScenes.IndexOf(s) > SourceIndex && dataListViewScenes.IndexOf(s) <= TargetIndex && s != SourceScene)
                                s.sort_order--;
                        }
                    }
                }

                dataListViewScenes.SelectedObject = SourceScene;                
                zvsEntityControl.zvsContext.SaveChanges();
                e.RefreshObjects();
                dataListViewScenes.Sort(this.SceneSortCol, SortOrder.Ascending);
            }           
        }

        private void OpenScenePropertiesWindow()
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                formSceneProperties = new formPropertiesScene((scene)dataListViewScenes.SelectedObject);         
                formSceneProperties.ShowDialog();
            }
        }

        private void dataListViewScenes_DoubleClick_1(object sender, EventArgs e)
        {
            OpenScenePropertiesWindow();
        }

        private void btn_EditScene_Click(object sender, EventArgs e)
        {
            OpenScenePropertiesWindow();
        }

        private void propertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenScenePropertiesWindow();
        }

        private void editSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenScenePropertiesWindow();
        }
        
        private void runSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scene selectedscene = (scene)dataListViewScenes.SelectedObject;

            if (selectedscene != null)
            {
                Logger.WriteToLog(Urgency.INFO, selectedscene.RunScene(), "MAIN");
            }
            else
                MessageBox.Show("Please select a scene.", ProgramName);
        }
       
        #endregion

        #region Scene Command Handling

        public class SceneCommandDropSink : SimpleDropSink
        {
            public SceneCommandDropSink()
            {
                this.CanDropBetween = false;
                this.CanDropOnBackground = true;
                this.CanDropOnItem = true;
            }

            protected override void OnModelCanDrop(ModelDropEventArgs args)
            {
                base.OnModelCanDrop(args);

                if (args.Handled)
                    return;

                args.Effect = DragDropEffects.Move;

                // If we are rearranging a list, don't allow drops on the background
                if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView)
                {
                    args.Effect = DragDropEffects.None;
                    args.DropTargetLocation = DropTargetLocation.None;
                }
            }

            protected override void OnModelDropped(ModelDropEventArgs args)
            {
                base.OnModelDropped(args);
            }
        }

        //scene commands 
        private void dataListViewSceneCMDs_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Item != null)
                e.MenuStrip = cmsSceneCMD;            
        }

        private void deleteCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected scene command(s)?",
                                zvsEntityControl.zvsNameAndVersion,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

                foreach (scene_commands selected_sceneCMD in dataListViewSceneCMDs.SelectedObjects)                
                    if (selected_sceneCMD != null)                    
                        zvsEntityControl.zvsContext.scene_commands.DeleteObject(selected_sceneCMD);

                zvsEntityControl.zvsContext.SaveChanges();
                
                foreach (scene_commands sc in sceneCMDsList)                
                    sc.sort_order = sceneCMDsList.IndexOf(sc);
                
               zvsEntityControl.zvsContext.SaveChanges();

               dataListViewSceneCMDs.Sort(this.SceneCMDOrderCol, SortOrder.Ascending);
            }
        }     

        private void editCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (scene_commands selected_sceneCMD in dataListViewSceneCMDs.SelectedObjects)
            {
                if (selected_sceneCMD != null)
                {
                    if (selected_sceneCMD.scene.is_running)
                    {
                        MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                        return;
                    }

                    switch ((command_types)selected_sceneCMD.command_type_id)
                    {
                        case command_types.builtin:
                            AddEditSceneBuiltinCMD b_cmd_editForm = new AddEditSceneBuiltinCMD(selected_sceneCMD);
                            b_cmd_editForm.ShowDialog();
                            break;
                        case command_types.device_command:
                        case command_types.device_type_command:
                            {
                                device d = zvsEntityControl.zvsContext.devices.Single(o => o.id == selected_sceneCMD.device_id);
                                if (d != null)
                                {
                                    AddEditSceneDeviceCMD editCMDform = new AddEditSceneDeviceCMD(d, selected_sceneCMD);
                                    editCMDform.ShowDialog();
                                }                            
                            break;
                            }
                    }


                }
            }
        }               

        private void dataListViewSceneCMDs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteCommandToolStripMenuItem_Click(this, new EventArgs());
            }
        }

        private void dataListViewSceneCMDs_DoubleClick(object sender, EventArgs e)
        {
            editCommandToolStripMenuItem_Click(this, new EventArgs());
        }

        private void dataListViewSceneCMDs_ModelCanDrop(object sender, ModelDropEventArgs e)
        {
            if (e.SourceModels[0].GetType().Equals(typeof(device)))
            {
                e.Effect = DragDropEffects.Copy;
                e.InfoMessage = "Create new action for this device";
            }
            else if (e.SourceModels[0].GetType().Equals(typeof(scene_commands)))
            {
                e.Effect = DragDropEffects.Move;
                e.InfoMessage = "Rearrage Order";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                e.InfoMessage = "Can not drop this here.";
            }
        }

        private void btnAddBltInCMD_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                scene selected_scene = (scene)dataListViewScenes.SelectedObject;

                scene_commands scmd = new scene_commands { scene_id  = selected_scene.id, sort_order = selected_scene.scene_commands.Count()};                
                AddEditSceneBuiltinCMD addCMDform = new AddEditSceneBuiltinCMD(scmd, sceneCMDsList);
                addCMDform.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a scene!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void dataListViewSceneCMDs_ModelDropped(object sender, ModelDropEventArgs e)
        {
            int TargetIndex = 0;

            //Handle if dropped into empty Action box
            if (e.TargetModel == null)
                TargetIndex = 0;
            else
                TargetIndex = e.DropTargetIndex;

           // Handle Device Drop
            if (e.SourceModels[0].GetType().Equals(typeof(device)))
            {
                scene selected_scene = (scene)dataListViewScenes.SelectedObject;
                if (selected_scene != null)
                {
                    if (selected_scene.is_running)
                    {
                        MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                        return;
                    }

                    foreach (device selected_device in e.SourceModels)
                    {
                        if (selected_device != null)
                        {
                            scene_commands scmd = new scene_commands { scene_id = selected_scene.id, sort_order = selected_scene.scene_commands.Count()};
                            AddEditSceneDeviceCMD addCMDform = new AddEditSceneDeviceCMD(selected_device, scmd, sceneCMDsList);
                            addCMDform.ShowDialog();
                        }                        
                    }
                }      
            }
            else if (e.SourceModels[0].GetType().Equals(typeof(scene_commands)))
            {
                //Rearrage Actions
                scene selected_scene = (scene)dataListViewScenes.SelectedObject;
                if (selected_scene != null)
                {
                    //DIE if running.
                    if (selected_scene.is_running)
                    {
                        MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                        return;
                    }

                    scene_commands SourceSceneCMD = (scene_commands)e.SourceModels[0];
                    int SourceIndex = dataListViewSceneCMDs.IndexOf(SourceSceneCMD);

                    if (SourceIndex != TargetIndex)
                    {
                        SourceSceneCMD.sort_order = TargetIndex ;

                        foreach (scene_commands sc in sceneCMDsList)
                        {
                            //moving up
                            if (SourceIndex > TargetIndex)
                            {
                                if (dataListViewSceneCMDs.IndexOf(sc) >= TargetIndex && dataListViewSceneCMDs.IndexOf(sc) <= SourceIndex && sc != SourceSceneCMD)
                                    sc.sort_order++;
                            }
                            //moving down
                            else
                            {
                                if (dataListViewSceneCMDs.IndexOf(sc) > SourceIndex && dataListViewSceneCMDs.IndexOf(sc) <= TargetIndex && sc != SourceSceneCMD)
                                    sc.sort_order--;
                            }
                        }
                    }

                    dataListViewSceneCMDs.SelectedObject = SourceSceneCMD;
                    zvsEntityControl.zvsContext.SaveChanges();
                    e.RefreshObjects();
                    dataListViewSceneCMDs.Sort(this.SceneCMDOrderCol, SortOrder.Ascending);
                }
            }
        }    

        #endregion

        #region Object List Box Handling

        private void dataListViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            {
                device d = (device)dataListViewDevices.SelectedObjects[0];
                if (d != null)
                {
                    uc_object_values1.UpdateControl(d);
                    uc_object_values1.Visible = true;
                }
                else
                    uc_object_values1.UpdateControl(null);
            }
            else
            {
                uc_object_values1.UpdateControl(null);
            }
        }       
        
        private void OpenDevicePropertyWindow()
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            {
                foreach (device d in dataListViewDevices.SelectedObjects)
                {
                    DeviceProperties properties = new DeviceProperties(d);
                    properties.Show();
                }
            }
            else
                MessageBox.Show("Please select at least one object.", "zVirtualScenes");
        }

        private void dataListViewDevices_DoubleClick(object sender, EventArgs e)
        {           
            OpenDevicePropertyWindow();
        }

        private void dataListViewDevices_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripDevicesNull;
            else
            {
                ContextMenuStrip contextMenuStripDevicesDynamicCMDs = new ContextMenuStrip();
                contextMenuStripDevicesDynamicCMDs.Items.Clear();

                device d = (device)e.Item.RowObject;

                //Create a new context menu
                ToolStripMenuItem CMDcontainer = new ToolStripMenuItem();
                CMDcontainer.Text = d.friendly_name;
                CMDcontainer.Tag = "dynamic_cmd_menu";

                foreach (device_type_commands c in d.device_types.device_type_commands)
                {
                    switch ((Data_Types)c.arg_data_type)
                    {
                        case Data_Types.NONE:
                            {
                                ToolStripMenuItem item = new ToolStripMenuItem();
                                item.Name = string.Empty;
                                item.Text = c.friendly_name;
                                item.ToolTipText = c.description;
                                item.Tag = c;                                
                                item.Click += new EventHandler(dynamic_CMD_item_Click);
                                CMDcontainer.DropDownItems.Add(item);
                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //ROOT MENU
                                ToolStripMenuItem item = new ToolStripMenuItem();
                                item.Name = string.Empty;
                                item.Text = c.friendly_name;
                                item.ToolTipText = c.description;

                                foreach (device_type_command_options option in c.device_type_command_options)
                                {                                   
                                    ToolStripMenuItem option_item = new ToolStripMenuItem();
                                    option_item.Name = option.option;
                                    option_item.Text = option.option;
                                    option_item.Tag = c;
                                    option_item.Click += new EventHandler(dynamic_CMD_item_Click);
                                    item.DropDownItems.Add(option_item);
                                }
                                CMDcontainer.DropDownItems.Add(item);
                                break;
                            }
                    }
                }

                foreach (device_commands c in d.device_commands)
                {
                    switch ((Data_Types)c.arg_data_type)
                    {
                        case Data_Types.NONE:
                            {
                                ToolStripMenuItem item = new ToolStripMenuItem();
                                item.Name = string.Empty;
                                item.Text = c.friendly_name;
                                item.ToolTipText = c.description;
                                item.Tag = c;
                                item.Click += new EventHandler(dynamic_CMD_item_Click);
                                CMDcontainer.DropDownItems.Add(item);
                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //ROOT MENU
                                ToolStripMenuItem item = new ToolStripMenuItem();
                                item.Name = string.Empty;
                                item.Text = c.friendly_name;
                                item.ToolTipText = c.description;

                                foreach (device_command_options option in c.device_command_options)
                                {
                                    ToolStripMenuItem option_item = new ToolStripMenuItem();
                                    option_item.Name = option.name;
                                    option_item.Text = option.name;
                                    option_item.Tag = c;
                                    option_item.Click += new EventHandler(dynamic_CMD_item_Click);
                                    item.DropDownItems.Add(option_item);
                                }
                                CMDcontainer.DropDownItems.Add(item);
                                break;
                            }
                    }
                }

                e.MenuStrip = contextMenuStripDevicesDynamicCMDs;
                if (CMDcontainer.DropDownItems.Count > 0)
                    contextMenuStripDevicesDynamicCMDs.Items.Insert(0, CMDcontainer);

                ToolStripMenuItem repoll = new ToolStripMenuItem();
                repoll.Text = "Repoll Device";
                repoll.Click += new EventHandler(Repoll_Click);
                repoll.Tag = d;

                contextMenuStripDevicesDynamicCMDs.Items.Add(repoll);
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Delete Device", null, new EventHandler(DeleteDevice_Click)));
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Activate Groups", null, new EventHandler(ActivateGroups_Click)));
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Edit Groups", null, new EventHandler(EditGroups_Click)));                
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Properties", null, new EventHandler(deviceProperties_Click)));

            }
        }   

        private void dynamic_CMD_item_Click(object sender, EventArgs e)
        {
            //Item.Name = CMD ARG
            //Item.TAg = CMD object
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Tag.GetType() == typeof(device_type_commands))
            {
                device_type_commands cmd = (device_type_commands)item.Tag;                    
                device d = (device)dataListViewDevices.SelectedObject;
                if (d != null)
                    cmd.Run(d.id,item.Name);
            }

            else if (item.Tag.GetType() == typeof(device_commands))
            {
                device_commands cmd = (device_commands)item.Tag;
                cmd.Run(item.Name);
            }
            
        }

        private void Repoll_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            device d = (device)item.Tag;

            builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "REPOLL_ME");
            if (cmd != null)                         
                cmd.Run(d.id.ToString());           
        }

        private void DeleteDevice_Click(object sender, EventArgs e)
        {
            device d = (device)dataListViewDevices.SelectedObject;
            if (d != null)
            {
                deviceList.Remove(d);
                zvsEntityControl.zvsContext.SaveChanges();
            }
        }

        private void EditGroups_Click(object sender, EventArgs e)
        {
            ShowGroupEditor();
        }

        private  void  deviceProperties_Click(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }

        private void ActivateGroups_Click(object sender, EventArgs e)
        {
            ShowActivateGroups();
        }                      

        private void devicePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }

        private void manuallyRepollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "REPOLL_ME");
            if (cmd != null)
            {
                foreach (device selecteddevice in dataListViewDevices.SelectedObjects)
                    cmd.Run(selecteddevice.id.ToString());
            }
        }

        private void repollAllDevicesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "REPOLL_ALL");
            if (cmd != null)
                cmd.Run(); 
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }

        #endregion        

        #region ToolBar Events

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramSettings formProgramSettingss = new ProgramSettings(this);
            formProgramSettingss.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editGroupsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ShowGroupEditor();
        }

        private void acitvateGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowActivateGroups();
        }

        //private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //DatabaseConnection FormDatabaseConnection = new DatabaseConnection();
        //    //FormDatabaseConnection.ShowDialog();
        //}

        //private void entireDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    switch (MessageBox.Show("Are you sure you want to delete all data?  ALL YOUR PLUGIN SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
        //    {
        //        case DialogResult.Yes:
        //            //if (string.IsNullOrEmpty(zvsAPI.Database.ClearDatabase()))
        //                Environment.Exit(0);
        //            break;
        //    }
        //}      

        private void repollAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "REPOLL_ALL");
            if (cmd != null)
                cmd.Run(); 
        }

        private void SetNamesDevOnly()
        {
            foreach (device d in zvsEntityControl.zvsContext.devices)
            {
                switch (d.node_id)
                {
                    case 1:
                        d.friendly_name = "Aeon Labs Z-Stick Series 2";
                        break;
                    case 3:
                        d.friendly_name = "Master Bathtub Light";
                        break;
                    case 4:
                        d.friendly_name = "Masterbath Mirror Light";
                        break;                    
                    case 5:
                        d.friendly_name = "Masterbed Hallway Light";
                        break;
                    case 6:
                        d.friendly_name = "Masterbed East Light";
                        break;
                    case 7:
                        d.friendly_name = "Masterbed Bed Light";
                        break;
                    case 8:
                        d.friendly_name = "Office Light";
                        break;
                    case 9:
                        d.friendly_name = "Family Hallway Light";
                        break;
                    case 10:
                        d.friendly_name = "Outside Entry Light";
                        break;
                    case 11:
                        d.friendly_name = "Entryway Light";
                        break;
                    case 12:
                        d.friendly_name = "Can Lights";
                        break;
                    case 13:
                        d.friendly_name = "Pourch Light";
                        break;
                    case 14:
                        d.friendly_name = "Dining Table Light";
                        break;
                    case 15:
                        d.friendly_name = "Fan Light";
                        break;
                    case 16:
                        d.friendly_name = "Kitchen Light";
                        break;
                    case 17:
                        d.friendly_name = "Rear Garage Light";
                        break;
                    case 18:
                        d.friendly_name = "Driveway Light";
                        break;
                    case 19:
                        d.friendly_name = "TV Backlight";
                        break;
                    case 20:
                        d.friendly_name = "Fireplace Light";
                        break;                
                    case 22:
                        d.friendly_name = "Label Printer";
                        break;
                    case 23:
                        d.friendly_name = "Brother Printer";
                        break;
                    case 24:
                        d.friendly_name = "South Thermostat";
                        break;
                    case 25:
                        d.friendly_name = "Masterbed Window Fan";
                        break;
                    case 26:
                        d.friendly_name = "Masterbed Thermostat";
                       break;
                    case 27:
                       d.friendly_name = "Aeon Labs Z-Stick Series 1 (Secondary)";
                       break;
                }
            }
            zvsEntityControl.zvsContext.SaveChanges();
        }
        #endregion

        private void MainTabControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F5))
            {
                builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "REPOLL_ALL");
                if (cmd != null)                
                    cmd.Run();                  
            }

            if ((e.KeyCode == Keys.F6))
            {
                //TEMP FUNCTION
                SetNamesDevOnly();
            }

        }

        #endregion                 

        #region Task Scheduler Execution

        private void timer_TaskRunner_Tick(object sender, EventArgs e)
        {
            
                foreach (scheduled_tasks task in taskList)
                {
                    if (task.Enabled)
                    {
                        if (task.Frequency.HasValue)
                        {
                            switch ((scheduled_tasks.frequencys)task.Frequency)
                            {
                                case scheduled_tasks.frequencys.Seconds:
                                    if (task.StartTime.HasValue)
                                    {
                                        int sec = (int)(DateTime.Now - task.StartTime.Value).TotalSeconds;
                                        if (sec % task.RecurSeconds == 0)
                                        {
                                            task.Run();
                                        }
                                    }
                                    break;
                                case scheduled_tasks.frequencys.Daily:
                                    if (task.StartTime.HasValue)
                                    {
                                        if ((DateTime.Now.Date - task.StartTime.Value.Date).TotalDays % task.RecurDays == 0)
                                        {
                                            Double SecondsBetweenTime = (task.StartTime.Value.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
                                            if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
                                                task.Run();
                                        }
                                    }
                                    break;
                                case scheduled_tasks.frequencys.Weekly:
                                    if (task.StartTime.HasValue)
                                    {
                                        if (((Int32)(DateTime.Now.Date - task.StartTime.Value.Date).TotalDays / 7) % task.RecurWeeks == 0)  //IF RUN THIS WEEK
                                        {
                                            if (ShouldRunToday(task))  //IF RUN THIS DAY 
                                            {
                                                Double SecondsBetweenTime = (task.StartTime.Value.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
                                                if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
                                                    task.Run();
                                            }
                                        }
                                    }
                                    break;
                                case scheduled_tasks.frequencys.Once:
                                    if (task.StartTime.HasValue)
                                    {
                                        Double SecondsBetween = (DateTime.Now - task.StartTime.Value).TotalSeconds;
                                        if (SecondsBetween < 1 && SecondsBetween > 0)
                                            task.Run();
                                    }
                                    break;
                            }
                        }
                    }
                }

        }

        private bool ShouldRunToday(scheduled_tasks task)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (task.RecurMonday.HasValue && task.RecurMonday.Value)
                        return true;
                    break;

                case DayOfWeek.Tuesday:
                    if (task.RecurTuesday.HasValue && task.RecurTuesday.Value)
                        return true;
                    break;

                case DayOfWeek.Wednesday:
                    if (task.RecurWednesday.HasValue && task.RecurWednesday.Value)
                        return true;
                    break;

                case DayOfWeek.Thursday:
                    if (task.RecurThursday.HasValue && task.RecurThursday.Value)
                        return true;
                    break;

                case DayOfWeek.Friday:
                    if (task.RecurFriday.HasValue && task.RecurFriday.Value)
                        return true;
                    break;

                case DayOfWeek.Saturday:
                    if (task.RecurSaturday.HasValue && task.RecurSaturday.Value)
                        return true;
                    break;

                case DayOfWeek.Sunday:
                    if (task.RecurTuesday.HasValue && task.RecurTuesday.Value)
                        return true;
                    break;
            }

            return false;
        }        

        public static int NumberOfWeeks(DateTime dateFrom, DateTime dateTo)
        {
            TimeSpan Span = dateTo.Subtract(dateFrom);

            if (Span.Days <= 7)
            {
                if (dateFrom.DayOfWeek > dateTo.DayOfWeek)
                {
                    return 2;
                }

                return 1;
            }

            int Days = Span.Days - 7 + (int)dateFrom.DayOfWeek;
            int WeekCount = 1;
            int DayCount = 0;

            for (WeekCount = 1; DayCount < Days; WeekCount++)
            {
                DayCount += 7;
            }

            return WeekCount;
        }

        #endregion

        #region Task Scheduler GUI

        #region Methods

        private void LoadGui(scheduled_tasks Task)
        {
            textBox_TaskName.Enabled = true;
            comboBox_FrequencyTask.Enabled = true;
            numericUpDownOccurDays.Enabled = true;
            checkBox_EnabledTask.Enabled = true;
            dateTimePickerStartTask.Enabled = true;
            numericUpDownOccurWeeks.Enabled = true;
            numericUpDownOccurSeconds.Enabled = true;
            checkBox_RecurMonday.Enabled = true;
            checkBox_RecurTuesday.Enabled = true;
            checkBox_RecurWednesday.Enabled = true;
            checkBox_RecurThursday.Enabled = true;
            checkBox_RecurFriday.Enabled = true;
            checkBox_RecurSaturday.Enabled = true;
            checkBox_RecurSunday.Enabled = true;
            comboBox_ActionsTask.Enabled = true;

            textBox_TaskName.Text = Task.friendly_name;
            checkBox_EnabledTask.Checked = Task.Enabled;

            if (Task.Frequency.HasValue) 
                comboBox_FrequencyTask.SelectedIndex = (int)Task.Frequency;
           
            if(Task.StartTime.HasValue)
                dateTimePickerStartTask.Value = Task.StartTime.Value;

            if (Task.RecurWeeks.HasValue)
            numericUpDownOccurWeeks.Value = (decimal)Task.RecurWeeks;

            if (Task.RecurDays.HasValue)
            numericUpDownOccurDays.Value = (decimal)Task.RecurDays;

            if (Task.RecurSeconds.HasValue)
            numericUpDownOccurSeconds.Value = (decimal)Task.RecurSeconds;

            if (Task.RecurMonday.HasValue)
            checkBox_RecurMonday.Checked = Task.RecurMonday.Value;

            if (Task.RecurTuesday.HasValue)
            checkBox_RecurTuesday.Checked = Task.RecurTuesday.Value;

            if (Task.RecurWednesday.HasValue)
            checkBox_RecurWednesday.Checked = Task.RecurWednesday.Value;

            if (Task.RecurThursday.HasValue)
            checkBox_RecurThursday.Checked = Task.RecurThursday.Value;

            if (Task.RecurFriday.HasValue)
            checkBox_RecurFriday.Checked = Task.RecurFriday.Value;

            if (Task.RecurSaturday.HasValue)
            checkBox_RecurSaturday.Checked = Task.RecurSaturday.Value;

            if (Task.RecurSunday.HasValue)
            checkBox_RecurSunday.Checked = Task.RecurSunday.Value;

            //Look for Scene, if it was deleted then set index to -1
            scene selected_scene = zvsEntityControl.zvsContext.scenes.SingleOrDefault(s => s.id == Task.Scene_id);
            if(selected_scene != null)
                comboBox_ActionsTask.SelectedItem = selected_scene;
            else
                comboBox_ActionsTask.SelectedIndex = -1;           
        }

        private void AddNewTask()
        {
            scheduled_tasks task = new scheduled_tasks { friendly_name = "New Task", Frequency = (int)scheduled_tasks.frequencys.Daily, Scene_id = 0, Enabled = false };
            taskList.Add(task);
            zvsEntityControl.zvsContext.SaveChanges();
        }

        #endregion

        #region GUI Event Handling

        private void dataListTasks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteTask();
            }
        }

        private void comboBox_FrequencyTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox_Daily.Visible = false;
            groupBox_Weekly.Visible = false;
            groupBox_Seconds.Visible = false;

            switch (comboBox_FrequencyTask.SelectedIndex)
            {
                case (int)scheduled_tasks.frequencys.Daily:
                    groupBox_Daily.Visible = true;
                    break;
                case (int)scheduled_tasks.frequencys.Weekly:
                    groupBox_Weekly.Visible = true;
                    break;
                case (int)scheduled_tasks.frequencys.Seconds:
                    groupBox_Seconds.Visible = true;
                    break;
            }
        }

        private void button_SaveTask_Click(object sender, EventArgs e)
        {
            scheduled_tasks SelectedTask = (scheduled_tasks)dataListTasks.SelectedObject;
            if (SelectedTask != null)
            {
                //Action
                if (comboBox_ActionsTask.SelectedIndex != -1)
                {
                    scene SelectedScene = (scene)comboBox_ActionsTask.SelectedItem;
                    if (SelectedScene != null)
                        SelectedTask.Scene_id = SelectedScene.id;
                }
                else
                {
                    MessageBox.Show("Error Saving\n\nPlease select a scene to activate before saving.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Task Name
                if (textBox_TaskName.Text != "")
                    SelectedTask.friendly_name = textBox_TaskName.Text;
                else
                {
                    MessageBox.Show("Error Saving\n\nTask name not vaild.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Frequency
                SelectedTask.Frequency = (int)Enum.Parse(typeof(scheduled_tasks.frequencys), comboBox_FrequencyTask.SelectedItem.ToString());

                //Endabled 
                SelectedTask.Enabled = checkBox_EnabledTask.Checked;

                //DateTime
                SelectedTask.StartTime = dateTimePickerStartTask.Value;

                //Recur Days 
                if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(scheduled_tasks.frequencys.Daily.ToString()))
                {
                    SelectedTask.RecurDays = (int)numericUpDownOccurDays.Value;
                }
                else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(scheduled_tasks.frequencys.Seconds.ToString()))
                {
                    SelectedTask.RecurSeconds = (int)numericUpDownOccurSeconds.Value;
                }
                else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(scheduled_tasks.frequencys.Weekly.ToString()))
                {
                    #region Weekly
                    SelectedTask.RecurWeeks = (int)numericUpDownOccurWeeks.Value;
                    SelectedTask.RecurMonday = checkBox_RecurMonday.Checked;
                    SelectedTask.RecurTuesday = checkBox_RecurTuesday.Checked;
                    SelectedTask.RecurWednesday = checkBox_RecurWednesday.Checked;
                    SelectedTask.RecurThursday = checkBox_RecurThursday.Checked;
                    SelectedTask.RecurFriday = checkBox_RecurFriday.Checked;
                    SelectedTask.RecurSaturday = checkBox_RecurSaturday.Checked;
                    SelectedTask.RecurSunday = checkBox_RecurSunday.Checked;
                    #endregion
                }               

                zvsEntityControl.zvsContext.SaveChanges();
            }
        }

        private void dataListTasks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedObject != null)
                LoadGui((scheduled_tasks)dataListTasks.SelectedObject);
            else
            {
                textBox_TaskName.Enabled = false;
                comboBox_FrequencyTask.Enabled = false;
                numericUpDownOccurWeeks.Enabled = false;
                numericUpDownOccurSeconds.Enabled = false;
                numericUpDownOccurDays.Enabled = false;
                checkBox_EnabledTask.Enabled = false;
                dateTimePickerStartTask.Enabled = false;
                checkBox_RecurMonday.Enabled = false;
                checkBox_RecurTuesday.Enabled = false;
                checkBox_RecurWednesday.Enabled = false;
                checkBox_RecurThursday.Enabled = false;
                checkBox_RecurFriday.Enabled = false;
                checkBox_RecurSaturday.Enabled = false;
                checkBox_RecurSunday.Enabled = false;
                comboBox_ActionsTask.Enabled = false;
            }

        }

        private void deleteTask()
        {
            scheduled_tasks task = (scheduled_tasks)dataListTasks.SelectedObject;

            if (task != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the '" + task.friendly_name + "' task?", "Are you sure?",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    taskList.Remove(task);
                    zvsEntityControl.zvsContext.SaveChanges();
                }
            }
            else
                MessageBox.Show("Please select a task to delete.", ProgramName);
        }

        private void dataListTasks_CellRightClick_1(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripTasksNull;
            else
                e.MenuStrip = contextMenuStripTasks;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTask();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteTask();
        }

        private void toolStripAddTaks_Click(object sender, EventArgs e)
        {
            AddNewTask();
        }

        #endregion

        #endregion

        #region Groups
        private void ShowActivateGroups()
        {
            if (grpActivateForm == null || grpActivateForm.IsDisposed)
            {
                grpActivateForm = new ActivateGroup();
                grpActivateForm.Show();
            }

            grpActivateForm.Activate();
        }

        private void ShowGroupEditor()
        {
            if (grpEditorForm == null || grpEditorForm.IsDisposed)
            {
                grpEditorForm = new GroupEditor();
                grpEditorForm.Show();
            }

            grpEditorForm.Activate();
        }
        #endregion

        #region Events
        private void btnNewEvent_Click(object sender, EventArgs e)
        {
            //ScriptEditor scriptEditor = new ScriptEditor(0);
            //scriptEditor.Show();
        }

        private void btnEditEvent_Click(object sender, EventArgs e)
        {
            if (dataListEvents.SelectedIndex > -1)
            {
                //int scriptId;
                //int.TryParse(_masterEvents.Rows[dataListEvents.SelectedIndex]["id"].ToString(), out scriptId);
                //ScriptEditor scriptEditor = new ScriptEditor(scriptId);
                //scriptEditor.Show();
            }
        }

        private void btnDeleteEvent_Click(object sender, EventArgs e)
        {
            if (dataListEvents.SelectedIndex > -1)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this event?", "Are you sure?",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                 //   int scriptId;
                  //  int.TryParse(_masterEvents.Rows[dataListEvents.SelectedIndex]["id"].ToString(), out scriptId);
                    //DatabaseControl.DeleteEventScript(scriptId);
                 //   UpdateScripts = true;
                }
            }
        }
        #endregion 

        
         
    }
}