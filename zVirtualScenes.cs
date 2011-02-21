using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using ControlThink.ZWave;
using ControlThink.ZWave.Devices;

namespace zVirtualScenesApplication
{
    public partial class zVirtualScenes : Form
    {
        //Setup Log
        private string APP_PATH;
        public string ProgramName = "zVirtualScenes - v" + Application.ProductVersion;
        public readonly ZWaveController ControlThinkController = new ZWaveController();
        GlobalFunctions GlbFnct = new GlobalFunctions();

        //Simlpe Delegates for other threads and processes to interact with
        public delegate void LogThisDelegate(int type, string message);
        public delegate void ControlThinkGetDevicesDelegate();
        public delegate void RunSimpleActionDelegate(Action action);

        //DATA OBJECTS
        private BindingList<String> MasterLog = new BindingList<string>();
        public BindingList<Device> MasterDevices = new BindingList<Device>();
        public BindingList<Scene> MasterScenes = new BindingList<Scene>();
        public BindingList<CustomDeviceProperties> CustomDeviceProperties = new BindingList<CustomDeviceProperties>();
        public Settings zVScenesSettings = new Settings();

        public zVirtualScenes()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.zVirtualScenes_FormClosing);
        }

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {
            this.Text = ProgramName;
            APP_PATH = Path.GetDirectoryName(Application.ExecutablePath) + "\\";

            //Setup Log
            listBoxLog.DataSource = MasterLog;
            LogThis(1, ProgramName + " - v" + Application.ProductVersion + " START");

            //Load XML Saved Settings
            LoadSettings();
            UpdateSettingsGUI();
            comboBoxHeatCoolMode.DataSource = Enum.GetNames(typeof(Device.ThermostatMode));
            comboBoxFanMode.DataSource = Enum.GetNames(typeof(Device.ThermostatFanMode));
            comboBoxEnergyMode.DataSource = Enum.GetNames(typeof(Device.EnergyMode));

            //Query Zcommander for Devices
            ControlThinkConnect();
            ControlThinkGetDevices();

            //Bind GUI to data
            listBoxDevices.DataSource = MasterDevices;
            listBoxScenes.DataSource = MasterScenes;

            comboBoxHeatCoolMode.SelectedIndex = 0;

            //Start HTTP
            StartHTPP();

            //LightSwitch Clients
            if (zVScenesSettings.LightSwitchEnabled)
            {
                LightSwitchInterface LightSwitchInt = new LightSwitchInterface(this);
                LightSwitchInt.OpenLightSwitchSocket();
            }
        }

        private void zVirtualScenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsToFile();
            if (ControlThinkController.IsConnected)
                ControlThinkController.Disconnect();

            SaveSettingsToFile();         

            Environment.Exit(1);
        }      

    #region ControlThink ZWave Controller Code

        private void ControlThinkConnect()
        {
            if (ControlThinkController.IsConnected)
                return;
            try
            {
                ControlThinkController.SynchronizingObject = this;
                ControlThinkController.Connected += new System.EventHandler(ControlThinkUSBConnectedEvent);
                ControlThinkController.Disconnected += new System.EventHandler(ControlThinkUSBDisconnectEvent);
                ControlThinkController.ControllerNotResponding += new System.EventHandler(ControlThinkUSBNotRespondingEvent);
                ControlThinkController.LevelChanged += new ZWaveController.LevelChangedEventHandler(ControlThinkUSBLevelChangedEvent);
                ControlThinkController.Connect();
            }
            catch (Exception e)
            {
                LogThis(2,"ControlThink USB Cennection Error: " + e);
            }
        }

        public void ControlThinkGetDevices()
        {
            if (this.InvokeRequired)
                this.Invoke(new ControlThinkGetDevicesDelegate(ControlThinkGetDevices));
            else
            {
                int saveSelected = listBoxDevices.SelectedIndex; 
                MasterDevices.Clear();

                if (ControlThinkController.IsConnected)
                {
                    foreach (ZWaveDevice device in ControlThinkController.Devices)
                    {
                        try
                        {
                            //Allowed Z-wave Devices
                            if (device is ControlThink.ZWave.Devices.Specific.MultilevelPowerSwitch ||
                               device is ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 ||
                               device is ControlThink.ZWave.Devices.Specific.GeneralThermostat)
                            {
                                //Convert Device to Action
                                Device newDevice = new Device(this);
                                newDevice.HomeID = ControlThinkController.HomeID;
                                newDevice.NodeID = device.NodeID;
                                newDevice.Level = device.Level;

                                if (device is ControlThink.ZWave.Devices.Specific.MultilevelPowerSwitch)
                                {
                                    newDevice.Type = "MultilevelPowerSwitch";
                                    newDevice.Name = "MultilevelPowerSwitch";
                                }
                                if (device is ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 || device is ControlThink.ZWave.Devices.Specific.GeneralThermostat)
                                {
                                    newDevice.Type = "GeneralThermostatV2";
                                    newDevice.Name = "GeneralThermostatV2";

                                    ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;
                                    newDevice.Temp = (int)thermostat.ThermostatTemperature.ToFahrenheit();
                                    newDevice.CoolPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
                                    newDevice.HeatPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                    newDevice.FanMode = (int)thermostat.ThermostatFanMode;
                                    newDevice.HeatCoolMode = (int)thermostat.ThermostatMode;
                                    newDevice.Level = thermostat.Level;
                                    newDevice.CurrentState = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();
                                }

                                //Overwirte Name from the Custom Device Saved Data if present.
                                foreach (CustomDeviceProperties cDevice in CustomDeviceProperties)
                                {
                                    if (newDevice.GlbUniqueID() == cDevice.GlbUniqueID())
                                        newDevice.Name = cDevice.Name;
                                }

                                MasterDevices.Add(newDevice);
                            }
                        }
                        catch (Exception e)
                        {
                            LogThis(2, "ControlThink USB Trouble Loading Devices: " + e);
                        }
                    }
                    LogThis(1, "ControlThink USB Loaded " + MasterDevices.Count() + " Devices.");
                    
                    //reset GUI items
                    try { listBoxDevices.SelectedIndex = saveSelected; }
                    catch { }
                }
            }
        }

        private void ControlThinkUSBConnectedEvent(object sender, EventArgs e)
        {
            LogThis(1, "ControlThink USB Connected to HomeId - " + Convert.ToString(ControlThinkController.HomeID));
        }

        private void ControlThinkUSBDisconnectEvent(object sender, EventArgs e)
        {
            LogThis(1, "ControlThink USB Disconnected.");
        }

        private void ControlThinkUSBNotRespondingEvent(object sender, EventArgs e)
        {
            LogThis(2, "ControlThink USB Not Responding.");
            try
            {
                ControlThinkController.Disconnect();
            }
            catch
            {                
            }
        }

        private void ControlThinkUSBLevelChangedEvent(object sender, EventArgs e)
        {
            MessageBox.Show("Level Change");
            LogThis(1, "ControlThink USB Level Change: " + e);
        }

        #endregion 

    #region HTTP Handling

        public void StartHTPP()
        {
            if (zVScenesSettings.zHTTPListenEnabled)
            {
                try
                {
                    HttpServer httpServer = new HttpServer(zVScenesSettings.ZHTTPPort, this);
                    Thread thread = new Thread(new ThreadStart(httpServer.listen));
                    thread.Start();
                    LogThis(1, "Started HTTP Listening on all adapters.");
                }
                catch (Exception e)
                {
                    LogThis(2, "FAILED to Start HTTP Listening: " + e);
                }
            }
            else
                LogThis(1, "HTTP Listening DISABLED in settings.");
        }

        #endregion

    #region Settings TAB

        private void UpdateSettingsGUI()
        {
            //Http Listen
            txtb_httpPort.Text = Convert.ToString(zVScenesSettings.ZHTTPPort);            
            txtb_exampleURL.Text = "http://localhost:" + zVScenesSettings.ZHTTPPort + "/zVirtualScene?cmd=RunScene&Scene=1";

            if (zVScenesSettings.zHTTPListenEnabled)
                checkBoxHTTPEnable.Checked = true;
            else
                checkBoxHTTPEnable.Checked = false;


            //LightSwitch
            textBoxLSLimit.Text = Convert.ToString(zVScenesSettings.LightSwitchMaxConnections);
            textBoxLSPassword.Text = Convert.ToString(zVScenesSettings.LightSwitchPassword);
            textBoxLSport.Text = Convert.ToString(zVScenesSettings.LightSwitchPort); 

            if(zVScenesSettings.LightSwitchEnabled)
                checkBoxLSEnabled.Checked = true;
            else
                checkBoxLSEnabled.Checked = false;

            if (zVScenesSettings.LightSwitchVerbose)
                 checkBoxLSDebugVerbose.Checked = true;
            else
                 checkBoxLSDebugVerbose.Checked = false;
        }

        private void btn_SaveSettings_Click(object sender, EventArgs e)
        {
            bool saveOK = true;             

            //HTTP Listen
            try  {
                zVScenesSettings.ZHTTPPort = Convert.ToInt32(txtb_httpPort.Text);
            }
            catch {
                saveOK = false;
                MessageBox.Show("Invalid HTTP Port.", ProgramName);
            }

            if (checkBoxHTTPEnable.Checked)
                zVScenesSettings.zHTTPListenEnabled = true;
            else
                zVScenesSettings.zHTTPListenEnabled = false;

            //LightSwitch
            zVScenesSettings.LightSwitchPassword = textBoxLSPassword.Text;
            if (checkBoxLSEnabled.Checked)
                zVScenesSettings.LightSwitchEnabled = true;
            else
                zVScenesSettings.LightSwitchEnabled = false;
            
            if (checkBoxLSDebugVerbose.Checked)
                zVScenesSettings.LightSwitchVerbose = true;
            else
                zVScenesSettings.LightSwitchVerbose = false;

            try
            {
                zVScenesSettings.LightSwitchPort = Convert.ToInt32(textBoxLSport.Text);
            }
            catch
            {
                saveOK = false;
                MessageBox.Show("Invalid LightSwitch Port.", ProgramName);
            }
            
            try
            {
                zVScenesSettings.LightSwitchMaxConnections = Convert.ToInt32(textBoxLSLimit.Text);
            }
            catch
            {
                saveOK = false;
                MessageBox.Show("Invalid LightSwitch Max Connections.", ProgramName);
            }


            if(saveOK)
                MessageBox.Show("Settings Saved. Please restart the program to enable or disable HTTP or LightSwitch servies.", ProgramName);

            SaveSettingsToFile();

        }

        #endregion

    #region File I/O

        private void SaveSettingsToFile()
        {
            try
                {
                Stream stream = File.Open(APP_PATH + "zVirtualScenes-Scenes.xml", FileMode.Create);
                XmlSerializer SScenes = new XmlSerializer(MasterScenes.GetType());
                SScenes.Serialize(stream, MasterScenes);            
                stream.Close();

                Stream SettingsStream = File.Open(APP_PATH + "zVirtualScenes-Settings.xml", FileMode.Create);
                XmlSerializer SSettings = new XmlSerializer(zVScenesSettings.GetType());
                SSettings.Serialize(SettingsStream, zVScenesSettings);
                SettingsStream.Close();

                Stream CustomDevicePropertiesStream = File.Open(APP_PATH + "zVirtualScenes-CustomDeviceProperties.xml", FileMode.Create);
                XmlSerializer SCustomDeviceProperties = new XmlSerializer(CustomDeviceProperties.GetType());
                SCustomDeviceProperties.Serialize(CustomDevicePropertiesStream, CustomDeviceProperties);
                CustomDevicePropertiesStream.Close();

                LogThis(1, "Saved Settings XML.");
                
                }
            catch (Exception e)
            {
                LogThis(2, "Error saving XML: (" + e + ")");
            }

            //SAVE LOG
            try
            {
                StreamWriter SW = new System.IO.StreamWriter(APP_PATH + "zVirtualScenes.log",  false, Encoding.ASCII);                
                foreach(string item in MasterLog)
                    SW.WriteLine(item);
                SW.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving LOG: (" + e + ")");
            }


        }

        private void LoadSettings()
        {
            if (File.Exists(APP_PATH + "zVirtualScenes-Scenes.xml"))
            {
                try
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(BindingList<Scene>));
                    FileStream myFileStream = new FileStream(APP_PATH + "zVirtualScenes-Scenes.xml", FileMode.Open);
                    MasterScenes = (BindingList<Scene>)ScenesSerializer.Deserialize(myFileStream);                
                    myFileStream.Close();
                }
                catch (Exception e)
                {
                    LogThis(2, "Error loading Scene XML: (" + e + ")");
                }
            }
            else
            {
                //Create 10 default scenes
                for (int id = 1; id < 11; id++)
                {
                    Scene scene = new Scene();
                    scene.ID = id;
                    scene.Name = "Scene " + id;
                    MasterScenes.Add(scene);
                }
            }

            if (File.Exists(APP_PATH + "zVirtualScenes-Settings.xml"))
            {
                try
                {
                    XmlSerializer SettingsSerializer = new XmlSerializer(typeof(Settings));
                    FileStream SettingsileStream = new FileStream(APP_PATH + "zVirtualScenes-Settings.xml", FileMode.Open);
                    zVScenesSettings = (Settings)SettingsSerializer.Deserialize(SettingsileStream);
                    SettingsileStream.Close();
                    UpdateSettingsGUI();
                }
                catch (Exception e)
                {
                    LogThis(2, "Error loading Settings XML: (" + e + ")");
                }
            }

            if (File.Exists(APP_PATH + "zVirtualScenes-CustomDeviceProperties.xml"))
            {
                try
                {
                    XmlSerializer CustomDevicePropertiesSerializer = new XmlSerializer(typeof(BindingList<CustomDeviceProperties>));
                    FileStream CustomDevicePropertiesileStream = new FileStream(APP_PATH + "zVirtualScenes-CustomDeviceProperties.xml", FileMode.Open);
                    CustomDeviceProperties = (BindingList<CustomDeviceProperties>)CustomDevicePropertiesSerializer.Deserialize(CustomDevicePropertiesileStream);
                    CustomDevicePropertiesileStream.Close();
                }
                catch (Exception e)
                {
                    LogThis(2, "Error loading Settings XML: (" + e + ")");
                }
            }

            LogThis(1, "Loaded Program Settings.");
        }

        #endregion

    #region GUI Events

        private void btn_RefreshDevices_Click(object sender, EventArgs e)
        {
            ControlThinkGetDevices();
        }

        private void btn_AddtoScene_Click(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1 && listBoxScenes.SelectedIndex != -1)
            {
                Device selecteddevice = (Device)listBoxDevices.SelectedItem;
                Action action = CreateActionFromUserInput(selecteddevice);

                if (action != null)
                {
                    //GET ID OF SCENE SELECTED
                    Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                    //ADD ACTION TO SELECTED SCENE
                    foreach (Scene scene in MasterScenes)
                    {
                        if (selectedscene.ID == scene.ID)
                        {
                            scene.Actions.Add(action);
                        }
                    }
                }
            }
            else
                MessageBox.Show("Please select one device and one scene.", ProgramName);
        }

        private void btn_runScene_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                runScene(selectedscene);
            }
            else
                MessageBox.Show("Please select a scene.", ProgramName);
        }

        private void SaveScene_btn_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                if (scenename_txtbx.Text != "")
                {
                    //GET ID OF SCENE SELECTED
                    Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                    int sceneID = selectedscene.ID;

                    //CHANGE NAME OF SELECTED SCENE
                    foreach (Scene scene in MasterScenes)
                    {
                        if (sceneID == scene.ID)
                            scene.Name = scenename_txtbx.Text;
                    }

                    lbl_sceneActions.Text = "Scene " + selectedscene.ID.ToString() + " Actions (" + selectedscene.Name + ")";

                }
                else
                    MessageBox.Show("Please enter a scene name.", ProgramName);
            }
            else
                MessageBox.Show("Please select a scene.", ProgramName);

        }

        private void btn_DelAction_Click(object sender, EventArgs e)
        {
            if (listBoxSceneActions.SelectedIndex != -1)
            {
                //GET ID OF SCENE SELECTED
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                int ActionID = listBoxSceneActions.SelectedIndex;

                //Locate Scene
                foreach (Scene scene in MasterScenes)
                {
                    if (selectedscene.ID == scene.ID)
                    {
                        scene.Actions.RemoveAt(ActionID);
                    }
                }

                if (listBoxSceneActions.Items.Count > 0)
                    listBoxSceneActions.SelectedIndex = 0;

            }
            else
                MessageBox.Show("Please select a Action.", ProgramName);
        }

        private void btn_MoveUp_Click(object sender, EventArgs e)
        {
            if (listBoxSceneActions.SelectedIndex > 0)
            {
                //GET ID OF SCENE SELECTED
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                int ActionID = listBoxSceneActions.SelectedIndex;

                //Locate Scene
                foreach (Scene scene in MasterScenes)
                {
                    if (selectedscene.ID == scene.ID)
                    {
                        Action temp = (Action)listBoxSceneActions.SelectedItem;
                        scene.Actions.RemoveAt(ActionID);
                        scene.Actions.Insert(ActionID - 1, temp);
                    }
                }
                listBoxSceneActions.SelectedIndex = ActionID - 1;

            }
        }

        private void btn_MoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxSceneActions.SelectedIndex < listBoxSceneActions.Items.Count - 1)
            {
                //GET ID OF SCENE SELECTED
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                int ActionID = listBoxSceneActions.SelectedIndex;

                //Locate Scene
                foreach (Scene scene in MasterScenes)
                {
                    if (selectedscene.ID == scene.ID)
                    {
                        Action temp = (Action)listBoxSceneActions.SelectedItem;
                        scene.Actions.RemoveAt(ActionID);
                        scene.Actions.Insert(ActionID + 1, temp);
                    }
                }
                listBoxSceneActions.SelectedIndex = ActionID + 1;
            }
        }


       

        private void btn_RunCommand_Click(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1)
            {
                Action action = CreateActionFromUserInput((Device)listBoxDevices.SelectedItem);

                if (action != null)
                    action.RunAction(this);

            }
        }

        private void btn_SetDeviceName_Click(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1)
            {
                if (txtb_deviceName.Text != "")
                {
                    Device selecteddevice = (Device)listBoxDevices.SelectedItem;

                    //Set Device List Name
                    selecteddevice.Name = txtb_deviceName.Text;

                    //Save into custom List for furture use
                    bool found = false;
                    foreach (CustomDeviceProperties cDevice in CustomDeviceProperties)
                    {
                        if (selecteddevice.HomeID == cDevice.HomeID && selecteddevice.NodeID == cDevice.NodeID)
                        {
                            cDevice.Name = txtb_deviceName.Text;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        CustomDeviceProperties newcDevice = new CustomDeviceProperties();
                        newcDevice.Name = txtb_deviceName.Text;
                        newcDevice.HomeID = selecteddevice.HomeID;
                        newcDevice.NodeID = selecteddevice.NodeID;
                        CustomDeviceProperties.Add(newcDevice);
                    }

                    //Replace name in each Action
                    foreach (Scene scene in MasterScenes)
                    {
                        foreach (Action action in scene.Actions)
                        {
                            if (action.GlbUniqueID() == selecteddevice.GlbUniqueID())
                                action.Name = selecteddevice.Name;
                        }
                    }
                }
                else
                    MessageBox.Show("Invalid device name.", ProgramName);
            }
            else
                MessageBox.Show("You must select a device.", ProgramName);
        }

        private void buttonAddScene_Click(object sender, EventArgs e)
        {
            //Get the last used largest unique ID
            int max = 0;
            foreach (Scene _scenes in MasterScenes)
                if (_scenes.ID > max)
                    max = _scenes.ID;

            //Use the next avialable ID
            max++;

            Scene scene = new Scene();
            scene.ID = max;
            scene.Name = "Scene " + max;
            MasterScenes.Add(scene);
        }

        private void buttonDelScene_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
                MasterScenes.RemoveAt(listBoxScenes.SelectedIndex);
        }

        private void btn_SceneMoveUp_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex > 0)
            {
                //GET ID OF SCENE SELECTED
                int ActionID = listBoxScenes.SelectedIndex;
                Scene temp = (Scene)listBoxScenes.SelectedItem;
                MasterScenes.RemoveAt(ActionID);
                MasterScenes.Insert(ActionID - 1, temp);
                listBoxScenes.SelectedIndex = ActionID - 1;

            }
        }

        private void btn_sceneMoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex < listBoxScenes.Items.Count - 1)
            {
                //GET ID OF SCENE SELECTED
                int ActionID = listBoxScenes.SelectedIndex;
                Scene temp = (Scene)listBoxScenes.SelectedItem;
                MasterScenes.RemoveAt(ActionID);
                MasterScenes.Insert(ActionID + 1, temp);
                listBoxScenes.SelectedIndex = ActionID + 1;

            }
        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1)
            {
                Device selecteddevice = (Device)listBoxDevices.SelectedItem;
                groupBoxCommands.Text = "Create Action for " + selecteddevice.Name + " (" + selecteddevice.Type + ")";

                if (selecteddevice.Type == "MultilevelPowerSwitch")
                {
                    txtbox_level.Enabled = true;
                    comboBoxHeatCoolMode.Enabled = false;
                    comboBoxEnergyMode.Enabled = false;
                    comboBoxFanMode.Enabled = false;

                    checkBoxeditCP.Checked = false;
                    checkBoxeditHP.Checked = false;
                    checkBoxeditCP.Enabled = false;
                    checkBoxeditHP.Enabled = false;

                    txtbox_level.Text = selecteddevice.Level.ToString();
                }
                else if (selecteddevice.Type == "GeneralThermostatV2" || selecteddevice.Type == "GeneralThermostat")
                {
                    txtbox_level.Enabled = false;
                    comboBoxHeatCoolMode.Enabled = true;
                    comboBoxEnergyMode.Enabled = true;
                    comboBoxFanMode.Enabled = true;
                    checkBoxeditCP.Enabled = true;
                    checkBoxeditCP.Checked = false;
                    checkBoxeditHP.Enabled = true;
                    checkBoxeditHP.Checked = false;


                    comboBoxHeatCoolMode.SelectedIndex = comboBoxHeatCoolMode.Items.Count - 1;
                    comboBoxEnergyMode.SelectedIndex = comboBoxEnergyMode.Items.Count - 1;
                    comboBoxFanMode.SelectedIndex = comboBoxFanMode.Items.Count - 1;

                    txtbx_HeatPoint.Text = selecteddevice.HeatPoint.ToString();
                    textBoxCoolPoint.Text = selecteddevice.CoolPoint.ToString();
                    labelCurrentTemp.Text = "Current Temperature: " + selecteddevice.Temp.ToString() + "°";                    
                    
                }
            }
        }

        private void listBoxScenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                listBoxSceneActions.DataSource = selectedscene.Actions;
                lbl_sceneActions.Text = "Scene " + selectedscene.ID.ToString() + " (" + selectedscene.Name + ") Actions";
            }
        }

    #endregion

    #region Invokeable Functions
        /// <summary>
        /// Logs an event that can be called from any thread.  Self Invokes.
        /// </summary>
        /// <param name="type">1 = INFO, 2 = ERROR, DEFAULT INFO</param>
        /// <param name="message">Log Event Message</param>
        public void LogThis(int type, string message)
        {
            string datetime = DateTime.Now.ToString("s");
            string typename;
            if (type == 2)
                typename = "[ERROR]";
            else
                typename = "[INFO ]";

            if (this.InvokeRequired)
                this.Invoke(new LogThisDelegate(LogThis), new object[] { type, message });
            else
                MasterLog.Add(datetime + " " + typename + " - " + message + "\n");            
        }

        public void RunSimpleAction(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(new RunSimpleActionDelegate(RunSimpleAction), new object[] { action });
            else
            {
                action.RunAction(this);
            }
        }
        #endregion

    #region General Use Functions

        private Action CreateActionFromUserInput(Device _device)
        {
            //Cast device to action            
            Action action = (Action)_device;

            if (action.Type == "MultilevelPowerSwitch")
            {
                //ERROR CHECK INPUTS
                try
                {
                    byte level = Convert.ToByte(txtbox_level.Text);
                    if (level < 100 || level == 255)
                        action.Level = level;
                    else
                        throw new ArgumentException("Invalid Level.");
                }
                catch   
                {
                    MessageBox.Show("Invalid Level.", ProgramName);
                    return null;
                }

            }
            else if (action.Type == "" || action.Type == "GeneralThermostat")
            {   
                action.HeatCoolMode = (int)Enum.Parse(typeof(Device.ThermostatMode),comboBoxHeatCoolMode.SelectedValue.ToString());
                action.FanMode = (int)Enum.Parse(typeof(Device.ThermostatFanMode), comboBoxFanMode.SelectedValue.ToString());
                action.EngeryMode = (int)Enum.Parse(typeof(Device.EnergyMode), comboBoxEnergyMode.SelectedValue.ToString());

                //Make sure atleast one thermo action was chosen
                if (action.HeatCoolMode == -1 && action.FanMode == -1 && action.EngeryMode == -1 && checkBoxeditHP.Checked == false && checkBoxeditCP.Checked == false )
                {
                    MessageBox.Show("Please select at least one Temperature Mode.", ProgramName);
                    return null;
                }

                if (checkBoxeditHP.Checked == true)
                {
                    try { action.HeatPoint = Convert.ToInt32(txtbx_HeatPoint.Text); }
                    catch
                    {
                        MessageBox.Show("Invalid Heat Point.", ProgramName);
                        return null;
                    }
                }

                if (checkBoxeditCP.Checked == true)
                {
                    try { action.CoolPoint = Convert.ToInt32(textBoxCoolPoint.Text); }
                    catch
                    {
                        MessageBox.Show("Invalid Cool Point..", ProgramName);
                        return null;
                    }
                }
            }
            return action; 
        }

        private void runScene(Scene _scene)
        {
            if (_scene.Actions.Count > 0)
            {
                foreach (Action action in _scene.Actions)
                {
                    action.RunAction(this);
                }

                LogThis(1, "Ran Scene #" + _scene.ID.ToString() + " (" + _scene.Name + ") with " + _scene.Actions.Count() + " actions.");
            }
            else
                LogThis(1, "Attepmted to run scene with no action.");
        }

        #endregion

        private void checkBoxeditCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxeditCP.Checked == true)            
                textBoxCoolPoint.Enabled = true;
            else
                textBoxCoolPoint.Enabled = false;            
        }

        private void checkBoxeditHP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxeditHP.Checked == true)
                txtbx_HeatPoint.Enabled = true;
            else
                txtbx_HeatPoint.Enabled = false;
        }

    }
}
