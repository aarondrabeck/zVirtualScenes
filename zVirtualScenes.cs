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

namespace zVirtualScenesApplication
{
    public delegate void HTTPRequestEvent(string request);

    public partial class zVirtualScenes : Form
    {
        public HTTPRequestEvent DelegateAddLog;
        private string APP_PATH;        
        const string ProgramName = "zVirtualScenes";
        GlobalFunctions GlbFnct = new GlobalFunctions();

        //DATA
        List<Device> MasterDevices = new List<Device>();
        List<Scene> MasterScenes = new List<Scene>();
        Settings zVScenesSettings = new Settings();

        public zVirtualScenes()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.zVirtualScenes_FormClosing);
            DelegateAddLog = new HTTPRequestEvent(this.HandleHTTP);
        }

        public void HandleHTTP(string request)
        {
            GlbFnct.AddLog(1, "HTTPSrv: " + request);
            string acceptedCMD = "/zVirtualScene?cmd=RunScene&Scene=";
            
            if (request.Contains(acceptedCMD))
            {
                int scene = 0;
                try
                {
                    scene = Convert.ToInt32(request.Remove(0, acceptedCMD.Length));
                }
                catch
                {
                    GlbFnct.AddLog(2, "HTTPSrv: Failed to read scene number from HTTP Request.");
                }

                if (scene > 0)
                {
                    foreach (Scene thisscene in MasterScenes)
                        if (thisscene.ID == scene)
                            runScene(thisscene);
                }
            }
        }

        public void StartHTPP()
        {
            if (zVScenesSettings.zHTTPListenEnabled)
            {

                try
                {
                    HttpServer httpServer = new HttpServer(zVScenesSettings.ZHTTPPort, this);
                    Thread thread = new Thread(new ThreadStart(httpServer.listen));
                    thread.Start();
                    GlbFnct.AddLog(1, "Started HTTP Listening on all adapters.");
                }
                catch (Exception e)
                {
                    GlbFnct.AddLog(2, "FAILED to Start HTTP Listening: " + e);
                }
            }
            else
                GlbFnct.AddLog(1, "HTTP Listening DISABLED in settings.");
        }

        private void zVirtualScenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsToFile();
            Environment.Exit(1);
        }

        private void zVirtualScenes_Load(object sender, EventArgs e)
        {
            this.Text = ProgramName + " - v" + Application.ProductVersion;
            APP_PATH = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
         
            //Setup Log
            GlbFnct.log = this.LogRichTxtBx;
            GlbFnct.AddLog(1,ProgramName + " - v" + Application.ProductVersion + " START");

            //Get Settings from XML
            LoadSettings();

            //Query Zcommander for Devices
            FetchDeviceList();            

            //LOAD SCENES INTO GUI
            RefreshScenes();
            comboBoxTermoMode.SelectedIndex = 0;

            //Start HTTP
            StartHTPP();
        }

        private void btn_RefreshDevices_Click(object sender, EventArgs e)
        {
            FetchDeviceList();
        }

        public void FetchDeviceList()
        {
            //Clear Device List
            MasterDevices.Clear();

            string DeviceList = GlbFnct.HTTPSend(GlbFnct.GetZComURL(zVScenesSettings) + "command=device");
    
            if (DeviceList.Contains("DEVICE"))
            {
                using (StringReader reader = new StringReader(DeviceList))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("DEVICE"))
                        {
                            //DEVICE~Fireplace~2~0~MultilevelPowerSwitch
                            string[] parts = line.Split('~');

                            Device device = new Device();
                            device.Name = parts[1];
                            device.ID = Convert.ToInt32(parts[2]);
                            device.Level = Convert.ToInt32(parts[3]);
                            device.Type = parts[4];
                            MasterDevices.Add(device);
                        }
                    }
                }
            }
            GlbFnct.AddLog(1, "Found " + MasterDevices.Count() + " Devices.");

            listBoxDevices.Items.Clear();

            foreach (Device device in MasterDevices)
                listBoxDevices.Items.Add(device);
        }

        private void RefreshScenes()
        {
            listBoxScenes.Items.Clear();
            foreach (Scene scene in MasterScenes)
            {
                listBoxScenes.Items.Add(scene);
            }
        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Device selecteddevice = (Device)listBoxDevices.SelectedItem;
            lbl_type.Text = "Type: " + selecteddevice.Type;

            if (selecteddevice.Type == "MultilevelPowerSwitch")
            {
                txtbox_level.Enabled = true;
                txtbx_temp.Enabled = false;
                comboBoxTermoMode.Enabled = false;

                txtbox_level.Text = selecteddevice.Level.ToString();
            }
            else if (selecteddevice.Type == "GeneralThermostatV2")
            {
                txtbox_level.Enabled = false;
                txtbx_temp.Enabled = true;
                comboBoxTermoMode.Enabled = true;

                txtbx_temp.Text = selecteddevice.Level.ToString();

            }
        }

        private void btn_AddtoScene_Click(object sender, EventArgs e)
        {
            if (listBoxDevices.SelectedIndex != -1 && listBoxScenes.SelectedIndex != -1)
            {
                Action action = new Action();
                Device selecteddevice = (Device)listBoxDevices.SelectedItem;

                action.Type = selecteddevice.Type;
                action.Name = selecteddevice.Name;
                action.ID = selecteddevice.ID;

                if (action.Type == "MultilevelPowerSwitch")
                {
                   //ERROR CHECK INPUTS
                    try
                    {
                        int level = Convert.ToInt32(txtbox_level.Text);
                        if (level > -1 && level < 100 || level == 255)
                            action.Level = level;
                        else
                            throw new ArgumentException("Invalid Level.");
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Level.", ProgramName);
                        return;
                    }

                }
                else if (action.Type == "GeneralThermostatV2")
                {
                    //ERROR CHECK INPUTS
                    try
                    {
                        int temp = Convert.ToInt32(txtbx_temp.Text);
                        if (temp > -1 && temp < 121)
                            action.Temp = temp;
                        else
                            throw new ArgumentException("Invalid Temperature.");
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Temperature.", ProgramName);
                        return;
                    }

                    //SLECTED MODE 
                    action.Mode = comboBoxTermoMode.SelectedIndex;
                }


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
                
                //REFRESH ACTION IN GUI
                reloadActions();

            }
            else
                MessageBox.Show("Please select one device and one scene.", ProgramName);
        }

        private void listBoxScenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadActions();
            Scene selectedscene = (Scene)listBoxScenes.SelectedItem;
            lbl_sceneActions.Text = "Scene " + selectedscene.ID.ToString() + " (" + selectedscene.name + ") Actions";            
        }

        private void reloadActions()
        {
            listBoxSceneActions.Items.Clear();
            Scene selectedscene = (Scene)listBoxScenes.SelectedItem;

            foreach (Action action in selectedscene.Actions)
                listBoxSceneActions.Items.Add(action);    
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

        private void runScene(Scene _scene)
        {
            if (_scene.Actions.Count > 0)
            {
                foreach (Action action in _scene.Actions)
                {
                    string cmd = action.ExecuteThisAction(zVScenesSettings);
                    GlbFnct.AddLog(1, "Sent:  " + cmd);
                }

                GlbFnct.AddLog(1, "Ran Scene #" + _scene.ID.ToString() + " (" + _scene.name + ") with " + _scene.Actions.Count() + " actions.");

                FetchDeviceList();
            }
            else
                MessageBox.Show("No actions in scene.", ProgramName);                
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
                            scene.name = scenename_txtbx.Text;
                    }

                    RefreshScenes();
                    lbl_sceneActions.Text = "Scene " + selectedscene.ID.ToString() + " Actions (" + selectedscene.name + ")";
                    
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
                reloadActions();
                if(listBoxSceneActions.Items.Count > 0)
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
                reloadActions();
                listBoxSceneActions.SelectedIndex = ActionID - 1;
                
            }
        }

        private void btn_MoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxSceneActions.SelectedIndex < listBoxSceneActions.Items.Count -1)
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
                reloadActions();
                listBoxSceneActions.SelectedIndex = ActionID + 1;
              
            }
        }

        #region Settings TAB

        private void UpdateSettingsGUI()
        {
            //ZCommander
            txtb_ServerIP.Text = zVScenesSettings.ZcomIP;
            txtB_ServerPort.Text = Convert.ToString(zVScenesSettings.ZcomPort);

            //Http Listen
            txtb_httpPort.Text = Convert.ToString(zVScenesSettings.ZHTTPPort);
            if (zVScenesSettings.zHTTPListenEnabled)
                checkBoxHTTPEnable.Checked = true;
            else
                checkBoxHTTPEnable.Checked = false;

            txtb_exampleURL.Text = "http://localhost:" + zVScenesSettings.ZHTTPPort + "/zVirtualScene?cmd=RunScene&Scene=1";
        }

        private void btn_SaveSettings_Click(object sender, EventArgs e)
        {
            bool saveOK = true; 

            //Zcommander
            zVScenesSettings.ZcomIP = txtb_ServerIP.Text;
            try
            {
                zVScenesSettings.ZcomPort = Convert.ToInt32(txtB_ServerPort.Text);
            }
            catch
            {
                saveOK = false;
                MessageBox.Show("Invalid ZCommander Port.", ProgramName);
            }

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


            if(saveOK)
                MessageBox.Show("Settings Saved. Please restart the program for HTTP settings to take effect.", ProgramName);

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
                GlbFnct.AddLog(1, "Saved Scenes to XML.");

                //Serialize Settings
                Stream SettingsStream = File.Open(APP_PATH + "zVirtualScenes-Settings.xml", FileMode.Create);
                XmlSerializer SSettings = new XmlSerializer(zVScenesSettings.GetType());
                SSettings.Serialize(SettingsStream, zVScenesSettings);
                SettingsStream.Close();
                GlbFnct.AddLog(1, "Saved Settings to XML.");
                }
            catch (Exception e)
            {
                GlbFnct.AddLog(2, "Error saving XML: (" + e + ")");
            }

            //SAVE LOG
            try
            {
                StreamWriter SW = new System.IO.StreamWriter(APP_PATH + "zVirtualScenes.log",  false, Encoding.ASCII);
                SW.Write(LogRichTxtBx.Text);
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
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<Scene>));
                    FileStream myFileStream = new FileStream(APP_PATH + "zVirtualScenes-Scenes.xml", FileMode.Open);
                    MasterScenes = (List<Scene>)ScenesSerializer.Deserialize(myFileStream);                
                    myFileStream.Close();
                    GlbFnct.AddLog(1, "Loaded Scenes XML.");
                    RefreshScenes();
                }
                catch (Exception e)
                {
                    GlbFnct.AddLog(2, "Error loading Scene XML: (" + e + ")");
                }
            }
            else
            {
                //Create 100 default scenes
                for (int id = 1; id < 101; id++)
                {
                    Scene scene = new Scene();
                    scene.ID = id;
                    scene.name = "Scene " + id;
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
                    GlbFnct.AddLog(1, "Loaded Settings XML.");
                    UpdateSettingsGUI();
                }
                catch (Exception e)
                {
                    GlbFnct.AddLog(2, "Error loading Settings XML: (" + e + ")");
                }
            }
        }

        #endregion

    }
}
