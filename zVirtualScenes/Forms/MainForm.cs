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
using zVirtualScenesAPI.Events;
using zVirtualScenesAPI.Structs;
using zVirtualScenesApplication.Forms;
using zVirtualScenesApplication.PluginSystem;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon.ValueCommon;
using zVirtualScenesApplication.Structs;

namespace zVirtualScenesApplication
{
    public partial class MainForm : Form
    {
        GroupEditor grpEditorForm;
        ActivateGroup grpActivateForm;
        public string ProgramName = API.GetProgramNameAndVersion;        
        
        //Forms and Controllers
        private formPropertiesScene formSceneProperties = new formPropertiesScene();       

        //Delegates
        public delegate void anonymousEventDelegate(object sender, EventArgs e);

        //CORE OBJECTS
        private BindingList<LogItem> _masterlog = new BindingList<LogItem>();
        private BindingList<Scene> _masterScenes = new BindingList<Scene>();
        private List<zwObject> _masterDevices = new List<zwObject>();        
        private BindingList<Task> _masterTasks = new BindingList<Task>();

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

            dataListTasks.CellRightClick += dataListTasks_CellRightClick;            
            Logger.LogItemPostAdd += Logger_LogItemPostAdd;
            DatabaseControl.ObjectModified += DatabaseControl_ObjectModified;
            zVirtualSceneEvents.ValueDataChangedEvent +=new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);
            zVirtualSceneEvents.SceneChangedEvent += new zVirtualSceneEvents.SceneChangedEventHandler(zVirtualSceneEvents_SceneChangedEvent);
            zVirtualSceneEvents.SceneCMDChangedEvent += new zVirtualSceneEvents.SceneCMDChangedEventHandler(zVirtualSceneEvents_SceneCMDChangedEvent);
            zVirtualSceneEvents.SceneRunCompleteEvent += new zVirtualSceneEvents.SceneRunCompleteEventHandler(zVirtualSceneEvents_SceneRunCompleteEvent);
            zVirtualSceneEvents.CommandRunCompleteEvent += new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
            zVirtualSceneEvents.ScheduledTaskChangedEvent += new zVirtualSceneEvents.ScheduledTaskChangedEventHandler(zVirtualSceneEvents_ScheduledTaskChangedEvent);
        }             

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {
            this.Text = ProgramName;            
            dataListViewLog.DataSource = _masterlog;
            Logger.WriteToLog(UrgencyLevel.INFO, "STARTED", "MainForm");

            pm = new PluginManager();
            //Start checking the DB for commands to run
            Thread t = new Thread(pm.RunCommand);
            t.Start();

            //Bind data to GUI elements
            // Devices
            _masterDevices = zwObject.ConvertObjDataTabletoObjList(DatabaseControl.GetObjects(true)); 
            dataListViewDevices.DataSource = _masterDevices;

            // Events
            _masterEvents = DatabaseControl.GetEventScripts();
            dataListEvents.DataSource = _masterEvents;

            // Scenes
            _masterScenes = API.Scenes.GetScenes();
            dataListViewScenes.DataSource = _masterScenes;
            // Scenes (allow rearrage but not drag and drop from other sources)
            dataListViewScenes.DropSink = new SceneDropSink();            
            if(dataListViewScenes.Items.Count >0)
                dataListViewScenes.SelectedIndex = 0;

            // Scene Commands       
            dataListViewSceneCMDs.DropSink = new SceneCommandDropSink();          

            #region Task Scheduler

            //Load Tasks            
            _masterTasks = API.ScheduledTasks.GetTasks();

            comboBox_FrequencyTask.DataSource = Enum.GetNames(typeof(Task.frequencys));
            dataListTasks.DataSource = _masterTasks;
            comboBox_ActionsTask.DataSource = _masterScenes;

            //Add default timer item if list is empty
            if (_masterTasks.Count < 1)
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
            pm.StopCommandThread();
            Application.Exit();
        }

        #region Subcribed API Events

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

        void zVirtualSceneEvents_SceneChangedEvent(int SceneID)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.SceneChangedEventHandler(zVirtualSceneEvents_SceneChangedEvent), new object[] { SceneID });
            else
            {
                Scene PreviouslySelectedScene = null;
                if (dataListViewScenes.SelectedObject != null)
                {
                    PreviouslySelectedScene = (Scene)dataListViewScenes.SelectedObject;
                }

                _masterScenes = API.Scenes.GetScenes();
                dataListViewScenes.DataSource = null;
                dataListViewScenes.DataSource = _masterScenes;

                comboBox_ActionsTask.DataSource = null;
                comboBox_ActionsTask.DataSource = _masterScenes;

                if (PreviouslySelectedScene != null)
                {
                    Scene NewSelectedScene = _masterScenes.FirstOrDefault(s => s.id == PreviouslySelectedScene.id);

                    if (NewSelectedScene != null)
                    {
                        dataListViewScenes.SelectedObject = NewSelectedScene;
                        dataListViewScenes.EnsureVisible(_masterScenes.IndexOf(NewSelectedScene));
                    }
                    else
                    {
                        if (_masterScenes.Count > 0)
                            dataListViewScenes.SelectedIndex = 0;
                    }
                }
                else
                {
                    //This probably means there was a new scene added
                    Scene NewSelectedScene = _masterScenes.FirstOrDefault(s => s.id == SceneID);

                    if (NewSelectedScene != null)
                    {
                        dataListViewScenes.SelectedObject = NewSelectedScene;
                        dataListViewScenes.EnsureVisible(_masterScenes.IndexOf(NewSelectedScene));
                    }
                }
            }
        }

        void zVirtualSceneEvents_SceneCMDChangedEvent(int SceneID)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.SceneCMDChangedEventHandler(zVirtualSceneEvents_SceneCMDChangedEvent), new object[] { SceneID });
            else
            {
                int index = dataListViewScenes.SelectedIndex;
                int indexSceneCMD = dataListViewSceneCMDs.SelectedIndex;

                foreach (Scene scene in _masterScenes)
                {
                    if (scene.id == SceneID)
                    {
                        Scene s = API.Scenes.GetScene(scene.id);
                        if (scene != null)
                        {
                            int sindex = _masterScenes.IndexOf(scene);
                            _masterScenes.Remove(scene);
                            _masterScenes.Insert(sindex, s);
                        }
                        break;
                    }
                }

                if (dataListViewScenes.Items.Count > index && index > 0)
                {
                    dataListViewScenes.SelectedIndex = index;
                    dataListViewScenes.EnsureVisible(index);
                }
                else
                {
                    if (dataListViewScenes.Items.Count > 0)
                    {
                        dataListViewScenes.SelectedIndex = 0;
                        dataListViewScenes.EnsureVisible(0);
                    }
                }

                if (dataListViewSceneCMDs.Items.Count > indexSceneCMD && indexSceneCMD > 0)
                {
                    dataListViewSceneCMDs.SelectedIndex = indexSceneCMD;
                    dataListViewSceneCMDs.EnsureVisible(indexSceneCMD);
                }
                else
                {
                    if (dataListViewSceneCMDs.Items.Count > 0)
                    {
                        dataListViewSceneCMDs.SelectedIndex = 0;
                        dataListViewSceneCMDs.EnsureVisible(0);
                    }
                }

            }
        }

        void zVirtualSceneEvents_ValueChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent), new object[] { ObjectId, ValueID, label, Value, PreviousValue });
            else
            {
                string objName = API.Object.GetObjectName(ObjectId);
                //labelLastEvent.Text = objName + " " + label + " changed to " + Value + ".";                

                if (String.IsNullOrEmpty(objName))
                    objName = "Object#" + ObjectId;

                if (!String.IsNullOrEmpty(PreviousValue))
                    Logger.WriteToLog(UrgencyLevel.INFO, objName + " " + label + " changed to " + Value + " from " + PreviousValue + ".", "EVENT");
                else
                    Logger.WriteToLog(UrgencyLevel.INFO, objName + " " + label + " changed to " + Value + ".", "EVENT");

                RefreshDeviceList();
                RefreshObjectValuesUserControl();

            }
        }

        void zVirtualSceneEvents_SceneRunCompleteEvent(int SceneID, int ErrorCount)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.SceneRunCompleteEventHandler(zVirtualSceneEvents_SceneRunCompleteEvent), new object[] { SceneID, ErrorCount });
            else
            {

                Scene scene = API.Scenes.GetScene(SceneID);
                if (scene != null)
                {
                    Logger.WriteToLog(UrgencyLevel.INFO, "Scene '" + scene.txt_name + "' has completed with " + ErrorCount + " errors.", "EVENT");
                }
            }
        }

        void zVirtualSceneEvents_CommandRunCompleteEvent(int CommandID, bool withErrors, string txtError)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent), new object[] { CommandID, withErrors, txtError });
            else
            {
                if (withErrors)
                    Logger.WriteToLog(UrgencyLevel.INFO, "Qued command #'" + CommandID.ToString() + "' has completed with errors.", "EVENT");
            }
        }

        void DatabaseControl_ObjectModified(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new anonymousEventDelegate(DatabaseControl_ObjectModified), new object[] { sender, e });
            else
            {
                RefreshDeviceList();
            }

        }

        void zVirtualSceneEvents_ScheduledTaskChangedEvent(int TaskID)
        {
            if (this.InvokeRequired)
                this.Invoke(new zVirtualSceneEvents.ScheduledTaskChangedEventHandler(zVirtualSceneEvents_ScheduledTaskChangedEvent), new object[] { TaskID });
            else
            {
                _masterTasks = API.ScheduledTasks.GetTasks();

                //Select it 
                dataListTasks.DataSource = null;
                dataListTasks.DataSource = _masterTasks;

                Task task = _masterTasks.FirstOrDefault(t => t.ID == TaskID);
                if (task != null)
                    dataListTasks.SelectedObject = task;
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
               
        #region File I/O

        private void SaveUserSettingsToFile(string path)
        {
            //try
            //{
            //    Stream stream = File.Open(path + "zVirtualScenes-Scenes.xml", FileMode.Create);
            //    XmlSerializer SScenes = new XmlSerializer(MasterScenes.GetType());
            //    SScenes.Serialize(stream, MasterScenes);
            //    stream.Close();

            //    Stream SettingsStream = File.Open(path + "zVirtualScenes-Settings.xml", FileMode.Create);
            //    XmlSerializer SSettings = new XmlSerializer(zVScenesSettings.GetType());
            //    SSettings.Serialize(SettingsStream, zVScenesSettings);
            //    SettingsStream.Close();

            //    Stream CustomDevicePropertiesStream = File.Open(path + "zVirtualScenes-ZWaveDeviceUserSettings.xml", FileMode.Create);
            //    XmlSerializer SCustomDeviceProperties = new XmlSerializer(SavedZWaveDeviceUserSettings.GetType());
            //    SCustomDeviceProperties.Serialize(CustomDevicePropertiesStream, SavedZWaveDeviceUserSettings);
            //    CustomDevicePropertiesStream.Close();

            //    Stream TimerEventsStream = File.Open(path + "zVirtualScenes-ScheduledTasks.xml", FileMode.Create);
            //    XmlSerializer STimerEvents = new XmlSerializer(MasterTimerEvents.GetType());
            //    STimerEvents.Serialize(TimerEventsStream, MasterTimerEvents);
            //    TimerEventsStream.Close();

            //}
            //catch (Exception e)
            //{
            //    AddLogEntry(UrgencyLevel.ERROR, "Error saving XML: (" + e.Message + ")");
            //}
        }

        private void SaveLogToFile()
        {
            //SAVE LOG
            //try
            //{
            //    StreamWriter SW = new System.IO.StreamWriter(APP_PATH + "zVirtualScenes.log", false, Encoding.ASCII);
            //    foreach (LogItem item in Masterlog)
            //        SW.Write(item.ToString());
            //    SW.Close();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Error saving LOG: (" + e.Message + ")");
            //}
        }

        private void LoadSettingsFromXML()
        {
            //if (File.Exists(APP_PATH + "zVirtualScenes-Scenes.xml"))
            //{
            //    try
            //    {
            //        //Open the file written above and read values from it.       
            //        XmlSerializer ScenesSerializer = new XmlSerializer(typeof(BindingList<Scene>));
            //        FileStream myFileStream = new FileStream(APP_PATH + "zVirtualScenes-Scenes.xml", FileMode.Open);
            //        MasterScenes = (BindingList<Scene>)ScenesSerializer.Deserialize(myFileStream);
            //        myFileStream.Close();
            //    }
            //    catch (Exception e)
            //    {
            //        AddLogEntry(UrgencyLevel.ERROR, "Error loading Scene XML: (" + e.Message + ")");
            //    }
            //}
            //else
            //{
            //    //Create 10 default scenes
            //    for (int id = 1; id < 11; id++)
            //    {
            //        Scene scene = new Scene();
            //        scene.ID = id;
            //        scene.Name = "Scene " + id;
            //        MasterScenes.Add(scene);
            //    }
            //}

            //if (File.Exists(APP_PATH + "zVirtualScenes-Settings.xml"))
            //{
            //    try
            //    {
            //        XmlSerializer SettingsSerializer = new XmlSerializer(typeof(UserSettings));
            //        FileStream SettingsileStream = new FileStream(APP_PATH + "zVirtualScenes-Settings.xml", FileMode.Open);
            //        zVScenesSettings = (UserSettings)SettingsSerializer.Deserialize(SettingsileStream);
            //        SettingsileStream.Close();
            //    }
            //    catch (Exception e)
            //    {
            //        AddLogEntry(UrgencyLevel.ERROR, "Error loading Settings XML: (" + e.Message + ")");
            //    }
            //}

            //if (File.Exists(APP_PATH + "zVirtualScenes-ZWaveDeviceUserSettings.xml"))
            //{
            //    try
            //    {
            //        XmlSerializer CustomDevicePropertiesSerializer = new XmlSerializer(typeof(BindingList<ZWaveDeviceUserSettings>));
            //        FileStream CustomDevicePropertiesileStream = new FileStream(APP_PATH + "zVirtualScenes-ZWaveDeviceUserSettings.xml", FileMode.Open);
            //        SavedZWaveDeviceUserSettings = (BindingList<ZWaveDeviceUserSettings>)CustomDevicePropertiesSerializer.Deserialize(CustomDevicePropertiesileStream);
            //        CustomDevicePropertiesileStream.Close();
            //    }
            //    catch (Exception e)
            //    {
            //        AddLogEntry(UrgencyLevel.ERROR, "Error loading Settings XML: (" + e.Message + ")");
            //    }
            //}

            //if (File.Exists(APP_PATH + "zVirtualScenes-ScheduledTasks.xml"))
            //{
            //    try
            //    {
            //        XmlSerializer TimerEventSerializer = new XmlSerializer(typeof(BindingList<Task>));
            //        FileStream TimerEventStream = new FileStream(APP_PATH + "zVirtualScenes-ScheduledTasks.xml", FileMode.Open);
            //        MasterTimerEvents = (BindingList<Task>)TimerEventSerializer.Deserialize(TimerEventStream);
            //        TimerEventStream.Close();
            //    }
            //    catch (Exception e)
            //    {
            //        AddLogEntry(UrgencyLevel.ERROR, "Error loading Settings XML: (" + e.Message + ")");
            //    }
            //}
            //AddLogEntry(UrgencyLevel.INFO, "Loaded Program Settings.");
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
            if (dataListViewScenes.SelectedIndex != -1)
            {
                dataListViewSceneCMDs.Visible = true;
                Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;
                dataListViewSceneCMDs.DataSource = selectedscene.scene_commands;
                lbl_sceneActions.Text = "Scene " + selectedscene.id.ToString() + " '" + selectedscene.txt_name + "' Actions";
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
                Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;
                int newsceneID = API.Scenes.Add(selectedscene.txt_name + " Copy");

                foreach (SceneCommands sc in selectedscene.scene_commands)
                {
                    SceneCommands newSCMD = new SceneCommands();
                    newSCMD.ObjectId = sc.ObjectId;
                    newSCMD.cmdtype = sc.cmdtype;
                    newSCMD.CommandId = sc.CommandId;
                    newSCMD.Argument = sc.Argument;
                    newSCMD.order = sc.order;
                    API.Scenes.AddSceneCommand(newsceneID, newSCMD);
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
            int index = dataListViewScenes.SelectedIndex - 1;
                        
            if (dataListViewScenes.SelectedIndex != -1 && dataListViewScenes.Items.Count > 1)
            {
                Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;
                if (MessageBox.Show("Are you sure you want to delete this scene?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    selectedscene.Remove();                    
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
            API.Scenes.AddBlankScene();            
            int index = dataListViewScenes.Items.Count -1 ;
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
            if (e.SourceModels[0].GetType().Equals(typeof(Scene)))
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
            if (e.SourceModels[0].GetType().Equals(typeof(Scene)))
            {

                Scene SourceScene = (Scene)e.SourceModels[0];
                int SourceIndex = _masterScenes.IndexOf(SourceScene);

                switch (e.DropTargetLocation)
                {
                    case DropTargetLocation.BelowItem:
                        TargetIndex = TargetIndex + 1;
                        break;
                }

                _masterScenes.Insert(TargetIndex, SourceScene);

                if (TargetIndex > SourceIndex)
                {
                    _masterScenes.RemoveAt(SourceIndex);
                    TargetIndex--;
                }
                else
                {
                    _masterScenes.RemoveAt(SourceIndex + 1);
                }

                dataListViewScenes.SelectedIndex = TargetIndex;
                dataListViewScenes.EnsureVisible(TargetIndex);

                e.RefreshObjects();
                API.Scenes.SaveOrder(_masterScenes);
            }
        }

        private void OpenScenePropertiesWindow()
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                formSceneProperties = new formPropertiesScene();
                formSceneProperties._scene = (Scene)dataListViewScenes.SelectedObject;
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

        private void btnAddCMD_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                Scene selected_scene = (Scene)dataListViewScenes.SelectedObject;

                AddEditSceneObjectCMD addCMDform = new AddEditSceneObjectCMD(_masterDevices);
                if (addCMDform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SceneCommands newSCMD = new SceneCommands();
                    newSCMD.ObjectId = addCMDform.ObjectID;
                    newSCMD.cmdtype = addCMDform.CMD.cmdtype;
                    newSCMD.CommandId = addCMDform.CMD.CommandId;
                    newSCMD.Argument = addCMDform.argument;
                    newSCMD.order = 99;
                    API.Scenes.AddSceneCommand(selected_scene.id, newSCMD);
                }
            }
            else
            {
                MessageBox.Show("Please select a scene!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void btnAddBltInCMD_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1)
            {
                Scene selected_scene = (Scene)dataListViewScenes.SelectedObject;

                AddEditSceneBuiltinCMD addCMDform = new AddEditSceneBuiltinCMD(_masterDevices);
                if (addCMDform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SceneCommands newSCMD = new SceneCommands();
                    newSCMD.cmdtype = addCMDform.CMD.cmdtype;
                    newSCMD.CommandId = addCMDform.CMD.CommandId;
                    newSCMD.Argument = addCMDform.argument;
                    newSCMD.order = 99;
                    API.Scenes.AddSceneCommand(selected_scene.id, newSCMD);
                }
            }
            else
            {
                MessageBox.Show("Please select a scene!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void runSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedObject != null)
            {
                Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;
                Logger.WriteToLog(UrgencyLevel.INFO, selectedscene.RunScene(), "MAIN");
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
            else
                e.MenuStrip = cmsSceneCMDnull;
        }

        private void addCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddCMD_Click(this, new EventArgs());
        }

        private void deleteCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1 && dataListViewSceneCMDs.SelectedObjects.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected scene command(s)?",
                                    API.GetProgramNameAndVersion,
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Scene selected_scene = (Scene)dataListViewScenes.SelectedObject;

                    foreach (SceneCommands selected_cmd in dataListViewSceneCMDs.SelectedObjects)
                    {
                        API.Scenes.RemoveSceneCommand(selected_scene.id, selected_cmd);
                    }
                }
            }
        }

        private void editCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListViewScenes.SelectedIndex != -1 && dataListViewSceneCMDs.SelectedObjects.Count > 0)
            {
                Scene selected_scene = (Scene)dataListViewScenes.SelectedObject;

                //TODO: add
                //    if (selectedScene.isRunning)
                //    {
                //        //MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                //        return;
                //    }

                foreach (SceneCommands selected_cmd in dataListViewSceneCMDs.SelectedObjects)
                {
                    switch (selected_cmd.cmdtype)
                    {
                        case cmdType.Builtin:
                            AddEditSceneBuiltinCMD editBuiltinCMDform = new AddEditSceneBuiltinCMD(_masterDevices);
                            editBuiltinCMDform.SceneCMDtoEdit = selected_cmd;
                            if (editBuiltinCMDform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                selected_cmd.cmdtype = editBuiltinCMDform.CMD.cmdtype;
                                selected_cmd.CommandId = editBuiltinCMDform.CMD.CommandId;
                                selected_cmd.Argument = editBuiltinCMDform.argument;
                                API.Scenes.UpdateSceneCommand(selected_scene.id, selected_cmd);
                            }
                            break;
                        default:
                            AddEditSceneObjectCMD editObjCMDform = new AddEditSceneObjectCMD(_masterDevices);
                            editObjCMDform.SceneCMDtoEdit = selected_cmd;
                            if (editObjCMDform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                selected_cmd.ObjectId = editObjCMDform.ObjectID;
                                selected_cmd.cmdtype = editObjCMDform.CMD.cmdtype;
                                selected_cmd.CommandId = editObjCMDform.CMD.CommandId;
                                selected_cmd.Argument = editObjCMDform.argument;
                                API.Scenes.UpdateSceneCommand(selected_scene.id, selected_cmd);
                            }
                            break;
                    }
                }
            }
        }

        private void addCommandToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            addCommandToolStripMenuItem_Click(this, new EventArgs());
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
            if (e.SourceModels[0].GetType().Equals(typeof(zwObject)))
            {
                e.Effect = DragDropEffects.Copy;
                e.InfoMessage = "Create new action for this device";
            }
            else if (e.SourceModels[0].GetType().Equals(typeof(SceneCommands)))
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

        private void dataListViewSceneCMDs_ModelDropped(object sender, ModelDropEventArgs e)
        {
            int TargetIndex = 0;

            //Handle if dropped into empty Action box
            if (e.TargetModel == null)
                TargetIndex = 0;
            else
                TargetIndex = e.DropTargetIndex;

            //Handle Device Drop
            //if (e.SourceModels[0].GetType().ToString() == "zVirtualScenesApplication.ZWaveDevice")
            //{
            //    Scene selectedScene = (Scene)dataListViewScenes.SelectedObject;
            //    if (selectedScene.isRunning)
            //    {
            //        //MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
            //        return;
            //    }

            //    foreach (ZWaveDevice SourceDevice in e.SourceModels)
            //    {
            //        if (SourceDevice.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch || SourceDevice.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch || SourceDevice.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
            //        {
            //            //Create Simple Action from Source
            //            Action TheAction = new Action();
            //            TheAction = (Action)SourceDevice;

            //            if (SourceDevice.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
            //                TheAction.HeatCoolMode = (int)ZWaveDevice.ThermostatMode.Off;

            //            Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;
            //            //If the last item in the list and dropped at the bottome, Add it to the bottom of list.
            //            if (TargetIndex == selectedscene.Actions.Count() - 1 && e.DropTargetLocation == DropTargetLocation.BelowItem)
            //                TargetIndex++;

            //            //Add it to the scene action list
            //            selectedscene.Actions.Insert(TargetIndex, TheAction);
            //        }
            //    }
            //    return;
            //}
            if (e.SourceModels[0].GetType().Equals(typeof(SceneCommands)))
            {
                //Rearrage Actions
                Scene selectedscene = (Scene)dataListViewScenes.SelectedObject;

                //DIE if running.
                //if (selectedscene.isRunning)
                //{
                //    MessageBox.Show("Cannot modify scene when it is running.", ProgramName);
                //    return;
                //}

                int SourceIndex = selectedscene.scene_commands.IndexOf((SceneCommands)e.SourceModels[0]);
                int MovingTarget = TargetIndex;

                //IF WE ARE REARRANGING DOWNWARD, and drop and item above subtract one from the target because that is the ACTUAL ID of insert. 
                if (SourceIndex < TargetIndex && e.DropTargetLocation == DropTargetLocation.AboveItem)
                    MovingTarget--;

                //labelSceneRunStatus.Text = "sourceIndex:" + selectedscene.Actions.IndexOf((Action)e.SourceModels[0]) + ", targetindex:" + TargetIndex + ", loc:" + e.DropTargetLocation.ToString();
                foreach (SceneCommands sCMD in e.SourceModels)
                {
                    SceneCommands tmpSCmd = sCMD;
                    selectedscene.scene_commands.Remove(sCMD);
                    selectedscene.scene_commands.Insert(MovingTarget, tmpSCmd);

                    //If the first item in the selection is being moved ABOVE, Increase target so the order of the selected actions are not reversed. 
                    if (TargetIndex <= SourceIndex)
                        MovingTarget++;
                }
                API.Scenes.SaveCMDOrder(selectedscene.scene_commands);
            }
        }    

        #endregion

        #region Object List Box Handling

        private void dataListViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshObjectValuesUserControl();
        }

        private void RefreshObjectValuesUserControl()
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            {
                zwObject zwObj = (zwObject)dataListViewDevices.SelectedObjects[0];
                uc_object_values1.UpdateControl(zwObj);
                uc_object_values1.Visible = true;
            }
            else
            {
                uc_object_values1.UpdateControl(null);
            }
        }        

        private void RefreshDeviceList()
        {
            int lastSelectionIndex = dataListViewDevices.SelectedIndex;

            _masterDevices = zwObject.ConvertObjDataTabletoObjList(DatabaseControl.GetObjects(true));
            dataListViewDevices.DataSource = null;
            dataListViewDevices.DataSource = _masterDevices;

            if (dataListViewDevices.Items.Count > lastSelectionIndex)
                dataListViewDevices.SelectedIndex = lastSelectionIndex;

        }

        private void dataListViewDevices_ItemsChanging(object sender, ItemsChangingEventArgs e)
        {
            //label_devicecount.Text = DatabaseControl.GetObjects().Rows.Count + " devices";
        }

        private void OpenDevicePropertyWindow()
        {
            if (dataListViewDevices.SelectedObjects.Count > 0)
            { 
                foreach (zwObject zwObj in dataListViewDevices.SelectedObjects)
                {
                    // TODO: Open the properties window for this object!
                    ObjectProperties properties = new ObjectProperties(zwObj.ID);
                    properties.Show();

                    //        if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                    //        {
                    //            formPropertiesBinSwitch formPropertiesBinSwitch = new formPropertiesBinSwitch(this, selecteddevice);
                    //            formPropertiesBinSwitch.ShowDialog();
                    //        }
                    //        else if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                    //        {
                    //            formPropertiesMultiLevelSwitch formPropertiesMultiLevelSwitch = new formPropertiesMultiLevelSwitch(this, selecteddevice);
                    //            formPropertiesMultiLevelSwitch.ShowDialog();
                    //        }
                    //        else if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                    //        {
                    //            formPropertiesThermostat formPropertiesThermostat = new formPropertiesThermostat(this, selecteddevice);
                    //            formPropertiesThermostat.ShowDialog();
                    //        }
                }
            }
            else
                MessageBox.Show("Please select at least one object.", "zVirtualScenes");
        }

        private void dataListViewDevices_DoubleClick(object sender, EventArgs e)
        {
            //Get the Col Index the user clicked on...
            //Point pt = Cursor.Position;
            //pt = dataListViewDevices.PointToClient(pt);
            //ListViewHitTestInfo info = this.dataListViewDevices.HitTest(pt);
            //int SubitemIndex = info.Item.SubItems.IndexOf(info.SubItem);

            ////if they clicked on a device status col, show action popup else show device properties
            //if (SubitemIndex >= 2 && SubitemIndex <= 7)
            //    CreateNewActionFromZWaveDevice();
            //else
            OpenDevicePropertyWindow();
        }

        private void findNewDevicesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //ControlThinkInt.ConnectAndFindDevices();
        }

        private void dataListViewDevices_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Item == null)
                e.MenuStrip = contextMenuStripDevicesNull;
            else
            {
                ContextMenuStrip contextMenuStripDevicesDynamicCMDs = new ContextMenuStrip();
                contextMenuStripDevicesDynamicCMDs.Items.Clear();

                zwObject zwObj = (zwObject)e.Item.RowObject;
                               
                //Create a new context menu
                ToolStripMenuItem CMDcontainer = new ToolStripMenuItem();
                CMDcontainer.Name = zwObj.Name;
                CMDcontainer.Text = zwObj.Name;
                CMDcontainer.Tag = "dynamic_cmd_menu";

                //API.GetAllObjectTypeCommandForObject(zwObj.ID).Rows
                List<Command> CommandsForThisObject = API.Commands.GetAllObjectCommandsForObjectasCMD(zwObj.ID);
                CommandsForThisObject.AddRange(API.Commands.GetAllObjectTypeCommandsForObjectasCMD(zwObj.ID));

                foreach (Command c in CommandsForThisObject)
                {
                    switch (c.paramType)
                    {
                        case ParamType.NONE:
                        {
                            QuedCommand zCMD = new QuedCommand();
                            zCMD.CommandId = c.CommandId;
                            zCMD.cmdtype = c.cmdtype;
                            zCMD.ObjectId = zwObj.ID;
                                
                            ToolStripMenuItem item = new ToolStripMenuItem();
                            item.Name = c.Name;
                            item.Text = c.FriendlyName;
                            item.ToolTipText = c.HelpText;
                            item.Tag = zCMD;
                            item.Click += new EventHandler(dynamic_CMD_item_Click);
                            CMDcontainer.DropDownItems.Add(item);
                            break;
                        }
                        case ParamType.LIST:
                        {
                            //ROOT MENU
                            ToolStripMenuItem item = new ToolStripMenuItem();
                            item.Name = c.Name;
                            item.Text = c.FriendlyName;
                            item.ToolTipText = c.HelpText;

                            //MAKE SUB MENU
                            List<string> options = new List<string>();
                            if (c.cmdtype == cmdType.Object)
                                options = API.Commands.GetObjectCommandOptions(c.CommandId);
                            else if (c.cmdtype == cmdType.ObjectType)
                                options = API.Commands.GetObjectTypeCommandOptions(c.CommandId);

                            foreach (string option in options)
                            {
                                QuedCommand zCMD = new QuedCommand();
                                zCMD.CommandId = c.CommandId;
                                zCMD.cmdtype = c.cmdtype;
                                zCMD.ObjectId = zwObj.ID;
                                zCMD.Argument = option;

                                ToolStripMenuItem option_item = new ToolStripMenuItem();
                                option_item.Name = option;
                                option_item.Text = option;
                                option_item.Tag = zCMD;
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
                repoll.Name = "Repoll Device";
                repoll.Text = "Repoll Device";
                repoll.Click += new EventHandler(Repoll_Click);
                repoll.Tag = zwObj.ID;

                contextMenuStripDevicesDynamicCMDs.Items.Add(repoll);
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Activate Groups", null, new EventHandler(ActivateGroups_Click)));
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Edit Groups", null, new EventHandler(EditGroups_Click)));
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripSeparator());
                contextMenuStripDevicesDynamicCMDs.Items.Add(new ToolStripMenuItem("Properties", null, new EventHandler(deviceProperties_Click)));

            }
        }

   

        private void dynamic_CMD_item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            QuedCommand zCMD = (QuedCommand)item.Tag;
            API.Commands.InstallQueCommandAndProcess(zCMD);        
        }

        private void Repoll_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;          
            int id = (int)item.Tag;

            int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ME");
            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Builtin, CommandId = cmdId, Argument = id.ToString() });
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

         void adjustLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewActionFromZWaveDevice();
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
            int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ALL");
            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Builtin, CommandId = cmdId});
        }

        private void findNewDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ControlThinkInt.ConnectAndFindDevices();
        }

        /// <summary>
        /// ADD ZWAVE DEIVCE ACTIONS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewActionFromZWaveDevice()
        {
            //if (dataListViewDevices.SelectedObjects.Count > 0 && dataListViewScenes.SelectedObjects.Count > 0)
            //    foreach (ZWaveDevice selecteddevice in dataListViewDevices.SelectedObjects)
            //    {
            //        if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
            //        {
            //            formAddEditActionBinSwitch formAddEditActionBinSwitch = new formAddEditActionBinSwitch(this, (Scene)dataListViewScenes.SelectedObject, selecteddevice, dataListViewActions.SelectedIndex + 1);
            //            formAddEditActionBinSwitch.ShowDialog();
            //        }
            //        else if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
            //        {
            //            formAddEditActionMultiLevelSwitch formAddEditActionMultiLevelSwitch = new formAddEditActionMultiLevelSwitch(this, (Scene)dataListViewScenes.SelectedObject, selecteddevice, dataListViewActions.SelectedIndex + 1);
            //            formAddEditActionMultiLevelSwitch.ShowDialog();
            //        }
            //        else if (selecteddevice.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
            //        {
            //            formAddEditActionThermostat formAddEditActionThermostat = new formAddEditActionThermostat(this, (Scene)dataListViewScenes.SelectedObject, selecteddevice, dataListViewActions.SelectedIndex + 1);
            //            formAddEditActionThermostat.ShowDialog();
            //        }
            //    }
            //else
            //    MessageBox.Show("Please select a ZWave device and one scene.", ProgramName);
        }

        private void btn_AddAction_Click(object sender, EventArgs e)
        {
            CreateNewActionFromZWaveDevice();
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
            DatabaseConnection FormDatabaseConnection = new DatabaseConnection();
            FormDatabaseConnection.ShowDialog();
        }

        private void entireDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all data?  ALL YOUR PLUGIN SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    if (string.IsNullOrEmpty(API.Database.ClearDatabase()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void pluginDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all plugin data?  ALL YOUR PLUGIN SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    if (string.IsNullOrEmpty(API.PluginSettings.ClearAllPluginData()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void objectPropertyDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all object property data?  ALL YOUR OBJECT PROPERTY SETTINGS WILL BE ERASED!  \n\n Note: The program will close after this process and will have to be restarted!", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    if (string.IsNullOrEmpty(API.Object.Properties.ClearObjectProperties()))
                        Environment.Exit(0);
                    break;
            }
        }

        private void commandDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Are you sure you want to delete all command data?", API.GetProgramNameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                case DialogResult.Yes:
                    if (string.IsNullOrEmpty(API.Commands.ClearAllCommands()))
                        MessageBox.Show("All Command Data Cleared. \n\n Please restart the program to rebuild commands.", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void repollAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ALL");
            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Builtin, CommandId = cmdId });
        }

        private void aaronRestoreNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            API.Database.NonQuery(
                                 "UPDATE `objects` SET `txt_object_name`='Aeon Labs Z-Stick Series 2' WHERE `node_id`='1';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bathtub Light' WHERE `node_id`='3';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bath Mirror Light' WHERE `node_id`='4';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bed Hallzway Light' WHERE `node_id`='5';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bedroom East Light' WHERE `node_id`='6';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bedroom Bed Light' WHERE `node_id`='7';" +
                            "UPDATE `objects` SET `txt_object_name`='Office Light' WHERE `node_id`='8';" +
                            "UPDATE `objects` SET `txt_object_name`='Family Hallway Light' WHERE `node_id`='9';" +
                            "UPDATE `objects` SET `txt_object_name`='Outside Entry Light' WHERE `node_id`='10';" +
                            "UPDATE `objects` SET `txt_object_name`='Entryway Light' WHERE `node_id`='11';" +
                            "UPDATE `objects` SET `txt_object_name`='Can Lights' WHERE `node_id`='12';" +
                            "UPDATE `objects` SET `txt_object_name`='Pourch Light' WHERE `node_id`='13';" +
                            "UPDATE `objects` SET `txt_object_name`='Dining Table Light' WHERE `node_id`='14';" +
                            "UPDATE `objects` SET `txt_object_name`='Fan Light' WHERE `node_id`='15';" +
                            "UPDATE `objects` SET `txt_object_name`='Kitchen Light' WHERE `node_id`='16';" +
                            "UPDATE `objects` SET `txt_object_name`='Rear Garage Light' WHERE `node_id`='17';" +
                            "UPDATE `objects` SET `txt_object_name`='Driveway Light' WHERE `node_id`='18';" +
                            "UPDATE `objects` SET `txt_object_name`='TV Backlight (LG)' WHERE `node_id`='19';" +
                            "UPDATE `objects` SET `txt_object_name`='Fireplace Light' WHERE `node_id`='20';" +
                            "UPDATE `objects` SET `txt_object_name`='Brother Printer' WHERE `node_id`='22';" +
                            "UPDATE `objects` SET `txt_object_name`='Hairagami Printer' WHERE `node_id`='23';" +
                            "UPDATE `objects` SET `txt_object_name`='South Thermostat' WHERE `node_id`='24';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Window Fan' WHERE `node_id`='25';" +
                            "UPDATE `objects` SET `txt_object_name`='Master Bed Thermostat' WHERE `node_id`='26';");
        }
        #endregion

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F5))
            {
                int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ALL");
                API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Builtin, CommandId = cmdId });
            }

        }      

        #endregion

        #region Invokeable Functions
        /// <summary>
        /// Logs an event that can be called from any thread.  Self Invokes.
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="message">Log Event Message</param>
        public void AddLogEntry(UrgencyLevel urgency, string message, string theInterface = "MAIN")
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

            //TODO:  MAKE EVENT BASED
            if (UpdateScripts)
            {
                _masterEvents = DatabaseControl.GetEventScripts();
                dataListEvents.DataSource = null;
                dataListEvents.DataSource = _masterEvents;
                UpdateScripts = false;
            }

            foreach (Task task in _masterTasks)
            {
                if (task.Enabled)
                {
                    switch (task.Frequency)
                    {
                        case Task.frequencys.Seconds:
                            int sec =  (int)(DateTime.Now - task.StartTime).TotalSeconds;
                            if (sec % task.RecurSeconds == 0)
                            {                                
                                RunScheduledTaskScene(task.SceneID, task.Name);
                            }
                            break;
                        case Task.frequencys.Daily:
                            if ((DateTime.Now.Date - task.StartTime.Date).TotalDays % task.RecurDays == 0)
                            {
                                Double SecondsBetweenTime = (task.StartTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
                                if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
                                    RunScheduledTaskScene(task.SceneID, task.Name);
                            }
                            break;
                        case Task.frequencys.Weekly:
                            if (((Int32)(DateTime.Now.Date - task.StartTime.Date).TotalDays / 7) % task.RecurWeeks == 0)  //IF RUN THIS WEEK
                            {
                                if (ShouldRunToday(task, DateTime.Now))  //IF RUN THIS DAY 
                                {
                                    Double SecondsBetweenTime = (task.StartTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds;
                                    if (SecondsBetweenTime < 1 && SecondsBetweenTime > 0)
                                        RunScheduledTaskScene(task.SceneID, task.Name);
                                }
                            }
                            break;
                        case Task.frequencys.Once:
                            Double SecondsBetween = (DateTime.Now - task.StartTime).TotalSeconds;
                            if (SecondsBetween < 1 && SecondsBetween > 0)
                                RunScheduledTaskScene(task.SceneID, task.Name);
                            break;
                    }
                }
            }
        }

        private bool ShouldRunToday(Task task, DateTime Today)
        {
            switch (Today.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (task.RecurMonday)
                        return true;
                    break;

                case DayOfWeek.Tuesday:
                    if (task.RecurTuesday)
                        return true;
                    break;

                case DayOfWeek.Wednesday:
                    if (task.RecurWednesday)
                        return true;
                    break;

                case DayOfWeek.Thursday:
                    if (task.RecurThursday)
                        return true;
                    break;

                case DayOfWeek.Friday:
                    if (task.RecurFriday)
                        return true;
                    break;

                case DayOfWeek.Saturday:
                    if (task.RecurSaturday)
                        return true;
                    break;

                case DayOfWeek.Sunday:
                    if (task.RecurTuesday)
                        return true;
                    break;
            }

            return false;
        }

        private void RunScheduledTaskScene(int SceneID, string taskname)
        {
            Scene scene = _masterScenes.FirstOrDefault(s => s.id == SceneID);

            if (scene != null)
            {
                string result = scene.RunScene();
                Logger.WriteToLog(UrgencyLevel.INFO, "Scheduled task '" + taskname + "': " + result, "TASK");             
            }
            else
                Logger.WriteToLog(UrgencyLevel.WARNING, "Scheduled task '" + taskname + "': Failed to find scene ID '" + SceneID.ToString() + "'.", "TASK");
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

        private void LoadGui(Task Task)
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

            textBox_TaskName.Text = Task.Name;
            comboBox_FrequencyTask.SelectedIndex = (int)Task.Frequency;            
            checkBox_EnabledTask.Checked = Task.Enabled;
            dateTimePickerStartTask.Value = Task.StartTime;
            numericUpDownOccurWeeks.Value = Task.RecurWeeks;
            numericUpDownOccurDays.Value = Task.RecurDays;
            numericUpDownOccurSeconds.Value = Task.RecurSeconds;
            checkBox_RecurMonday.Checked = Task.RecurMonday;
            checkBox_RecurTuesday.Checked = Task.RecurTuesday;
            checkBox_RecurWednesday.Checked = Task.RecurWednesday;
            checkBox_RecurThursday.Checked = Task.RecurThursday;
            checkBox_RecurFriday.Checked = Task.RecurFriday;
            checkBox_RecurSaturday.Checked = Task.RecurSaturday;
            checkBox_RecurSunday.Checked = Task.RecurSunday;

            //Look for Scene, if it was deleted then set index to -1
            bool found = false;
            foreach (Scene scene in _masterScenes)
            {
                if (Task.SceneID == scene.id)
                {
                    found = true;
                    comboBox_ActionsTask.SelectedItem = scene;
                    break;
                }
            }
            if (!found)
                comboBox_ActionsTask.SelectedIndex = -1;
        }

        private void AddNewTask()
        {
            API.ScheduledTasks.Add(new Task());            
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
                case (int)Task.frequencys.Daily:
                    groupBox_Daily.Visible = true;
                    break;
                case (int)Task.frequencys.Weekly:
                    groupBox_Weekly.Visible = true; 
                    break;
                case (int)Task.frequencys.Seconds:
                    groupBox_Seconds.Visible = true;
                    break;
            }
        }

        private void button_SaveTask_Click(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedObject != null)
            {
                Task SelectedTask = (Task)dataListTasks.SelectedObject;

                //Task Name
                if (textBox_TaskName.Text != "")
                    SelectedTask.Name = textBox_TaskName.Text;
                else
                {
                    MessageBox.Show("Error Saving\n\nTask name not vaild.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                    return;
                }

                //Frequency
                SelectedTask.Frequency = (Task.frequencys)Enum.Parse(typeof(Task.frequencys), comboBox_FrequencyTask.SelectedItem.ToString());

                //Endabled 
                SelectedTask.Enabled = checkBox_EnabledTask.Checked;

                //DateTime
                SelectedTask.StartTime = dateTimePickerStartTask.Value;

                //Recur Days 
                if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Daily.ToString()))
                {
                   SelectedTask.RecurDays = (int)numericUpDownOccurDays.Value;                   
                }
                else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Seconds.ToString()))
                {
                    SelectedTask.RecurSeconds = (int)numericUpDownOccurSeconds.Value;
                }
                else if (comboBox_FrequencyTask.SelectedValue.ToString().Equals(Task.frequencys.Weekly.ToString()))
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

                //Action
                if (comboBox_ActionsTask.SelectedIndex != -1)
                {
                    Scene SelectedScene = (Scene)comboBox_ActionsTask.SelectedItem;
                    SelectedTask.SceneID = SelectedScene.id;
                }
                else
                {
                    MessageBox.Show("Error Saving\n\nPlease select a scene to activate before saving.", ProgramName,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                SelectedTask.Update();
            }
        }

        private void dataListTasks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (dataListTasks.SelectedObject != null)
                LoadGui((Task)dataListTasks.SelectedObject);
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
            if (dataListTasks.SelectedObject != null)
            {
                Task selectedTask = (Task)dataListTasks.SelectedObject;

                if (MessageBox.Show("Are you sure you want to delete the '" + selectedTask.Name + "' task?", "Are you sure?",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    selectedTask.Remove();

                    if (_masterTasks.Count > 0)
                        dataListTasks.SelectedIndex = 0;                    
                } 
            }
            else
                MessageBox.Show("Please select a task to delete.", ProgramName);
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
            ScriptEditor scriptEditor = new ScriptEditor(0);
            scriptEditor.Show();
        }

        private void btnEditEvent_Click(object sender, EventArgs e)
        {
            if (dataListEvents.SelectedIndex > -1)
            {
                int scriptId;
                int.TryParse(_masterEvents.Rows[dataListEvents.SelectedIndex]["id"].ToString(), out scriptId);
                ScriptEditor scriptEditor = new ScriptEditor(scriptId);
                scriptEditor.Show();
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
                    DatabaseControl.DeleteEventScript(scriptId);
                    UpdateScripts = true;
                }
            }
        }
        #endregion        

        //TODO: Make Plugins for this stuff
        #region Make Plugin for this stuff

        

        //growl.RegisterGrowl();
        
        //try
        //{
        //    if (netservice == null)
        //        PublishZeroconf();

        //}
        //catch (Exception ex)
        //{
        //    AddLogEntry(UrgencyLevel.ERROR, ex.Message, ZEROCONF_LOG_ENTRY);
        //}

        

        #region ZeroConf/Bonjour
        public static string ZEROCONF_LOG_ENTRY = "ZERO CONFIG";

        private void PublishZeroconf()
        {

            //Version bonjourVersion = NetService.DaemonVersion;
            //String domain = "";
            //String type = "_zvsxmlsocket._tcp.";
            //String name = "zVirtualScenesXMLSocket " + Environment.MachineName;
            //netservice = new NetService(domain, type, name, zVScenesSettings.XMLSocketPort);
            //netservice.AllowMultithreadedCallbacks = true;
            //netservice.DidPublishService += new NetService.ServicePublished(publishService_DidPublishService);
            //netservice.DidNotPublishService += new NetService.ServiceNotPublished(publishService_DidNotPublishService);

            ///* HARDCODE TXT RECORD */
            //System.Collections.Hashtable dict = new System.Collections.Hashtable();
            //dict.Add("txtvers", "1");
            //dict.Add("ServiceName", name);
            //dict.Add("MachineName", Environment.MachineName);
            //dict.Add("OS", Environment.OSVersion.ToString());
            //dict.Add("IPAddress", "127.0.0.1");
            //dict.Add("Version", Application.ProductVersion);
            //netservice.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
            //netservice.Publish();

            //type = "_lightswitch._tcp.";
            //name = "Lightswitch " + Environment.MachineName;
            //netservice = new NetService(domain, type, name, zVScenesSettings.LightSwitchPort);
            //netservice.AllowMultithreadedCallbacks = true;
            //netservice.DidPublishService += new NetService.ServicePublished(publishService_DidPublishService);
            //netservice.DidNotPublishService += new NetService.ServiceNotPublished(publishService_DidNotPublishService);

            ///* HARDCODE TXT RECORD */
            //dict = new System.Collections.Hashtable();
            //dict.Add("txtvers", "1");
            //dict.Add("ServiceName", name);
            //dict.Add("MachineName", Environment.MachineName);
            //dict.Add("OS", Environment.OSVersion.ToString());
            //dict.Add("IPAddress", "127.0.0.1");
            //dict.Add("Version", Application.ProductVersion);
            //netservice.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
            //netservice.Publish();
        }

        //void publishService_DidPublishService(NetService service)
        //{
        //    string result = String.Format("Published Service: domain({0}) type({1}) name({2})", service.Domain, service.Type, service.Name);
        //    AddLogEntry(UrgencyLevel.INFO, result, ZEROCONF_LOG_ENTRY);
        //}

        //void publishService_DidNotPublishService(NetService service, DNSServiceException ex)
        //{
        //    AddLogEntry(UrgencyLevel.ERROR, ex.Message, ZEROCONF_LOG_ENTRY);
        //}

        #endregion 

        //private KeyboardHook hook = new KeyboardHook();
        //private GrowlInterface growl = new GrowlInterface();
        //private NetService netservice = null;

        #region Register Global Hot Keys
        //try
        //{
        //    hook.form = this;
        //    hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D0);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D1);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D2);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D3);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D4);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D5);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D6);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D7);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D8);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D9);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.A);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.B);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.C);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.E);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.F);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.G);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.H);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.I);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.J);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.K);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.L);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.M);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.N);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.O);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.P);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Q);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.R);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.S);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.T);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.U);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.V);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.W);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.X);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Y);
        //    hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Z);
        //    AddLogEntry(UrgencyLevel.INFO, "Registered global hotkeys.", KeyboardHook.LOG_INTERFACE);
        //}
        //catch (Exception ex)
        //{
        //    AddLogEntry(UrgencyLevel.ERROR, "Failed to register global hotkeys. - " + ex.Message, KeyboardHook.LOG_INTERFACE);
        //}

        #endregion

        #region Hot Key Handling

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            //string modifiers = e.Modifier.ToString().Replace(", ", "_");
            //string KeysPresseed = modifiers + "_" + e.Key.ToString();

            ////Learn Mode
            //if (formSceneProperties.isOpen)
            //    formSceneProperties.SetGlobalHotKey(KeysPresseed);
            ////Run Mode
            //else
            //{
            //    foreach (Scene thiscene in MasterScenes)
            //    {
            //        if (Enum.GetName(typeof(CustomHotKeys), thiscene.GlobalHotKey) == KeysPresseed)
            //        {
            //            SceneResult result = thiscene.Run(this);
            //            AddLogEntry((UrgencyLevel)result.ResultType, "Global HotKey Interface:  (" + KeysPresseed + ") " + result.Description);
            //        }
            //    }
            //}

        }

        public enum CustomHotKeys
        {
            None = 0,
            Alt_Control_Win_A = 1,
            Alt_Control_Win_B = 2,
            Alt_Control_Win_C = 3,
            Alt_Control_Win_D = 4,
            Alt_Control_Win_E = 5,
            Alt_Control_Win_F = 6,
            Alt_Control_Win_G = 7,
            Alt_Control_Win_H = 8,
            Alt_Control_Win_I = 9,
            Alt_Control_Win_J = 10,
            Alt_Control_Win_K = 11,
            Alt_Control_Win_L = 12,
            Alt_Control_Win_M = 13,
            Alt_Control_Win_N = 14,
            Alt_Control_Win_O = 15,
            Alt_Control_Win_P = 16,
            Alt_Control_Win_Q = 17,
            Alt_Control_Win_R = 18,
            Alt_Control_Win_S = 19,
            Alt_Control_Win_T = 20,
            Alt_Control_Win_U = 21,
            Alt_Control_Win_V = 22,
            Alt_Control_Win_W = 23,
            Alt_Control_Win_X = 24,
            Alt_Control_Win_Y = 25,
            Alt_Control_Win_Z = 26,
            Alt_Control_Win_D1 = 27,
            Alt_Control_Win_D2 = 28,
            Alt_Control_Win_D3 = 29,
            Alt_Control_Win_D4 = 30,
            Alt_Control_Win_D5 = 31,
            Alt_Control_Win_D6 = 32,
            Alt_Control_Win_D7 = 33,
            Alt_Control_Win_D8 = 34,
            Alt_Control_Win_D9 = 35,
            Alt_Control_Win_D0 = 36

        }

        #endregion

        #endregion

    }
}