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
        public string ProgramName = zvsEntityControl.GetProgramNameAndVersion;
        private ObjectQuery<device> deviceListQuery;
        private IBindingList sceneList;
        private IBindingList sceneCMDsList;

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
        private DataTable _masterEvents;

        // Plugin Stuff
        public PluginManager pm;

        public static bool UpdateScripts;  

        public MainForm()
        {
            InitializeComponent();

            //Load form size
            GeometryFromString(Properties.Settings.Default.WindowGeometry, this);
            FormClosing += zVirtualScenes_FormClosing;

            this.MeterCol.Renderer = new BarRenderer(0, 99);
            dataListViewDevices.CellRightClick += dataListViewDevices_CellRightClick;

           // dataListTasks.CellRightClick += dataListTasks_CellRightClick;            
            Logger.LogItemPostAdd += Logger_LogItemPostAdd;
            
            //Events
            builtin_command_que.BuiltinCommandRunCompleteEvent += new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);
            device_type_command_que.DeviceTypeCommandRunCompleteEvent += new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);
            device_command_que.DeviceCommandRunCompleteEvent += new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);
            zvsEntityControl.SceneRunStartedEvent += new zvsEntityControl.SceneRunStartedEventHandler(zvsEntityControl_SceneRunStartedEvent);
            zvsEntityControl.SceneRunCompleteEvent += new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent);

            //zvsEvents.ValueDataChangedEvent +=new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);
            //zvsEvents.SceneChangedEvent += new zvsEvents.SceneChangedEventHandler(zVirtualSceneEvents_SceneChangedEvent);
            //zvsEvents.SceneCMDChangedEvent += new zvsEvents.SceneCMDChangedEventHandler(zVirtualSceneEvents_SceneCMDChangedEvent);
            
            
            //zvsEvents.CommandRunCompleteEvent += new zvsEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
            //zvsEvents.ScheduledTaskChangedEvent += new zvsEvents.ScheduledTaskChangedEventHandler(zVirtualSceneEvents_ScheduledTaskChangedEvent);
        }

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {            
            this.Text = ProgramName;            
            dataListViewLog.DataSource = _masterlog;
            Logger.WriteToLog(Urgency.INFO, "STARTED", "MainForm");

            pm = new PluginManager();

            zvsEntityControl.zvsContext.SavingChanges += new EventHandler(zvsContext_SavingChanges);
        
            
            //Bind data to GUI elements
            deviceListQuery = zvsEntityControl.zvsContext.devices;
            
            dataListViewDevices.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);
            dataListViewDeviceSmallList.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);
          
            // Scenes            
            ObjectQuery<scene> sceneQuery1 =zvsEntityControl.zvsContext.scenes;
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
            //_masterTasks = zvsAPI.ScheduledTasks.GetTasks();

            //comboBox_FrequencyTask.DataSource = Enum.GetNames(typeof(Task.frequencys));
            //dataListTasks.DataSource = _masterTasks;
            //comboBox_ActionsTask.DataSource = _masterScenes;

            //Add default timer item if list is empty
           // if (_masterTasks.Count < 1)
            //    AddNewTask();
            //else
           //     dataListTasks.SelectedIndex = 0;
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

        void zvsContext_SavingChanges(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(zvsContext_SavingChanges), new object[] { sender, e });
            else
            {

                dataListViewDevices.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);
                dataListViewDeviceSmallList.DataSource = deviceListQuery.Execute(MergeOption.OverwriteChanges);
                //sceneList = new BindingList<scene>(sceneListQuery.OrderBy(s => s.sort_order).ToList());
                //dataListViewScenes.DataSource = ((ObjectQuery)(sceneListQuery.OrderBy(s => s.sort_order))).Execute(MergeOption.AppendOnly);


               // System.Timers.Timer t = (System.Timers.Timer)sender;
               // t.Enabled = false;

                //System.Timers.Timer t = new System.Timers.Timer();
                //t.Interval = 100;
                //t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
                //t.Enabled = true;
            }
        }

        public delegate void t_ElapsedDelegate(object sender, System.Timers.ElapsedEventArgs e);
        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new t_ElapsedDelegate(t_Elapsed), new object[] { sender, e });
            else
            {

                //dataListViewDevices.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);
                //dataListViewDeviceSmallList.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);


                //dataListViewScenes.DataSource = sceneList;
                //System.Timers.Timer t = (System.Timers.Timer)sender;
                //t.Enabled = false;

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

        void zvsEntityControl_SceneRunCompleteEvent(scene s, int ErrorCount)
        {
            if (this.InvokeRequired)
                this.Invoke(new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent), new object[] { s, ErrorCount });
            else
            {
                if (s != null)
                {
                    Logger.WriteToLog(Urgency.INFO, "Scene '" + s.friendly_name + "' has completed with " + ErrorCount + " errors.", "EVENT");
                }
            }
        }

      

        //void zVirtualSceneEvents_ValueChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        //{
        //    if (this.InvokeRequired)
        //        this.Invoke(new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent), new object[] { ObjectId, ValueID, label, Value, PreviousValue });
        //    else
        //    {
        //        //string objName = zvsAPI.Object.GetObjectName(ObjectId);
        //        ////labelLastEvent.Text = objName + " " + label + " changed to " + Value + ".";                

        //        //if (String.IsNullOrEmpty(objName))
        //        //    objName = "Object#" + ObjectId;

        //        //if (!String.IsNullOrEmpty(PreviousValue))
        //        //    Logger.WriteToLog(UrgencyLevel.INFO, objName + " " + label + " changed to " + Value + " from " + PreviousValue + ".", "EVENT");
        //        //else
        //        //    Logger.WriteToLog(UrgencyLevel.INFO, objName + " " + label + " changed to " + Value + ".", "EVENT");

        //        //RefreshDeviceList();
        //        //RefreshObjectValuesUserControl();

        //    }
        //}


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

        void zVirtualSceneEvents_ScheduledTaskChangedEvent(int TaskID)
        {
            //if (this.InvokeRequired)
            //    this.Invoke(new zvsEvents.ScheduledTaskChangedEventHandler(zVirtualSceneEvents_ScheduledTaskChangedEvent), new object[] { TaskID });
            //else
            //{
            //    _masterTasks = zvsAPI.ScheduledTasks.GetTasks();

            //    //Select it 
            //    dataListTasks.DataSource = null;
            //    dataListTasks.DataSource = _masterTasks;

            //    Task task = _masterTasks.SingleOrDefault(t => t.ID == TaskID);
            //    if (task != null)
            //        dataListTasks.SelectedObject = task;
            //}
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
                if (MessageBox.Show("Are you sure you want to delete this scene?", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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
                selectedscene.RunScene();
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
                                zvsEntityControl.GetProgramNameAndVersion,
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
                MessageBox.Show("Please select a scene!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                    ObjectProperties properties = new ObjectProperties(d);
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
            //    ContextMenuStrip contextMenuStripDevicesDynamicCMDs = new ContextMenuStrip();
            //    contextMenuStripDevicesDynamicCMDs.Items.Clear();

            //    zwObject zwObj = (zwObject)e.Item.RowObject;
                               
            //    //Create a new context menu
            //    ToolStripMenuItem CMDcontainer = new ToolStripMenuItem();
            //    CMDcontainer.Name = zwObj.Name;
            //    CMDcontainer.Text = zwObj.Name;
            //    CMDcontainer.Tag = "dynamic_cmd_menu";

            //    //API.GetAllObjectTypeCommandForObject(zwObj.ID).Rows
            //    List<Command> CommandsForThisObject = zvsAPI.Commands.GetAllObjectCommandsForObjectasCMD(zwObj.ID);
            //    CommandsForThisObject.AddRange(zvsAPI.Commands.GetAllObjectTypeCommandsForObjectasCMD(zwObj.ID));

            //    foreach (Command c in CommandsForThisObject)
            //    {
            //        switch (c.paramType)
            //        {
            //            case Data_Types.NONE:
            //            {
            //                QuedCommand zCMD = new QuedCommand();
            //                zCMD.CommandId = c.CommandId;
            //                zCMD.cmdtype = c.cmdtype;
            //                zCMD.ObjectId = zwObj.ID;
                                
            //                ToolStripMenuItem item = new ToolStripMenuItem();
            //                item.Name = c.Name;
            //                item.Text = c.FriendlyName;
            //                item.ToolTipText = c.HelpText;
            //                item.Tag = zCMD;
            //                item.Click += new EventHandler(dynamic_CMD_item_Click);
            //                CMDcontainer.DropDownItems.Add(item);
            //                break;
            //            }
            //            case Data_Types.LIST:
            //            {
            //                //ROOT MENU
            //                ToolStripMenuItem item = new ToolStripMenuItem();
            //                item.Name = c.Name;
            //                item.Text = c.FriendlyName;
            //                item.ToolTipText = c.HelpText;

            //                //MAKE SUB MENU
            //                List<string> options = new List<string>();
            //                if (c.cmdtype == commandScopeType.Object)
            //                    options = zvsAPI.Commands.GetObjectCommandOptions(c.CommandId);
            //                else if (c.cmdtype == commandScopeType.ObjectType)
            //                    options = zvsAPI.Commands.GetObjectTypeCommandOptions(c.CommandId);

            //                foreach (string option in options)
            //                {
            //                    QuedCommand zCMD = new QuedCommand();
            //                    zCMD.CommandId = c.CommandId;
            //                    zCMD.cmdtype = c.cmdtype;
            //                    zCMD.ObjectId = zwObj.ID;
            //                    zCMD.Argument = option;

            //                    ToolStripMenuItem option_item = new ToolStripMenuItem();
            //                    option_item.Name = option;
            //                    option_item.Text = option;
            //                    option_item.Tag = zCMD;
            //                    option_item.Click += new EventHandler(dynamic_CMD_item_Click);
            //                    item.DropDownItems.Add(option_item);
            //                }
            //                CMDcontainer.DropDownItems.Add(item);
            //                break;
            //            }
            //        }
            //    }

            //    e.MenuStrip = contextMenuStripDevicesDynamicCMDs;
            //    if (CMDcontainer.DropDownItems.Count > 0)
            //        contextMenuStripDevicesDynamicCMDs.Items.Insert(0, CMDcontainer);

            //    ToolStripMenuItem repoll = new ToolStripMenuItem();
            //    repoll.Name = "Repoll Device";
            //    repoll.Text = "Repoll Device";
            //    repoll.Click += new EventHandler(Repoll_Click);
            //    repoll.Tag = zwObj.ID;

            //    contextMenuStripDevicesDynamicCMDs.Items.Add(repoll);
            //    contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
            //    contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Activate Groups", null, new EventHandler(ActivateGroups_Click)));
            //    contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Edit Groups", null, new EventHandler(EditGroups_Click)));
            //    contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
            //    contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Properties", null, new EventHandler(deviceProperties_Click)));

            }
        }   

        private void dynamic_CMD_item_Click(object sender, EventArgs e)
        {
            //ToolStripMenuItem item = (ToolStripMenuItem)sender;
            //QuedCommand zCMD = (QuedCommand)item.Tag;
            //zvsAPI.Commands.InstallQueCommandAndProcess(zCMD);        
        }

        private void Repoll_Click(object sender, EventArgs e)
        {
            //ToolStripMenuItem item = (ToolStripMenuItem)sender;          
            //int id = (int)item.Tag;

            //int cmdId = zvsAPI.Commands.GetBuiltinCommandId("REPOLL_ME");
            //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Builtin, CommandId = cmdId, Argument = id.ToString() });
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
            //if (dataListViewDevices.SelectedObjects.Count > 0)
            //    foreach (ZWaveDevice selecteddevice in dataListViewDevices.SelectedObjects)
            //        ControlThinkInt.RepollDevices(selecteddevice.NodeID);
            //else
            //    MessageBox.Show("Please select at least one device.", ProgramName);
        }

        private void repollAllDevicesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //int cmdId = zvsAPI.Commands.GetBuiltinCommandId("REPOLL_ALL");
            //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Builtin, CommandId = cmdId});
        }

        private void findNewDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ControlThinkInt.ConnectAndFindDevices();
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


        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DatabaseConnection FormDatabaseConnection = new DatabaseConnection();
            //FormDatabaseConnection.ShowDialog();
        }

        private void entireDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all data?  ALL YOUR PLUGIN SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    //if (string.IsNullOrEmpty(zvsAPI.Database.ClearDatabase()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void pluginDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all plugin data?  ALL YOUR PLUGIN SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    //if (string.IsNullOrEmpty(zvsAPI.PluginSettings.ClearAllPluginData()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void objectPropertyDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all object property data?  ALL YOUR OBJECT PROPERTY SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    //if (string.IsNullOrEmpty(zvsAPI.Object.Properties.ClearObjectProperties()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void commandDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all command data?", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    //if (string.IsNullOrEmpty(zvsAPI.Commands.ClearAllCommands()))
                        MessageBox.Show("All Command Data Cleared. \n\n Please restart the program to rebuild commands.", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void repollAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int cmdId = zvsAPI.Commands.GetBuiltinCommandId("REPOLL_ALL");
            //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Builtin, CommandId = cmdId });
        }

        private void aaronRestoreNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //zvsAPI.Database.NonQuery(
            //                     "UPDATE `objects` SET `txt_object_name`='Aeon Labs Z-Stick Series 2' WHERE `node_id`='1';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bathtub Light' WHERE `node_id`='3';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bath Mirror Light' WHERE `node_id`='4';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bed Hallzway Light' WHERE `node_id`='5';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bedroom East Light' WHERE `node_id`='6';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bedroom Bed Light' WHERE `node_id`='7';" +
            //                "UPDATE `objects` SET `txt_object_name`='Office Light' WHERE `node_id`='8';" +
            //                "UPDATE `objects` SET `txt_object_name`='Family Hallway Light' WHERE `node_id`='9';" +
            //                "UPDATE `objects` SET `txt_object_name`='Outside Entry Light' WHERE `node_id`='10';" +
            //                "UPDATE `objects` SET `txt_object_name`='Entryway Light' WHERE `node_id`='11';" +
            //                "UPDATE `objects` SET `txt_object_name`='Can Lights' WHERE `node_id`='12';" +
            //                "UPDATE `objects` SET `txt_object_name`='Pourch Light' WHERE `node_id`='13';" +
            //                "UPDATE `objects` SET `txt_object_name`='Dining Table Light' WHERE `node_id`='14';" +
            //                "UPDATE `objects` SET `txt_object_name`='Fan Light' WHERE `node_id`='15';" +
            //                "UPDATE `objects` SET `txt_object_name`='Kitchen Light' WHERE `node_id`='16';" +
            //                "UPDATE `objects` SET `txt_object_name`='Rear Garage Light' WHERE `node_id`='17';" +
            //                "UPDATE `objects` SET `txt_object_name`='Driveway Light' WHERE `node_id`='18';" +
            //                "UPDATE `objects` SET `txt_object_name`='TV Backlight (LG)' WHERE `node_id`='19';" +
            //                "UPDATE `objects` SET `txt_object_name`='Fireplace Light' WHERE `node_id`='20';" +
            //                "UPDATE `objects` SET `txt_object_name`='Brother Printer' WHERE `node_id`='22';" +
            //                "UPDATE `objects` SET `txt_object_name`='Hairagami Printer' WHERE `node_id`='23';" +
            //                "UPDATE `objects` SET `txt_object_name`='South Thermostat' WHERE `node_id`='24';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Window Fan' WHERE `node_id`='25';" +
            //                "UPDATE `objects` SET `txt_object_name`='Master Bed Thermostat' WHERE `node_id`='26';");
        }
        #endregion

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //if ((e.KeyCode == Keys.F5))
            //{
            //    int cmdId = zvsAPI.Commands.GetBuiltinCommandId("REPOLL_ALL");
            //    zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Builtin, CommandId = cmdId });
            //}

        }

        #endregion

        #region Invokeable Functions
        /// <summary>
        /// Logs an event that can be called from any thread.  Self Invokes.
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="message">Log Event Message</param>
        public void AddLogEntry(Urgency urgency, string message, string theInterface = "MAIN")
        {
            //if (this.InvokeRequired && !this.IsDisposed)
            //{
            //    try
            //    {
            //        this.Invoke(new LogThisDelegate(AddLogEntry), new object[] { urgency, message, theInterface });
            //    }
            //    catch (ObjectDisposedException)
            //    { 
            //        //Sometimes called when jabber sends messages after program has been disposed. 
            //        //Jabber implementation has thread sync issues.
            //    }
            //}
            //else
            //{
            //    LogItem item = new LogItem();
            //    item.Urgency = urgency;
            //    item.Source = theInterface;
            //    item.Description = message;

            //    if (Masterlog.Count > zVScenesSettings.LongLinesLimit)
            //        Masterlog.RemoveAt(0);

            //    Masterlog.Add(item);
            //}
        }

        public void SetlabelSceneRunStatus(string text)
        {
            //if (this.InvokeRequired)
            //    this.Invoke(new SetlabelSceneRunStatusDelegate(SetlabelSceneRunStatus), new object[] { text });
            //else
            //    labelSceneRunStatus.Text = text;
        }

        public void RepollDevices(byte node)
        {
            //if (this.InvokeRequired)
            //    this.Invoke(new RepollDevicesDelegate(RepollDevices), new object[] { node });
            //else
            //    ControlThinkInt.RepollDevices(node);
        }

        #endregion           

        #region Task Scheduler Execution

        private void timer_TaskRunner_Tick(object sender, EventArgs e)
        {

            ////TODO:  MAKE EVENT BASED
            //if (UpdateScripts)
            //{
            //    _masterEvents = DatabaseControl.GetEventScripts();
            //    dataListEvents.DataSource = null;
            //    dataListEvents.DataSource = _masterEvents;
            //    UpdateScripts = false;
            //}

            //foreach (Task task in _masterTasks)
            //{
            //    if (task.Enabled)
            //    {
            //        switch (task.Frequency)
            //        {
            //            case Task.frequencys.Seconds:
            //                int sec = (int)(DateTime.Now - task.StartTime).TotalSeconds;
            //                if (sec % task.RecurSeconds == 0)
            //                {
            //                    RunScheduledTaskScene(task.SceneID, task.Name);
            //                }
            //                break;
            //            case Task.frequencys.Daily:
            //                if ((DateTime.Now.Date - task.StartTime.Date).TotalDays % task.RecurDays == 0)
            //                {
            //                    Double SecondsBetweenTime = (task.StartTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
            //                    if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
            //                        RunScheduledTaskScene(task.SceneID, task.Name);
            //                }
            //                break;
            //            case Task.frequencys.Weekly:
            //                if (((Int32)(DateTime.Now.Date - task.StartTime.Date).TotalDays / 7) % task.RecurWeeks == 0)  //IF RUN THIS WEEK
            //                {
            //                    if (ShouldRunToday(task, DateTime.Now))  //IF RUN THIS DAY 
            //                    {
            //                        Double SecondsBetweenTime = (task.StartTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
            //                        if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
            //                            RunScheduledTaskScene(task.SceneID, task.Name);
            //                    }
            //                }
            //                break;
            //            case Task.frequencys.Once:
            //                Double SecondsBetween = (DateTime.Now - task.StartTime).TotalSeconds;
            //                if (SecondsBetween < 1 && SecondsBetween > 0)
            //                    RunScheduledTaskScene(task.SceneID, task.Name);
            //                break;
            //        }
            //    }
            //}
        }

        //private bool ShouldRunToday(Task task, DateTime Today)
        //{
        //    switch (Today.DayOfWeek)
        //    {
        //        case DayOfWeek.Monday:
        //            if (task.RecurMonday)
        //                return true;
        //            break;

        //        case DayOfWeek.Tuesday:
        //            if (task.RecurTuesday)
        //                return true;
        //            break;

        //        case DayOfWeek.Wednesday:
        //            if (task.RecurWednesday)
        //                return true;
        //            break;

        //        case DayOfWeek.Thursday:
        //            if (task.RecurThursday)
        //                return true;
        //            break;

        //        case DayOfWeek.Friday:
        //            if (task.RecurFriday)
        //                return true;
        //            break;

        //        case DayOfWeek.Saturday:
        //            if (task.RecurSaturday)
        //                return true;
        //            break;

        //        case DayOfWeek.Sunday:
        //            if (task.RecurTuesday)
        //                return true;
        //            break;
        //    }

        //    return false;
        //}

        private void RunScheduledTaskScene(int SceneID, string taskname)
        {
            //Scene scene = _masterScenes.SingleOrDefault(s => s.id == SceneID);

            //if (scene != null)
            //{
            //    string result = scene.RunScene();
            //    Logger.WriteToLog(UrgencyLevel.INFO, "Scheduled task '" + taskname + "': " + result, "TASK");
            //}
            //else
            //    Logger.WriteToLog(UrgencyLevel.WARNING, "Scheduled task '" + taskname + "': Failed to find scene ID '" + SceneID.ToString() + "'.", "TASK");
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

        //private void LoadGui(Task Task)
        //{
        //    //textBox_TaskName.Enabled = true;
        //    //comboBox_FrequencyTask.Enabled = true;
        //    //numericUpDownOccurDays.Enabled = true;
        //    //checkBox_EnabledTask.Enabled = true;
        //    //dateTimePickerStartTask.Enabled = true;
        //    //numericUpDownOccurWeeks.Enabled = true;
        //    //numericUpDownOccurSeconds.Enabled = true;
        //    //checkBox_RecurMonday.Enabled = true;
        //    //checkBox_RecurTuesday.Enabled = true;
        //    //checkBox_RecurWednesday.Enabled = true;
        //    //checkBox_RecurThursday.Enabled = true;
        //    //checkBox_RecurFriday.Enabled = true;
        //    //checkBox_RecurSaturday.Enabled = true;
        //    //checkBox_RecurSunday.Enabled = true;
        //    //comboBox_ActionsTask.Enabled = true;

        //    //textBox_TaskName.Text = Task.Name;
        //    //comboBox_FrequencyTask.SelectedIndex = (int)Task.Frequency;
        //    //checkBox_EnabledTask.Checked = Task.Enabled;
        //    //dateTimePickerStartTask.Value = Task.StartTime;
        //    //numericUpDownOccurWeeks.Value = Task.RecurWeeks;
        //    //numericUpDownOccurDays.Value = Task.RecurDays;
        //    //numericUpDownOccurSeconds.Value = Task.RecurSeconds;
        //    //checkBox_RecurMonday.Checked = Task.RecurMonday;
        //    //checkBox_RecurTuesday.Checked = Task.RecurTuesday;
        //    //checkBox_RecurWednesday.Checked = Task.RecurWednesday;
        //    //checkBox_RecurThursday.Checked = Task.RecurThursday;
        //    //checkBox_RecurFriday.Checked = Task.RecurFriday;
        //    //checkBox_RecurSaturday.Checked = Task.RecurSaturday;
        //    //checkBox_RecurSunday.Checked = Task.RecurSunday;

        //    ////Look for Scene, if it was deleted then set index to -1
        //    //bool found = false;
        //    //foreach (Scene scene in _masterScenes)
        //    //{
        //    //    if (Task.SceneID == scene.id)
        //    //    {
        //    //        found = true;
        //    //        comboBox_ActionsTask.SelectedItem = scene;
        //    //        break;
        //    //    }
        //    //}
        //    //if (!found)
        //    //    comboBox_ActionsTask.SelectedIndex = -1;
        //}

        private void AddNewTask()
        {
            //zvsAPI.ScheduledTasks.Add(new Task());
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
            //groupBox_Daily.Visible = false;
            //groupBox_Weekly.Visible = false;
            //groupBox_Seconds.Visible = false;

            //switch (comboBox_FrequencyTask.SelectedIndex)
            //{
            //    case (int)Task.frequencys.Daily:
            //        groupBox_Daily.Visible = true;
            //        break;
            //    case (int)Task.frequencys.Weekly:
            //        groupBox_Weekly.Visible = true;
            //        break;
            //    case (int)Task.frequencys.Seconds:
            //        groupBox_Seconds.Visible = true;
            //        break;
            //}
        }

        private void button_SaveTask_Click(object sender, EventArgs e)
        {
            //if (dataListTasks.SelectedObject != null)
            //{
            //    Task SelectedTask = (Task)dataListTasks.SelectedObject;

            //    //Task Name
            //    if (textBox_TaskName.Text != "")
            //        SelectedTask.Name = textBox_TaskName.Text;
            //    else
            //    {
            //        MessageBox.Show("Error Saving\n\nTask name not vaild.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }

            //    //Frequency
            //    SelectedTask.Frequency = (Task.frequencys)Enum.Parse(typeof(Task.frequencys), comboBox_FrequencyTask.SelectedItem.ToString());

            //    //Endabled 
            //    SelectedTask.Enabled = checkBox_EnabledTask.Checked;

            //    //DateTime
            //    SelectedTask.StartTime = dateTimePickerStartTask.Value;

            //    //Recur Days 
            //    if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Daily.ToString()))
            //    {
            //        SelectedTask.RecurDays = (int)numericUpDownOccurDays.Value;
            //    }
            //    else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Seconds.ToString()))
            //    {
            //        SelectedTask.RecurSeconds = (int)numericUpDownOccurSeconds.Value;
            //    }
            //    else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Weekly.ToString()))
            //    {
            //        #region Weekly
            //        SelectedTask.RecurWeeks = (int)numericUpDownOccurWeeks.Value;
            //        SelectedTask.RecurMonday = checkBox_RecurMonday.Checked;
            //        SelectedTask.RecurTuesday = checkBox_RecurTuesday.Checked;
            //        SelectedTask.RecurWednesday = checkBox_RecurWednesday.Checked;
            //        SelectedTask.RecurThursday = checkBox_RecurThursday.Checked;
            //        SelectedTask.RecurFriday = checkBox_RecurFriday.Checked;
            //        SelectedTask.RecurSaturday = checkBox_RecurSaturday.Checked;
            //        SelectedTask.RecurSunday = checkBox_RecurSunday.Checked;
            //        #endregion
            //    }

            //    //Action
            //    if (comboBox_ActionsTask.SelectedIndex != -1)
            //    {
            //        Scene SelectedScene = (Scene)comboBox_ActionsTask.SelectedItem;
            //        SelectedTask.SceneID = SelectedScene.id;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Error Saving\n\nPlease select a scene to activate before saving.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }

            //    SelectedTask.Update();
            //}
        }

        private void dataListTasks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //if (dataListTasks.SelectedObject != null)
            //    LoadGui((Task)dataListTasks.SelectedObject);
            //else
            //{
            //    textBox_TaskName.Enabled = false;
            //    comboBox_FrequencyTask.Enabled = false;
            //    numericUpDownOccurWeeks.Enabled = false;
            //    numericUpDownOccurSeconds.Enabled = false;
            //    numericUpDownOccurDays.Enabled = false;
            //    checkBox_EnabledTask.Enabled = false;
            //    dateTimePickerStartTask.Enabled = false;
            //    checkBox_RecurMonday.Enabled = false;
            //    checkBox_RecurTuesday.Enabled = false;
            //    checkBox_RecurWednesday.Enabled = false;
            //    checkBox_RecurThursday.Enabled = false;
            //    checkBox_RecurFriday.Enabled = false;
            //    checkBox_RecurSaturday.Enabled = false;
            //    checkBox_RecurSunday.Enabled = false;
            //    comboBox_ActionsTask.Enabled = false;
            //}

        }

        private void deleteTask()
        {
            //if (dataListTasks.SelectedObject != null)
            //{
            //    Task selectedTask = (Task)dataListTasks.SelectedObject;

            //    if (MessageBox.Show("Are you sure you want to delete the '" + selectedTask.Name + "' task?", "Are you sure?",
            //                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        selectedTask.Remove();

            //        if (_masterTasks.Count > 0)
            //            dataListTasks.SelectedIndex = 0;
            //    }
            //}
            //else
            //    MessageBox.Show("Please select a task to delete.", ProgramName);
        }

        private void dataListTasks_CellRightClick(object sender, CellRightClickEventArgs e)
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
                int scriptId;
                int.TryParse(_masterEvents.Rows[dataListEvents.SelectedIndex]["id"].ToString(), out scriptId);
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
                    int scriptId;
                    int.TryParse(_masterEvents.Rows[dataListEvents.SelectedIndex]["id"].ToString(), out scriptId);
                    //DatabaseControl.DeleteEventScript(scriptId);
                    UpdateScripts = true;
                }
            }
        }
        #endregion 
    }
}