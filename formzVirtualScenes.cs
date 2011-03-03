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
using System.Timers;
using jabber.client;
using System.Runtime.InteropServices;


namespace zVirtualScenesApplication
{
    public partial class formzVirtualScenes : Form
    {
        //Setup Log
        private string APP_PATH;
        public string ProgramName = "zVirtualScenes - v" + Application.ProductVersion;

        //Forms and Controllers
        public readonly ZWaveController ControlThinkController = new ZWaveController();
        private formPropertiesScene formSceneProperties = new formPropertiesScene();
                
        private LightSwitchInterface LightSwitchInt;
        private JabberInterface jabber;
        private KeyboardHook hook = new KeyboardHook();

        //Delegates
        public delegate void LogThisDelegate(int type, string message);
        public delegate void SetlabelSceneRunStatusDelegate(string text);
        public delegate void ControlThinkConnectDelegate();
        public delegate void DeviceInfoChange_HandlerDelegate(string GlbUniqueID, zVirtualScenesApplication.ControlThinkRefresh.changeType TypeOfChange);
        public delegate void onRemoteButtonPressDelegate(string msg, string param1, string param);

        //CORE OBJECTS
        private BindingList<String> MasterLog = new BindingList<string>();
        public BindingList<Device> MasterDevices = new BindingList<Device>();
        public BindingList<Scene> MasterScenes = new BindingList<Scene>();
        public BindingList<CustomDeviceProperties> CustomDeviceProperties = new BindingList<CustomDeviceProperties>();
        public Settings zVScenesSettings = new Settings();        

        public formzVirtualScenes()
        {
            InitializeComponent();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.zVirtualScenes_FormClosing);
        }        

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {
            this.Text = ProgramName;
            APP_PATH = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            labelSceneRunStatus.Text = "";
            comboBoxNonZWAction.SelectedIndex = 0;

            //Setup Log
            listBoxLog.DataSource = MasterLog;
            LogThis(1, ProgramName + " STARTED");

            //Load XML Saved Settings
            LoadSettings();
            UpdateSettingsGUI();            

            //Query Zcommander for Devices
            ControlThinkConnect();
            ControlThinkGetDevices();

            //Bind data to GUI elements
            listBoxDevices.DataSource = MasterDevices;
            listBoxScenes.DataSource = MasterScenes;

            //Register event handlers for each scene
            RegisterSceneHandlers();

            //Start HTTP INTERFACE
            StartHTPP();

            //LightSwitch Clients
            if (zVScenesSettings.LightSwitchEnabled)
            {
                LightSwitchInt = new LightSwitchInterface(this);
                LightSwitchInt.OpenLightSwitchSocket();
            }

            //Start Listening for device changes
            ControlThinkRefresh refresher = new ControlThinkRefresh(this);
            new Thread(new ThreadStart(refresher.RefreshThread)).Start();
            refresher.DeviceInfoChange += new ControlThinkRefresh.DeviceInfoChangeEventHandler(DeviceInfoChange_Handler);             
    
            //JABBER
            if (zVScenesSettings.JabberEnanbled)
            {
                jabber = new JabberInterface(this);
                jabber.Connect();
            }

            #region Register Global Hot Keys    
            try
            {
                hook.form = this;
                hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D0);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D1);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D2);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D3);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D4);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D5);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D6);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D7);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D8);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D9);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.A);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.B);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.C);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.D);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.E);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.F);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.G);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.H);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.I);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.J);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.K);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.L);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.M);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.N);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.O);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.P);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Q);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.R);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.S);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.T);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.U);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.V);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.W);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.X);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Y);
                hook.RegisterHotKey((ModifierKeys)1 | (ModifierKeys)2 | (ModifierKeys)8, Keys.Z);
                LogThis(1, "Global HotKey Interface:  Registered global hotkeys.");
            }
            catch (Exception ex)
            {
                LogThis(2, "Global HotKey Interface:  Failed to register global hotkeys. - " + ex.Message);
            }

            #endregion
    }

        private void RegisterSceneHandlers()
        {
            foreach (Scene thisScene in MasterScenes)
                thisScene.SceneExecutionFinishedEvent += new SceneExecutionFinished(SceneExecutionFinsihed_Handler);
        }

        private void zVirtualScenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsToFile();
            if (ControlThinkController.IsConnected)
                ControlThinkController.Disconnect();

            SaveSettingsToFile();

            if (jabber != null)
                jabber.Disconnect();


            Environment.Exit(1);
        }

        private void DeviceInfoChange_Handler(string GlbUniqueID, ControlThinkRefresh.changeType TypeOfChange)
        {
            if (this.InvokeRequired)
                this.Invoke(new DeviceInfoChange_HandlerDelegate(DeviceInfoChange_Handler), new object[] { GlbUniqueID, TypeOfChange });
            else
            {
                foreach (Device device in MasterDevices)
                {
                    if (GlbUniqueID == device.GlbUniqueID())
                    {
                        string notification = "Event Notification Error";
                        string notificationprefix = DateTime.Now.ToString("T") + ": ";

                        if (device.Type == "BinaryPowerSwitch" && TypeOfChange == ControlThinkRefresh.changeType.LevelChanged)
                        {
                            notification = device.Name + " state changed from " + (device.prevLevel > 0 ? "ON" : "OFF") + " to " + (device.Level > 0 ? "ON" : "OFF") + ".";

                            if (device.SendJabberNotifications)
                                jabber.SendMessage(notificationprefix + notification);
                        }
                        if (device.Type == "MultilevelPowerSwitch" && TypeOfChange == ControlThinkRefresh.changeType.LevelChanged)
                        {
                            notification = device.Name + " level changed from " + device.prevLevel + " to " + device.Level + ".";

                            if (device.SendJabberNotifications)
                                jabber.SendMessage(notificationprefix + notification);
                        }
                        else if (device.Type.Contains("GeneralThermostat"))
                        {
                            if (TypeOfChange == ControlThinkRefresh.changeType.TempChanged)
                            {
                                notification = device.Name + " temperature changed from " + device.prevTemp + " degrees to " + device.Temp + " degrees.";
                                string urgetnotification = "URGENT! " + device.Name + " temperature is above/below alert temp. Temperature is " + device.Temp + " degrees.";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 0)
                                {
                                    if (device.Temp >= device.MaxAlertTemp || device.Temp <= device.MinAlertTemp)
                                    {
                                        jabber.SendMessage(notificationprefix + urgetnotification);
                                        LogThis(1, urgetnotification);
                                    }
                                }

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 1)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.CoolPointChanged)
                            {
                                notification = device.Name + " cool point changed from " + device.prevCoolPoint + " to " + device.CoolPoint + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.HeatPointChanged)
                            {
                                notification = device.Name + " heat point changed from " + device.prevHeatPoint + " to " + device.HeatPoint + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.FanModeChanged)
                            {
                                notification = device.Name + " fan mode changed from " + Enum.GetName(typeof(Device.ThermostatFanMode), device.prevFanMode) + " to " + Enum.GetName(typeof(Device.ThermostatFanMode), device.FanMode) + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.HeatCoolModeChanged)
                            {
                                notification = device.Name + " mode changed from " + Enum.GetName(typeof(Device.ThermostatMode), device.prevHeatCoolMode) + " to " + Enum.GetName(typeof(Device.ThermostatMode), device.HeatCoolMode) + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.LevelChanged)
                            {
                                notification = device.Name + " energy state changed from " + Enum.GetName(typeof(Device.EnergyMode), device.prevLevel) + " to " + Enum.GetName(typeof(Device.EnergyMode), device.Level) + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == ControlThinkRefresh.changeType.CurrentStateChanged)
                            {
                                notification = device.Name + " changed state from " + device.prevCurrentState + " to " + device.CurrentState + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 3)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                        }
                        LogThis(1, notification);
                        labelLastEvent.Text = notification;
                    }
                }
                listBoxDevices.DataSource = null;
                listBoxDevices.DataSource = MasterDevices;
            }
        }

    #region Hot Key Handling

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            string modifiers = e.Modifier.ToString().Replace(", ", "_");
            string KeysPresseed = modifiers + "_" + e.Key.ToString();

            //Learn Mode
            if (formSceneProperties.isOpen)
                formSceneProperties.SetGlobalHotKey(KeysPresseed);
            //Run Mode
            else
            {
                foreach (Scene thiscene in MasterScenes)
                {
                    if (Enum.GetName(typeof(CustomHotKeys), thiscene.GlobalHotKey) == KeysPresseed)
                    {
                        SceneResult result = thiscene.Run(ControlThinkController);
                        LogThis((int)result.ResultType, "Global HotKey Interface:  (" + KeysPresseed + ") " + result.Description);
                    }
                }
            }

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

    #region ControlThink ZWave Controller Code

        public void ControlThinkConnect()
        {
            if (this.InvokeRequired)
                this.Invoke(new ControlThinkConnectDelegate(ControlThinkConnect));
            else
            {
                if (ControlThinkController.IsConnected)
                    return;
                try
                {
                    ControlThinkController.SynchronizingObject = this;
                    ControlThinkController.Connected += new System.EventHandler(ControlThinkUSBConnectedEvent);
                    ControlThinkController.Disconnected += new System.EventHandler(ControlThinkUSBDisconnectEvent);
                    ControlThinkController.ControllerNotResponding += new System.EventHandler(ControlThinkUSBNotRespondingEvent);
                    ControlThinkController.Connect();
                }
                catch (Exception e)
                {
                    LogThis(2, "ControlThink USB Cennection Error: " + e);
                }
            }
        }

        public void ControlThinkGetDevices()
        {
            int saveSelected = listBoxDevices.SelectedIndex; 
            MasterDevices.Clear();

            if (ControlThinkController.IsConnected)
            {
                foreach (ZWaveDevice device in ControlThinkController.Devices)
                {
                    try
                    {
                        LogThis(1, "Found " + device.ToString() + ".");
                        
                        if (!device.ToString().Contains("Controller")) //Do not include ZWave controllers for now...
                        {
                            //Convert Device to Action
                            Device newDevice = new Device(this);
                            newDevice.HomeID = ControlThinkController.HomeID;
                            newDevice.NodeID = device.NodeID;
                            newDevice.Level = device.Level;
                            newDevice.Type = device.ToString().Replace("ControlThink.ZWave.Devices.Specific.", "");
                            newDevice.Name = newDevice.Type;
                           
                            if (newDevice.Type.Contains("GeneralThermostat"))
                            {
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
                                {
                                    newDevice.Name = cDevice.Name;
                                    newDevice.NotificationDetailLevel = cDevice.NotificationDetailLevel;
                                    newDevice.SendJabberNotifications = cDevice.SendJabberNotifications;
                                    newDevice.MaxAlertTemp = cDevice.MaxAlertTemp;
                                    newDevice.MinAlertTemp = cDevice.MinAlertTemp;
                                }
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
            else
                ControlThinkConnect();
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

     #endregion 

    #region HTTP INTERFACE

        public void StartHTPP()
        {
            if (zVScenesSettings.zHTTPListenEnabled)
            {
                try
                {
                    HttpServer httpServer = new HttpServer(zVScenesSettings.ZHTTPPort, this);
                    Thread thread = new Thread(new ThreadStart(httpServer.listen));
                    thread.Start();
                    LogThis(1, "HTTP Interface: Started listening for HTTP commands on all adapters.");
                }
                catch (Exception e)
                {
                    LogThis(2, "HTTP Interface: FAILED to Start HTTP Listening: " + e);
                }
            }
            else
                LogThis(1, "HTTP Interface: HTTP Listening DISABLED in settings.");
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

            //JAbber
            textBoxJabberPassword.Text = zVScenesSettings.JabberPassword;
            textBoxJabberUser.Text= zVScenesSettings.JabberUser;
            textBoxJabberServer.Text = zVScenesSettings.JabberServer;
            textBoxJabberUserTo.Text = zVScenesSettings.JabberSendToUser;
            if (zVScenesSettings.JabberEnanbled == true )
                checkBoxJabberEnabled.Checked = true;
            else
                checkBoxJabberEnabled.Checked = false;

            if (zVScenesSettings.JabberVerbose == true)
                checkBoxJabberVerbose.Checked = true;
            else
                checkBoxJabberVerbose.Checked = false;
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
                    SW.Write(item);
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

        public void SelectListBoxActionItem(int ID)
        {
            listBoxSceneActions.SelectedIndex = ID;
        }

        private void lookForNewDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlThinkGetDevices();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            bool saveOK = true;

            //HTTP Listen
            try
            {
                zVScenesSettings.ZHTTPPort = Convert.ToInt32(txtb_httpPort.Text);
            }
            catch
            {
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

            //JABBER
            zVScenesSettings.JabberPassword = textBoxJabberPassword.Text;
            zVScenesSettings.JabberUser = textBoxJabberUser.Text;
            zVScenesSettings.JabberServer = textBoxJabberServer.Text;
            zVScenesSettings.JabberSendToUser = textBoxJabberUserTo.Text;
            if (checkBoxJabberEnabled.Checked)
                zVScenesSettings.JabberEnanbled = true;
            else
                zVScenesSettings.JabberEnanbled = false;

            if (checkBoxJabberVerbose.Checked)
                zVScenesSettings.JabberVerbose = true;
            else
                zVScenesSettings.JabberVerbose = false;

            if (saveOK)
                MessageBox.Show("Settings Saved. Please restart the program to enable or disable HTTP or LightSwitch servies.", ProgramName);

            SaveSettingsToFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToFile();
        }
        
        private void SceneExecutionFinsihed_Handler(object sender, SceneResult _SceneResult)
        {
            LogThis((int)_SceneResult.ResultType, _SceneResult.Description);

            //Send to LightSwitchClients
            if (zVScenesSettings.LightSwitchEnabled)
            {
                LightSwitchInt.BroadcastMessage("MSG~" + _SceneResult.Description);              
            }
            
            //Invoke because called from another thread. 
            SetlabelSceneRunStatus(_SceneResult.Description);
        }
        
        private void btn_runScene_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                SceneResult result = selectedscene.Run(ControlThinkController);
                LogThis((int)result.ResultType, "GUI: [USER] " + result.Description);
                labelSceneRunStatus.Text = result.ResultType + " " + result.Description;
            }
            else
                MessageBox.Show("Please select a scene.", ProgramName);
        }
        
        private void btn_DelAction_Click(object sender, EventArgs e)
        {
            int savedIndex = listBoxSceneActions.SelectedIndex;

            if (listBoxSceneActions.SelectedIndex != -1)
            {
                MasterScenes[listBoxScenes.SelectedIndex].Actions.RemoveAt(listBoxSceneActions.SelectedIndex);
            }
            else
                MessageBox.Show("Please select an action.", ProgramName);
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
            //Register Handler
            scene.SceneExecutionFinishedEvent += new SceneExecutionFinished(SceneExecutionFinsihed_Handler);
            MasterScenes.Add(scene);            
        }

        private void buttonDelScene_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1 && listBoxScenes.Items.Count > 1)
            {
                //Unregister Handler
                MasterScenes[listBoxScenes.SelectedIndex].SceneExecutionFinishedEvent -= new SceneExecutionFinished(SceneExecutionFinsihed_Handler);
                MasterScenes.RemoveAt(listBoxScenes.SelectedIndex);
            }
            else
                MessageBox.Show("Please select a scene.", ProgramName);
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

        private void OpenScenePropertiesWindow()
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                formSceneProperties = new formPropertiesScene();
                formSceneProperties._zVirtualScenesMain = this;
                formSceneProperties._SelectedSceneIndex = listBoxScenes.SelectedIndex;
                formSceneProperties.ShowDialog();
            }    
        }        

        private void listBoxScenes_DoubleClick(object sender, EventArgs e)
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

        private void OpenDevicePropertyWindow()
        {
            if (listBoxDevices.SelectedIndex != -1)
            {
                Device selecteddevice = (Device)listBoxDevices.SelectedItem;

                if (selecteddevice.Type == "BinaryPowerSwitch")
                {
                    formPropertiesBinSwitch formPropertiesBinSwitch = new formPropertiesBinSwitch(this, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex);
                        formPropertiesBinSwitch.ShowDialog();                    
                }
                else if (selecteddevice.Type == "MultilevelPowerSwitch")
                {
                    formPropertiesMultiLevelSwitch formPropertiesMultiLevelSwitch = new formPropertiesMultiLevelSwitch(this, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex);
                        formPropertiesMultiLevelSwitch.ShowDialog();
                }
                else if (selecteddevice.Type.Contains("GeneralThermostat"))
                {
                    formPropertiesThermostat formPropertiesThermostat = new formPropertiesThermostat(this, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex);
                        formPropertiesThermostat.ShowDialog();
                }
            }
            else
                MessageBox.Show("You must select a ZWave device. ", ProgramName);
        }

        private void listBoxDevices_DoubleClick(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }

        private void btn_EditDevice_Click_1(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }

        /// <summary>
        /// EDIT ANY TYPE OF ACTION
        /// </summary>
        private void editAction()
        {
            if (listBoxSceneActions.SelectedIndex != -1)
            {
                Action selectedaction = (Action)listBoxSceneActions.SelectedItem;

                if (selectedaction.Type == "LauchAPP")
                {
                        formAddEditEXEC formAddEditEXEC = new formAddEditEXEC(this, true, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                        formAddEditEXEC.ShowDialog();
                }
                else if (selectedaction.Type == "DelayTimer")
                {
                    formAddEditTimeDelay formAddEditTimeDelay = new formAddEditTimeDelay(this, true, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                        formAddEditTimeDelay.ShowDialog();
                }
                else if (selectedaction.Type == "BinaryPowerSwitch")
                {
                    formAddEditActionBinSwitch formAddEditActionBinSwitch = new formAddEditActionBinSwitch(this, true, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                        formAddEditActionBinSwitch.ShowDialog();
                }
                else if (selectedaction.Type == "MultilevelPowerSwitch")
                {
                    formAddEditActionMultiLevelSwitch formAddEditActionMultiLevelSwitch = new formAddEditActionMultiLevelSwitch(this, true, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                        formAddEditActionMultiLevelSwitch.ShowDialog();
                }
                else if (selectedaction.Type.Contains("GeneralThermostat"))
                {
                    formAddEditActionThermostat formAddEditActionThermostat = new formAddEditActionThermostat(this, true, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                        formAddEditActionThermostat.ShowDialog();
                }
            }
            else
                MessageBox.Show("Please select an action.", ProgramName);
        }

        private void listBoxSceneActions_DoubleClick(object sender, EventArgs e)
        {
            editAction();
        }

        private void buttonEditAction_Click(object sender, EventArgs e)
        {
            editAction();
        }

        /// <summary>
        /// ADD ZWAVE DEIVCE ACTIONS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewActionFromZWaveDevice()
        {
            if (listBoxDevices.SelectedIndex != -1 && listBoxScenes.SelectedIndex != -1)
            {
                Device selecteddevice = (Device)listBoxDevices.SelectedItem;

                if (selecteddevice.Type == "BinaryPowerSwitch")
                {
                    formAddEditActionBinSwitch formAddEditActionBinSwitch = new formAddEditActionBinSwitch(this, false, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                    formAddEditActionBinSwitch.ShowDialog();
                }
                else if (selecteddevice.Type == "MultilevelPowerSwitch")
                {
                    formAddEditActionMultiLevelSwitch formAddEditActionMultiLevelSwitch = new formAddEditActionMultiLevelSwitch(this, false, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                    formAddEditActionMultiLevelSwitch.ShowDialog();
                }
                else if (selecteddevice.Type.Contains("GeneralThermostat"))
                {
                    formAddEditActionThermostat formAddEditActionThermostat = new formAddEditActionThermostat(this, false, listBoxDevices.SelectedIndex, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                    formAddEditActionThermostat.ShowDialog();
                }
            }
            else
                MessageBox.Show("Please select a ZWave device. ", ProgramName);
        }

        private void buttonChangeDeviceLevel_Click(object sender, EventArgs e)
        {
            CreateNewActionFromZWaveDevice();
        }

        private void btn_AddAction_Click(object sender, EventArgs e)
        {
            CreateNewActionFromZWaveDevice();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDevicePropertyWindow();
        }
        
        private void listBoxScenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                listBoxSceneActions.DataSource = selectedscene.Actions;
                lbl_sceneActions.Text = "Scene " + selectedscene.ID.ToString() + " '" + selectedscene.Name + "' Actions";
            }
        }        

        private void btn_createnonzwaction_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                if (comboBoxNonZWAction.SelectedIndex == 0)  //Create Timer
                {
                    formAddEditTimeDelay formAddEditTimeDelay = new formAddEditTimeDelay(this, false, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                   formAddEditTimeDelay.ShowDialog();
                }
                else if (comboBoxNonZWAction.SelectedIndex == 1) //Add EXE Action
                {
                    formAddEditEXEC formAddEditEXEC = new formAddEditEXEC(this, false, listBoxScenes.SelectedIndex, listBoxSceneActions.SelectedIndex);
                    formAddEditEXEC.ShowDialog();                 
                }
                else
                    MessageBox.Show("Please select an action type from the drop down. ", ProgramName);
            }
            else
                MessageBox.Show("Please select one device and one scene.", ProgramName);

        }

        private void reconnectToControlThinkUSBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlThinkConnect();
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
            {
                MasterLog.Add(datetime + " " + typename + " - " + message + "\n");
                listBoxLog.SelectedIndex = listBoxLog.Items.Count-1; 
            }
        }

        private void SetlabelSceneRunStatus(string text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetlabelSceneRunStatusDelegate(SetlabelSceneRunStatus), new object[] { text });
            else
                labelSceneRunStatus.Text = text;
        }
       
        #endregion

        

        

    }
}
