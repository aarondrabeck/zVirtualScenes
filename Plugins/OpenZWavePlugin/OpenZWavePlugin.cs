using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using OpenZWaveDotNet;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using zVirtualScenesAPI;

using OpenZWavePlugin.Forms;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;


using System.Drawing;
using System.Text;
using zVirtualScenesCommon.Util;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace OpenZWavePlugin
{
    [Export(typeof(Plugin))]
    public class OpenZWavePlugin : Plugin
    {               
        private ZWManager m_manager = null;
        private ZWOptions m_options = null;
        ZWNotification m_notification = null;
        UInt32 m_homeId = 0;
        List<Node> m_nodeList = new List<Node>();
        private bool FinishedInitialPoll = false;
        private string LaastEventNameValueId = "9999058723211334119";

        public OpenZWavePlugin()
            : base("OPENZWAVE",
               "Open ZWave Plugin",
                "This plug-in interfaces zVirtualScenes with OpenZWave using the OpenZWave open-source project."
                ) { }
       
        protected override bool StartPlugin()
        {           
            
            try
            {
                WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin started.");
                
                // Create the Options                
                m_options = new ZWOptions();
                m_options.Create(Environment.CurrentDirectory + @"\plugins\config\", 
                                 Environment.CurrentDirectory + @"\plugins\", @"");
                m_options.Lock();
                m_manager = new ZWManager();
                m_manager.Create();
                m_manager.OnNotification += NotificationHandler;
                
                bool useHID = false;
                //bool.TryParse(GetSettingValue("HID"), out useHID);

                if (!useHID)
                {
                    string comPort = GetSettingValue("COMPORT");
                    if (comPort != "0")
                    {
                        m_manager.AddDriver(@"\\.\COM" + comPort);
                    }
                }
                else
                {
                    //m_manager.AddHidDriver();
                }
                
                int pollint = 0;
                int.TryParse(GetSettingValue("POLLINT"), out pollint);
                if (pollint != 0)
                {                    
                    m_manager.SetPollInterval(pollint);
                }                
            }
            catch (Exception e)
            {
                WriteToLog(Urgency.ERROR, e.Message);
                return false;
            }

            return true;
        }
        
        protected override bool StopPlugin()
        {
            WriteToLog(Urgency.INFO, Friendly_Name + " plugin stopped.");            
            m_manager.OnNotification -= NotificationHandler;
            string comPort = GetSettingValue("COMPORT");                   
            m_manager.RemoveDriver(@"\\.\COM" + comPort);
            m_manager.Destroy();            
            m_options.Destroy();
            m_options = null;
            m_manager = null;
            IsReady = false;
            return true;            
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            switch (settingName)
            {
                case "Com Port":
                    // Set the port here
                    break;
                case "Polling Interval":
                    // Set the polling interval here
                    break;
            }
        }              

        public override void Initialize()
        {            
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "COMPORT",
                friendly_name = "Com Port",
                value = (3).ToString(),
                value_data_type = (int)Data_Types.COMPORT,
                description = "The COM port that your z-wave controller is assigned to."
            });

            //DefineOrUpdateSetting(new plugin_settings
            //{
            //    name = "HID",
            //    friendly_name = "Use HID",
            //    value = false.ToString(),
            //    value_data_type = (int)Data_Types.BOOL,
            //    description = "Use HID rather than COM port. (use this for ControlThink Sticks)"
            //});

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "POLLINT",
                friendly_name = "Polling Interval",
                value = (360).ToString(),
                value_data_type = (int)Data_Types.INTEGER,
                description = "The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network traffic. "
            });

            //Controller Type Devices
            device_types controller_dt = new device_types  { name = "CONTROLLER", friendly_name = "OpenZWave Controller", show_in_list = true };
            controller_dt.device_type_commands.Add(new device_type_commands { name = "RESET", friendly_name = "Reset Controller", arg_data_type = (int)Data_Types.NONE, description = "Erases all Z-Wave network settings from your controller." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "ADDDEVICE", friendly_name = "Add Device to Network", arg_data_type = (int)Data_Types.NONE, description = "Adds a ZWave Device to your network." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "AddController", friendly_name = "Add Controller to Network", arg_data_type = (int)Data_Types.NONE, description = "Adds a ZWave Controller to your network." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "CreateNewPrimary", friendly_name = "Create New Primary", arg_data_type = (int)Data_Types.NONE, description = "Puts the target controller into receive configuration mode." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "ReceiveConfiguration", friendly_name = "Receive Configuration", arg_data_type = (int)Data_Types.NONE, description = "Receives the network configuration from another controller." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "RemoveController", friendly_name = "Remove Controller", arg_data_type = (int)Data_Types.NONE, description = "Removes a Controller from your network." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "RemoveDevice", friendly_name = "Remove Device", arg_data_type = (int)Data_Types.NONE, description = "Removes a Device from your network." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "TransferPrimaryRole", friendly_name = "Transfer Primary Role", arg_data_type = (int)Data_Types.NONE, description = "Transfers the primary role\nto another controller." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "HasNodeFailed", friendly_name = "Has Node Failed", arg_data_type = (int)Data_Types.NONE, description = "Tests whether a node has failed." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "RemoveFailedNode", friendly_name = "Remove Failed Node", arg_data_type = (int)Data_Types.NONE, description = "Removes the failed node from the controller's list." });
            controller_dt.device_type_commands.Add(new device_type_commands { name = "ReplaceFailedNode", friendly_name = "Replace Failed Node", arg_data_type = (int)Data_Types.NONE, description = "Tests the failed node." });
            DefineOrUpdateDeviceType(controller_dt);

            //Switch Type Devices
            device_types switch_dt = new device_types { name = "SWITCH", friendly_name = "OpenZWave Binary", show_in_list = true };
            switch_dt.device_type_commands.Add(new device_type_commands { name = "TURNON", friendly_name = "Turn On", arg_data_type = (int)Data_Types.NONE, description = "Activates a switch." });
            switch_dt.device_type_commands.Add(new device_type_commands { name = "TURNOFF", friendly_name = "Turn Off", arg_data_type = (int)Data_Types.NONE, description = "Deactivates a switch." });
            switch_dt.device_type_commands.Add(new device_type_commands { name = "MOMENTARY", friendly_name = "Turn On for X milliseconds", arg_data_type = (int)Data_Types.INTEGER, description = "Turns a device on for the specified number of milliseconds and then turns the device back off." });
            DefineOrUpdateDeviceType(switch_dt);

            //Dimmer Type Devices
            device_types dimmer_dt = new device_types { name = "DIMMER", friendly_name = "OpenZWave Dimmer", show_in_list = true };
            dimmer_dt.device_type_commands.Add(new device_type_commands { name = "TURNON", friendly_name = "Turn On", arg_data_type = (int)Data_Types.NONE, description = "Activates a dimmer." });
            dimmer_dt.device_type_commands.Add(new device_type_commands { name = "TURNOFF", friendly_name = "Turn Off", arg_data_type = (int)Data_Types.NONE, description = "Deactivates a dimmer." });
            
            device_type_commands dimmer_preset_cmd =  new device_type_commands { name = "SETPRESETLEVEL", friendly_name = "Set Level", arg_data_type = (int)Data_Types.LIST, description = "Sets a dimmer to a preset level." };
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "0%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "20%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "40%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "60%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "80%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "100%" } );
            dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { option = "255" } );
            dimmer_dt.device_type_commands.Add(dimmer_preset_cmd);           

            DefineOrUpdateDeviceType(dimmer_dt);

            //Thermostat Type Devices
            device_types thermo_dt = new device_types { name = "THERMOSTAT", friendly_name = "OpenZWave Thermostat", show_in_list = true };
            thermo_dt.device_type_commands.Add(new device_type_commands { name = "SETENERGYMODE", friendly_name = "Set Energy Mode", arg_data_type = (int)Data_Types.NONE, description = "Set thermosat to Energy Mode." });
            thermo_dt.device_type_commands.Add(new device_type_commands { name = "SETCONFORTMODE", friendly_name = "Set Confort Mode", arg_data_type = (int)Data_Types.NONE, description = "Set thermosat to Confort Mode. (Run)" });
            DefineOrUpdateDeviceType(thermo_dt);

            //Door Lock Type Devices
            device_types lock_dt = new device_types { name = "DOORLOCK", friendly_name = "OpenZWave Doorlock", show_in_list = true };
            DefineOrUpdateDeviceType(lock_dt);

            //Sensors
            device_types sensor_dt = new device_types { name = "SENSOR", friendly_name = "OpenZWave Sensor", show_in_list = true };
            DefineOrUpdateDeviceType(sensor_dt);

            device_propertys.DefineOrUpdateDeviceProperty(new device_propertys
            {
                name = "DEFAULONLEVEL",
                friendly_name = "Level that an object is set to when using the on command.",
                default_value = "99",
                value_data_type = (int)Data_Types.BYTE
            });

            device_propertys.DefineOrUpdateDeviceProperty(new device_propertys
            {
                name = "ENABLEREPOLLONLEVELCHANGE",
                friendly_name = "Repoll dimmers 5 seconds after a level change is received?",
                default_value = true.ToString(),
                value_data_type = (int)Data_Types.BOOL
            });

            
                        
            //TODO: Make a new DeviceAPIProperty that is API specific for types of settings that applies OpenZWave Devices           

            ////TEMP 
            //DefineDevice(new device { node_id = 1, device_type_id = GetDeviceType("DIMMER").id, friendly_name = "Test Device 1", last_heard_from = DateTime.Now});
            //DefineDevice(new device { node_id = 2, device_type_id = GetDeviceType("DIMMER").id, friendly_name = "Test Device 2", last_heard_from = DateTime.Now });

            //int i = 2;
            //System.Timers.Timer t = new System.Timers.Timer();
            //t.Interval = 5000;
            //t.Elapsed += (sender, e) =>
            //{
            //    i++;
            //    //zvsEntityControl.zvsContext.devices.FirstOrDefault(d => d.node_id == 1).last_heard_from = DateTime.Now;
            //    //zvsEntityControl.zvsContext.SaveChanges();


            //    DefineOrUpdateDeviceValue(new device_values
            //    {
            //        device_id = zvsEntityControl.zvsContext.devices.FirstOrDefault(d => d.node_id == 1).id,
            //        value_id = "1!",
            //        label_name = "Basic",
            //        genre = "Genre",
            //        index = "Index",
            //        type = "Type",
            //        commandClassId = "Coomand Class",
            //        value = (i % 2 == 0 ? "99" : "50")
            //    });

            //    //DefineDevice(new device { node_id = i, device_type_id = GetDeviceType("DIMMER").id, friendly_name = "Test Device " + i, last_heard_from = DateTime.Now });

            //};
            //t.Enabled = true;


            
        }

        public override bool ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
            if(cmd.device.device_types.name == "CONTROLLER")
            {
                switch (cmd.device_type_commands.name)
                {
                    case "RESET":
                        {
                            m_manager.ResetController(m_homeId);
                            return true;
                        }
                    case "ADDDEVICE":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddDevice, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "AddController":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddController, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "CreateNewPrimary":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.CreateNewPrimary, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "ReceiveConfiguration":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReceiveConfiguration, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "RemoveController":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveController, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "RemoveDevice":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveDevice, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "TransferPrimaryRole":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.TransferPrimaryRole, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "HasNodeFailed":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.HasNodeFailed, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "RemoveFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveFailedNode, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                    case "ReplaceFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReplaceFailedNode, (byte)cmd.device.node_id);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            return true;
                        }
                }
            }
            else if (cmd.device.device_types.name == "SWITCH")
            {
                switch (cmd.device_type_commands.name)
                {
                    case "MOMENTARY":
                        {
                            int delay = 1000;
                            int.TryParse(cmd.arg, out delay);

                            m_manager.SetNodeOn(m_homeId, (byte)cmd.device.node_id);
                            System.Timers.Timer t = new System.Timers.Timer();
                            t.Interval = delay;
                            t.Elapsed += (sender, e) =>
                            {
                                m_manager.SetNodeOff(m_homeId, (byte)cmd.device.node_id);
                                t.Enabled = false;
                                t.Dispose();
                            };
                            t.Enabled = true;
                            return true;
                            
                        }
                    case "TURNON":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)cmd.device.node_id);
                            return true;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.device.node_id);
                            break;
                        }
                }
            }
            else if (cmd.device.device_types.name == "DIMMER")
            {
                switch (cmd.device_type_commands.name)
                {
                    case "TURNON":
                        {
                            byte defaultonlevel = 99;
                            byte.TryParse(device_property_values.GetDevicePropertyValue(zvsEntityControl.zvsContext, cmd.device_id, "DEFAULONLEVEL"), out defaultonlevel);
                            m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, defaultonlevel);
                            return true;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.device.node_id);
                            return true;
                        }
                    case "SETPRESETLEVEL":
                        {
                            switch (cmd.arg)
                            {
                                case "0%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(0));
                                    break;
                                case "20%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(20));
                                    break;
                                case "40%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(40));
                                    break;
                                case "60%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(60));
                                    break;
                                case "80%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(80));
                                    break;
                                case "100%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(100));
                                    break;
                                case "255":
                                    m_manager.SetNodeLevel(m_homeId, (byte)cmd.device.node_id, Convert.ToByte(255));
                                    break;
                            }
                            return true;
                        }
                }
            }
            else if (cmd.device.device_types.name == "THERMOSTAT")
            {
                switch (cmd.device_type_commands.name)
                {
                    case "SETENERGYMODE":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)cmd.device.node_id);
                            return true;
                        }
                    case "SETCONFORTMODE":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)cmd.device.node_id);
                            return true;
                        }                  
                }
            }

            return false;
        }

        public override bool ProcessDeviceCommand(device_command_que cmd)
        {
            if (cmd.device_commands.name.Contains("DYNAMIC_CMD_"))
            {
                //Get more info from this Node from OpenZWave
                Node node = GetNode(m_homeId, (byte)cmd.device.node_id);

                switch ((Data_Types)cmd.device_commands.arg_data_type)
                {
                    case Data_Types.BYTE:
                        {
                            byte b = 0;
                            byte.TryParse(cmd.arg, out b);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.device_commands.custom_data1))
                                    m_manager.SetValue(v.ValueID, b);
                            return true;
                        }
                    case Data_Types.BOOL:
                        {
                            bool b = true;
                            bool.TryParse(cmd.arg, out b);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.device_commands.custom_data1))
                                    m_manager.SetValue(v.ValueID, b);
                            return true;
                        }
                    case Data_Types.DECIMAL:
                        {
                            float f = Convert.ToSingle(cmd.arg);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.device_commands.custom_data1))
                                    m_manager.SetValue(v.ValueID, f);
                            return true;
                        }
                    case Data_Types.LIST:
                    case Data_Types.STRING:
                        {
                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.device_commands.custom_data1))
                                    m_manager.SetValue(v.ValueID, cmd.arg);
                            return true;
                        }
                    case Data_Types.INTEGER:
                        {
                            int i = 0;
                            int.TryParse(cmd.arg, out i);

                            foreach (Value v in node.Values)
                                if (m_manager.GetValueLabel(v.ValueID).Equals(cmd.device_commands.custom_data1))
                                    m_manager.SetValue(v.ValueID, i);
                            return true;
                        }
                }
            }
            return false;
        }

        public override bool Repoll(device device)
        {
            m_manager.RequestNodeState(m_homeId, Convert.ToByte(device.node_id));
            return true;
        }

        public override bool ActivateGroup(long groupID)
        {
            IQueryable<device> devices = GetDeviceInGroup(groupID);

            if (devices != null)
            {
                foreach (device d in devices)
                {
                    switch (d.device_types.name)
                    {
                        case "SWITCH":
                            m_manager.SetNodeOn(m_homeId, Convert.ToByte(d.node_id));
                            break;
                        case "DIMMER":
                            byte defaultonlevel = 99;
                            byte.TryParse(device_property_values.GetDevicePropertyValue(zvsEntityControl.zvsContext, d.id, "DEFAULONLEVEL"), out defaultonlevel);
                            m_manager.SetNodeLevel(m_homeId, Convert.ToByte(d.node_id), defaultonlevel);
                            break;
                    }

                }
            }
            return true;
        }

        public override bool DeactivateGroup(long groupID)
        {
            IQueryable<device> devices = GetDeviceInGroup(groupID);

            if (devices != null)
            {
                foreach (device d in devices)
                {
                    switch (d.device_types.name)
                    {
                        case "SWITCH":
                            m_manager.SetNodeOff(m_homeId, Convert.ToByte(d.node_id));
                            break;
                        case "DIMMER":
                          
                            m_manager.SetNodeLevel(m_homeId, Convert.ToByte(d.node_id), 0);
                            break;
                    }

                }
            }
            return true;
        }
        
        #region OpenZWave Interface

        public void NotificationHandler(ZWNotification notification)
        {
            m_notification = notification;
            NotificationHandler();
            m_notification = null;
        }

        private void NotificationHandler()
        {
            //osae.AddToLog("Notification: " + m_notification.GetType().ToString(), false);
            switch (m_notification.GetType())
            {
                case ZWNotification.Type.ValueAdded:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        device d = GetDevices().FirstOrDefault(o => o.node_id == node.ID);
                        if (d != null)
                        {
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


                            Console.WriteLine("OpenZWave Plugin | [ValueAdded] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data + ", result: " +b.ToString());

                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = d.id,
                                value_id = vid.GetId().ToString(),
                                label_name = value.Label,
                                genre = value.Genre,
                                index = value.Index,
                                type = value.Type,
                                commandClassId = value.CommandClassID,
                                value = data,
                                read_only = read_only
                            });

                            #region Install Dynamic Commands

                            if (!read_only)
                            {
                                Data_Types pType = Data_Types.NONE;

                                //Set param types for command
                                switch (vid.GetType())
                                {
                                    case ZWValueID.ValueType.List:
                                        pType = Data_Types.LIST;
                                        break;
                                    case ZWValueID.ValueType.Byte:
                                        pType = Data_Types.BYTE;
                                        break;
                                    case ZWValueID.ValueType.Decimal:
                                        pType = Data_Types.DECIMAL;
                                        break;
                                    case ZWValueID.ValueType.Int:
                                        pType = Data_Types.INTEGER;
                                        break;
                                    case ZWValueID.ValueType.String:
                                        pType = Data_Types.STRING;
                                        break;
                                    case ZWValueID.ValueType.Short:
                                        pType = Data_Types.SHORT;
                                        break;
                                    case ZWValueID.ValueType.Bool:
                                        pType = Data_Types.BOOL;
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


                                device_commands dynamic_dc = new device_commands {
                                    device_id =d.id,
                                    name = "DYNAMIC_CMD_" + value.Label.ToUpper(),
                                    friendly_name = "Set " + value.Label, 
                                    arg_data_type = (int)pType,
                                    help = value.Help, 
                                    custom_data1 = value.Label,
                                    custom_data2 = vid.GetId().ToString(),
                                    sort_order = order };
                               
                                //Special case for lists add additional info
                                if (vid.GetType() == ZWValueID.ValueType.List)
                                {
                                    //Install the allowed options/values
                                    String[] options;
                                    if (m_manager.GetValueListItems(vid, out options))
                                        foreach (string option in options)
                                            dynamic_dc.device_command_options.Add(new device_command_options { name = option });
                                }

                                DefineOrUpdateDeviceCommand(dynamic_dc);
                            }
                            #endregion
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

                            Console.WriteLine("OpenZWave Plugin | [ValueRemoved] Node:" + node.ID + ",Label:" + m_manager.GetValueLabel(vid));

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
                        //try
                        //{
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

                            Console.WriteLine("OpenZWave Plugin | [ValueChanged] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);   
                     
                            device d = GetDevices().FirstOrDefault(o => o.node_id == node.ID);

                            if (d != null)
                            {
                                

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

                                    device_commands dc = d.device_commands.FirstOrDefault(c => c.custom_data2 == vid.GetId().ToString());

                                    if (dc != null)
                                    {
                                        //After Value is Added, Value Name other value properties can change so update.
                                        dc.friendly_name = "Set " + value.Label;
                                        dc.help = value.Help;
                                        dc.custom_data1 = value.Label;
                                        dc.sort_order = order;
                                    }
                                }

                                //Some dimmers take x number of seconds to dim to desired level.  Therefor the level recieved here initially is a 
                                //level between old level and new level. (if going from 0 to 100 we get 84 here).
                                //To get the real level repoll the device a second or two after a level change was recieved.     
                                bool EnableDimmerRepoll = false;
                                bool.TryParse(device_property_values.GetDevicePropertyValue(zvsEntityControl.zvsContext,d.id, "ENABLEREPOLLONLEVELCHANGE"), out EnableDimmerRepoll);

                                if (FinishedInitialPoll && EnableDimmerRepoll)
                                {
                                    switch (node.Label)
                                    {
                                        case "Multilevel Switch":
                                        case "Multilevel Power Switch":
                                            {

                                                switch (value.Label)
                                                {
                                                    case "Level":
                                                    case "Basic":
                                                        device_values dv_basic = d.device_values.FirstOrDefault(v => v.value_id == vid.GetId().ToString());
                                                        if (dv_basic != null)
                                                        {
                                                            string prevVal = dv_basic.value;
                                                            //If it is truely new
                                                            if (!prevVal.Equals(data))
                                                            {
                                                                System.Timers.Timer t = new System.Timers.Timer();
                                                                t.Interval = 5000;
                                                                t.Elapsed += (sender, e) =>
                                                                {
                                                                    m_manager.RefreshNodeInfo(m_homeId, (byte)d.node_id);
                                                                    t.Enabled = false;
                                                                };
                                                                t.Enabled = true;
                                                            }
                                                        }
                                                            break;                                                        
                                                }
                                                break;
                                            }
                                    }
                                }

                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == vid.GetId().ToString());
                                if (dv != null)
                                {
                                    dv.index = value.Index;
                                    string prev_value = dv.value;
                                    dv.value = data;
                                    dv.type = value.Type;
                                    dv.genre = value.Genre;
                                    dv.label_name = value.Label;

                                    //Update las heard from
                                    //NOTE:  THIS TRIGGERS THE REDAW OF THE DEVICE LIST SO MAKE SURE OTHER VALUES ARE SET FIRST
                                    d.last_heard_from = DateTime.Now;

                                    zvsEntityControl.zvsContext.SaveChanges();

                                    if (!prev_value.Equals(data))
                                    {
                                        dv.DeviceValueDataChanged(prev_value);
                                    }


                                }
                                else
                                {
                                    WriteToLog(Urgency.WARNING, string.Format("Device value change event on '{0}' occured but could not find a device value with id {1} in database.", value.Label, value.Index)); 
                                }
                            }
                            else
                            {
                                WriteToLog(Urgency.WARNING, "Getting changes on an unknown device!");
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
                        Console.WriteLine("OpenZWave Plugin | [Group]"); ;
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

                            Console.WriteLine("OpenZWave Plugin | [NodeAdded] ID:" + node.ID.ToString() + " Added");
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

                        Console.WriteLine("OpenZWave Plugin | [NodeNew] ID:" + node.ID.ToString() + " Added");                        
                        break;
                    }

                case ZWNotification.Type.NodeRemoved:
                    {
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == m_notification.GetNodeId())
                            {
                                Console.WriteLine("OpenZWave Plugin | [NodeRemoved] ID:" + node.ID.ToString());
                                m_nodeList.Remove(node);
                                break;
                            }
                        }
                        break;
                    }                  

                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                        }
                        string deviceName = "UNKNOWN";
                        device_types device_type = null;

                        if (node != null)
                        {
                            WriteToLog(Urgency.INFO, "OpenZWave Plugin | [Node Protocol Info] " + node.Label);

                            switch (node.Label)
                            {
                                case "Toggle Switch":
                                case "Binary Toggle Switch":
                                case "Binary Switch":
                                case "Binary Power Switch":
                                case "Binary Scene Switch":  
                                case "Binary Toggle Remote Switch":
                                    deviceName = "OpenZWave Switch " + node.ID;
                                    device_type = GetDeviceType("SWITCH");
                                    break;
                                case "Multilevel Toggle Remote Switch":
                                case "Multilevel Remote Switch":
                                case "Multilevel Toggle Switch":
                                case "Multilevel Switch":
                                case "Multilevel Power Switch":
                                case "Multilevel Scene Switch":
                                    deviceName = "OpenZWave Dimmer " + node.ID;
                                    device_type = GetDeviceType("DIMMER");
                                    break;                                
                                case "Multiposition Motor":
                                case "Motor Control Class A":
                                case "Motor Control Class B":
                                case "Motor Control Class C":
                                     deviceName = "Variable Motor Control " + node.ID;
                                    device_type = GetDeviceType("DIMMER");
                                    break;
                                case "General Thermostat V2":
                                case "Heating Thermostat":
                                case "General Thermostat":
                                case "Setback Schedule Thermostat":
                                case "Setpoint Thermostat":
                                case "Setback Thermostat":
                                    deviceName = "OpenZWave Thermostat " + node.ID;
                                    device_type = GetDeviceType("THERMOSTAT");
                                    break;
                                case "Static PC Controller":
                                case "Static Controller":
                                case "Portable Remote Controller":
                                case "Portable Installer Tool":
                                case "Static Scene Controller":
                                case "Static Installer Tool":
                                     deviceName = "OpenZWave Controller " + node.ID;
                                    device_type = GetDeviceType("CONTROLLER");
                                     break;
                                case "Secure Keypad Door Lock":
                                case "Advanced Door Lock":
                                case "Door Lock":
                                case "Entry Control":
                                     deviceName = "OpenZWave Door Lock " + node.ID;
                                    device_type = GetDeviceType("DOORLOCK");
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
                                    device_type = GetDeviceType("SENSOR");
                                    break;
                                default:
                                    {
                                        Console.WriteLine("OpenZWave Plugin | [Node Label] " + node.Label);
                                        break;
                                    }
                            }
                            if (device_type != null)
                            {
                                device device = GetDevices().FirstOrDefault(d => d.node_id == node.ID);
                                //If we dont already have the device
                                if (device == null)
                                {
                                    device = new device
                                    {
                                        node_id = node.ID,
                                        device_types = device_type,
                                        friendly_name = deviceName
                                    };
                                    zvsEntityControl.zvsContext.devices.AddObject(device);
                                    zvsEntityControl.zvsContext.SaveChanges();
                                    zvsEntityControl.DeviceAdded(this, new EventArgs());
                                }     

                                #region Last Event Value Storeage
                                //Node event value placeholder                               
                                DefineOrUpdateDeviceValue(new device_values
                                {
                                    device_id = device.id,
                                    value_id = LaastEventNameValueId,
                                    label_name = "Last Node Event Value",
                                    genre = "Custom",
                                    index = "0",
                                    type = "Byte",
                                    commandClassId = "0",
                                    value = "0",
                                    read_only = true
                                });
                                
                                #endregion
                            }
                            else
                                WriteToLog(Urgency.WARNING, string.Format("Found unknown device '{0}', node #{1}!", node.Label, node.ID));
                            
                        }
                        break;
                    }

                case ZWNotification.Type.NodeNaming:
                    {
                        string ManufacturerNameValueId = "9999058723211334120";
                        string ProductNameValueId = "9999058723211334121";
                        string NodeLocationValueId = "9999058723211334122";
                        string NodeNameValueId = "9999058723211334123";

                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID);
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID);
                            node.Location = m_manager.GetNodeLocation(m_homeId, node.ID);
                            node.Name = m_manager.GetNodeName(m_homeId, node.ID);
                            
                            device d = GetDevices().FirstOrDefault(o => o.node_id == node.ID);
                            if (d != null)
                            {
                                //lets store the manufacturer name and product name in the values table.   
                                //Giving ManufacturerName a random value_id 9999058723211334120                                                           
                                DefineOrUpdateDeviceValue(new device_values
                                {
                                    device_id = d.id,
                                    value_id = ManufacturerNameValueId,
                                    label_name = "Manufacturer Name",
                                    genre = "Custom",
                                    index = "0",
                                    type = "String",
                                    commandClassId = "0",
                                    value = node.Manufacturer,
                                    read_only = true
                                });                             
                                DefineOrUpdateDeviceValue(new device_values
                                {
                                    device_id = d.id,
                                    value_id = ProductNameValueId,
                                    label_name = "Product Name",
                                    genre = "Custom",
                                    index = "0",
                                    type = "String",
                                    commandClassId = "0",
                                    value = node.Product,
                                    read_only = true
                                });                                                 
                                DefineOrUpdateDeviceValue(new device_values
                                {
                                    device_id = d.id,
                                    value_id = NodeLocationValueId,
                                    label_name = "Node Location",
                                    genre = "Custom",
                                    index = "0",
                                    type = "String",
                                    commandClassId = "0",
                                    value = node.Location,
                                    read_only = true
                                });                                          
                                DefineOrUpdateDeviceValue(new device_values
                                {
                                    device_id = d.id,
                                    value_id = NodeNameValueId,
                                    label_name = "Node Name",
                                    genre = "Custom",
                                    index = "0",
                                    type = "String",
                                    commandClassId = "0",
                                    value = node.Name,
                                    read_only = true
                                });         
                            }   
                            Console.WriteLine("OpenZWave Plugin | [NodeNaming] Node:" + node.ID + ", Product:" + node.Product + ", Manufacturer:" + node.Manufacturer + ")");
                        }
                        break;
                    }                  

                case ZWNotification.Type.NodeEvent:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId()); 
                        byte gevent = m_notification.GetEvent();

                        if (node != null)
                        {
                            WriteToLog(Urgency.INFO, string.Format("[NodeEvent] Node: {0}, Event Byte: {1}", node.ID, gevent));

                            #region Last Event Value Storeage
                            device d = GetDevices().FirstOrDefault(o => o.node_id == node.ID);
                            if (d != null)
                            {
                                //Node event value placeholder
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == LaastEventNameValueId);
                                if (dv != null)
                                {
                                    dv.value = gevent.ToString();
                                    zvsEntityControl.zvsContext.SaveChanges();
                              
                                    //Since events are work differently than values fire the value change event every time we recieve the event regardless if 
                                    //it is the same value or not.
                                    dv.DeviceValueDataChanged("0");
                                }
                            }
                            #endregion
                        }
                        break;
                        
                    }

                case ZWNotification.Type.PollingDisabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [PollingDisabled] Node:" + node.ID);
                        }

                        break;
                    }

                case ZWNotification.Type.PollingEnabled:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            Console.WriteLine("OpenZWave Plugin | [PollingEnabled] Node:" + node.ID);
                        }
                        break;
                    }

                case ZWNotification.Type.DriverReady:
                    {
                        m_homeId = m_notification.GetHomeId();
                        WriteToLog(Urgency.INFO, "Initializing: Driver with Home ID 0x" + m_homeId);
                        Console.WriteLine("OpenZWave Plugin | [DriverReady] Initializing...driver with Home ID 0x" + m_homeId);                        
                        break;
                    }           

                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());                                               

                        if (node != null)
                        {
                            
                            device d = GetDevices().FirstOrDefault(o => o.node_id == node.ID);

                            if (d != null)
                            {
                                d.last_heard_from = DateTime.Now;
                            }
                            zvsEntityControl.zvsContext.SaveChanges();
                                                        
                            WriteToLog(Urgency.INFO, "Initializing: Node " + node.ID + " query complete.");
                            Console.WriteLine("OpenZWave Plugin | [NodeQueriesComplete] Initializing...node " + node.ID + " query complete.");
                        }

                        break;
                    }                    

                case ZWNotification.Type.AllNodesQueried:
                    {
                        foreach (Node n in m_nodeList)
                        {
                            device d = GetDevices().FirstOrDefault(o => o.node_id == n.ID); 
                            
                            if (d != null)
                            {
                                if (device_property_values.GetDevicePropertyValue(zvsEntityControl.zvsContext,d.id, "ENABLEPOLLING").ToUpper().Equals("TRUE"))
                                    EnablePolling(n.ID);
                            }                           
                        }

                        WriteToLog(Urgency.INFO, "Ready:  All nodes queried. Plug-in now ready.");
                        IsReady = true;
                        
                        FinishedInitialPoll = true;
                        break;
                    }

                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        WriteToLog(Urgency.INFO, "Ready:  Awake nodes queried (but not some sleeping nodes).");
                        Console.WriteLine("OpenZWave Plugin | Ready:  Awake nodes queried (but not some sleeping nodes).");
                        break;
                    }                   
            }
        }

        string GetValue(ZWValueID v)
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
                    Int32 r4;
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
                        //todo:  put some decent values in here
                        break;
                    case "Secure Keypad Door Lock":
                    case "Advanced Door Lock":
                    case "Door Lock":
                    case "Entry Control":
                        foreach (Value v in n.Values)
                        {
                            //todo:  put some decent values in here
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
                             //todo:  put some decent values in here
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
