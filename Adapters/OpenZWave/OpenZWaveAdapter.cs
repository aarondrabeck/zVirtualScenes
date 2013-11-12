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
using zvs.Processor;
using System.Diagnostics;
using zvs.Entities;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Data.Entity;

namespace OpenZWavePlugin
{
    [Export(typeof(zvsAdapter))]
    public class OpenZWaveAdapter : zvsAdapter
    {
        private async void OpenZWaveAdapter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                await StopOpenzwaveAsync();
                await StartOpenzwaveAsync();
            }
        }

        public override Guid AdapterGuid
        {
            get { return Guid.Parse("70f91ca6-08bb-406a-a60f-aeb13f50aae8"); }
        }

        public override string Name
        {
            get { return "OpenZWave Adapter for ZVS"; }
        }

        public override string Description
        {
            get { return "This adapter provides OpenZWave functionality in zVirtualScenes using the OpenZWave open-source project."; }
        }

        private enum OpenzWaveDeviceTypes
        {
            UNKNOWN,
            CONTROLLER,
            SWITCH,
            DIMMER,
            THERMOSTAT,
            DOORLOCK,
            SENSOR
        }

        private enum OpenzWaveDeviceTypeSettings
        {
            DEFAULT_DIMMER_ON_LEVEL,
            ENABLE_REPOLL_ON_LEVEL_CHANGE,
            REPOLLING_ENABLED
        }

        private int ControllerTypeId = 0;
        private int SwitchTypeId = 0;
        private int DimmerTypeId = 0;
        private int ThermoTypeId = 0;
        private int UnknownTypeId = 0;
        private int LockTypeId = 0;
        private int SensorTypeId = 0;

        public override async Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
            //Controller Type Devices
            DeviceType controller_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.CONTROLLER.ToString(), Name = "OpenZWave Controller", ShowInList = true };
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RESET", Name = "Reset Controller", ArgumentType = DataType.NONE, Description = "Erases all Z-Wave network settings from your controller. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ADDDEVICE", Name = "Add Device to Network", ArgumentType = DataType.NONE, Description = "Adds a ZWave Device to your network. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "AddController", Name = "Add Controller to Network", ArgumentType = DataType.NONE, Description = "Adds a ZWave Controller to your network. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "CreateNewPrimary", Name = "Create New Primary", ArgumentType = DataType.NONE, Description = "Puts the target controller into receive configuration mode. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ReceiveConfiguration", Name = "Receive Configuration", ArgumentType = DataType.NONE, Description = "Receives the network configuration from another controller. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveController", Name = "Remove Controller", ArgumentType = DataType.NONE, Description = "Removes a Controller from your network. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveDevice", Name = "Remove Device", ArgumentType = DataType.NONE, Description = "Removes a Device from your network. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TransferPrimaryRole", Name = "Transfer Primary Role", ArgumentType = DataType.NONE, Description = "Transfers the primary role to another controller. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "HasNodeFailed", Name = "Has Node Failed", ArgumentType = DataType.NONE, Description = "Tests whether a node has failed. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "RemoveFailedNode", Name = "Remove Failed Node", ArgumentType = DataType.NONE, Description = "Removes the failed node from the controller's list. Argument2 = DeviceId." });
            controller_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "ReplaceFailedNode", Name = "Replace Failed Node", ArgumentType = DataType.NONE, Description = "Tests the failed node. Argument2 = DeviceId." });
            ControllerTypeId = await deviceTypeBuilder.RegisterAsync(controller_dt);

            //Switch Type Devices
            DeviceType switch_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.SWITCH.ToString(), Name = "OpenZWave Binary", ShowInList = true };
            switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a switch. Argument2 = DeviceId." });
            switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a switch. Argument2 = DeviceId." });
            switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "MOMENTARY", Name = "Turn On for X milliseconds", ArgumentType = DataType.INTEGER, Description = "Turns a device on for the specified number of milliseconds and then turns the device back off. Argument2 = DeviceId." });
            SwitchTypeId = await deviceTypeBuilder.RegisterAsync(switch_dt);

            //Dimmer Type Devices
            DeviceType dimmer_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.DIMMER.ToString(), Name = "OpenZWave Dimmer", ShowInList = true };
            dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a dimmer. Argument2 = DeviceId." });
            dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a dimmer. Argument2 = DeviceId." });

            DeviceTypeCommand dimmer_preset_cmd = new DeviceTypeCommand { UniqueIdentifier = "SETPRESETLEVEL", Name = "Set Level", ArgumentType = DataType.LIST, Description = "Sets a dimmer to a preset level. Argument2 = DeviceId." };
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "0%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "20%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "40%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "60%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "80%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "100%" });
            dimmer_preset_cmd.Options.Add(new CommandOption { Name = "255" });
            dimmer_dt.Commands.Add(dimmer_preset_cmd);
            DimmerTypeId = await deviceTypeBuilder.RegisterAsync(dimmer_dt);

            //Thermostat Type Devices
            DeviceType thermo_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.THERMOSTAT.ToString(), Name = "OpenZWave Thermostat", ShowInList = true };
            thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETENERGYMODE", Name = "Set Energy Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Energy Mode. Argument2 = DeviceId." });
            thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETCONFORTMODE", Name = "Set Comfort Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Comfort Mode. (Run) Argument2 = DeviceId." });
            ThermoTypeId = await deviceTypeBuilder.RegisterAsync(thermo_dt);

            DeviceType unknwon_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.UNKNOWN.ToString(), Name = "OpenZWave Unknown", ShowInList = true };
            UnknownTypeId = await deviceTypeBuilder.RegisterAsync(unknwon_dt);

            //Door Lock Type Devices
            DeviceType lock_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.DOORLOCK.ToString(), Name = "OpenZWave Door lock", ShowInList = true };
            LockTypeId = await deviceTypeBuilder.RegisterAsync(lock_dt);

            //Sensors
            DeviceType sensor_dt = new DeviceType { UniqueIdentifier = OpenzWaveDeviceTypes.SENSOR.ToString(), Name = "OpenZWave Sensor", ShowInList = true };
            SensorTypeId = await deviceTypeBuilder.RegisterAsync(sensor_dt);
        }

        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var comSetting = new AdapterSetting
             {
                 Name = "Com Port",
                 Value = (3).ToString(),
                 ValueType = DataType.COMPORT,
                 Description = "The COM port that your z-wave controller is assigned to."
             };

            var useHIDsetting = new AdapterSetting
           {
               Name = "Use HID",
               Value = false.ToString(),
               ValueType = DataType.BOOL,
               Description = "Use HID rather than COM port. (use this for ControlThink Sticks)"
           };
            var pollIntSetting = new AdapterSetting
           {
               Name = "Polling interval",
               Value = (360).ToString(),
               ValueType = DataType.INTEGER,
               Description = "The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network traffic. "
           };

            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(comSetting, o => o.ComportSetting);
            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(useHIDsetting, o => o.UseHIDSetting);
            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(pollIntSetting, o => o.PollingIntervalSetting);

            using (zvsContext context = new zvsContext())
            {
                await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
                {
                    UniqueIdentifier = OpenzWaveDeviceTypeSettings.DEFAULT_DIMMER_ON_LEVEL.ToString(),
                    DeviceTypeId = DimmerTypeId,
                    Name = "Default Level",
                    Description = "Level that an device is set to when using the 'ON' command.",
                    Value = "99",//default value
                    ValueType = DataType.BYTE
                });

                await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
                {
                    UniqueIdentifier = OpenzWaveDeviceTypeSettings.ENABLE_REPOLL_ON_LEVEL_CHANGE.ToString(),
                    DeviceTypeId = DimmerTypeId,
                    Name = "Enable re-poll on level change",
                    Description = "Re-poll dimmers 3 seconds after a level change is received?",
                    Value = true.ToString(), //default value
                    ValueType = DataType.BOOL
                });

                await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
                {
                    UniqueIdentifier = OpenzWaveDeviceTypeSettings.REPOLLING_ENABLED.ToString(),
                    DeviceTypeId = DimmerTypeId,
                    Name = "Enable polling for this device",
                    Description = "Toggles automatic polling for a device.",
                    Value = false.ToString(), //default value
                    ValueType = DataType.BOOL
                });
            }
           
        }

        public override async Task StartAsync()
        {
            await StartOpenzwaveAsync();
        }

        public override async Task StopAsync()
        {
            await StopOpenzwaveAsync();
        }

        //Settings Cache 
        private bool _UseHIDSetting = false;
        public bool UseHIDSetting
        {
            get { return _UseHIDSetting; }
            set
            {
                if (value != _UseHIDSetting)
                {
                    _UseHIDSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ComportSetting = "10";
        public string ComportSetting
        {
            get { return _ComportSetting; }
            set
            {
                if (value != _ComportSetting)
                {
                    _ComportSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _PollingIntervalSetting = 0;
        public int PollingIntervalSetting
        {
            get { return _PollingIntervalSetting; }
            set
            {
                if (value != _PollingIntervalSetting)
                {
                    _PollingIntervalSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool InitialPollingComplete = false;

        //OpenzWave Data
        private ZWManager m_manager = null;
        private ZWOptions m_options = null;
        ZWNotification m_notification = null;
        UInt32 m_homeId = 0;
        List<Node> m_nodeList = new List<Node>();
        private string LastEventNameValueId = "LEN1";
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<OpenZWaveAdapter>();

        private bool isShuttingDown = false;
        private HashSet<byte> NodesReady = new HashSet<byte>();

        private int FindDeviceTypeId(string nodeLabel)
        {
            switch (nodeLabel)
            {
                case "Toggle Switch":
                case "Binary Toggle Switch":
                case "Binary Switch":
                case "Binary Power Switch":
                case "Binary Scene Switch":
                case "Binary Toggle Remote Switch":
                    return SwitchTypeId;
                case "Multilevel Toggle Remote Switch":
                case "Multilevel Remote Switch":
                case "Multilevel Toggle Switch":
                case "Multilevel Switch":
                case "Multilevel Power Switch":
                case "Multilevel Scene Switch":
                    return DimmerTypeId;
                case "Multiposition Motor":
                case "Motor Control Class A":
                case "Motor Control Class B":
                case "Motor Control Class C":
                    return DimmerTypeId;
                case "General Thermostat V2":
                case "Heating Thermostat":
                case "General Thermostat":
                case "Setback Schedule Thermostat":
                case "Setpoint Thermostat":
                case "Setback Thermostat":
                case "Thermostat":
                    return ThermoTypeId;
                case "Remote Controller":
                case "Static PC Controller":
                case "Static Controller":
                case "Portable Remote Controller":
                case "Portable Installer Tool":
                case "Static Scene Controller":
                case "Static Installer Tool":
                    return ControllerTypeId;
                case "Secure Keypad Door Lock":
                case "Advanced Door Lock":
                case "Door Lock":
                case "Entry Control":
                    return LockTypeId;
                case "Alarm Sensor":
                case "Basic Routing Alarm Sensor":
                case "Routing Alarm Sensor":
                case "Basic Sensor Alarm Sensor":
                case "Sensor Alarm Sensor":
                case "Advanced Sensor Alarm Sensor":
                case "Basic Routing Smoke Sensor":
                case "Routing Smoke Sensor":
                case "Basic Sensor Smoke Sensor":
                case "Sensor Smoke Sensor":
                case "Advanced Sensor Smoke Sensor":
                case "Routing Binary Sensor":
                case "Routing Multilevel Sensor":
                    return SensorTypeId;
                default:
                    {
                        return UnknownTypeId;
                    }
            }
        }

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

        private Task StartOpenzwaveAsync()
        {
            if (isShuttingDown)
            {
                Core.log.InfoFormat("{0} driver cannot start because it is still shutting down", this.Name);
                return Task.FromResult(0);
            }

            this.PropertyChanged += OpenZWaveAdapter_PropertyChanged;
            try
            {
                Core.log.InfoFormat("OpenZwave driver starting on {0}", UseHIDSetting ? "HID" : "COM" + ComportSetting);

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

                if (!UseHIDSetting)
                {
                    if (ComportSetting != "0")
                    {
                        m_manager.AddDriver(@"\\.\COM" + ComportSetting);
                    }
                }
                else
                {
                    m_manager.AddDriver("HID Controller", ZWControllerInterface.Hid);
                }


                if (PollingIntervalSetting != 0)
                {
                    m_manager.SetPollInterval(PollingIntervalSetting, true);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }

            return Task.FromResult(0);
        }

        private async Task StopOpenzwaveAsync()
        {
            if (!isShuttingDown)
            {
                this.PropertyChanged -= OpenZWaveAdapter_PropertyChanged;

                isShuttingDown = true;
                InitialPollingComplete = false;

                await Task.Run(() =>
                {
                    //EKK this is blocking and can be slow
                    if (m_manager != null)
                    {
                        m_manager.OnNotification -= NotificationHandler;
                        m_manager.RemoveDriver(@"\\.\COM" + ComportSetting);
                        m_manager.Destroy();
                        m_manager = null;
                    }

                    if (m_options != null)
                    {
                        m_options.Destroy();
                        m_options = null;
                    }
                });

                isShuttingDown = false;
                Core.log.Info("OpenZwave driver stopped");
            }
        }
        public override async Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument)
        {
            var nodeNumber = Convert.ToByte(device.NodeNumber);
            if (!isNodeReady(nodeNumber))
            {
                log.ErrorFormat("Failed to issue command on {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                return;
            }

            if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.CONTROLLER.ToString())
            {
                #region Controller Commands
                switch (command.UniqueIdentifier)
                {
                    case "RESET":
                        {
                            m_manager.ResetController(m_homeId);
                            break;
                        }
                    case "ADDDEVICE":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.AddDevice, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }

                    case "CreateNewPrimary":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.CreateNewPrimary, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReceiveConfiguration":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReceiveConfiguration, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }

                    case "RemoveDevice":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveDevice, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "TransferPrimaryRole":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.TransferPrimaryRole, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "HasNodeFailed":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.HasNodeFailed, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "RemoveFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.RemoveFailedNode, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReplaceFailedNode":
                        {
                            ControllerCommandDlg dlg = new ControllerCommandDlg(m_manager, m_homeId, ZWControllerCommand.ReplaceFailedNode, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.SWITCH.ToString())
            {
                #region Switch command handeling
                switch (command.UniqueIdentifier)
                {
                    case "MOMENTARY":
                        {
                            int delay = 1000;
                            int.TryParse(argument, out delay);
                            byte nodeID = (byte)device.NodeNumber;

                            m_manager.SetNodeOn(m_homeId, nodeID);
                            await Task.Delay(delay);
                            m_manager.SetNodeOff(m_homeId, nodeID);

                            break;

                        }
                    case "TURNON":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.DIMMER.ToString())
            {
                #region Dimmer command handling
                switch (command.UniqueIdentifier)
                {
                    case "TURNON":
                        {
                            using (var context = new zvsContext())
                            {
                                var value = await device.GetDeviceTypeValueAsync(OpenzWaveDeviceTypeSettings.DEFAULT_DIMMER_ON_LEVEL.ToString(), context);

                                if (value != null)
                                {
                                    byte bValue = byte.TryParse(value, out bValue) ? bValue : (byte)99;
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, bValue);
                                    break;
                                }
                            }

                            m_manager.SetNodeOn(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "TURNOFF":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "SETPRESETLEVEL":
                        {
                            switch (argument)
                            {
                                case "0%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(0));
                                    break;
                                case "20%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(20));
                                    break;
                                case "40%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(40));
                                    break;
                                case "60%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(60));
                                    break;
                                case "80%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(80));
                                    break;
                                case "100%":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(100));
                                    break;
                                case "255":
                                    m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, Convert.ToByte(255));
                                    break;
                            }
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.THERMOSTAT.ToString())
            {
                #region Thermostat Command Handling
                switch (command.UniqueIdentifier)
                {
                    case "SETENERGYMODE":
                        {
                            m_manager.SetNodeOff(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "SETCONFORTMODE":
                        {
                            m_manager.SetNodeOn(m_homeId, (byte)device.NodeNumber);
                            break;
                        }
                }
                #endregion
            }
        }

        public override Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2)
        {
            DeviceCommand deviceTypeCommand = (DeviceCommand)command;

            if (command.UniqueIdentifier.Contains("DYNAMIC_CMD_"))
            {
                var nodeNumber = Convert.ToByte(device.NodeNumber);

                //Get more info from this Node from OpenZWave
                Node node = GetNode(m_homeId, nodeNumber);

                if (!isNodeReady(nodeNumber))
                {
                    log.ErrorFormat("Failed to issue command on {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                    return Task.FromResult(0);
                }

                switch (command.ArgumentType)
                {
                    case DataType.BYTE:
                        {
                            byte b = 0;
                            byte.TryParse(argument, out b);

                            var Value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString().Equals(command.CustomData2));
                            if (Value != null)
                                m_manager.SetValue(Value.ValueID, b);
                            break;
                        }
                    case DataType.BOOL:
                        {
                            bool b = true;
                            bool.TryParse(argument, out b);

                            var Value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString().Equals(command.CustomData2));
                            if (Value != null)
                                m_manager.SetValue(Value.ValueID, b);
                            break;
                        }
                    case DataType.DECIMAL:
                        {
                            float f = Convert.ToSingle(argument);

                            var Value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString().Equals(command.CustomData2));
                            if (Value != null)
                                m_manager.SetValue(Value.ValueID, f);
                            break;
                        }
                    case DataType.LIST:
                    case DataType.STRING:
                        {
                            var Value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString().Equals(command.CustomData2));
                            if (Value != null)
                                m_manager.SetValue(Value.ValueID, argument);
                            break;
                        }
                    case DataType.INTEGER:
                        {
                            int i = 0;
                            int.TryParse(argument, out i);

                            var Value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString().Equals(command.CustomData2));
                            if (Value != null)
                                m_manager.SetValue(Value.ValueID, i);
                            break;
                        }
                }
            }
            return Task.FromResult(0);
        }

        public override Task RepollAsync(Device device, zvsContext context)
        {
            var nodeNumber = Convert.ToByte(device.NodeNumber);

            if (!isNodeReady(nodeNumber))
            {
                log.ErrorFormat("Re-poll node {0} failed, node not ready.", nodeNumber);
                return Task.FromResult(0);
            }

            m_manager.RequestNodeState(m_homeId, nodeNumber);
            return Task.FromResult(0);
        }

        public override async Task ActivateGroupAsync(Group group, zvsContext context)
        {
            var devices = await context.Devices
                .Include(d => d.Type)
                .Where(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid)
                .Where(o => o.Groups.Any(g => g.Id == group.Id))
                .ToListAsync();

            foreach (var device in devices)
            {
                var nodeNumber = Convert.ToByte(device.NodeNumber);
                if (!isNodeReady(nodeNumber))
                {
                    log.ErrorFormat("Failed to activate group member {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                    continue;
                }

                if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.DIMMER.ToString())
                {
                    var value = await device.GetDeviceTypeValueAsync(OpenzWaveDeviceTypeSettings.DEFAULT_DIMMER_ON_LEVEL.ToString(), context);

                    if (value != null)
                    {
                        byte bValue = byte.TryParse(value, out bValue) ? bValue : (byte)99;
                        m_manager.SetNodeLevel(m_homeId, (byte)device.NodeNumber, bValue);
                        continue;
                    }
                }

                m_manager.SetNodeOn(m_homeId, nodeNumber);
            }
        }

        public override async Task DeactivateGroupAsync(Group group, zvsContext context)
        {
            var devices = await context.Devices
                .Where(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid)
                .Where(o => o.Groups.Any(g => g.Id == group.Id))
                .ToListAsync();

            foreach (var device in devices)
            {
                var nodeNumber = Convert.ToByte(device.NodeNumber);
                if (!isNodeReady(nodeNumber))
                {
                    log.ErrorFormat("Failed to deactivate group member {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                    continue;
                }

                m_manager.SetNodeOff(m_homeId, nodeNumber);
            }
        }

        private bool isNodeReady(byte NodeId)
        {
            return NodesReady.Contains(NodeId);
        }

        private async Task AddNewDeviceToDatabase(byte NodeId)
        {
            #region Add device to database

            using (zvsContext context = new zvsContext())
            {
                Device ozw_device = await context.Devices
                    .FirstOrDefaultAsync(d => d.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                        d.NodeNumber == NodeId);

                //If already have the device, don't install a duplicate
                if (ozw_device != null)
                    return;

                ozw_device = new Device
                {
                    NodeNumber = NodeId,
                    DeviceTypeId = UnknownTypeId,
                    Name = "Unknown OpenZwave Device",
                    CurrentLevelInt = 0,
                    CurrentLevelText = ""
                };

                context.Devices.Add(ozw_device);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
            #endregion
        }

        private DataType TranslateDataType(ZWValueID.ValueType type)
        {
            //Set parameter types for command
            switch (type)
            {
                case ZWValueID.ValueType.List:
                    return DataType.LIST;
                case ZWValueID.ValueType.Byte:
                    return DataType.BYTE;
                case ZWValueID.ValueType.Decimal:
                    return DataType.DECIMAL;
                case ZWValueID.ValueType.Int:
                    return DataType.INTEGER;
                case ZWValueID.ValueType.String:
                    return DataType.STRING;
                case ZWValueID.ValueType.Short:
                    return DataType.SHORT;
                case ZWValueID.ValueType.Bool:
                    return DataType.BOOL;
                default:
                    return DataType.NONE;
            }
        }

        private int EvaluateOrder(string Genre)
        {
            switch (Genre)
            {
                case "User":
                    return 1;
                case "Config":
                    return 2;
                default:
                    return 99;
            }
        }

        #region OpenZWave interface

        public void NotificationHandler(ZWNotification notification)
        {
            m_notification = notification;
            NotificationHandler();
            m_notification = null;
        }

        private HashSet<int> NodeValuesRepolling = new HashSet<int>();
        private async void NotificationHandler()
        {
            switch (m_notification.GetType())
            {

                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        #region NodeProtocolInfo

                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                            Debug.WriteLine("[Node Protocol Info] " + node.Label);

                            //Find device type
                            var deviceTypeId = FindDeviceTypeId(node.Label);
                            if (deviceTypeId == UnknownTypeId)
                                log.Warn("[Unknown Node Label] " + node.Label);

                            using (zvsContext context = new zvsContext())
                            {
                                Device ozw_device = await context.Devices
                                    .FirstOrDefaultAsync(d => d.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                        d.NodeNumber == node.ID);

                                //If we don't already have the device
                                if (ozw_device == null)
                                    break;

                                if (ozw_device.DeviceTypeId != deviceTypeId)
                                {
                                    ozw_device.DeviceTypeId = deviceTypeId;

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        Core.log.Error(result.Message);
                                }

                                #region Last Event Value Storage
                                //Node event value placeholder 
                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = ozw_device.Id,
                                    UniqueIdentifier = LastEventNameValueId,
                                    Name = "Last Node Event Value",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.BYTE,
                                    CommandClass = "0",
                                    Value = "0",
                                    isReadOnly = true
                                }, ozw_device, context);
                                #endregion
                            }
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.ValueAdded:
                    {
                        #region ValueAdded

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
                        var vIdString = vid.GetId().ToString();

#if DEBUG
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
#endif

                        string data = "";
                        bool b = m_manager.GetValueAsString(vid, out data);

                        Debug.WriteLine("[ValueAdded] Node: {0}, Label: {1}, Data: {2}, result: {3}",
                            node.ID,
                            value.Label,
                            data,
                            b.ToString());

                        using (zvsContext context = new zvsContext())
                        {
                            Device d = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                o.NodeNumber == node.ID);

                            if (d == null)
                            {
                                log.Warn("ValueAdded called on a node id that was not found in the database");
                                break;
                            }

                            //Values are 'unknown' at this point so don't report a value change. 
                            await DeviceValueBuilder.RegisterAsync(new DeviceValue
                            {
                                DeviceId = d.Id,
                                UniqueIdentifier = vIdString,
                                Name = value.Label,
                                Genre = value.Genre,
                                Index = value.Index,
                                CommandClass = value.CommandClassID,
                                Value = data,
                                ValueType = ConvertType(vid),
                                isReadOnly = read_only
                            }, d, context, true);

                            #region Install Dynamic Commands

                            if (!read_only || !string.IsNullOrEmpty(value.Label))
                            {
                                DataType pType = TranslateDataType(vid.GetType());

                                DeviceCommand dynamic_dc = new DeviceCommand
                                {
                                    Device = d,
                                    DeviceId = d.Id,
                                    UniqueIdentifier = string.Format("DYNAMIC_CMD_{0}_{1}", value.Label.ToUpper(), vid.GetId().ToString()),
                                    Name = string.Format("Set {0}", value.Label),
                                    ArgumentType = pType,
                                    Help = string.IsNullOrEmpty(value.Help) ? string.Empty : value.Help,
                                    CustomData1 = string.IsNullOrEmpty(value.Label) ? string.Empty : value.Label,
                                    CustomData2 = string.IsNullOrEmpty(vIdString) ? string.Empty : vIdString,
                                    SortOrder = EvaluateOrder(value.Genre)
                                };

                                //Special case for lists add additional info
                                if (pType == DataType.LIST)
                                {
                                    //Install the allowed options/values
                                    String[] options;
                                    if (m_manager.GetValueListItems(vid, out options))
                                        foreach (string option in options)
                                            dynamic_dc.Options.Add(new CommandOption { Name = option });
                                }

                                await DeviceCommandBuilder.RegisterAsync(dynamic_dc, context);

                            }
                            #endregion
                        }
#if DEBUG
                        sw.Stop();
                        Debug.WriteLine("Added new Openzwave Value in {0}", sw.Elapsed.ToString() as object);
#endif
                        break;
                        #endregion
                    }
                case ZWNotification.Type.ValueRemoved:
                    {
                        #region ValueRemoved

                        try
                        {
                            Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                            ZWValueID vid = m_notification.GetValueID();
                            Value val = node.GetValue(vid);

                            Debug.WriteLine("[ValueRemoved] Node:" + node.ID + ",Label:" + m_manager.GetValueLabel(vid));

                            node.RemoveValue(val);
                            //TODO: Remove from values and command table
                        }
                        catch (Exception ex)
                        {
                            log.Error("ValueRemoved error: " + ex.Message);
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.ValueChanged:
                    {
                        #region ValueChanged
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

                        Debug.WriteLine("[ValueChanged] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);

                        using (zvsContext context = new zvsContext())
                        {
                            Device device = await context.Devices
                                .Include(o => o.Type)
                                .FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                    o.NodeNumber == node.ID);

                            if (device == null)
                            {
                                log.Warn("ValueChanged called on a node id that was not found in the database");
                                break;
                            }

                            //Update device value
                            await DeviceValueBuilder.RegisterAsync(new DeviceValue
                            {
                                DeviceId = device.Id,
                                UniqueIdentifier = vid.GetId().ToString(),
                                Name = value.Label,
                                Genre = value.Genre,
                                Index = value.Index,
                                CommandClass = value.CommandClassID,
                                Value = data,
                                ValueType = ConvertType(vid),
                                isReadOnly = read_only
                            }, device, context);

                            #region Update Device Status Properties
                            //Update Current Status Field
                            var changed = false;
                            if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.THERMOSTAT.ToString())
                            {
                                if (value.Label == "Temperature")
                                {
                                    double level = 0;
                                    double.TryParse(data, out level);
                                    var levelTxt = string.Format("{0}° F", level);

                                    if (device.CurrentLevelInt != level)
                                    {
                                        device.CurrentLevelInt = level;
                                        changed = true;
                                    }

                                    if (device.CurrentLevelText != levelTxt)
                                    {
                                        device.CurrentLevelText = levelTxt;
                                        changed = true;
                                    }
                                }
                            }
                            else if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.SWITCH.ToString())
                            {
                                if (value.Label == "Basic")
                                {
                                    double level = 0;
                                    if (double.TryParse(data, out level))
                                    {
                                        var levelOnOff = level > 0 ? 100 : 0;
                                        var leveltxt = level > 0 ? "On" : "Off";

                                        if (device.CurrentLevelInt != levelOnOff)
                                        {
                                            device.CurrentLevelInt = levelOnOff;
                                            changed = true;
                                        }

                                        if (device.CurrentLevelText != leveltxt)
                                        {
                                            device.CurrentLevelText = leveltxt;
                                            changed = true;
                                        }
                                    }
                                }
                                else if (value.Label == "Switch" || value.Label == "Level") //Some Intermatic devices do not set basic when changing status
                                {
                                    bool state = false;
                                    if (bool.TryParse(data, out state))
                                    {
                                        var levelOnOff = state ? 100 : 0;
                                        var leveltxt = state ? "On" : "Off";

                                        if (device.CurrentLevelInt != levelOnOff)
                                        {
                                            device.CurrentLevelInt = levelOnOff;
                                            changed = true;
                                        }

                                        if (device.CurrentLevelText != leveltxt)
                                        {
                                            device.CurrentLevelText = leveltxt;
                                            changed = true;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (value.Label == "Basic")
                                {
                                    double level = 0;
                                    double.TryParse(data, out level);
                                    var levelInt = (int)level;
                                    var levelTxt = level + "%";

                                    if (device.CurrentLevelInt != levelInt)
                                    {
                                        device.CurrentLevelInt = levelInt;
                                        changed = true;
                                    }

                                    if (device.CurrentLevelText != levelTxt)
                                    {
                                        device.CurrentLevelText = levelTxt;
                                        changed = true;
                                    }
                                }
                            }

                            if (changed)
                            {
                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    Core.log.Error(result.Message);
                            }
                            #endregion

                            #region Update Device Commands
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

                                var vidId = vid.GetId().ToString();
                                var dc = await context.DeviceCommands.FirstOrDefaultAsync(o => o.DeviceId == device.Id &&
                                    o.CustomData2 == vidId);

                                if (dc != null)
                                {
                                    //After Value is Added, Value Name other values properties can change so update.
                                    dc.Name = "Set " + value.Label;
                                    dc.Help = value.Help;
                                    dc.CustomData1 = value.Label;
                                    dc.SortOrder = order;
                                }
                            }
                            #endregion

                            #region Repoll Dimmers

                            //Some dimmers take x number of seconds to dim to desired level.  Therefore the level received here initially is a 
                            //level between old level and new level. (if going from 0 to 100 we get 84 here).
                            //To get the real level re-poll the device a second or two after a level change was received.     
                            bool EnableDimmerRepoll = bool.TryParse(await device.GetDeviceSettingAsync(OpenzWaveDeviceTypeSettings.ENABLE_REPOLL_ON_LEVEL_CHANGE.ToString(), context), out EnableDimmerRepoll) ? EnableDimmerRepoll : false;

                            if (InitialPollingComplete &&
                                EnableDimmerRepoll &&
                                device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.DIMMER.ToString() &&
                                value.Label == "Basic")
                            {
                                //only allow each device to re-poll 1 time.
                                if (!NodeValuesRepolling.Contains(device.NodeNumber))
                                {
                                    NodeValuesRepolling.Add(device.NodeNumber);

                                    await Task.Delay(3500);
                                    m_manager.RefreshValue(vid);
                                    Debug.WriteLine(string.Format("Node {0} value re-polled", device.NodeNumber));

                                    //Do not allow another re-poll for 10 seconds
                                    await Task.Delay(10000);
                                    NodeValuesRepolling.Remove(device.NodeNumber);
                                }
                            }
                            #endregion

                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.Group:
                    {
                        #region Group
                        Debug.WriteLine("[Group]"); ;
                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeAdded:
                    {
                        #region NodeAdded
                        // if this node was in zwcfg*.xml, this is the first node notification
                        // if not, the NodeNew notification should already have been received
                        //if (GetNode(m_notification.GetHomeId(), m_notification.GetNodeId()) == null)
                        //{
                        Node node = new Node();
                        node.ID = m_notification.GetNodeId();
                        node.HomeID = m_notification.GetHomeId();
                        m_nodeList.Add(node);

                        Debug.WriteLine("[NodeAdded] ID:" + node.ID.ToString() + " Added");
                        await AddNewDeviceToDatabase(node.ID);

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeNew:
                    {
                        #region NodeNew
                        // Add the new node to our list (and flag as uninitialized)
                        Node node = new Node();
                        node.ID = m_notification.GetNodeId();
                        node.HomeID = m_notification.GetHomeId();
                        m_nodeList.Add(node);

                        Debug.WriteLine("[NodeNew] ID:" + node.ID.ToString() + " Added");
                        await AddNewDeviceToDatabase(node.ID);

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeRemoved:
                    {
                        #region NodeRemoved
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == m_notification.GetNodeId())
                            {
                                Debug.WriteLine("[NodeRemoved] ID:" + node.ID.ToString());
                                m_nodeList.Remove(node);
                                break;
                            }
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeNaming:
                    {
                        #region NodeNaming
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

                            Debug.WriteLine("[NodeNaming] Node:" + node.ID + ", Product:" + node.Product + ", Manufacturer:" + node.Manufacturer + ")");

                            using (zvsContext context = new zvsContext())
                            {
                                Device device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                    o.NodeNumber == node.ID);

                                if (device == null)
                                {
                                    log.Warn("NodeNaming called on a node id that was not found in the database");
                                    break;
                                }

                                //lets store the manufacturer name and product name in the values table.   
                                //Giving ManufacturerName a random value_id 9999058723211334120                                                           
                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = ManufacturerNameValueId,
                                    Name = "Manufacturer Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Manufacturer,
                                    isReadOnly = true
                                }, device, context);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = ProductNameValueId,
                                    Name = "Product Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Product,
                                    isReadOnly = true
                                }, device, context);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = NodeLocationValueId,
                                    Name = "Node Location",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Location,
                                    isReadOnly = true
                                }, device, context);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = NodeNameValueId,
                                    Name = "Node Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Name,
                                    isReadOnly = true
                                }, device, context);
                            }
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeEvent:
                    {
                        #region NodeEvent
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        byte gevent = m_notification.GetEvent();

                        if (node == null)
                            break;

                        log.Info(string.Format("[NodeEvent] Node: {0}, Event Byte: {1}", node.ID, gevent));

                        using (zvsContext context = new zvsContext())
                        {
                            Device device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                    o.NodeNumber == node.ID);

                            if (device == null)
                            {
                                log.Warn("NodeNaming called on a node id that was not found in the database");
                                break;
                            }

                            var dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == device.Id &&
                                o.UniqueIdentifier == LastEventNameValueId);

                            //Node event value placeholder
                            if (dv == null)
                                break;

                            dv.Value = gevent.ToString();

                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                Core.log.Error(result.Message);

                            //Since open wave events are differently than values changes, we need to fire the value change event every time we receive the 
                            //event regardless if it is the same value or not.
                            dv.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(dv.Id, dv.Value, string.Empty));
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.DriverReady:
                    {
                        #region DriverReady
                        NodesReady.Clear();

                        m_homeId = m_notification.GetHomeId();
                        log.InfoFormat("Initializing...driver with Home ID 0x{0} is ready.", m_homeId.ToString("X8"));

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        #region NodeQueriesComplete
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            log.InfoFormat("Initializing...node {0} queries complete", node.ID);

                            if (!NodesReady.Contains(node.ID))
                                NodesReady.Add(node.ID);

                            //await UpdateLastHeardFrom(node.ID);
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                    {
                        #region EssentialNodeQueriesComplete
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            log.InfoFormat("Initializing...node {0} essential queries complete", node.ID);

                            if (!NodesReady.Contains(node.ID))
                                NodesReady.Add(node.ID);

                            //await UpdateLastHeardFrom(node.ID);
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.AllNodesQueried:
                    {
                        #region AllNodesQueried
                        //This is an important message to see.  It tells you that you can start issuing commands
                        log.Info("Ready:  All nodes queried");
                        InitialPollingComplete = true;
                        m_manager.WriteConfig(m_notification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.AllNodesQueriedSomeDead:
                    {
                        #region AllNodesQueriedSomeDead
                        //This is an important message to see.  It tells you that you can start issuing commands
                        log.Info("Ready:  All nodes queried but some are dead.");
                        InitialPollingComplete = true;
                        m_manager.WriteConfig(m_notification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        #region AwakeNodesQueried
                        log.Info("Ready:  Awake nodes queried (but not some sleeping nodes)");
                        InitialPollingComplete = true;
                        m_manager.WriteConfig(m_notification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.PollingDisabled:
                    {
                        #region PollingDisabled
                        log.Info("Polling disabled notification");
                        break;
                        #endregion
                    }
                case ZWNotification.Type.PollingEnabled:
                    {
                        #region PollingEnabled
                        log.Info("Polling enabled notification");
                        break;
                        #endregion
                    }
                case ZWNotification.Type.SceneEvent:
                    {
                        #region SceneEvent
                        log.Info("Scene event notification received");
                        break;
                        #endregion
                    }
            }
        }

        private async Task EnablePollingOnDevices()
        {
            foreach (Node n in m_nodeList)
                using (zvsContext context = new zvsContext())
                {
                    Device device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                    o.NodeNumber == n.ID);

                    if (device == null)
                    {
                        log.Warn("EnablePollingOnDevices called on a node id that was not found in the database");
                        continue;
                    }

                    bool EnableRepoll = bool.TryParse(await device.GetDeviceSettingAsync(OpenzWaveDeviceTypeSettings.REPOLLING_ENABLED.ToString(), context), out EnableRepoll) ? EnableRepoll : false;

                    if (EnableRepoll)
                        EnablePolling(n.ID);
                }
        }

        //private async Task UpdateLastHeardFrom(byte NodeId)
        //{
        //    using (zvsContext context = new zvsContext())
        //    {
        //        Device device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == this.AdapterGuid &&
        //                           o.NodeNumber == NodeId);

        //        if (device != null)
        //        {
        //            device.LastHeardFrom = DateTime.Now;
        //        }

        //        var result = await context.TrySaveChangesAsync();
        //        if (result.HasError)
        //            Core.log.Error(result.Message);
        //    }

        //}

        private DataType ConvertType(ZWValueID v)
        {
            DataType dataType = DataType.NONE;
            ZWValueID.ValueType openZwaveVType = v.GetType();

            if (openZwaveVType == ZWValueID.ValueType.Bool)
            {
                dataType = DataType.BOOL;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Button)
            {
                dataType = DataType.STRING;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Byte)
            {
                dataType = DataType.BYTE;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Decimal)
            {
                dataType = DataType.DECIMAL;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Int)
            {
                dataType = DataType.INTEGER;
            }
            else if (openZwaveVType == ZWValueID.ValueType.List)
            {
                dataType = DataType.LIST;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Schedule)
            {
                dataType = DataType.STRING;
            }
            else if (openZwaveVType == ZWValueID.ValueType.Short)
            {
                dataType = DataType.SHORT;
            }
            else if (openZwaveVType == ZWValueID.ValueType.String)
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
                log.Error("Error attempting to enable polling: " + ex.Message);
            }
        }

        #endregion




    }
}
