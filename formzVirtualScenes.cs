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

namespace zVirtualScenesApplication
{
    public partial class formzVirtualScenes : Form
    {
        //Setup Log
        private string APP_PATH;
        public string ProgramName = "zVirtualScenes - v" + Application.ProductVersion;
        public readonly ZWaveController ControlThinkController = new ZWaveController();
        GlobalFunctions GlbFnct = new GlobalFunctions();

        private formDeviceProperties formDeviceProperty;
        private formSceneProperties formSceneProperties;

        //Simlpe Delegates for other threads and processes to interact with
        public delegate void LogThisDelegate(int type, string message);
        public delegate void ControlThinkRefreshDevicesDelegate();
        public delegate void RunSimpleActionDelegate(Action action);
        public delegate void changeEventDelegate(string GlbUniqueID, string TypeOfChange);

        //DATA OBJECTS
        private BindingList<String> MasterLog = new BindingList<string>();
        public BindingList<Device> MasterDevices = new BindingList<Device>();
        public BindingList<Scene> MasterScenes = new BindingList<Scene>();
        public BindingList<CustomDeviceProperties> CustomDeviceProperties = new BindingList<CustomDeviceProperties>();
        public Settings zVScenesSettings = new Settings();


        private JabberInterface jabber;

        public formzVirtualScenes()
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
            LogThis(1, ProgramName + " START");

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

            //Start Listening for device changes
            ControlThinkRefresh refresher = new ControlThinkRefresh(this);
            new Thread(new ThreadStart(refresher.RefreshThread)).Start();
            refresher.DeviceInfoChange += new ControlThinkRefresh.DeviceInfoChangeEventHandler(refresher_DeviceInfoChange);        
    
            //JABBER
            if (zVScenesSettings.JabberEnanbled)
            {
                jabber = new JabberInterface(this);
                jabber.Connect();
            }
        }
       
        void refresher_DeviceInfoChange(string GlbUniqueID, string TypeOfChange)
        {
            changeEvent(GlbUniqueID, TypeOfChange);
        }

        private void changeEvent(string GlbUniqueID, string TypeOfChange)
        {
            if (this.InvokeRequired)
                this.Invoke(new changeEventDelegate(changeEvent), new object[] { GlbUniqueID, TypeOfChange });
            else
            {
                foreach (Device device in MasterDevices)
                {
                    if (GlbUniqueID == device.GlbUniqueID())
                    {
                        string notification = "Event Notification Error";
                        string notificationprefix = DateTime.Now.ToString("T") + ": ";
                        
                        if (device.Type == "MultilevelPowerSwitch" && TypeOfChange == "level")
                        {
                            notification = device.Name + " level changed from " + device.prevLevel + " to " + device.Level + ".";                            

                            if(device.SendJabberNotifications)
                                jabber.SendMessage(notificationprefix + notification);                        
                        }
                        else if (device.Type == "GeneralThermostatV2" || device.Type == "GeneralThermostat")
                        {
                            if (TypeOfChange == "Temp")
                            {
                                notification = device.Name + " temperature changed from " + device.prevTemp + " degrees to " + device.Temp + " degrees.";
                                string urgetnotification = "URGENT! " + device.Name + " temperature is above/below alert temp. Temperature is " + device.Temp + " degrees."; 

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 0)
                                {
                                    if (device.Temp <= device.MaxAlertTemp || device.Temp >= device.MinAlertTemp)
                                    {
                                        jabber.SendMessage(notificationprefix + urgetnotification);
                                        LogThis(1, urgetnotification);
                                    }
                                }

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 1)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "CoolPoint")
                            {
                                notification = device.Name + " cool point changed from " + device.prevCoolPoint + " to " + device.CoolPoint + ".";
                                
                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "HeatPoint")
                            {
                                notification = device.Name + " heat point changed from " + device.prevHeatPoint + " to " + device.HeatPoint + ".";

                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "FanMode")
                            {
                                notification = device.Name + " fan mode changed from " + Enum.GetName(typeof(Device.ThermostatFanMode), device.prevFanMode) + " to " + Enum.GetName(typeof(Device.ThermostatFanMode), device.FanMode) + ".";
                                
                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "HeatCoolMode")
                            {
                                notification = device.Name + " mode changed from " + Enum.GetName(typeof(Device.ThermostatMode), device.prevHeatCoolMode) + " to " + Enum.GetName(typeof(Device.ThermostatMode), device.HeatCoolMode) + ".";
                                
                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "Level")
                            {
                                notification = device.Name + " energy state changed from " + Enum.GetName(typeof(Device.EnergyMode), device.prevLevel) + " to " + Enum.GetName(typeof(Device.EnergyMode), device.Level) + ".";
                                
                                if (device.SendJabberNotifications && device.NotificationDetailLevel > 2)
                                    jabber.SendMessage(notificationprefix + notification);
                            }
                            if (TypeOfChange == "CurrentState")
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

        private void zVirtualScenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsToFile();
            if (ControlThinkController.IsConnected)
                ControlThinkController.Disconnect();

            SaveSettingsToFile();

            if(jabber != null)
                jabber.Disconnect();         


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
                ControlThinkController.Connect();
            }
            catch (Exception e)
            {
                LogThis(2,"ControlThink USB Cennection Error: " + e);
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
        }

        public void ControlThinkRefreshDevices()
        {
            if (this.InvokeRequired)
                this.Invoke(new ControlThinkRefreshDevicesDelegate(ControlThinkRefreshDevices));
            else
            {
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
                                foreach (Device thisDevice in MasterDevices)
                                {
                                    if (ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                                    {
                                        if (device is ControlThink.ZWave.Devices.Specific.MultilevelPowerSwitch)
                                            if(device.Level != thisDevice.Level)
                                                thisDevice.Level = device.Level;
                                        else if (device is ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 || device is ControlThink.ZWave.Devices.Specific.GeneralThermostat)
                                        {
                                            ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

                                            if((int)thermostat.ThermostatTemperature.ToFahrenheit() != thisDevice.Temp)
                                                thisDevice.Temp = (int)thermostat.ThermostatTemperature.ToFahrenheit();

                                            if (thisDevice.CoolPoint != (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit())
                                                thisDevice.CoolPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();

                                            if (thisDevice.HeatPoint != (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit())
                                            {
                                                thisDevice.HeatPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                            }
                                                
                                            if(thisDevice.FanMode != (int)thermostat.ThermostatFanMode)
                                                    thisDevice.FanMode = (int)thermostat.ThermostatFanMode;

                                            if(thisDevice.HeatCoolMode != (int)thermostat.ThermostatMode)
                                                thisDevice.HeatCoolMode = (int)thermostat.ThermostatMode;

                                            if (device.Level != thermostat.Level)
                                                    thisDevice.Level = thermostat.Level;
                                                
                                            if( thisDevice.CurrentState != thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString())
                                                thisDevice.CurrentState = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogThis(2, "ControlThink USB Trouble Getting Device Status: " + e);
                        }
                    }
                    LogThis(1, "ControlThink USB Refreshed all Devices.");
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
                {
                    action.RunAction(this);
                    LogThis(1, "Ran " + action.Name);
                }

            }
        }

        private void btn_SetDeviceName_Click(object sender, EventArgs e)
        {
           
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

                }
                else if (selecteddevice.Type.Contains("GeneralThermostat")) 
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

        private void listBoxScenes_DoubleClick(object sender, EventArgs e)
        {
             if (listBoxScenes.SelectedIndex != -1)
            {
                if (formSceneProperties == null || formSceneProperties.IsDisposed)
                {
                    formSceneProperties = new formSceneProperties(this, listBoxScenes.SelectedIndex);
                    formSceneProperties.Show();
                }
                formSceneProperties.Activate();
            }
            
        }
        
        private void propertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                if (formSceneProperties == null || formSceneProperties.IsDisposed)
                {
                    formSceneProperties = new formSceneProperties(this, listBoxScenes.SelectedIndex);
                    formSceneProperties.Show();
                }
                formSceneProperties.Activate();
            }
        }

        private void listBoxDevices_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1)
            {
                if (formDeviceProperty == null || formDeviceProperty.IsDisposed)
                {
                    formDeviceProperty = new formDeviceProperties(this, listBoxDevices.SelectedIndex);
                    formDeviceProperty.Show();

                }
                formDeviceProperty.Activate();
            }
        }       

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            if (listBoxDevices.SelectedIndex != -1)
            {
                if (formDeviceProperty == null || formDeviceProperty.IsDisposed)
                {
                    formDeviceProperty = new formDeviceProperties(this, listBoxDevices.SelectedIndex);
                    formDeviceProperty.Show();

                }
                formDeviceProperty.Activate();
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

        private void btn_createnonzwaction_Click(object sender, EventArgs e)
        {
            if (listBoxScenes.SelectedIndex != -1)
            {
                if (comboBoxNonZWAction.SelectedIndex == 0)
                {
                    //Add EXE Action
                    OpenFileDialog fdlg = new OpenFileDialog();
                    fdlg.Title = "Please select the file to run with this action.";
                    fdlg.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    fdlg.FilterIndex = 2;

                    if (fdlg.ShowDialog() == DialogResult.OK)
                    {
                        Action EXEAction = new Action();
                        EXEAction.EXEPath = fdlg.FileName;
                        EXEAction.Type = "LauchAPP";

                        //GET ID OF SCENE SELECTED
                        Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
                        //ADD ACTION TO SELECTED SCENE
                        foreach (Scene scene in MasterScenes)
                        {
                            if (selectedscene.ID == scene.ID)
                            {

                                scene.Actions.Add(EXEAction);
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Please select an action type from the drop down. ", ProgramName);
            }
            else
                MessageBox.Show("Please select one device and one scene.", ProgramName);

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
                MasterLog.Insert(0, datetime + " " + typename + " - " + message + "\n");            
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
            else if (action.Type.Contains("GeneralThermostat"))
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

    #endregion

       

        






    }
}
