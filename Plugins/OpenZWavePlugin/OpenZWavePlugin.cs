using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using OpenZWaveDotNet;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using OpenZWavePlugin.Forms;
using System.Drawing;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;
using zVirtualScenes;

using System.Diagnostics;
using zvs.Entities;

namespace OpenZWavePlugin
{
    [Export(typeof(zvsPlugin))]
    public class OpenZWavePlugin : zvsPlugin
    {
        private ZWManager m_manager = null;
        private ZWOptions m_options = null;
        ZWNotification m_notification = null;
        UInt32 m_homeId = 0;
        List<Node> m_nodeList = new List<Node>();
        private bool FinishedInitialPoll = false;
        private string LastEventNameValueId = "LEN1";
        private int verbosity = 1;
        private bool isShuttingDown = false;
        private bool _useHID = false;
        private string _comPort = "3";
        private int _pollint = 0;

        public static string LogPath
        {
            get
            {
                string path = Path.Combine(Utils.AppDataPath, @"openzwave\");
                if (!Directory.Exists(path))
                {
                    try { Directory.CreateDirectory(path); }
                    catch { }
                }

                return path + "\\";
            }
        }

        public OpenZWavePlugin()
            : base("OPENZWAVE",
               "Open ZWave Plug-in",
                "This plug-in interfaces zVirtualScenes with OpenZWave using the OpenZWave open-source project."
                ) { }

        public override void Initialize()
        {
            using (zvsContext Context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "COMPORT",
                    Name = "Com Port",
                    Value = (3).ToString(),
                    ValueType = DataType.COMPORT,
                    Description = "The COM port that your z-wave controller is assigned to."
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "HID",
                    Name = "Use HID",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "Use HID rather than COM port. (use this for ControlThink Sticks)"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "POLLint",
                    Name = "Polling interval",
                    Value = (360).ToString(),
                    ValueType = DataType.INTEGER,
                    Description = "The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network traffic. "
                }, Context);

                //Controller Type Devices
                DeviceType controller_dt = new DeviceType { UniqueIdentifier = "CONTROLLER", Name = "OpenZWave Controller", ShowInList = true };
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RESET", Name = "Reset Controller", ArgumentType = DataType.NONE, Description = "Erases all Z-Wave network settings from your controller." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ADDDEVICE", Name = "Add Device to Network", ArgumentType = DataType.NONE, Description = "Adds a ZWave Device to your network." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "AddController", Name = "Add Controller to Network", ArgumentType = DataType.NONE, Description = "Adds a ZWave Controller to your network." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "CreateNewPrimary", Name = "Create New Primary", ArgumentType = DataType.NONE, Description = "Puts the target controller into receive configuration mode." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ReceiveConfiguration", Name = "Receive Configuration", ArgumentType = DataType.NONE, Description = "Receives the network configuration from another controller." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveController", Name = "Remove Controller", ArgumentType = DataType.NONE, Description = "Removes a Controller from your network." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveDevice", Name = "Remove Device", ArgumentType = DataType.NONE, Description = "Removes a Device from your network." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TransferPrimaryRole", Name = "Transfer Primary Role", ArgumentType = DataType.NONE, Description = "Transfers the primary role to another controller." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "HasNodeFailed", Name = "Has Node Failed", ArgumentType = DataType.NONE, Description = "Tests whether a node has failed." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveFailedNode", Name = "Remove Failed Node", ArgumentType = DataType.NONE, Description = "Removes the failed node from the controller's list." });
                controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ReplaceFailedNode", Name = "Replace Failed Node", ArgumentType = DataType.NONE, Description = "Tests the failed node." });
                DefineOrUpdateDeviceType(controller_dt, Context);

                //Switch Type Devices
                DeviceType switch_dt = new DeviceType { UniqueIdentifier = "SWITCH", Name = "OpenZWave Binary", ShowInList = true };
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a switch." });
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a switch." });
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "MOMENTARY", Name = "Turn On for X milliseconds", ArgumentType = DataType.INTEGER, Description = "Turns a device on for the specified number of milliseconds and then turns the device back off." });
                DefineOrUpdateDeviceType(switch_dt, Context);

                //Dimmer Type Devices
                DeviceType dimmer_dt = new DeviceType { UniqueIdentifier = "DIMMER", Name = "OpenZWave Dimmer", ShowInList = true };
                dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a dimmer." });
                dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a dimmer." });

                DeviceTypeCommand dimmer_preset_cmd = new DeviceTypeCommand { UniqueIdentifier = "SETPRESETLEVEL", Name = "Set Level", ArgumentType = DataType.LIST, Description = "Sets a dimmer to a preset level." };
                dimmer_preset_cmd.Options.Add(new CommandOption  { Name = "0%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "20%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "40%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "60%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "80%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "100%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "255" });
                dimmer_dt.Commands.Add(dimmer_preset_cmd);

                DefineOrUpdateDeviceType(dimmer_dt, Context);

                //Thermostat Type Devices
                DeviceType thermo_dt = new DeviceType { UniqueIdentifier = "THERMOSTAT", Name = "OpenZWave Thermostat", ShowInList = true };
                thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETENERGYMODE", Name = "Set Energy Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Energy Mode." });
                thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETCONFORTMODE", Name = "Set Comfort Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Comfort Mode. (Run)" });
                DefineOrUpdateDeviceType(thermo_dt, Context);

                //Door Lock Type Devices
                DeviceType lock_dt = new DeviceType { UniqueIdentifier = "DOORLOCK", Name = "OpenZWave Door lock", ShowInList = true };
                DefineOrUpdateDeviceType(lock_dt, Context);

                //Sensors
                DeviceType sensor_dt = new DeviceType { UniqueIdentifier = "SENSOR", Name = "OpenZWave Sensor", ShowInList = true };
                DefineOrUpdateDeviceType(sensor_dt, Context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "DEFAULONLEVEL",
                    Name= "Default Level",
                    Description = "Level that an device is set to when using the 'ON' command.",
                    Value = "99",//default value
                    ValueType = DataType.BYTE
                }, Context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "ENABLEREPOLLONLEVELCHANGE",
                    Name = "Enable re-poll on level change",
                    Description = "Re-poll dimmers 3 seconds after a level change is received?",
                    Value = true.ToString(), //default value
                    ValueType = DataType.BOOL
                }, Context);

                bool.TryParse(GetSettingValue("HID", Context), out _useHID);
                _comPort = GetSettingValue("COMPORT", Context);
                int.TryParse(GetSettingValue("POLLint", Context), out _pollint);

            }

            //TODO: Make a new DeviceAPIProperty that is API specific for types of settings that applies OpenZWave Devices           

            ////TEMP 
            //DefineDevice(new device { node_id = 1, device_type_id = GetDeviceType("DIMMER").id, Name = "Test Device 1", last_heard_from = DateTime.Now});
            //DefineDevice(new device { node_id = 2, device_type_id = GetDeviceType("DIMMER").id, Name = "Test Device 2", last_heard_from = DateTime.Now });

            //int i = 2;
            //System.Timers.Timer t = new System.Timers.Timer();
            //t.interval = 5000;
            //t.Elapsed += (sender, e) =>
            //{
            //    i++;
            //    //zvsEntityControl.zvsContext.devices.FirstOrDefault(d => d.node_id == 1).last_heard_from = DateTime.Now;
            //    //zvsEntityControl.zvsContext.SaveChanges();


            //    DefineOrUpdateDeviceValue(new device_values
            //    {
            //        device_id = zvsEntityControl.zvsContext.devices.FirstOrDefault(d => d.node_id == 1).id,
            //        value_id = "1!",
            //        label_UniqueIdentifier = "Basic",
            //        genre = "Genre",
            //        index = "Index",
            //        type = "Type",
            //        commandClassId = "Coomand Class",
            //        value = (i % 2 == 0 ? "99" : "50")
            //    });

            //    //DefineDevice(new device { node_id = i, device_type_id = GetDeviceType("DIMMER").id, Name = "Test Device " + i, last_heard_from = DateTime.Now });

            //};
            //t.Enabled = true;



        }

        protected override void StartPlugin()
        {
            StartOpenzwave();
        }

        protected override void StopPlugin()
        {
            StopOpenzwave();
        }

        private void StartOpenzwave()
        {
            if (isShuttingDown)
            {
                WriteToLog(Urgency.INFO, this.Name + " driver cannot start because it is still shutting down");
                return;
            }

            try
            {
                WriteToLog(Urgency.INFO, string.Format("OpenZwave driver starting on {0}", _useHID ? "HID" : "COM" + _comPort));

                // Environment.CurrentDirectory returns wrong directory in Service environment so we have to make a trick
                string directoryName = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                // Create the Options                
                m_options = new ZWOptions();
                m_options.Create(directoryName + @"\config\",
                                 LogPath,
                                 @"");
                m_options.Lock();
                m_manager = new ZWManager();
                m_manager.Create();
                m_manager.OnNotification += NotificationHandler;

                if (!_useHID)
                {
                    if (_comPort != "0")
                    {
                        m_manager.AddDriver(@"\\.\COM" + _comPort);
                    }
                }
                else
                {
                    m_manager.AddDriver("HID Controller", ZWControllerInterface.Hid);
                }


                if (_pollint != 0)
                {
                    m_manager.SetPollInterval(_pollint, true);
                }

            }
            catch (Exception e)
            {
                WriteToLog(Urgency.ERROR, e.Message);
            }
        }

        private void StopOpenzwave()
        {
            if (!isShuttingDown)
            {
                isShuttingDown = true;
                IsReady = false;

                //EKK this is blocking and can be slow
                if (m_manager != null)
                {
                    m_manager.OnNotification -= NotificationHandler;
                    m_manager.RemoveDriver(@"\\.\COM" + _comPort);
                    m_manager.Destroy();
                    m_manager = null;
                }

                if (m_options != null)
                {
                    m_options.Destroy();
                    m_options = null;
                }

                isShuttingDown = false;
                WriteToLog(Urgency.INFO, "OpenZwave driver stopped");
            }
        }

        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue)
        {
            switch (settingUniqueIdentifier)
            {
                case "COMPORT":
                    {
                        if (Enabled)
                            StopOpenzwave();

                        _comPort = settingValue;

                        if (Enabled)
                            StartOpenzwave();

                        break;
                    }
                case "HID":
                    {
                        if (Enabled)
                            StopOpenzwave();

                        bool.TryParse(settingValue, out _useHID);

                        if (Enabled)
                            StartOpenzwave();

                        break;
                    }
                case "POLLint":
                    {
                        if (Enabled)
                            StopOpenzwave();

                        int.TryParse(settingValue, out _pollint);

                        if (Enabled)
                            StartOpenzwave();

                        break;
                    }
            }
        }

        public override void ProcessDeviceTypeCommand(QueuedDeviceTypeCommand cmd)
        {
            if (cmd.Device.Type.UniqueIdentifier == "CONTROLLER")
            {
                switch (cmd.Command.UniqueIdentifier)
                {
                    case "RESET":
                        {
                            m_manager.ResetController(m_homeId);
                            break;
                        }
                    case "ADDDEVICE":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddDevice, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "AddController":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddController, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "CreateNewPrimary":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.CreateNewPrimary, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReceiveConfiguration":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReceiveConfiguration, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "RemoveController":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveController, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "RemoveDevice":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveDevice, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "TransferPrimaryRole":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.TransferPrimaryRole, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "HasNodeFailed":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.HasNodeFailed, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "RemoveFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveFailedNode, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReplaceFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReplaceFailedNode, (byte)cmd.Device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                }
            }
            else if (cmd.Device.Type.UniqueIdentifier == "SWITCH")
            {
                switch (cmd.Command.UniqueIdentifier)
                {
                    case "MOMENTARY":
                        {
                            int delay = 1000;
                            int.TryParse(cmd.Argument, out delay);
                            byte nodeID = (byte)cmd.Device.NodeNumber;

                            m_manager.SetNodeOn(m_homeId, nodeID);
                            System.Timers.Timer t = new System.Timers.Timer();
                            t.Interval = delay;
                            t.Elapsed += (sender, e) =>
                            {
                                t.Stop();
                                m_manager.SetNodeOff(m_homeId, nodeID);
                                t.Dispose();
                            };
                            t.Start();
                            break;

                        }
                    case "TURNON":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)cmd.Device.NodeNumber);
                            break;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.Device.NodeNumber);
                            break;
                        }
                }
            }
            else if (cmd.Device.Type.UniqueIdentifier == "DIMMER")
            {
                switch (cmd.Command.UniqueIdentifier)
                {
                    case "TURNON":
                        {
                            using (zvsContext Context = new zvsContext())
                            {
                                byte defaultonlevel = 99;
                                byte.TryParse(DevicePropertyValue.GetDevicePropertyValue(Context, cmd.Device, "DEFAULONLEVEL"), out defaultonlevel);
                                m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, defaultonlevel);
                            }
                            break;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.Device.NodeNumber);
                            break;
                        }
                    case "SETPRESETLEVEL":
                        {
                            switch (cmd.Argument)
                            {
                                case "0%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(0));
                                    break;
                                case "20%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(20));
                                    break;
                                case "40%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(40));
                                    break;
                                case "60%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(60));
                                    break;
                                case "80%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(80));
                                    break;
                                case "100%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(100));
                                    break;
                                case "255":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.Device.NodeNumber, Convert.ToByte(255));
                                    break;
                            }
                            break;
                        }
                }
            }
            else if (cmd.Device.Type.UniqueIdentifier == "THERMOSTAT")
            {
                switch (cmd.Command.UniqueIdentifier)
                {
                    case "SETENERGYMODE":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.Device.NodeNumber);
                            break;
                        }
                    case "SETCONFORTMODE":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)cmd.Device.NodeNumber);
                            break;
                        }
                }
            }
        }

        public override void ProcessDeviceCommand(QueuedDeviceCommand cmd)
        {
            if (cmd.Command.UniqueIdentifier.Contains("DYNAMIC_CMD_"))
            {
                //Get more info from this Node from OpenZWave
                Node node = GetNode(m_homeId, (byte)cmd.Device.NodeNumber);

                switch (cmd.Command.ArgumentType)
                {
                    case DataType.BYTE:
                        {
                            byte b = 0;
                            byte.TryParse(cmd.Argument, out b);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.Command.CustomData1))
                                    m_manager.SetValue(v.ValueID, b);
                            break;
                        }
                    case DataType.BOOL:
                        {
                            bool b = true;
                            bool.TryParse(cmd.Argument, out b);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.Command.CustomData1))
                                    m_manager.SetValue(v.ValueID, b);
                            break;
                        }
                    case DataType.DECIMAL:
                        {
                            float f = Convert.ToSingle(cmd.Argument);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.Command.CustomData1))
                                    m_manager.SetValue(v.ValueID, f);
                            break;
                        }
                    case DataType.LIST:
                    case DataType.STRING:
                        {
                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.Command.CustomData1))
                                    m_manager.SetValue(v.ValueID, cmd.Argument);
                            break;
                        }
                    case DataType.INTEGER:
                        {
                            int i = 0;
                            int.TryParse(cmd.Argument, out i);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.Command.CustomData1))
                                    m_manager.SetValue(v.ValueID, i);
                            break;
                        }
                }
            }
        }

        public override void Repoll(Device device)
        {
            m_manager.RequestNodeState(m_homeId, Convert.ToByte(device.NodeNumber));
        }

        public override void ActivateGroup(int groupID)
        {
            using (zvsContext Context = new zvsContext())
            {
                IQueryable<Device> devices = GetDeviceInGroup(groupID, Context);
                if (devices != null)
                {
                    foreach (Device d in devices)
                    {
                        switch (d.Type.UniqueIdentifier)
                        {
                            case "SWITCH":
                                m_manager.SetNodeOn(m_homeId, Convert.ToByte(d.NodeNumber));
                                break;
                            case "DIMMER":
                                byte defaultonlevel = 99;
                                byte.TryParse(DevicePropertyValue.GetDevicePropertyValue(Context, d, "DEFAULONLEVEL"), out defaultonlevel);
                                m_manager.SetNodeLevel(m_homeId, Convert.ToByte(d.NodeNumber), defaultonlevel);
                                break;
                        }

                    }
                }
            }
        }

        public override void DeactivateGroup(int groupID)
        {
            using (zvsContext Context = new zvsContext())
            {
                IQueryable<Device> devices = GetDeviceInGroup(groupID, Context);
                if (devices != null)
                {
                    foreach (Device d in devices)
                    {
                        switch (d.Type.UniqueIdentifier)
                        {
                            case "SWITCH":
                                m_manager.SetNodeOff(m_homeId, Convert.ToByte(d.NodeNumber));
                                break;
                            case "DIMMER":

                                m_manager.SetNodeLevel(m_homeId, Convert.ToByte(d.NodeNumber), 0);
                                break;
                        }

                    }
                }
            }
        }

        #region OpenZWave interface

        public void NotificationHandler(ZWNotification notification)
        {
            m_notification = notification;
            NotificationHandler();
            m_notification = null;
        }

        private HybridDictionary timers = new HybridDictionary();
        private void NotificationHandler()
        {
            switch (m_notification.GetType())
            {
                case ZWNotification.Type.ValueAdded:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        ZWValueID vid = m_notification.GetValueID();
                        Value value = new Value();
                        value.ValueID = vid;
                        value.Label = m_manager.GetValueLabel(vid);
                        value.Genre = vid.GetGenre().ToString();
                        value.Index = vid.GetIndex().ToString();
                        value.Type = vid.GetType().ToString();
                        value.CommandClassID = vid.GetCommandClassId().ToString();
                        value.Help = m_manager.GetValueHelp(vid);
                        bool read_only = m_manager.IsValueReadOnly(vid);
                        node.AddValue(value);

                        string data = "";
                        bool b = m_manager.GetValueAsString(vid, out data);

                        using (zvsContext Context = new zvsContext())
                        {
                            Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == node.ID);
                            if (d != null)
                            {
                                if (verbosity > 4)
                                    WriteToLog(Urgency.INFO, "[ValueAdded] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data + ", result: " + b.ToString());

                                //Values are 'unknown' at this point so don't report a value change. 
                                DefineOrUpdateDeviceValue(new DeviceValue
                                {
                                    Device = d,
                                    UniqueIdentifier = vid.GetId().ToString(),
                                    Name = value.Label,
                                    Genre = value.Genre,
                                    Index = value.Index,
                                    CommandClass = value.CommandClassID,
                                    Value = data,
                                    ValueType = ConvertType(vid),
                                    isReadOnly = read_only
                                }, Context, true);

                                #region Install Dynamic Commands

                                if (!read_only)
                                {
                                    DataType pType = DataType.NONE;

                                    //Set parameter types for command
                                    switch (vid.GetType())
                                    {
                                        case ZWValueID.ValueType.List:
                                            pType = DataType.LIST;
                                            break;
                                        case ZWValueID.ValueType.Byte:
                                            pType = DataType.BYTE;
                                            break;
                                        case ZWValueID.ValueType.Decimal:
                                            pType = DataType.DECIMAL;
                                            break;
                                        case ZWValueID.ValueType.Int:
                                            pType = DataType.INTEGER;
                                            break;
                                        case ZWValueID.ValueType.String:
                                            pType = DataType.STRING;
                                            break;
                                        case ZWValueID.ValueType.Short:
                                            pType = DataType.SHORT;
                                            break;
                                        case ZWValueID.ValueType.Bool:
                                            pType = DataType.BOOL;
                                            break;
                                    }

                                    //Install the Node Specific Command
                                    int order;
                                    switch (value.Genre)
                                    {
                                        case "User":
                                            order = 1;
                                            break;
                                        case "Config":
                                            order = 2;
                                            break;
                                        default:
                                            order = 99;
                                            break;
                                    }


                                    DeviceCommand dynamic_dc = new DeviceCommand
                                    {
                                        Device = d,
                                        UniqueIdentifier = "DYNAMIC_CMD_" + value.Label.ToUpper(),
                                        Name = "Set " + value.Label,
                                        ArgumentType = pType,
                                        Help = value.Help,
                                        CustomData1 = value.Label,
                                        CustomData2 = vid.GetId().ToString(),
                                        SortOrder = order
                                    };

                                    //Special case for lists add additional info
                                    if (vid.GetType() == ZWValueID.ValueType.List)
                                    {
                                        //Install the allowed options/values
                                        String[] options;
                                        if (m_manager.GetValueListItems(vid, out options))
                                            foreach (string option in options)
                                                dynamic_dc.Options.Add(new CommandOption { Name = option });
                                    }

                                    DefineOrUpdateDeviceCommand(dynamic_dc, Context);
                                }
                                #endregion
                            }
                        }
                        break;
                    }

                case ZWNotification.Type.ValueRemoved:
                    {
                        try
                        {
                            Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                            ZWValueID vid = m_notification.GetValueID();
                            Value val = node.GetValue(vid);

                            if (verbosity > 4)
                                WriteToLog(Urgency.INFO, "[ValueRemoved] Node:" + node.ID + ",Label:" + m_manager.GetValueLabel(vid));

                            node.RemoveValue(val);
                            //TODO: Remove from values and command table
                        }
                        catch (Exception ex)
                        {
                            WriteToLog(Urgency.ERROR, "ValueRemoved error: " + ex.Message);
                        }
                        break;
                    }

                case ZWNotification.Type.ValueChanged:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        ZWValueID vid = m_notification.GetValueID();
                        Value value = new Value();
                        value.ValueID = vid;
                        value.Label = m_manager.GetValueLabel(vid);
                        value.Genre = vid.GetGenre().ToString();
                        value.Index = vid.GetIndex().ToString();
                        value.Type = vid.GetType().ToString();
                        value.CommandClassID = vid.GetCommandClassId().ToString();
                        value.Help = m_manager.GetValueHelp(vid);
                        bool read_only = m_manager.IsValueReadOnly(vid);

                        string data = GetValue(vid);
                        //m_manager.GetValueAsString(vid, out data);                          

                        if (verbosity > 4)
                            WriteToLog(Urgency.INFO, "[ValueChanged] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);


                        using (zvsContext Context = new zvsContext())
                        {
                            Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == node.ID);
                            if (d != null)
                            {
                                // d.last_heard_from = DateTime.Now;
                                //db.SaveChanges();

                                //Update Device Commands
                                if (!read_only)
                                {
                                    //User commands are more important so lets see them first in the GUIs
                                    int order;
                                    switch (value.Genre)
                                    {
                                        case "User":
                                            order = 1;
                                            break;
                                        case "Config":
                                            order = 2;
                                            break;
                                        default:
                                            order = 99;
                                            break;
                                    }

                                    DeviceCommand dc = d.Commands.FirstOrDefault(c => c.CustomData2 == vid.GetId().ToString());

                                    if (dc != null)
                                    {
                                        //After Value is Added, Value Name other value properties can change so update.
                                        dc.Name = "Set " + value.Label;
                                        dc.Help = value.Help;
                                        dc.CustomData1 = value.Label;
                                        dc.SortOrder = order;
                                    }
                                }

                                //Some dimmers take x number of seconds to dim to desired level.  Therefore the level received here initially is a 
                                //level between old level and new level. (if going from 0 to 100 we get 84 here).
                                //To get the real level re-poll the device a second or two after a level change was received.     
                                bool EnableDimmerRepoll = false;
                                bool.TryParse(DevicePropertyValue.GetDevicePropertyValue(Context, d, "ENABLEREPOLLONLEVELCHANGE"), out EnableDimmerRepoll);

                                if (FinishedInitialPoll && EnableDimmerRepoll)
                                {
                                    if (d.Type != null && d.Type == GetDeviceType("DIMMER", Context))
                                    {
                                        switch (value.Label)
                                        {
                                            case "Basic":
                                                DeviceValue dv_basic = d.Values.FirstOrDefault(v => v.UniqueIdentifier == vid.GetId().ToString());
                                                if (dv_basic != null)
                                                {
                                                    string prevVal = dv_basic.Value;
                                                    //If it is truly new
                                                    if (!prevVal.Equals(data))
                                                    {
                                                        //only allow each device to re-poll 1 time.
                                                        if (timers.Contains(d.NodeNumber))
                                                        {
                                                            Console.WriteLine(string.Format("Timer {0} restarted.", d.DeviceId));
                                                            System.Timers.Timer t = (System.Timers.Timer)timers[d.NodeNumber];
                                                            t.Stop();
                                                            t.Start();
                                                        }
                                                        else
                                                        {
                                                            System.Timers.Timer t = new System.Timers.Timer();
                                                            timers.Add(d.NodeNumber, t);
                                                            t.Interval = 2000;
                                                            t.Elapsed += (sender, e) =>
                                                            {
                                                                m_manager.RefreshNodeInfo(m_homeId, (byte)d.NodeNumber);
                                                                t.Stop();
                                                                Console.WriteLine(string.Format("Timer {0} Elapsed.", d.NodeNumber));
                                                                timers.Remove(d.NodeNumber);
                                                            };
                                                            t.Start();
                                                            Console.WriteLine(string.Format("Timer {0} started.", d.NodeNumber));
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }

                                //Update Current Status Field
                                if (d.Type != null && d.Type == GetDeviceType("THERMOSTAT", Context))
                                {
                                    if (value.Label == "Temperature")
                                    {
                                        int level = 0;
                                        int.TryParse(data, out level);

                                        d.CurrentLevelInt = level;
                                        d.CurrentLevelText = level + "° F";
                                        Context.SaveChanges();
                                    }
                                }
                                else if (d.Type != null && d.Type == GetDeviceType("SWITCH", Context))
                                {
                                    if (value.Label == "Basic")
                                    {
                                        int level = 0;
                                        if (int.TryParse(data, out level))
                                        {
                                            d.CurrentLevelInt = level > 0 ? 100 : 0;
                                            d.CurrentLevelText = level > 0 ? "On" : "Off";
                                            Context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    if (value.Label == "Basic")
                                    {
                                        int level = 0;
                                        int.TryParse(data, out level);

                                        d.CurrentLevelInt = level;
                                        d.CurrentLevelText = level + "%";
                                        Context.SaveChanges();
                                        Context.SaveChanges();
                                    }
                                }

                                DefineOrUpdateDeviceValue(new DeviceValue
                                {
                                    Device = d,
                                    UniqueIdentifier = vid.GetId().ToString(),
                                    Name = value.Label,
                                    Genre = value.Genre,
                                    Index = value.Index,
                                    CommandClass = value.CommandClassID,
                                    Value = data,
                                    ValueType = ConvertType(vid),
                                    isReadOnly = read_only
                                }, Context);
                            }
                            else
                            {
                                WriteToLog(Urgency.WARNING, "Getting changes on an unknown device!");
                            }

                        }

                        //}
                        //catch (Exception ex)
                        //{
                        //    WriteToLog(Urgency.ERROR, "error: " + ex.Message);
                        //}
                        break;
                    }

                case ZWNotification.Type.Group:
                    {
                        if (verbosity > 4)
                            WriteToLog(Urgency.INFO, "[Group]"); ;
                        break;
                    }

                case ZWNotification.Type.NodeAdded:
                    {
                        // if this node was in zwcfg*.xml, this is the first node notification
                        // if not, the NodeNew notification should already have been received
                        //if (GetNode(m_notification.GetHomeId(), m_notification.GetNodeId()) == null)
                        //{
                        Node node = new Node();
                        node.ID = m_notification.GetNodeId();
                        node.HomeID = m_notification.GetHomeId();
                        m_nodeList.Add(node);

                        if (verbosity > 4)
                            WriteToLog(Urgency.INFO, "[NodeAdded] ID:" + node.ID.ToString() + " Added");
                        //}
                        break;
                    }

                case ZWNotification.Type.NodeNew:
                    {
                        // Add the new node to our list (and flag as uninitialized)
                        Node node = new Node();
                        node.ID = m_notification.GetNodeId();
                        node.HomeID = m_notification.GetHomeId();
                        m_nodeList.Add(node);

                        if (verbosity > 4)
                            WriteToLog(Urgency.INFO, "[NodeNew] ID:" + node.ID.ToString() + " Added");
                        break;
                    }

                case ZWNotification.Type.NodeRemoved:
                    {
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == m_notification.GetNodeId())
                            {
                                if (verbosity > 4)
                                    WriteToLog(Urgency.INFO, "[NodeRemoved] ID:" + node.ID.ToString());
                                m_nodeList.Remove(node);
                                break;
                            }
                        }
                        break;
                    }

                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        using (zvsContext Context = new zvsContext())
                        {
                            Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                            if (node != null)
                            {
                                node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                            }
                            string deviceName = "UNKNOWN";
                            DeviceType device_type = null;

                            if (node != null)
                            {
                                if (verbosity > 4)
                                    WriteToLog(Urgency.INFO, "[Node Protocol Info] " + node.Label);

                                switch (node.Label)
                                {
                                    case "Toggle Switch":
                                    case "Binary Toggle Switch":
                                    case "Binary Switch":
                                    case "Binary Power Switch":
                                    case "Binary Scene Switch":
                                    case "Binary Toggle Remote Switch":
                                        deviceName = "OpenZWave Switch " + node.ID;
                                        device_type = GetDeviceType("SWITCH", Context);
                                        break;
                                    case "Multilevel Toggle Remote Switch":
                                    case "Multilevel Remote Switch":
                                    case "Multilevel Toggle Switch":
                                    case "Multilevel Switch":
                                    case "Multilevel Power Switch":
                                    case "Multilevel Scene Switch":
                                        deviceName = "OpenZWave Dimmer " + node.ID;
                                        device_type = GetDeviceType("DIMMER", Context);
                                        break;
                                    case "Multiposition Motor":
                                    case "Motor Control Class A":
                                    case "Motor Control Class B":
                                    case "Motor Control Class C":
                                        deviceName = "Variable Motor Control " + node.ID;
                                        device_type = GetDeviceType("DIMMER", Context);
                                        break;
                                    case "General Thermostat V2":
                                    case "Heating Thermostat":
                                    case "General Thermostat":
                                    case "Setback Schedule Thermostat":
                                    case "Setpoint Thermostat":
                                    case "Setback Thermostat":
                                    case "Thermostat":
                                        deviceName = "OpenZWave Thermostat " + node.ID;
                                        device_type = GetDeviceType("THERMOSTAT", Context);
                                        break;
                                    case "Remote Controller":
                                    case "Static PC Controller":
                                    case "Static Controller":
                                    case "Portable Remote Controller":
                                    case "Portable Installer Tool":
                                    case "Static Scene Controller":
                                    case "Static Installer Tool":
                                        deviceName = "OpenZWave Controller " + node.ID;
                                        device_type = GetDeviceType("CONTROLLER", Context);
                                        break;
                                    case "Secure Keypad Door Lock":
                                    case "Advanced Door Lock":
                                    case "Door Lock":
                                    case "Entry Control":
                                        deviceName = "OpenZWave Door Lock " + node.ID;
                                        device_type = GetDeviceType("DOORLOCK", Context);
                                        break;
                                    case "Alarm Sensor":
                                    case "Basic Routing Alarm Sensor":
                                    case "Routing Alarm Sensor":
                                    case "Basic Zensor Alarm Sensor":
                                    case "Zensor Alarm Sensor":
                                    case "Advanced Zensor Alarm Sensor":
                                    case "Basic Routing Smoke Sensor":
                                    case "Routing Smoke Sensor":
                                    case "Basic Zensor Smoke Sensor":
                                    case "Zensor Smoke Sensor":
                                    case "Advanced Zensor Smoke Sensor":
                                    case "Routing Binary Sensor":
                                    case "Routing Multilevel Sensor":
                                        deviceName = "OpenZWave Sensor " + node.ID;
                                        device_type = GetDeviceType("SENSOR", Context);
                                        break;
                                    default:
                                        {
                                            if (verbosity > 2)
                                                WriteToLog(Urgency.INFO, "[Node Label] " + node.Label);
                                            break;
                                        }
                                }
                                if (device_type != null)
                                {
                                    Device ozw_device = GetMyPluginsDevices(Context).FirstOrDefault(d => d.NodeNumber == node.ID);
                                    //If we don't already have the device
                                    if (ozw_device == null)
                                    {
                                        ozw_device = new Device
                                        {
                                            NodeNumber = node.ID,
                                            Type = device_type,
                                            Name = deviceName,
                                            CurrentLevelInt = 0,
                                            CurrentLevelText = ""
                                        };

                                        Context.Devices.Add(ozw_device);
                                        Context.SaveChanges();

                                    }

                                    #region Last Event Value Storeage
                                    //Node event value placeholder                               
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = ozw_device,
                                        UniqueIdentifier = LastEventNameValueId,
                                        Name = "Last Node Event Value",
                                        Genre = "Custom",
                                        Index = "0",
                                        ValueType = DataType.BYTE, 
                                        CommandClass = "0",
                                        Value = "0",
                                        isReadOnly = true
                                    }, Context);
                                    #endregion

                                }
                                else
                                    WriteToLog(Urgency.WARNING, string.Format("Found unknown device '{0}', node #{1}!", node.Label, node.ID));
                            }
                        }
                        break;
                    }

                case ZWNotification.Type.NodeNaming:
                    {
                        string ManufacturerNameValueId = "MN1";
                        string ProductNameValueId = "PN1";
                        string NodeLocationValueId = "NL1";
                        string NodeNameValueId = "NN1";

                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID);
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID);
                            node.Location = m_manager.GetNodeLocation(m_homeId, node.ID);
                            node.Name = m_manager.GetNodeName(m_homeId, node.ID);

                            using (zvsContext Context = new zvsContext())
                            {
                                Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == node.ID);
                                if (d != null)
                                {
                                    //lets store the manufacturer name and product name in the values table.   
                                    //Giving ManufacturerName a random value_id 9999058723211334120                                                           
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = d, 
                                        UniqueIdentifier = ManufacturerNameValueId,
                                        Name = "Manufacturer Name",
                                        Genre = "Custom",
                                        Index = "0",
                                        ValueType = DataType.STRING, 
                                        CommandClass = "0",
                                        Value = node.Manufacturer,
                                        isReadOnly = true
                                    }, Context);
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = d,
                                        UniqueIdentifier = ProductNameValueId,
                                        Name = "Product Name",
                                        Genre = "Custom",
                                        Index = "0",
                                        ValueType = DataType.STRING, 
                                        CommandClass = "0",
                                        Value = node.Product,
                                        isReadOnly = true
                                    }, Context);
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = d,
                                        UniqueIdentifier = NodeLocationValueId,
                                        Name = "Node Location",
                                        Genre = "Custom",
                                        Index = "0",
                                        ValueType = DataType.STRING, 
                                        CommandClass = "0",
                                        Value = node.Location,
                                        isReadOnly = true
                                    }, Context);
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = d,
                                        UniqueIdentifier = NodeNameValueId,
                                        Name = "Node Name",
                                        Genre = "Custom",
                                        Index = "0",
                                        ValueType = DataType.STRING, 
                                        CommandClass = "0",
                                        Value = node.Name,
                                        isReadOnly = true
                                    }, Context);
                                }
                            }
                        }
                        if (verbosity > 3)
                            WriteToLog(Urgency.INFO, "[NodeNaming] Node:" + node.ID + ", Product:" + node.Product + ", Manufacturer:" + node.Manufacturer + ")");

                        break;
                    }

                case ZWNotification.Type.NodeEvent:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        byte gevent = m_notification.GetEvent();

                        if (node != null)
                        {
                            if (verbosity > 4)
                                WriteToLog(Urgency.INFO, string.Format("[NodeEvent] Node: {0}, Event Byte: {1}", node.ID, gevent));

                            using (zvsContext Context = new zvsContext())
                            {
                                #region Last Event Value Storeage
                                Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == node.ID);
                                if (d != null)
                                {
                                    //Node event value placeholder
                                    DeviceValue dv = d.Values.FirstOrDefault(v => v.UniqueIdentifier == LastEventNameValueId);
                                    if (dv != null)
                                    {
                                        dv.Value = gevent.ToString();
                                        Context.SaveChanges();

                                        //Since events are differently than values fire the value change event every time we recieve the event regardless if 
                                        //it is the same value or not.
                                        dv.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(dv.DeviceValueId, string.Empty, dv.Value));
                                    }
                                }
                                #endregion
                            }

                        }
                        break;

                    }

                case ZWNotification.Type.PollingDisabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            if (verbosity > 4)
                                WriteToLog(Urgency.INFO, "[PollingDisabled] Node:" + node.ID);
                        }

                        break;
                    }

                case ZWNotification.Type.PollingEnabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            if (verbosity > 4)
                                WriteToLog(Urgency.INFO, "[PollingEnabled] Node:" + node.ID);
                        }
                        break;
                    }

                case ZWNotification.Type.DriverReady:
                    {

                        m_homeId = m_notification.GetHomeId();

                        WriteToLog(Urgency.INFO, "Initializing...driver with Home ID 0x" + m_homeId);

                        break;
                    }

                case ZWNotification.Type.NodeQueriesComplete:
                    {

                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            using (zvsContext Context = new zvsContext())
                            {
                                Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == node.ID);
                                if (d != null)
                                {
                                    d.LastHeardFrom = DateTime.Now;
                                }
                                Context.SaveChanges();
                            }

                            if (verbosity > 0)
                                WriteToLog(Urgency.INFO, "[NodeQueriesComplete] node " + node.ID + " query complete.");
                        }

                        break;
                    }

                case ZWNotification.Type.AllNodesQueried:
                    {
                        foreach (Node n in m_nodeList)
                        {
                            using (zvsContext Context = new zvsContext())
                            {
                                Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == n.ID);

                                if (d != null)
                                {
                                    if (DevicePropertyValue.GetDevicePropertyValue(Context, d, "ENABLEPOLLING").ToUpper().Equals("TRUE"))
                                        EnablePolling(n.ID);
                                }
                            }
                        }


                        WriteToLog(Urgency.INFO, "Ready:  All nodes queried. Plug-in now ready.");
                        IsReady = true;

                        FinishedInitialPoll = true;
                        break;
                    }

                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        using (zvsContext Context = new zvsContext())
                        {
                            foreach (Node n in m_nodeList)
                            {
                                Device d = GetMyPluginsDevices(Context).FirstOrDefault(o => o.NodeNumber == n.ID);

                                if (d != null)
                                {
                                    if (DevicePropertyValue.GetDevicePropertyValue(Context, d, "ENABLEPOLLING").ToUpper().Equals("TRUE"))
                                        EnablePolling(n.ID);
                                }
                            }
                        }

                        WriteToLog(Urgency.INFO, "Ready:  Awake nodes queried (but not some sleeping nodes).");
                        IsReady = true;

                        FinishedInitialPoll = true;

                        break;
                    }
            }
        }

        private DataType ConvertType(ZWValueID v)
        {
            DataType dataType = DataType.NONE;
            ZWValueID.ValueType openZwaveVType = v.GetType();

            if(openZwaveVType == ZWValueID.ValueType.Bool)
            {
                 dataType = DataType.BOOL;
            }
            else if(openZwaveVType == ZWValueID.ValueType.Button)
            {
                 dataType = DataType.STRING;
            }
            else if(openZwaveVType == ZWValueID.ValueType.Byte)
            {
                 dataType = DataType.BYTE;
            }
            else if(openZwaveVType == ZWValueID.ValueType.Decimal)
            {
                 dataType = DataType.DECIMAL;
            }
            else if(openZwaveVType == ZWValueID.ValueType.Int)
            {
                 dataType = DataType.INTEGER;
            }
            else if(openZwaveVType == ZWValueID.ValueType.List)
            {
                 dataType = DataType.LIST;
            }
            else if(openZwaveVType == ZWValueID.ValueType.Schedule)
            {
                 dataType = DataType.STRING;            
            }
            else if(openZwaveVType == ZWValueID.ValueType.Short)
            {
                 dataType = DataType.SHORT;
            }
            else if(openZwaveVType == ZWValueID.ValueType.String)
            {
                 dataType = DataType.STRING;
            }
            return dataType;
        }

        private string GetValue(ZWValueID v)
        {
            switch (v.GetType())
            {
                case ZWValueID.ValueType.Bool:
                    bool r1;
                    m_manager.GetValueAsBool(v, out r1);
                    return r1.ToString();
                case ZWValueID.ValueType.Byte:
                    byte r2;
                    m_manager.GetValueAsByte(v, out r2);
                    return r2.ToString();
                case ZWValueID.ValueType.Decimal:
                    decimal r3;
                    m_manager.GetValueAsDecimal(v, out r3);
                    return r3.ToString();
                case ZWValueID.ValueType.Int:
                    int r4;
                    m_manager.GetValueAsInt(v, out r4);
                    return r4.ToString();
                case ZWValueID.ValueType.List:
                    // string[] r5;
                    //  m_manager.GetValueListSelection(v, out r5);
                    //string r6 = "";
                    //foreach (string s in r5)
                    // {
                    //     r6 += s;
                    //    r6 += "/";
                    //}
                    string r6 = string.Empty;
                    m_manager.GetValueListSelection(v, out r6);
                    return r6;
                case ZWValueID.ValueType.Schedule:
                    return "Schedule";
                case ZWValueID.ValueType.Short:
                    short r7;
                    m_manager.GetValueAsShort(v, out r7);
                    return r7.ToString();
                case ZWValueID.ValueType.String:
                    string r8;
                    m_manager.GetValueAsString(v, out r8);
                    return r8;
                default:
                    return "";
            }
        }

        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (Node node in m_nodeList)
            {
                if ((node.ID == nodeId) && (node.HomeID == homeId))
                {
                    return node;
                }
            }
            return new Node();
        }

        private void EnablePolling(byte nid)
        {
            try
            {
                Node n = GetNode(m_homeId, nid);
                ZWValueID zv = null;
                switch (n.Label)
                {
                    case "Toggle Switch":
                    case "Binary Toggle Switch":
                    case "Binary Switch":
                    case "Binary Power Switch":
                    case "Binary Scene Switch":
                    case "Binary Toggle Remote Switch":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Switch")
                                zv = v.ValueID;
                        }
                        break;
                    case "Multilevel Toggle Remote Switch":
                    case "Multilevel Remote Switch":
                    case "Multilevel Toggle Switch":
                    case "Multilevel Switch":
                    case "Multilevel Power Switch":
                    case "Multilevel Scene Switch":
                    case "Multiposition Motor":
                    case "Motor Control Class A":
                    case "Motor Control Class B":
                    case "Motor Control Class C":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == "User" && v.Label == "Level")
                                zv = v.ValueID;
                        }
                        break;
                    case "General Thermostat V2":
                    case "Heating Thermostat":
                    case "General Thermostat":
                    case "Setback Schedule Thermostat":
                    case "Setpoint Thermostat":
                    case "Setback Thermostat":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Temperature")
                                zv = v.ValueID;
                        }
                        break;
                    case "Static PC Controller":
                    case "Static Controller":
                    case "Portable Remote Controller":
                    case "Portable Installer Tool":
                    case "Static Scene Controller":
                    case "Static Installer Tool":
                        break;
                    case "Secure Keypad Door Lock":
                    case "Advanced Door Lock":
                    case "Door Lock":
                    case "Entry Control":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == "User" && v.Label == "Basic")
                                zv = v.ValueID;
                        }
                        break;
                    case "Alarm Sensor":
                    case "Basic Routing Alarm Sensor":
                    case "Routing Alarm Sensor":
                    case "Basic Zensor Alarm Sensor":
                    case "Zensor Alarm Sensor":
                    case "Advanced Zensor Alarm Sensor":
                    case "Basic Routing Smoke Sensor":
                    case "Routing Smoke Sensor":
                    case "Basic Zensor Smoke Sensor":
                    case "Zensor Smoke Sensor":
                    case "Advanced Zensor Smoke Sensor":
                    case "Routing Binary Sensor":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == "User" && v.Label == "Basic")
                                zv = v.ValueID;
                        }
                        break;
                }
                if (zv != null)
                    m_manager.EnablePoll(zv);
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error attempting to enable polling: " + ex.Message);
            }
        }

        #endregion
    }
}
