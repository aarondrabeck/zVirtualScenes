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
using zVirtualScenesApplication.Scripting;

namespace zVirtualScenesApplication
{
    public partial class MainForm : Form
    {            
        public string ProgramName = zvsEntityControl.zvsNameAndVersion;

        //Save tasks in memory for performance reasons.
        List<scheduled_tasks> task_list;   

        //Forms
        private GroupEditor grpEditorForm;
        private ActivateGroup grpActivateForm;
        private formPropertiesScene formSceneProperties;               

        //Delegates
        public delegate void anonymousEventDelegate(object sender, EventArgs e);
        public delegate void anonymousStringDelegate(object sender, String s);
        public delegate void anonymousDelegate();

        //CORE OBJECTS
        private BindingList<LogItem> _masterlog = new BindingList<LogItem>();

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
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandlerMethod);

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
            device_values.DeviceValueAddedEvent += new device_values.DeviceValueAddedEventHandler(device_values_DeviceValueAddedEvent);
            zvsEntityControl.DeviceModified += new zvsEntityControl.DeviceModifiedEventHandler(zvsEntityControl_DeviceModified);
            device.Added += new device.DeviceAddedEventHandler(device_Added);
            zvsEntityControl.ScheduledTaskModified += new zvsEntityControl.ScheduledTaskModifiedEventHandler(zvsEntityControl_ScheduledTaskModified);
            zvsEntityControl.TriggerModified += new zvsEntityControl.TriggerModifiedEventHandler(zvsEntityControl_TriggerModified);
            zvsEntityControl.SceneModified += new zvsEntityControl.SceneModifiedEventHandler(zvsEntityControl_SceneModified);
        }
                   

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {            
            this.Text = ProgramName;
            notifyIcon1.Text = ProgramName;
            dataListViewLog.DataSource = _masterlog;
            Logger.WriteToLog(Urgency.INFO, "STARTED", "MainForm");

            pm = new PluginManager();

            //Populate Object List Views 
            SyncdataListViewDevices();
            SyncdataListViewDeviceSmallList();
            SyncdataListTriggers();

            #region Scenes
            SyncdataListViewScenes();
            //Scenes (allow rearrage but not drag and drop from other sources)
            dataListViewScenes.DropSink = new SceneDropSink();
            if (dataListViewScenes.Items.Count > 0)
                dataListViewScenes.SelectedIndex = 0;
            // Scene Commands       
            dataListViewSceneCMDs.DropSink = new SceneCommandDropSink();
           
            #endregion

            #region Task Scheduler
            SyncScheduledTaskList(); 
            comboBox_FrequencyTask.DataSource = Enum.GetNames(typeof(scheduled_tasks.frequencys));                      

            //Add default task item if list is empty
            if (dataListTasks.Items.Count < 1)
                AddNewTask();
            else
                dataListTasks.SelectedIndex = 0;

            comboBox_ActionsTask.DisplayMember = "friendly_name";
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

        private void SyncdataListViewDeviceSmallList()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                int s_index = dataListViewDeviceSmallList.SelectedIndex;
                dataListViewDeviceSmallList.DataSource = db.devices.OfType<device>().Execute(MergeOption.AppendOnly);


                if (s_index > 0)
                {
                    try
                    {
                        dataListViewDeviceSmallList.SelectedIndex = s_index;
                    }
                    catch { }
                }
            }
        }
        
        private void SyncdataListViewDevices()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                int s_index = dataListViewDevices.SelectedIndex;
                dataListViewDevices.DataSource = db.devices.OfType<device>().Execute(MergeOption.AppendOnly);
                dataListViewDevices.SelectedIndex = s_index;

                int index = dataListViewDevices.SelectedIndex;
                if (index > 0)
                    dataListViewDevices.EnsureVisible(index);
            }

            Console.WriteLine("------>>>>>>> UpdatedataListViewDevices <<<<<<<<<<<---------------------");
        }

        #region Subcribed API Events

        void zvsEntityControl_DeviceModified(object sender, string PropertyModified)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousStringDelegate(zvsEntityControl_DeviceModified), new object[] { sender, PropertyModified });
            else
            {
                if (PropertyModified.Equals("friendly_name") ||
                    PropertyModified.Equals("group") ||
                    PropertyModified.Equals("last_heard_from")||
                    PropertyModified.Equals("removed"))
                {                    
                    SyncdataListViewDevices();
                }

                if (PropertyModified.Equals("friendly_name") ||
                    PropertyModified.Equals("removed"))
                    SyncdataListViewDeviceSmallList();
            }
        }

        void device_Added(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(device_Added), new object[] { sender, e });
            else
            {
                SyncdataListViewDeviceSmallList();
                SyncdataListViewDevices();
            }
        }        
 
        void device_FriendlyNameChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(device_FriendlyNameChanged), new object[] { sender, e });
            else
            {
                SyncdataListViewDeviceSmallList();
                SyncdataListViewDevices();
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

        void device_values_DeviceValueAddedEvent(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(device_values_DeviceValueAddedEvent), new object[] { sender, e });
            else
            {
                device_values dv = (device_values)sender;
                //GUI UPDATING
                if (dv.label_name == "Basic" || dv.label_name == "Temperature")
                {
                    SyncdataListViewDevices();
                }
            }
        }

        void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            if (this.InvokeRequired)
                this.Invoke(new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent), new object[] { sender, args });
            else
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device_values dv = db.device_values.FirstOrDefault(v=> v.id == args.device_value_id);                  
                    if (dv != null)
                    {
                        //GUI UPDATING
                        if (dv.label_name == "Basic" || dv.label_name == "Temperature")
                        {
                            SyncdataListViewDevices();
                        }

                        string device_name = "Unknown";
                       
                        if (String.IsNullOrEmpty(dv.device.friendly_name))
                            device_name = "Device #" + dv.device_id;
                        else
                            device_name = dv.device.friendly_name;  

                        if (!String.IsNullOrEmpty(args.previousValue))
                            Logger.WriteToLog(Urgency.INFO, string.Format("{0} {1} changed from {2} to {3}.", device_name, dv.label_name, args.previousValue, dv.value), "EVENT");
                        else
                            Logger.WriteToLog(Urgency.INFO, string.Format("{0} {1} changed to {2}.", device_name, dv.label_name, dv.value), "EVENT");

                        // Check to see if previous value == new value. If so then the value didn't actually change!
                        if (args.previousValue != dv.value)
                        {
                            //Event Triggering
                            foreach (device_value_triggers trigger in dv.device_value_triggers.Where(t => t.enabled))
                            {
                                if (((device_value_triggers.TRIGGER_TYPE)trigger.trigger_type) == device_value_triggers.TRIGGER_TYPE.Basic)
                                {
                                    switch ((device_value_triggers.TRIGGER_OPERATORS)trigger.trigger_operator)
                                    {
                                        case device_value_triggers.TRIGGER_OPERATORS.EqualTo:
                                            {
                                                if (dv.value.Equals(trigger.trigger_value))
                                                {
                                                    Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                    Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
                                                }
                                                break;
                                            }
                                        case device_value_triggers.TRIGGER_OPERATORS.GreaterThan:
                                            {
                                                double deviceValue = 0;
                                                double triggerValue = 0;

                                                if (double.TryParse(dv.value, out deviceValue) && double.TryParse(trigger.trigger_value, out triggerValue))
                                                {
                                                    if (deviceValue > triggerValue)
                                                    {
                                                        Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                        Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
                                                    }
                                                }
                                                else
                                                    Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), "TRIGGER");

                                                break;
                                            }
                                        case device_value_triggers.TRIGGER_OPERATORS.LessThan:
                                            {
                                                double deviceValue = 0;
                                                double triggerValue = 0;

                                                if (double.TryParse(dv.value, out deviceValue) && double.TryParse(trigger.trigger_value, out triggerValue))
                                                {
                                                    if (deviceValue < triggerValue)
                                                    {
                                                        Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                        Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
                                                    }
                                                }
                                                else
                                                    Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), "TRIGGER");

                                                break;
                                            }
                                        case device_value_triggers.TRIGGER_OPERATORS.NotEqualTo:
                                            {
                                                if (!dv.value.Equals(trigger.trigger_value))
                                                {
                                                    Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                    Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
                                                }
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    ScriptManager.RunScript(trigger);
                                }
                            }
                        }

                    }
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






        #region Scenes

        private void SyncdataListViewScenes()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                int s_index = dataListViewScenes.SelectedIndex;
                //scene list in task scheduler and main scene window
                dataListViewScenes.DataSource = comboBox_ActionsTask.DataSource = db.scenes.OrderBy(s=>s.sort_order);
                dataListViewScenes.SelectedIndex = s_index;

                int index = dataListViewScenes.SelectedIndex;
                if (index > 0)
                    dataListViewScenes.EnsureVisible(index);
            }

            Console.WriteLine("------>>>>>>> SyncdataListViewScenes <<<<<<<<<<<---------------------");
        }

        private void dataListViewScenes_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            scene selectedscene = (scene)dataListViewScenes.SelectedObject;
            if (selectedscene != null)
            {
                dataListViewSceneCMDs.Visible = true;
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    dataListViewSceneCMDs.DataSource = db.scene_commands.Where(s => s.scene_id == selectedscene.id).OrderBy(s => s.sort_order);
                }
                lbl_sceneActions.Text = "Scene " + selectedscene.id.ToString() + " '" + selectedscene.friendly_name + "' Actions";
            }
            else
            {
                dataListViewSceneCMDs.Visible = false;
                lbl_sceneActions.Text = "No Scene Selected";
            }
        }

        public delegate void zvsEntityControl_SceneModifiedDelegate(object sender, long? SceneID);
        void zvsEntityControl_SceneModified(object sender, long? SceneID)
        {
            if (this.InvokeRequired)
                this.Invoke(new zvsEntityControl_SceneModifiedDelegate(zvsEntityControl_SceneModified), new object[] { sender, SceneID });
            else
            {
                SyncdataListViewScenes();
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
                    SyncdataListViewScenes();
                }
            }
        }

        void zvsEntityControl_SceneRunCompleteEvent(long scene_id, int ErrorCount)
        {
            if (this.InvokeRequired)
                this.Invoke(new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent), new object[] { scene_id, ErrorCount });
            else
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene _scene = db.scenes.FirstOrDefault(s => s.id == scene_id);
                    if (_scene != null)
                    {
                        Logger.WriteToLog(Urgency.INFO, "Scene '" + _scene.friendly_name + "' has completed with " + ErrorCount + " errors.", "EVENT");
                    }
                    else
                        Logger.WriteToLog(Urgency.INFO, "Scene #'" + scene_id + "' has completed with " + ErrorCount + " errors.", "EVENT");
                   
                }
                SyncdataListViewScenes();
            }
        }
        
        #region Scene ObjectDataView Handeling

        private void dataListViewScenes_CellRightClick_1(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripScenesNull;
            else
                e.MenuStrip = contextMenuStripScenes;
        }
               
        private void duplicateSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            scene selected_scene = (scene)dataListViewScenes.SelectedObject;
            if (selected_scene != null)
            {
                scene new_scene = new scene { friendly_name = selected_scene.friendly_name + " Copy", sort_order = dataListViewScenes.GetItemCount() + 1 };

                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    selected_scene = db.scenes.FirstOrDefault(s => s.id == selected_scene.id);
                    if (selected_scene != null)
                    {
                        foreach (scene_commands sc in selected_scene.scene_commands)
                        {
                            new_scene.scene_commands.Add(new scene_commands
                            {
                                arg = sc.arg,
                                command_id = sc.command_id,
                                command_type_id = sc.command_type_id,
                                sort_order = sc.sort_order,
                                device_id = sc.device_id
                            });
                        }
                    }
                
                    db.scenes.AddObject(new_scene);
                    db.SaveChanges();
                }
                zvsEntityControl.CallSceneModified(this, selected_scene.id);
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
                    using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                    {
                        selected_scene = db.scenes.FirstOrDefault(s => s.id == selected_scene.id);
                        if (selected_scene != null)
                        {
                            db.scenes.DeleteObject(selected_scene);
                            db.SaveChanges();

                            foreach (scene s in db.scenes)
                            {
                                s.sort_order = dataListViewScenes.IndexOf(s) + 1;
                            }
                            db.SaveChanges();
                        }
                    }
                    zvsEntityControl.CallSceneModified(this, null);
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
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                db.scenes.AddObject(new_s);
                db.SaveChanges();

                //todo: might case concurency issues
                zvsEntityControl.CallSceneModified(this, new_s.id);
            }            
        }

        public class SceneDropSink : SimpleDropSink
        {
            public SceneDropSink()
            {
                this.CanDropBetween = true;
                this.CanDropOnBackground = false;
                this.CanDropOnItem = false;
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
            e.Effect = DragDropEffects.None;
            e.InfoMessage = "Can not drop this here.";

            if (e.SourceModels[0].GetType().Equals(typeof(scene)) && e.TargetModel != null && e.TargetModel.GetType().Equals(typeof(scene)))
            {
                e.Effect = DragDropEffects.Move;
                e.InfoMessage = "Rearrage Order";
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

            if (e.SourceModels[0].GetType().Equals(typeof(scene)))
            {
                switch (e.DropTargetLocation)
                {
                    case DropTargetLocation.AboveItem:
                        //offset = 0 
                        dataListViewScenes.MoveObjects(TargetIndex, e.SourceModels);
                        dataListViewScenes.SelectedObjects = e.SourceModels;
                        break;
                    case DropTargetLocation.BelowItem:
                        //offset = 1 
                        dataListViewScenes.MoveObjects(TargetIndex +1, e.SourceModels);
                        dataListViewScenes.SelectedObjects = e.SourceModels;
                        break;
                }
            }

            //SAve the sort order to the DB
            foreach (scene scene_in_list in dataListViewScenes.Objects)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene scene = db.scenes.FirstOrDefault(s => s.id == scene_in_list.id);
                    if (scene != null)
                    {
                        scene.sort_order = dataListViewScenes.IndexOf(scene_in_list); 
                    }
                    db.SaveChanges();
                }
            }          
        }

        private void OpenScenePropertiesWindow()
        {
            scene selected_scene = (scene)dataListViewScenes.SelectedObject;
            if (selected_scene != null)
            {
                formSceneProperties = new formPropertiesScene(selected_scene.id);
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
            scene selected_scene = (scene)dataListViewScenes.SelectedObject;
            if (selected_scene != null)
            {
                Logger.WriteToLog(Urgency.INFO, selected_scene.RunScene(), "MAIN");
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
                this.CanDropBetween = true;
                this.CanDropOnBackground = true;
                this.CanDropOnItem = false;
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
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    foreach (scene_commands selected_sceneCMD in dataListViewSceneCMDs.SelectedObjects)
                    {
                        scene_commands cmd = db.scene_commands.FirstOrDefault(c => c.id == selected_sceneCMD.id);
                        if (cmd != null)
                            db.scene_commands.DeleteObject(cmd);
                    }

                    db.SaveChanges();

                    zvsEntityControl.CallSceneModified(this, null);
                }
            }
        }

        private void editCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (scene_commands cmd in dataListViewSceneCMDs.SelectedObjects)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene_commands selected_sceneCMD = db.scene_commands.FirstOrDefault(c => c.id == cmd.id);
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
                                AddEditSceneBuiltinCMD b_cmd_editForm = new AddEditSceneBuiltinCMD(selected_sceneCMD.id);
                                b_cmd_editForm.ShowDialog();
                                break;
                            case command_types.device_command:
                            case command_types.device_type_command:
                                {                                    
                                    AddEditSceneDeviceCMD editCMDform = new AddEditSceneDeviceCMD(selected_sceneCMD.id, selected_sceneCMD.device_id.Value);
                                    editCMDform.ShowDialog();                                    
                                    break;
                                }
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
                AddEditSceneBuiltinCMD addCMDform = new AddEditSceneBuiltinCMD(null, selected_scene.id);
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
                scene scene = (scene)dataListViewScenes.SelectedObject;
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene selected_scene = db.scenes.FirstOrDefault(c => c.id == scene.id);
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
                                int pos = TargetIndex;
                                switch (e.DropTargetLocation)
                                {
                                    case DropTargetLocation.BelowItem:
                                        pos += 1; 
                                        break;
                                }

                                AddEditSceneDeviceCMD addCMDform = new AddEditSceneDeviceCMD(null, selected_device.id, selected_scene.id, pos);
                                addCMDform.ShowDialog();

                            }
                        }
                    }
                }
            }
            else if (e.SourceModels[0].GetType().Equals(typeof(scene_commands)))
            {
                //Rearrage Actions
                scene scene = (scene)dataListViewScenes.SelectedObject;
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    scene selected_scene = db.scenes.FirstOrDefault(c => c.id == scene.id);
                    if (selected_scene != null)
                    {
                        if (selected_scene.is_running)
                        {
                            MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                            return;
                        }

                        switch (e.DropTargetLocation)
                        {
                            case DropTargetLocation.AboveItem:
                                //offset = 0 
                                dataListViewSceneCMDs.MoveObjects(TargetIndex, e.SourceModels);
                                dataListViewSceneCMDs.SelectedObjects = e.SourceModels;
                                break;
                            case DropTargetLocation.BelowItem:
                                //offset = 1 
                                dataListViewSceneCMDs.MoveObjects(TargetIndex + 1, e.SourceModels);
                                dataListViewSceneCMDs.SelectedObjects = e.SourceModels;
                                break;
                        }

                        //SAve the sort order to the DB
                        foreach (scene_commands cmd in dataListViewSceneCMDs.Objects)
                        {
                            scene_commands scene_cmd = db.scene_commands.FirstOrDefault(s => s.id == cmd.id);
                            if (scene_cmd != null)
                            {
                                scene_cmd.sort_order = dataListViewSceneCMDs.IndexOf(cmd);
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        #endregion
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

        #region Device List Box Handling

        private void dataListViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            {
                device d = (device)dataListViewDevices.SelectedObjects[0];
                if (d != null)
                {

                    uc_device_values1.UpdateControl(d.id);
                    uc_device_values1.Visible = true;
                }
                else
                    uc_device_values1.UpdateControl(0);
            }
            else
            {
                uc_device_values1.UpdateControl(0);
            }
        }       
        
        private void OpenDevicePropertyWindow()
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            {
                foreach (device d in dataListViewDevices.SelectedObjects)
                {
                    DeviceProperties properties = new DeviceProperties(d.id);
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
                
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device d = db.devices.FirstOrDefault(o => o.id == ((device)e.Item.RowObject).id);
                    if (d != null)
                    {
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

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_commands cmd = db.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ME");
                if (cmd != null)
                    cmd.Run(d.id.ToString());
            }
        }

        private void DeleteDevice_Click(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = db.devices.FirstOrDefault(o => o.id == ((device)dataListViewDevices.SelectedObject).id); 
                if (d != null)
                {
                    db.devices.DeleteObject(d);
                    db.SaveChanges();
                }
            }
            zvsEntityControl.CallDeviceModified(this, "removed");
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
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_commands cmd = db.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ME");
                if (cmd != null)
                {
                    foreach (device selecteddevice in dataListViewDevices.SelectedObjects)
                        cmd.Run(selecteddevice.id.ToString());
                }
            }
        }

        private void repollAllDevicesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_commands cmd = db.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ALL");
                if (cmd != null)
                    cmd.Run();
            }
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
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_commands cmd = db.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ALL");
                if (cmd != null)
                    cmd.Run();
            }
        }

        private void SetNamesDevOnly()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (device d in db.devices)
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
                        case 28:
                            d.friendly_name = "Xmas Lights";
                            break;
                    }
                }
                db.SaveChanges();
            }
        }
        #endregion

        private void MainTabControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F5))
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    builtin_commands cmd = db.builtin_commands.FirstOrDefault(c => c.name == "REPOLL_ALL");
                    if (cmd != null)
                        cmd.Run();
                }
            }

            if ((e.KeyCode == Keys.F6))
            {
                //TEMP FUNCTION
                SetNamesDevOnly();
            }

        }

               

        #region Task Scheduler Execution

        private void timer_TaskRunner_Tick(object sender, EventArgs e)
        {
            foreach (scheduled_tasks task in task_list)
            {
                if (task.Enabled)
                {
                    if (task.Frequency.HasValue)
                    {
                        switch ((scheduled_tasks.frequencys)task.Frequency)
                        {
                            case scheduled_tasks.frequencys.Seconds:
                                {
                                    if (task.StartTime.HasValue)
                                    {
                                        int sec = (int)(DateTime.Now - task.StartTime.Value).TotalSeconds;
                                        if (sec % task.RecurSeconds == 0)                                        
                                            task.Run();                                        
                                    }
                                    break;
                                }
                            case scheduled_tasks.frequencys.Daily:
                                {
                                    if (task.StartTime.HasValue)
                                    {
                                        //Console.WriteLine("totaldays:" + (DateTime.Now.Date - task.StartTime.Value.Date).TotalDays);
                                        if (task.RecurDays > 0 && ((DateTime.Now.Date - task.StartTime.Value.Date).TotalDays % task.RecurDays == 0))
                                        {
                                            TimeSpan TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                            TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds); //remove milli seconds
                                            
                                            //Console.WriteLine(string.Format("taskTofD: {0}, nowTofD: {1}", task.StartTime.Value.TimeOfDay, TimeNowToTheSeconds));                                            
                                            if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))                                            
                                                task.Run();                                            
                                        }
                                    }
                                    break;
                                }
                            case scheduled_tasks.frequencys.Weekly:
                                {
                                    if (task.StartTime.HasValue)
                                    {
                                        if (task.RecurWeeks > 0 && (((Int32)(DateTime.Now.Date - task.StartTime.Value.Date).TotalDays / 7) % task.RecurWeeks == 0))  //IF RUN THIS WEEK
                                        {
                                            if (ShouldRunToday(task))  //IF RUN THIS DAY 
                                            {
                                                TimeSpan TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                                TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);
                                          
                                                if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                                    task.Run();
                                            }
                                        }
                                    }
                                    break;
                                }
                            case scheduled_tasks.frequencys.Monthly:
                                {
                                    if (task.StartTime.HasValue)
                                    {
                                        int monthsapart = ((DateTime.Now.Year - task.StartTime.Value.Year) * 12) + DateTime.Now.Month - task.StartTime.Value.Month;
                                        //Console.WriteLine(string.Format("Months Apart: {0}", monthsapart));
                                        if (task.RecurMonth > 0 && monthsapart > -1 && monthsapart % task.RecurMonth == 0)  //IF RUN THIS Month
                                        {
                                            if (ShouldRunThisDayOfMonth(task))  //IF RUN THIS DAY 
                                            {
                                                TimeSpan TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                                TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);

                                                if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                                    task.Run();
                                            }
                                        }
                                    }

                                    break;
                                }
                            case scheduled_tasks.frequencys.Once:
                                {
                                    if (task.StartTime.HasValue)
                                    {
                                        TimeSpan TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                        TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);
                                           
                                        if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                            task.Run();
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private bool ShouldRunThisDayOfMonth(scheduled_tasks task)
        {
            switch (DateTime.Now.Day)
            {
                case 1:
                        if (task.RecurDay01.HasValue && task.RecurDay01.Value) { return true; }
                        break;
                case 2:
                        if (task.RecurDay02.HasValue && task.RecurDay02.Value) { return true; }
                        break;
                case 3:
                        if (task.RecurDay03.HasValue && task.RecurDay03.Value) { return true; }
                        break;
                case 4:
                        if (task.RecurDay04.HasValue && task.RecurDay04.Value) { return true; }
                        break;
                case 5:                    
                        if (task.RecurDay05.HasValue && task.RecurDay05.Value) { return true; }
                        break;                    
                case 6:                    
                        if (task.RecurDay06.HasValue && task.RecurDay06.Value) { return true; }
                        break;                    
                case 7:                    
                        if (task.RecurDay07.HasValue && task.RecurDay07.Value) { return true; }
                        break;                    
                case 8:                    
                        if (task.RecurDay08.HasValue && task.RecurDay08.Value) { return true; }
                        break;                    
                case 9:                    
                        if (task.RecurDay09.HasValue && task.RecurDay09.Value) { return true; }
                        break;                    
                case 10:                    
                        if (task.RecurDay10.HasValue && task.RecurDay10.Value) { return true; }
                        break;
                case 11:
                        if (task.RecurDay11.HasValue && task.RecurDay11.Value) { return true; }
                        break;
                case 12:
                        if (task.RecurDay12.HasValue && task.RecurDay12.Value) { return true; }
                        break;
                case 13:
                        if (task.RecurDay13.HasValue && task.RecurDay13.Value) { return true; }
                        break;
                case 14:
                        if (task.RecurDay14.HasValue && task.RecurDay14.Value) { return true; }
                        break;
                case 15:
                        if (task.RecurDay15.HasValue && task.RecurDay15.Value) { return true; }
                        break;
                case 16:
                        if (task.RecurDay16.HasValue && task.RecurDay16.Value) { return true; }
                        break;
                case 17:
                        if (task.RecurDay17.HasValue && task.RecurDay17.Value) { return true; }
                        break;
                case 18:
                        if (task.RecurDay18.HasValue && task.RecurDay18.Value) { return true; }
                        break;
                case 19:
                        if (task.RecurDay19.HasValue && task.RecurDay19.Value) { return true; }
                        break;
                case 20:
                        if (task.RecurDay20.HasValue && task.RecurDay20.Value) { return true; }
                        break;
                case 21:
                        if (task.RecurDay21.HasValue && task.RecurDay21.Value) { return true; }
                        break;
                case 22:
                        if (task.RecurDay22.HasValue && task.RecurDay22.Value) { return true; }
                        break;
                case 23:
                        if (task.RecurDay23.HasValue && task.RecurDay23.Value) { return true; }
                        break;
                case 24:
                        if (task.RecurDay24.HasValue && task.RecurDay24.Value) { return true; }
                        break;
                case 25:
                        if (task.RecurDay25.HasValue && task.RecurDay25.Value) { return true; }
                        break;
                case 26:
                        if (task.RecurDay26.HasValue && task.RecurDay26.Value) { return true; }
                        break;
                case 27:
                        if (task.RecurDay27.HasValue && task.RecurDay27.Value) { return true; }
                        break;
                case 28:
                        if (task.RecurDay28.HasValue && task.RecurDay28.Value) { return true; }
                        break;
                case 29:
                        if (task.RecurDay29.HasValue && task.RecurDay29.Value) { return true; }
                        break;
                case 30:
                        if (task.RecurDay30.HasValue && task.RecurDay30.Value) { return true; }
                        break;
                case 31:
                        if (task.RecurDay31.HasValue && task.RecurDay31.Value) { return true; }
                        break;
            }
            return false; 
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

        private void SyncScheduledTaskList()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                int s_index = dataListTasks.SelectedIndex;
                dataListTasks.DataSource = task_list = db.scheduled_tasks.ToList();
                dataListTasks.SelectedIndex = s_index;

                int index = dataListTasks.SelectedIndex;
                if (index > 0)
                    dataListTasks.EnsureVisible(index);
            }

            Console.WriteLine("------>>>>>>> Syned dataListTasks <<<<<<<<<<<---------------------");
        }

        void zvsEntityControl_ScheduledTaskModified(object sender, string PropertyModified)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousStringDelegate(zvsEntityControl_ScheduledTaskModified), new object[] { sender, PropertyModified });
            else
            {
                SyncScheduledTaskList();
            }
        }

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
            //monthly
            btn_odd.Enabled = true;
            btn_even.Enabled = true;
            numericUpDownOccurMonths.Enabled = true;
            checkBoxDay01.Enabled = true;
            checkBoxDay02.Enabled = true;
            checkBoxDay03.Enabled = true;
            checkBoxDay04.Enabled = true;
            checkBoxDay05.Enabled = true;
            checkBoxDay06.Enabled = true;
            checkBoxDay07.Enabled = true;
            checkBoxDay08.Enabled = true;
            checkBoxDay09.Enabled = true;
            checkBoxDay10.Enabled = true;
            checkBoxDay11.Enabled = true;
            checkBoxDay12.Enabled = true;
            checkBoxDay13.Enabled = true;
            checkBoxDay14.Enabled = true;
            checkBoxDay15.Enabled = true;
            checkBoxDay16.Enabled = true;
            checkBoxDay17.Enabled = true;
            checkBoxDay18.Enabled = true;
            checkBoxDay19.Enabled = true;
            checkBoxDay20.Enabled = true;
            checkBoxDay21.Enabled = true;
            checkBoxDay22.Enabled = true;
            checkBoxDay23.Enabled = true;
            checkBoxDay24.Enabled = true;
            checkBoxDay25.Enabled = true;
            checkBoxDay26.Enabled = true;
            checkBoxDay27.Enabled = true;
            checkBoxDay28.Enabled = true;
            checkBoxDay29.Enabled = true;
            checkBoxDay30.Enabled = true;
            checkBoxDay31.Enabled = true;            

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

            #region Monthly
            if (Task.RecurMonth.HasValue)
                numericUpDownOccurMonths.Value = (decimal)Task.RecurMonth.Value;

            if (Task.RecurDay01.HasValue) { checkBoxDay01.Checked = Task.RecurDay01.Value; }
            if (Task.RecurDay02.HasValue) { checkBoxDay02.Checked = Task.RecurDay02.Value; }
            if (Task.RecurDay03.HasValue) { checkBoxDay03.Checked = Task.RecurDay03.Value; }
            if (Task.RecurDay04.HasValue) { checkBoxDay04.Checked = Task.RecurDay04.Value; }
            if (Task.RecurDay05.HasValue) { checkBoxDay05.Checked = Task.RecurDay05.Value; }
            if (Task.RecurDay06.HasValue) { checkBoxDay06.Checked = Task.RecurDay06.Value; }
            if (Task.RecurDay07.HasValue) { checkBoxDay07.Checked = Task.RecurDay07.Value; }
            if (Task.RecurDay08.HasValue) { checkBoxDay08.Checked = Task.RecurDay08.Value; }
            if (Task.RecurDay09.HasValue) { checkBoxDay09.Checked = Task.RecurDay09.Value; }
            if (Task.RecurDay10.HasValue) { checkBoxDay10.Checked = Task.RecurDay10.Value; }
            if (Task.RecurDay11.HasValue) { checkBoxDay11.Checked = Task.RecurDay11.Value; }
            if (Task.RecurDay12.HasValue) { checkBoxDay12.Checked = Task.RecurDay12.Value; }
            if (Task.RecurDay13.HasValue) { checkBoxDay13.Checked = Task.RecurDay13.Value; }
            if (Task.RecurDay14.HasValue) { checkBoxDay14.Checked = Task.RecurDay14.Value; }
            if (Task.RecurDay15.HasValue) { checkBoxDay15.Checked = Task.RecurDay15.Value; }
            if (Task.RecurDay16.HasValue) { checkBoxDay16.Checked = Task.RecurDay16.Value; }
            if (Task.RecurDay17.HasValue) { checkBoxDay17.Checked = Task.RecurDay17.Value; }
            if (Task.RecurDay18.HasValue) { checkBoxDay18.Checked = Task.RecurDay18.Value; }
            if (Task.RecurDay19.HasValue) { checkBoxDay19.Checked = Task.RecurDay19.Value; }
            if (Task.RecurDay20.HasValue) { checkBoxDay20.Checked = Task.RecurDay20.Value; }
            if (Task.RecurDay21.HasValue) { checkBoxDay21.Checked = Task.RecurDay21.Value; }
            if (Task.RecurDay22.HasValue) { checkBoxDay22.Checked = Task.RecurDay22.Value; }
            if (Task.RecurDay23.HasValue) { checkBoxDay23.Checked = Task.RecurDay23.Value; }
            if (Task.RecurDay24.HasValue) { checkBoxDay24.Checked = Task.RecurDay24.Value; }
            if (Task.RecurDay25.HasValue) { checkBoxDay25.Checked = Task.RecurDay25.Value; }
            if (Task.RecurDay26.HasValue) { checkBoxDay26.Checked = Task.RecurDay26.Value; }
            if (Task.RecurDay27.HasValue) { checkBoxDay27.Checked = Task.RecurDay27.Value; }
            if (Task.RecurDay28.HasValue) { checkBoxDay28.Checked = Task.RecurDay28.Value; }
            if (Task.RecurDay29.HasValue) { checkBoxDay29.Checked = Task.RecurDay29.Value; }
            if (Task.RecurDay30.HasValue) { checkBoxDay30.Checked = Task.RecurDay30.Value; }
            if (Task.RecurDay31.HasValue) { checkBoxDay31.Checked = Task.RecurDay31.Value; }

            #endregion

            foreach (scene s in comboBox_ActionsTask.Items)
            {
                if (s.id == Task.Scene_id)
                {
                    comboBox_ActionsTask.SelectedItem = s;
                    break;
                }
            }
        }

        private void AddNewTask()
        {
            scheduled_tasks task = new scheduled_tasks { friendly_name = "New Task", Frequency = (int)scheduled_tasks.frequencys.Daily, Scene_id = 0, Enabled = false };           

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                db.scheduled_tasks.AddObject(task);
                db.SaveChanges();
                zvsEntityControl.CallScheduledTaskModified(this, "added");
            }
          
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
            groupBox_Montly.Visible = false; 

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
                case (int)scheduled_tasks.frequencys.Monthly:
                    groupBox_Montly.Visible = true;
                    break;                    
            }
        }

        private void button_SaveTask_Click(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                scheduled_tasks SelectedTask = db.scheduled_tasks.FirstOrDefault(o => o.id == ((scheduled_tasks)dataListTasks.SelectedObject).id);
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
                    else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(scheduled_tasks.frequencys.Monthly.ToString()))
                    {
                        #region Monthly
                        SelectedTask.RecurMonth = (int)numericUpDownOccurMonths.Value;
                        SelectedTask.RecurDay01 = checkBoxDay01.Checked;
                        SelectedTask.RecurDay02 = checkBoxDay02.Checked;
                        SelectedTask.RecurDay03 = checkBoxDay03.Checked;
                        SelectedTask.RecurDay04 = checkBoxDay04.Checked;
                        SelectedTask.RecurDay05 = checkBoxDay05.Checked;
                        SelectedTask.RecurDay06 = checkBoxDay06.Checked;
                        SelectedTask.RecurDay07 = checkBoxDay07.Checked;
                        SelectedTask.RecurDay08 = checkBoxDay08.Checked;
                        SelectedTask.RecurDay09 = checkBoxDay09.Checked;
                        SelectedTask.RecurDay10 = checkBoxDay10.Checked;
                        SelectedTask.RecurDay11 = checkBoxDay11.Checked;
                        SelectedTask.RecurDay12 = checkBoxDay12.Checked;
                        SelectedTask.RecurDay13 = checkBoxDay13.Checked;
                        SelectedTask.RecurDay14 = checkBoxDay14.Checked;
                        SelectedTask.RecurDay15 = checkBoxDay15.Checked;
                        SelectedTask.RecurDay16 = checkBoxDay16.Checked;
                        SelectedTask.RecurDay17 = checkBoxDay17.Checked;
                        SelectedTask.RecurDay18 = checkBoxDay18.Checked;
                        SelectedTask.RecurDay19 = checkBoxDay19.Checked;
                        SelectedTask.RecurDay20 = checkBoxDay20.Checked;
                        SelectedTask.RecurDay21 = checkBoxDay21.Checked;
                        SelectedTask.RecurDay22 = checkBoxDay22.Checked;
                        SelectedTask.RecurDay23 = checkBoxDay23.Checked;
                        SelectedTask.RecurDay24 = checkBoxDay24.Checked;
                        SelectedTask.RecurDay25 = checkBoxDay25.Checked;
                        SelectedTask.RecurDay26 = checkBoxDay26.Checked;
                        SelectedTask.RecurDay27 = checkBoxDay27.Checked;
                        SelectedTask.RecurDay28 = checkBoxDay28.Checked;
                        SelectedTask.RecurDay29 = checkBoxDay29.Checked;
                        SelectedTask.RecurDay30 = checkBoxDay30.Checked;
                        SelectedTask.RecurDay31 = checkBoxDay31.Checked;
                        #endregion
                    }

                    db.SaveChanges();

                    zvsEntityControl.CallScheduledTaskModified(this, "multiple_properties");
                }
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
                //monthly
                btn_odd.Enabled = false;
                btn_even.Enabled = false;
                numericUpDownOccurMonths.Enabled = false;
                checkBoxDay01.Enabled = false;
                checkBoxDay02.Enabled = false;
                checkBoxDay03.Enabled = false;
                checkBoxDay04.Enabled = false;
                checkBoxDay05.Enabled = false;
                checkBoxDay06.Enabled = false;
                checkBoxDay07.Enabled = false;
                checkBoxDay08.Enabled = false;
                checkBoxDay09.Enabled = false;
                checkBoxDay10.Enabled = false;
                checkBoxDay11.Enabled = false;
                checkBoxDay12.Enabled = false;
                checkBoxDay13.Enabled = false;
                checkBoxDay14.Enabled = false;
                checkBoxDay15.Enabled = false;
                checkBoxDay16.Enabled = false;
                checkBoxDay17.Enabled = false;
                checkBoxDay18.Enabled = false;
                checkBoxDay19.Enabled = false;
                checkBoxDay20.Enabled = false;
                checkBoxDay21.Enabled = false;
                checkBoxDay22.Enabled = false;
                checkBoxDay23.Enabled = false;
                checkBoxDay24.Enabled = false;
                checkBoxDay25.Enabled = false;
                checkBoxDay26.Enabled = false;
                checkBoxDay27.Enabled = false;
                checkBoxDay28.Enabled = false;
                checkBoxDay29.Enabled = false;
                checkBoxDay30.Enabled = false;
                checkBoxDay31.Enabled = false; 
            }
        }

        private void deleteTask()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                scheduled_tasks task = db.scheduled_tasks.FirstOrDefault(o => o.id == ((scheduled_tasks)dataListTasks.SelectedObject).id);
                if (task != null)
                {
                    if (MessageBox.Show("Are you sure you want to delete the '" + task.friendly_name + "' task?", "Are you sure?",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.scheduled_tasks.DeleteObject(task);
                        db.SaveChanges();
                        zvsEntityControl.CallScheduledTaskModified(this, "removed");
                    }
                }
                else
                    MessageBox.Show("Please select a task to delete.", ProgramName);
            }
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

        private void btn_clear_Click(object sender, EventArgs e)
        {
            checkBoxDay01.Checked = false;
            checkBoxDay02.Checked = false;
            checkBoxDay03.Checked = false;
            checkBoxDay04.Checked = false;
            checkBoxDay05.Checked = false;
            checkBoxDay06.Checked = false;
            checkBoxDay07.Checked = false;
            checkBoxDay08.Checked = false;
            checkBoxDay09.Checked = false;
            checkBoxDay10.Checked = false;
            checkBoxDay11.Checked = false;
            checkBoxDay12.Checked = false;
            checkBoxDay13.Checked = false;
            checkBoxDay14.Checked = false;
            checkBoxDay15.Checked = false;
            checkBoxDay16.Checked = false;
            checkBoxDay17.Checked = false;
            checkBoxDay18.Checked = false;
            checkBoxDay19.Checked = false;
            checkBoxDay20.Checked = false;
            checkBoxDay21.Checked = false;
            checkBoxDay22.Checked = false;
            checkBoxDay23.Checked = false;
            checkBoxDay24.Checked = false;
            checkBoxDay25.Checked = false;
            checkBoxDay26.Checked = false;
            checkBoxDay27.Checked = false;
            checkBoxDay28.Checked = false;
            checkBoxDay29.Checked = false;
            checkBoxDay30.Checked = false;
            checkBoxDay31.Checked = false;
        }

        private void btn_odd_Click(object sender, EventArgs e)
        {
            checkBoxDay01.Checked = true;
            checkBoxDay02.Checked = false;
            checkBoxDay03.Checked = true;
            checkBoxDay04.Checked = false;
            checkBoxDay05.Checked = true;
            checkBoxDay06.Checked = false;
            checkBoxDay07.Checked = true;
            checkBoxDay08.Checked = false;
            checkBoxDay09.Checked = true;
            checkBoxDay10.Checked = false;
            checkBoxDay11.Checked = true;
            checkBoxDay12.Checked = false;
            checkBoxDay13.Checked = true;
            checkBoxDay14.Checked = false;
            checkBoxDay15.Checked = true;
            checkBoxDay16.Checked = false;
            checkBoxDay17.Checked = true;
            checkBoxDay18.Checked = false;
            checkBoxDay19.Checked = true;
            checkBoxDay20.Checked = false;
            checkBoxDay21.Checked = true;
            checkBoxDay22.Checked = false;
            checkBoxDay23.Checked = true;
            checkBoxDay24.Checked = false;
            checkBoxDay25.Checked = true;
            checkBoxDay26.Checked = false;
            checkBoxDay27.Checked = true;
            checkBoxDay28.Checked = false;
            checkBoxDay29.Checked = true;
            checkBoxDay30.Checked = false;
            checkBoxDay31.Checked = true;
        }

        private void btn_even_Click(object sender, EventArgs e)
        {
            checkBoxDay01.Checked = false;
            checkBoxDay02.Checked = true;
            checkBoxDay03.Checked = false;
            checkBoxDay04.Checked = true;
            checkBoxDay05.Checked = false;
            checkBoxDay06.Checked = true;
            checkBoxDay07.Checked = false;
            checkBoxDay08.Checked = true;
            checkBoxDay09.Checked = false;
            checkBoxDay10.Checked = true;
            checkBoxDay11.Checked = false;
            checkBoxDay12.Checked = true;
            checkBoxDay13.Checked = false;
            checkBoxDay14.Checked = true;
            checkBoxDay15.Checked = false;
            checkBoxDay16.Checked = true;
            checkBoxDay17.Checked = false;
            checkBoxDay18.Checked = true;
            checkBoxDay19.Checked = false;
            checkBoxDay20.Checked = true;
            checkBoxDay21.Checked = false;
            checkBoxDay22.Checked = true;
            checkBoxDay23.Checked = false;
            checkBoxDay24.Checked = true;
            checkBoxDay25.Checked = false;
            checkBoxDay26.Checked = true;
            checkBoxDay27.Checked = false;
            checkBoxDay28.Checked = true;
            checkBoxDay29.Checked = false;
            checkBoxDay30.Checked = true;
            checkBoxDay31.Checked = false;
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

        #region Triggers

        void zvsEntityControl_TriggerModified(object sender, string PropertyModified)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousStringDelegate(zvsEntityControl_TriggerModified), new object[] { sender, PropertyModified });
            else
            {
                SyncdataListTriggers();
            }
        }

        private void SyncdataListTriggers()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                int s_index = dataListTriggers.SelectedIndex;
                dataListTriggers.DataSource = db.device_value_triggers.OfType<device_value_triggers>().Execute(MergeOption.AppendOnly);
                dataListTriggers.SelectedIndex = s_index;

                int index = dataListTriggers.SelectedIndex;
                if (index > 0)
                    dataListTriggers.EnsureVisible(index);
            }

            Console.WriteLine("------>>>>>>> SyncdataListTriggers <<<<<<<<<<<---------------------");
        }

        private void dataListEvents_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripTriggerNull;
            else
                e.MenuStrip = contextMenuStripTrigger;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataListEvents_DoubleClick(this, new EventArgs());
        } 

        private void dataListEvents_DoubleClick(object sender, EventArgs e)
        {
            if (dataListTriggers.SelectedObject != null)
            {
                long trigger_id = ((device_value_triggers)dataListTriggers.SelectedObject).id;
                if (((device_value_triggers.TRIGGER_TYPE)((device_value_triggers)dataListTriggers.SelectedObject).trigger_type) == device_value_triggers.TRIGGER_TYPE.Basic)
                {
                    AddEditBasicTriggers eventform = new AddEditBasicTriggers(trigger_id);
                    eventform.Show();
                }
                else
                {
                    AddEditScriptngTrigger eventform = new AddEditScriptngTrigger(trigger_id);
                    eventform.Show();
                }
            }
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddEditBasicTriggers eventform = new AddEditBasicTriggers(null);
            eventform.Show();
        }

        private void toolStripMenuItemEventsNull_Click(object sender, EventArgs e)
        {
            AddEditBasicTriggers eventform = new AddEditBasicTriggers( null);
            eventform.Show();
        }

        private void createEventAdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEditScriptngTrigger eventform = new AddEditScriptngTrigger(null);
            eventform.Show();
        }       

        private void deleteEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device_value_triggers selected_trigger = db.device_value_triggers.FirstOrDefault(o => o.id == ((device_value_triggers)dataListTriggers.SelectedObject).id);
                if (selected_trigger != null)
                {
                    if (MessageBox.Show("Are you sure you want to delete this trigger?", "Are you sure?",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.device_value_triggers.DeleteObject(selected_trigger);
                        db.SaveChanges();
                        zvsEntityControl.CallTriggerModified(this, "removed");                         
                    }
                }
            }
        }
        
        #endregion     

        #region Minimize to Toolbar
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.BalloonTipTitle = zvsEntityControl.zvsNameAndVersion;
                notifyIcon1.BalloonTipText = "zVirtualScenes has been minimized to the taskbar.";
                notifyIcon1.ShowBalloonTip(3000);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void showZVirtualSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitZVSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void groupBox_Montly_Enter(object sender, EventArgs e)
        {

        }

           
    }
}