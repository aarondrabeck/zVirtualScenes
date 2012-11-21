using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ControlThink.ZWave;
using ControlThink.ZWave.Devices;
using ControlThink.ZWave.Devices.Specific;
using zvs.Processor;
using zvs.Entities;


namespace ThinkStickHIDPlugin
{
    [Export(typeof(zvsPlugin))]
    public class ThinkStickPlugin : zvsPlugin
    {
        private readonly ZWaveController CTController = new ZWaveController();
        private List<ZWaveDevice> _CTDevices = new List<ZWaveDevice>();
        private Window CommandDialogWindow = null;
        private bool isPolling = true;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<ThinkStickPlugin>();
        public ThinkStickPlugin()
            : base("THINKSTICK",
               "ThinkStick HID Plug-in",
                "This plug-in interfaces zVirtualScenes with OpenZwave using the ThinkStick HID .net library."
                ) { }

        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                DeviceType generic_dt = new DeviceType { UniqueIdentifier = "GENERIC", Name = "ControlThink Device", ShowInList = true };
                DefineOrUpdateDeviceType(generic_dt, context);

                DeviceType controller_dt = new DeviceType { UniqueIdentifier = "CONTROLLER", Name = "ControlThink Controller", ShowInList = true };
                DefineOrUpdateDeviceType(controller_dt, context);

                DeviceType switch_dt = new DeviceType { UniqueIdentifier = "SWITCH", Name = "ControlThink Binary", ShowInList = true };
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a switch." });
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a switch." });
                switch_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "MOMENTARY", Name = "Turn On for X milliseconds", ArgumentType = DataType.INTEGER, Description = "Turns a device on for the specified number of milliseconds and then turns the device back off." });
                DefineOrUpdateDeviceType(switch_dt, context);

                DeviceType dimmer_dt = new DeviceType { UniqueIdentifier = "DIMMER", Name = "ControlThink Dimmer", ShowInList = true };
                dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNON", Name = "Turn On", ArgumentType = DataType.NONE, Description = "Activates a dimmer." });
                dimmer_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "TURNOFF", Name = "Turn Off", ArgumentType = DataType.NONE, Description = "Deactivates a dimmer." });

                DeviceTypeCommand dimmer_preset_cmd = new DeviceTypeCommand { UniqueIdentifier = "SETPRESETLEVEL", Name = "Set Basic", ArgumentType = DataType.LIST, Description = "Sets a dimmer to a preset level." };
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "0%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "20%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "40%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "60%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "80%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "100%" });
                dimmer_preset_cmd.Options.Add(new CommandOption { Name = "255" });
                dimmer_dt.Commands.Add(dimmer_preset_cmd);
                DefineOrUpdateDeviceType(dimmer_dt, context);

                DeviceType thermo_dt = new DeviceType { UniqueIdentifier = "THERMOSTAT", Name = "ControlThink Thermostat", ShowInList = true };
                thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETENERGYMODE", Name = "Set Energy Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Energy Mode." });
                thermo_dt.Commands.Add(new DeviceTypeCommand { UniqueIdentifier = "SETCONFORTMODE", Name = "Set Comfort Mode", ArgumentType = DataType.NONE, Description = "Set thermostat to Comfort Mode. (Run)" });
                DefineOrUpdateDeviceType(thermo_dt, context);

                DeviceType sensor_dt = new DeviceType { UniqueIdentifier = "SENSOR", Name = "ControlThink Sensor", ShowInList = true };
                DefineOrUpdateDeviceType(sensor_dt, context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "ENABLEREPOLLONLEVELCHANGE",
                    Name = "ThinkStick Re-poll",
                    Description = "Re-poll dimmers 3 seconds after a basic change is received?",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL
                }, context);
            }

            CTController.Connected += CTController_Connected;
            CTController.Disconnected += CTController_Disconnected;
        }

        protected override void StartPlugin()
        {
            CTController.ControllerNotResponding += CTController_ControllerNotResponding;
            CTController.LevelChanged += CTController_LevelChanged;

            try
            {
                if (!CTController.IsConnected)
                    CTController.Connect();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Failed to connect. {0}", e.Message));
                IsReady = false;
            }
        }

        protected override void StopPlugin()
        {
            CTController.ControllerNotResponding -= CTController_ControllerNotResponding;
            CTController.LevelChanged -= CTController_LevelChanged;


            try
            {
                if (CTController.IsConnected)
                    CTController.Disconnect();
            }
            catch (Exception e)
            {
                log.Fatal(string.Format("Failed to disconnect. {0}", e.Message), e);
                IsReady = false;
            }
        }

        protected override void SettingChanged(string settingsettingUniqueIdentifier, string settingValue)
        {
        }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd)
        {
            try
            {
                ZWaveDevice CTDevice = GetCTDevice((byte)cmd.Device.NodeNumber);
                if (CTDevice != null)
                {
                    if (cmd.Device.Type.UniqueIdentifier == "CONTROLLER")
                    {
                        if (CTDevice.NodeID == CTController.NodeID)
                        {
                            if (CommandDialogWindow == null)
                            {
                                string cmdName = cmd.Command.UniqueIdentifier;
                                Thread t = new Thread(() =>
                                {
                                    ControllerDialogWindow cdWindow = new ControllerDialogWindow(CTController, cmdName);
                                    CommandDialogWindow = cdWindow;
                                    cdWindow.Closed += (s, a) =>
                                    {
                                        CommandDialogWindow = null;
                                    };

                                    cdWindow.ShowDialog();
                                });
                                t.SetApartmentState(ApartmentState.STA);
                                t.Start();
                            }
                            else
                            {
                                CommandDialogWindow.Dispatcher.Invoke(new Action(() => CommandDialogWindow.Activate()));
                            }

                        }
                    }
                    else if (cmd.Device.Type.UniqueIdentifier == "THERMOSTAT")
                    {
                        GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;

                        switch (cmd.Command.UniqueIdentifier)
                        {
                            case "FAN_MODE":
                                {
                                    ThermostatFanMode mode = ThermostatFanMode.AutoLow;
                                    if (Enum.TryParse(cmd.Argument, out mode))
                                    {
                                        CTThermostat.ThermostatFanMode = mode;
                                    }
                                    break;
                                }
                            case "MODE":
                                {
                                    ThermostatMode mode = ThermostatMode.Off;
                                    if (Enum.TryParse(cmd.Argument, out mode))
                                    {
                                        CTThermostat.ThermostatMode = mode;
                                    }
                                    break;
                                }
                        }

                        //Dynamic SP's
                        if (cmd.Command.UniqueIdentifier.StartsWith("DYNAMIC_SP_R207_"))
                        {
                            string spTypeStr = cmd.Command.UniqueIdentifier.Replace("DYNAMIC_SP_R207_", "");
                            ThermostatSetpointType setPointType = ThermostatSetpointType.Cooling1;
                            decimal temp = 0;

                            if (Enum.TryParse(spTypeStr, out setPointType) && decimal.TryParse(cmd.Argument, out temp))
                            {
                                CTThermostat.ThermostatSetpoints[setPointType].Temperature = new Temperature(temp, TemperatureScale.Fahrenheit);
                            }
                        }
                    }
                    else if (cmd.Device.Type.UniqueIdentifier == "SWITCH" || cmd.Device.Type.UniqueIdentifier == "DIMMER")
                    {
                        if (cmd.Command.UniqueIdentifier == "BASIC")
                        {
                            byte level = 0;
                            byte.TryParse(cmd.Argument, out level);
                            AsyncSetLevel(CTDevice, level);

                        }
                    }

                    //Polling all devices
                    if (cmd.Command.UniqueIdentifier == "REPOLLINT")
                    {
                        int PollingInt = 0;

                        if (int.TryParse(cmd.Argument, out PollingInt))
                        {
                            //Set the Polling Interval
                            CTDevice.PollEnabled = PollingInt > 0;
                            CTDevice.PollInterval = TimeSpan.FromSeconds(PollingInt);

                            using (zvsContext context = new zvsContext())
                            {
                                UpdateDeviceValue(cmd.Device.Id, "REPOLLINT", PollingInt.ToString(), context);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error sending command. " + ex.Message);
            }
        }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd)
        {
            try
            {
                ZWaveDevice CTDevice = GetCTDevice((byte)cmd.Device.NodeNumber);
                if (CTDevice != null)
                {
                    if (cmd.Device.Type.UniqueIdentifier == "CONTROLLER")
                    {

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
                                    CTDevice.PowerOn();

                                    System.Timers.Timer t = new System.Timers.Timer();
                                    t.Interval = delay;
                                    t.Elapsed += (sender, e) =>
                                    {
                                        t.Stop();
                                        CTDevice.PowerOff();
                                        t.Dispose();
                                    };
                                    t.Start();

                                    break;
                                }
                            case "TURNON":
                                {

                                    CTDevice.PowerOn();

                                    break;
                                }
                            case "TURNOFF":
                                {

                                    CTDevice.PowerOff();
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

                                        AsyncSetLevel(CTDevice, defaultonlevel);
                                    }
                                    break;
                                }
                            case "TURNOFF":
                                {

                                    CTDevice.PowerOff();
                                    break;
                                }
                            case "SETPRESETLEVEL":
                                {

                                    switch (cmd.Argument)
                                    {
                                        case "0%":
                                            AsyncSetLevel(CTDevice, 0);
                                            break;
                                        case "20%":
                                            AsyncSetLevel(CTDevice, 20);
                                            break;
                                        case "40%":
                                            AsyncSetLevel(CTDevice, 40);
                                            break;
                                        case "60%":
                                            AsyncSetLevel(CTDevice, 60);
                                            break;
                                        case "80%":
                                            AsyncSetLevel(CTDevice, 80);
                                            break;
                                        case "100%":
                                            AsyncSetLevel(CTDevice, 99);
                                            break;
                                        case "255":
                                            AsyncSetLevel(CTDevice, 255);
                                            break;

                                    }
                                    break;

                                }

                        }
                    }
                    else if (cmd.Device.Type.UniqueIdentifier == "THERMOSTAT")
                    {

                        GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;

                        switch (cmd.Command.UniqueIdentifier)
                        {
                            case "SETENERGYMODE":
                                {
                                    AsyncSetLevel(CTThermostat, 0);
                                    break;
                                }
                            case "SETCONFORTMODE":
                                {
                                    AsyncSetLevel(CTThermostat, 255);
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error sending command. " + ex.Message);
            }
        }

        public override void Repoll(zvs.Entities.Device device)
        {
            ZWaveDevice CTDevice = GetCTDevice((byte)device.NodeNumber);
            if (CTDevice != null)
            {
                ManuallyPollDevice(CTDevice);
            }
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
                        ZWaveDevice CTDevice = GetCTDevice(Convert.ToByte(d.NodeNumber));
                        if (CTDevice != null)
                        {
                            try
                            {
                                switch (d.Type.UniqueIdentifier)
                                {
                                    case "SWITCH":
                                        AsyncSetLevel(CTDevice, 255);
                                        break;
                                    case "DIMMER":
                                        byte defaultonlevel = 99;
                                        byte.TryParse(DevicePropertyValue.GetDevicePropertyValue(Context, d, "DEFAULONLEVEL"), out defaultonlevel);
                                        AsyncSetLevel(CTDevice, defaultonlevel);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Error sending command. " + ex.Message);
                            }

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
                        ZWaveDevice CTDevice = GetCTDevice(Convert.ToByte(d.NodeNumber));
                        if (CTDevice != null)
                        {
                            try
                            {
                                switch (d.Type.UniqueIdentifier)
                                {
                                    case "DIMMER":
                                    case "SWITCH":
                                        AsyncSetLevel(CTDevice, 0);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Error sending command. " + ex.Message);
                            }
                        }

                    }
                }
            }
        }

        private ZWaveDevice GetCTDevice(byte NodeID)
        {
            foreach (ZWaveDevice CTDevice in _CTDevices)
                if (CTDevice.NodeID == NodeID)
                    return CTDevice;

            return null;
        }

        private void CTController_LevelChanged(object sender, ZWaveController.LevelChangedEventArgs e)
        {
            log.Info("Level changed global: " + e.Level + e.OriginDevice.NodeID);
        }

        private void CTController_ControllerNotResponding(object sender, EventArgs e)
        {
            log.Info("ControlThink HID USB Controller not responding.  Attempting to disconnect...");
            try
            {
                CTController.Disconnect();
            }
            catch
            {
            }
        }

        private void CTController_Connected(object sender, EventArgs e)
        {
            log.Info("Initializing: Driver with Home ID 0x" + CTController.HomeID + "...");
            log.Info("Initializing: Getting devices...");
            DiscoverDevices();
            log.Info("Initializing: Subscribing to events...");
            SubscribeEvents();
            log.Info("Initializing: Setting polling intervals...");
            SetPollingIntervals();
            IsReady = true;
            log.Info("Initializing Complete. Plugin Ready.");

            isPolling = true;
            log.Info("Polling each device...");
            ManuallyPollDevices();
            log.Info("Polling Complete.");
            isPolling = false;
        }

        private void CTController_Disconnected(object sender, EventArgs e)
        {
            IsReady = false;
            UnSubscribeEvents();
            log.Info("Disconnected: Plug-in shutdown.");
        }

        private void UnSubscribeEvents()
        {
            foreach (ZWaveDevice CTDevice in _CTDevices)
            {
                if (CTDevice is BinarySwitch)
                    CTDevice.LevelChanged -= Switch_LevelChanged;
                else if (CTDevice is MultilevelSwitch)
                    CTDevice.LevelChanged -= Dimmer_LevelChanged;
                else if (CTDevice is GeneralThermostatV2)
                {
                    GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;
                    CTThermostat.ThermostatFanModeChanged -= CTThermostat_ThermostatFanModeChanged;
                    CTThermostat.ThermostatFanStateChanged -= CTThermostat_ThermostatFanStateChanged;
                    CTThermostat.ThermostatModeChanged -= CTThermostat_ThermostatModeChanged;
                    CTThermostat.ThermostatOperatingStateChanged -= CTThermostat_ThermostatOperatingStateChanged;
                    CTThermostat.ThermostatTemperatureChanged -= CTThermostat_ThermostatTemperatureChanged;
                    CTThermostat.ThermostatSetpointChanged -= CTThermostat_ThermostatSetpointChanged;
                }
            }
            _CTDevices.Clear();
        }

        private void SubscribeEvents()
        {
            if (CTController.IsConnected)
            {
                _CTDevices.Clear();
                foreach (ZWaveDevice CTDevice in CTController.Devices)
                {
                    _CTDevices.Add(CTDevice);
                    if (CTDevice is BinarySwitch)
                        CTDevice.LevelChanged += Switch_LevelChanged;
                    else if (CTDevice is MultilevelSwitch)
                        CTDevice.LevelChanged += Dimmer_LevelChanged;
                    else if (CTDevice is GeneralThermostatV2)
                    {
                        GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;
                        CTThermostat.ThermostatFanModeChanged += CTThermostat_ThermostatFanModeChanged;
                        CTThermostat.ThermostatFanStateChanged += CTThermostat_ThermostatFanStateChanged;
                        CTThermostat.ThermostatModeChanged += CTThermostat_ThermostatModeChanged;
                        CTThermostat.ThermostatOperatingStateChanged += CTThermostat_ThermostatOperatingStateChanged;
                        CTThermostat.ThermostatTemperatureChanged += CTThermostat_ThermostatTemperatureChanged;
                        CTThermostat.ThermostatSetpointChanged += CTThermostat_ThermostatSetpointChanged;
                        CTThermostat.LevelChanged += CTThermostat_LevelChanged;
                    }
                }
            }
        }

        private void DiscoverDevices()
        {
            if (CTController.IsConnected)
            {
                using (zvsContext context = new zvsContext())
                {
                    #region Discover Devices
                    foreach (ZWaveDevice CTDevice in CTController.Devices)
                    {
                        //Look for this device in the DB
                        Device existingDevice = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTDevice.NodeID);
                        if (existingDevice == null)
                        {
                            existingDevice = new Device();
                            existingDevice.Type = GetDeviceType("GENERIC", context);
                            existingDevice.NodeNumber = CTDevice.NodeID;
                            existingDevice.Name = "ControlThink OpenZwave Device";
                            existingDevice.CurrentLevelInt = 0;
                            existingDevice.CurrentLevelText = "";
                            context.Devices.Add(existingDevice);
                            context.SaveChanges();
                        }

                        //Type
                        if (CTDevice is Controller)
                        {
                            existingDevice.Type = GetDeviceType("CONTROLLER", context);

                            if (existingDevice.Name == "ControlThink OpenZwave Device")
                                existingDevice.Name = "My ControlThink Controller";

                            #region Controller Commands
                            if (CTController.NodeID == CTDevice.NodeID)
                            {
                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginAddController",
                                    Name = "Add Controller",
                                    ArgumentType = DataType.NONE,
                                    Help = "Adds controller to network",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginAddDevice",
                                    Name = "Add Device",
                                    ArgumentType = DataType.NONE,
                                    Help = "Adds device to network",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginCreateNewPrimaryController",
                                    Name = "Create New Primary Controller",
                                    ArgumentType = DataType.NONE,
                                    Help = "Creates New Primary Controller",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginReceiveConfiguration",
                                    Name = "Receive Configuration",
                                    ArgumentType = DataType.NONE,
                                    Help = "Receives Configuration",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginRemoveController",
                                    Name = "Remove Controller",
                                    ArgumentType = DataType.NONE,
                                    Help = "Removes Controller",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                                {
                                    Device = existingDevice,
                                    UniqueIdentifier = "BeginRemoveDevice",
                                    Name = "Remove Device",
                                    ArgumentType = DataType.NONE,
                                    Help = "Removes Device",
                                    CustomData1 = "",
                                    CustomData2 = "",
                                    SortOrder = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new DeviceCommand
                               {
                                   Device = existingDevice,
                                   UniqueIdentifier = "BeginTransferPrimaryRole",
                                   Name = "Transfer Primary Role",
                                   ArgumentType = DataType.NONE,
                                   Help = "Transfer Primary Role",
                                   CustomData1 = "",
                                   CustomData2 = "",
                                   SortOrder = 1
                               }, context);
                            #endregion
                            }
                        }
                        else if (CTDevice is BinarySwitch)
                        {
                            existingDevice.Type = GetDeviceType("SWITCH", context);

                            if (existingDevice.Name == "ControlThink OpenZwave Device")
                                existingDevice.Name = "My ControlThink Switch";

                            #region BinarySwitch Level Value and Command
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.INTEGER,
                                CommandClass = "",
                                Value = "0",
                                isReadOnly = false
                            }, context, true);

                            DeviceCommand dc = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                ArgumentType = DataType.BYTE,
                                Help = "Changes the Switch Level",
                                CustomData1 = "",
                                CustomData2 = "BASIC",
                                SortOrder = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                        else if (CTDevice is MultilevelSwitch)
                        {
                            existingDevice.Type = GetDeviceType("DIMMER", context);

                            if (existingDevice.Name == "ControlThink OpenZwave Device")
                                existingDevice.Name = "My ControlThink Dimmer";

                            #region DimmerSwtich Level Value and Command
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.INTEGER,
                                CommandClass = "",
                                Value = "0",
                                isReadOnly = false
                            }, context, true);

                            DeviceCommand dc = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                ArgumentType = DataType.BYTE,
                                Help = "Changes the Dimmer Level",
                                CustomData1 = "",
                                CustomData2 = "BASIC",
                                SortOrder = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                        else if (CTDevice is GeneralThermostatV2)
                        {
                            GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;
                            existingDevice.Type = GetDeviceType("THERMOSTAT", context);

                            if (existingDevice.Name == "ControlThink OpenZwave Device")
                                existingDevice.Name = "My ControlThink Thermostat";

                            #region Temp
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "TEMPERATURE",
                                Name = "Temperature",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.INTEGER,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = true
                            }, context, true);
                            #endregion

                            #region Thermostat SetBack Mode
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "SETBACK",
                                Name = "SetBack Mode",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.INTEGER,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = false
                            }, context, true);
                            #endregion

                            #region Thermostat Fan Mode
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "FAN_MODE",
                                Name = "Fan Mode",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.STRING,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = false
                            }, context, true);
                            DeviceCommand dc = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "FAN_MODE",
                                Name = "Fan Mode",
                                ArgumentType = DataType.LIST,
                                Help = "Changes the Thermostat Fan Mode",
                                CustomData1 = "",
                                CustomData2 = "FAN_MODE",
                                SortOrder = 1
                            };


                            try
                            {
                                foreach (ThermostatFanMode mode in CTThermostat.SupportedThermostatFanModes)
                                    dc.Options.Add(new CommandOption { Name = mode.ToString() });
                            }
                            catch (Exception e)
                            {
                                log.Error("Error getting supported thermostat fan modes. " + e.Message);
                            }

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion

                            #region Thermostat Mode
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "MODE",
                                Name = "Mode",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.STRING,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = false
                            }, context, true);

                            DeviceCommand dc2 = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "MODE",
                                Name = "Mode",
                                ArgumentType = DataType.LIST,
                                Help = "Changes the Thermostat Mode",
                                CustomData1 = "",
                                CustomData2 = "MODE",
                                SortOrder = 2
                            };

                            try
                            {
                                foreach (ThermostatMode mode in CTThermostat.SupportedThermostatModes)
                                    dc2.Options.Add(new CommandOption { Name = mode.ToString() });
                            }
                            catch (Exception e)
                            {
                                log.Error("Error getting supported thermostat modes. " + e.Message);
                            }

                            DefineOrUpdateDeviceCommand(dc2, context);
                            #endregion

                            #region Thermostat Fan State
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "FAN_STATE",
                                Name = "Fan State",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.STRING,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = true
                            }, context, true);
                            #endregion

                            #region Thermostat Operating State
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "OPERATING_STATE",
                                Name = "Operating State",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.STRING,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = true
                            }, context, true);
                            #endregion

                            #region Thermostat Setpoint Collection
                            try
                            {
                                foreach (ThermostatSetpointType type in CTThermostat.SupportedThermostatSetpoints)
                                {
                                    ThermostatSetpoint sp = CTThermostat.ThermostatSetpoints[type];
                                    DefineOrUpdateDeviceValue(new DeviceValue
                                    {
                                        Device = existingDevice,
                                        UniqueIdentifier = "DYNAMIC_SP_R207_" + type.ToString(),
                                        Name = type.ToString(),
                                        Genre = "",
                                        Index = "",
                                        ValueType = DataType.INTEGER,
                                        CommandClass = "",
                                        Value = "",
                                        isReadOnly = false
                                    }, context, true);

                                    DeviceCommand dc3 = new DeviceCommand
                                    {
                                        Device = existingDevice,
                                        UniqueIdentifier = "DYNAMIC_SP_R207_" + type.ToString(),
                                        Name = "Set " + type.ToString(),
                                        ArgumentType = DataType.INTEGER,
                                        Help = "Changes the Thermostat " + type.ToString(),
                                        CustomData1 = "",
                                        CustomData2 = "DYNAMIC_SP_R207_" + type.ToString(),
                                        SortOrder = 1
                                    };
                                    DefineOrUpdateDeviceCommand(dc3, context);
                                }
                            }
                            catch (Exception e)
                            {
                                log.Error("Error getting supported thermostat setpoints. " + e.Message);
                            }
                            #endregion

                            //try
                            //{
                            //    bool alreadyAssociated = false;
                            //    //first, see if we are already associated with the thermostat.
                            //    foreach (ControlThink.ZWave.Devices.ZWaveDevice deviceWithinLoop in CTThermostat.Groups[1])
                            //    {
                            //        if (deviceWithinLoop.NodeID == CTController.NodeID)
                            //        {
                            //            alreadyAssociated = true;
                            //            break;
                            //        }
                            //    }

                            //    //if we are not associated with the thermostat, make it so.  This will ensure that the thermostat sends up updates in real time.
                            //    if (alreadyAssociated == false)
                            //        CTThermostat.Groups[1].Add(CTController.Devices.GetByNodeID(CTController.NodeID));
                            //}
                            //catch (NotSupportedException ex)
                            //{
                            //    //live status is not supported.
                            //    log.Error("Live status is not supported. " + ex.Message);
                            //}
                            //catch (Exception ex)
                            //{
                            //    log.Error("Live status error. " + ex.Message);
                            //}
                        }
                        else if (CTDevice is BinarySensor || CTDevice is MultilevelSensor)
                        {
                            existingDevice.Type = GetDeviceType("SENSOR", context);

                            if (existingDevice.Name == "ControlThink OpenZwave Device")
                                existingDevice.Name = "My ControlThink Sensor";

                            #region Sensor Level Value and Command
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.BYTE,
                                CommandClass = "",
                                Value = "",
                                isReadOnly = false
                            }, context, true);

                            DeviceCommand dc = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "BASIC",
                                Name = "Basic",
                                ArgumentType = DataType.BYTE,
                                Help = "Changes the Dimmer Level",
                                CustomData1 = "",
                                CustomData2 = "BASIC",
                                SortOrder = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                    }

                    #endregion
                    context.SaveChanges();
                }
            }
        }

        private void SetPollingIntervals()
        {
            if (CTController.IsConnected)
            {
                using (zvsContext context = new zvsContext())
                {
                    foreach (ZWaveDevice CTDevice in CTController.Devices)
                    {
                        //Look for this device in the DB
                        Device existingDevice = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTDevice.NodeID);
                        if (existingDevice != null)
                        {
                            int DefaultPollingInterval = 1200;

                            //Get the repoll interval saved previously if is there.
                            DeviceValue existingDV = existingDevice.Values.FirstOrDefault(o => o.UniqueIdentifier == "REPOLLINT");
                            if (existingDV != null)
                                int.TryParse(existingDV.Value, out DefaultPollingInterval);

                            try
                            {
                                //Set the Polling Interval
                                CTDevice.PollEnabled = DefaultPollingInterval > 0;
                                CTDevice.PollInterval = TimeSpan.FromSeconds(DefaultPollingInterval);
                            }
                            catch (Exception ex)
                            {
                                log.Error("Error setting polling. " + ex.Message);
                            }

                            #region Repoll Value and Command
                            DefineOrUpdateDeviceValue(new DeviceValue
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "REPOLLINT",
                                Name = "Re-poll Interval (Seconds). 0 = No Polling.",
                                Genre = "",
                                Index = "",
                                ValueType = DataType.INTEGER,
                                CommandClass = "",
                                Value = DefaultPollingInterval.ToString(),
                                isReadOnly = false
                            }, context, true);

                            DeviceCommand dc = new DeviceCommand
                            {
                                Device = existingDevice,
                                UniqueIdentifier = "REPOLLINT",
                                Name = "Re-poll Interval (Seconds)",
                                ArgumentType = DataType.INTEGER,
                                Help = "Re-poll Interval in seconds (0 = No Polling)",
                                CustomData1 = "",
                                CustomData2 = "REPOLLINT",
                                SortOrder = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                    }
                }
            }
        }

        private void ManuallyPollDevices()
        {
            if (CTController.IsConnected)
            {
                //instigate level changes so events trigger.
                foreach (ZWaveDevice CTDevice in CTController.Devices)
                {
                    ManuallyPollDevice(CTDevice);
                }
            }
        }

        private void ManuallyPollDevice(ZWaveDevice CTDevice)
        {
            if (CTController.IsConnected)
            {
                if (CTDevice is Controller)
                {

                }
                if (CTDevice is BinarySwitch || CTDevice is MultilevelSwitch || CTDevice is BinarySensor || CTDevice is MultilevelSensor || CTDevice is GeneralThermostatV2)
                {
                    try
                    {
                        byte level = CTDevice.Level;
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Polling error on node {0}. {1}", CTDevice.NodeID, e.Message));
                    }
                }

                if (CTDevice is GeneralThermostatV2)
                {
                    GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;
                    try
                    {
                        decimal temp = CTThermostat.ThermostatTemperature.Value;
                        ThermostatMode mode = CTThermostat.ThermostatMode;
                        ThermostatFanMode fanmode = CTThermostat.ThermostatFanMode;
                        ThermostatFanState FanState = CTThermostat.ThermostatFanState;
                        ThermostatOperatingState statue = CTThermostat.ThermostatOperatingState;

                        foreach (ThermostatSetpointType type in CTThermostat.SupportedThermostatSetpoints)
                        {
                            ThermostatSetpoint sp = CTThermostat.ThermostatSetpoints[type];
                            decimal t = sp.Temperature.Value;
                        }

                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Polling error on node {0}. {1}", CTDevice.NodeID, e.Message));
                    }
                }
            }

        }

        void CTThermostat_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "SETBACK", e.Level > 0 ? "Comfort Mode" : "Energy Mode", context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("Thermostat set back Changed on DEVICE NOT FOUND:" + e.Level.ToString());
                }
            }
        }

        void CTThermostat_ThermostatSetpointChanged(object sender, ThermostatSetpointChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "DYNAMIC_SP_R207_" + e.ThermostatSetpointType.ToString(), e.Temperature.ToFahrenheit().ToString(), context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("ThermostatSetpoint Changed on DEVICE NOT FOUND:" + e.ThermostatSetpointType.ToString());
                }
            }
        }

        void CTThermostat_ThermostatTemperatureChanged(object sender, ThermostatTemperatureChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        dev.CurrentLevelInt = (int)e.ThermostatTemperature.ToFahrenheit();
                        dev.CurrentLevelText = e.ThermostatTemperature.ToFahrenheit().ToString() + "° F";
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();

                        UpdateDeviceValue(dev.Id, "TEMPERATURE", e.ThermostatTemperature.ToFahrenheit().ToString(), context);
                    }
                    else
                        log.Error("TEMPERATURE Changed on DEVICE NOT FOUND:" + e.ThermostatTemperature.ToFahrenheit());
                }
            }
        }

        void CTThermostat_ThermostatOperatingStateChanged(object sender, ThermostatOperatingStateChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "OPERATING_STATE", e.ThermostatOperatingState.ToString(), context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("OPERATING_STATE Changed on DEVICE NOT FOUND:" + e.ThermostatOperatingState);
                }
            }
        }

        void CTThermostat_ThermostatModeChanged(object sender, ThermostatModeChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "MODE", e.ThermostatMode.ToString(), context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("MODE Changed on DEVICE NOT FOUND:" + e.ThermostatMode);
                }
            }
        }

        void CTThermostat_ThermostatFanStateChanged(object sender, ThermostatFanStateChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "FAN_STATE", e.ThermostatFanState.ToString(), context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("FAN_STATE Changed on DEVICE NOT FOUND:" + e.ThermostatFanState);
                }
            }
        }

        void CTThermostat_ThermostatFanModeChanged(object sender, ThermostatFanModeChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        UpdateDeviceValue(dev.Id, "FAN_MODE", e.ThermostatFanMode.ToString(), context);
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                        log.Error("ThermostatFanMode Changed on DEVICE NOT FOUND:" + e.ThermostatFanMode);
                }
            }
        }

        private HybridDictionary timers = new HybridDictionary();
        void Dimmer_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is MultilevelSwitch)
            {
                MultilevelSwitch CTDimmer = (MultilevelSwitch)sender;

                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTDimmer.NodeID);
                    if (dev != null)
                    {
                        if (dev.CurrentLevelInt != e.Level)
                        {
                            dev.CurrentLevelInt = e.Level;
                            dev.CurrentLevelText = e.Level + "%";
                            dev.LastHeardFrom = DateTime.Now;
                            context.SaveChanges();

                            UpdateDeviceValue(dev.Id, "BASIC", e.Level.ToString(), context);

                            //Some dimmers take x number of seconds to dim to desired level.  Therefor the level recieved here initially is a 
                            //level between old level and new level. (if going from 0 to 100 we get 84 here).
                            //To get the real level repoll the device a second or two after a level change was recieved.     
                            bool EnableDimmerRepoll = false;
                            bool.TryParse(DevicePropertyValue.GetDevicePropertyValue(context, dev, "ENABLEREPOLLONLEVELCHANGE"), out EnableDimmerRepoll);

                            if (!isPolling)
                            {
                                //only allow each device to re-poll 1 time.
                                if (timers.Contains(dev.NodeNumber))
                                {
                                    log.Info(string.Format("Timer {0} restarted.", dev.NodeNumber));
                                    System.Timers.Timer t = (System.Timers.Timer)timers[dev.NodeNumber];
                                    t.Stop();
                                    t.Start();
                                }
                                else
                                {
                                    System.Timers.Timer t = new System.Timers.Timer();
                                    timers.Add(dev.NodeNumber, t);
                                    t.Interval = 2000;
                                    t.Elapsed += (s, args) =>
                                    {
                                        ManuallyPollDevice(CTDimmer);
                                        t.Stop();
                                        log.Info(string.Format("Timer {0} Elapsed.", dev.NodeNumber));
                                        timers.Remove(dev.NodeNumber);
                                    };
                                    t.Start();
                                    log.Info(string.Format("Timer {0} started.", dev.NodeNumber));
                                }
                            }
                        }
                    }
                    else
                        log.Error("Level Changed on DEVICE NOT FOUND:" + e.Level);
                }
            }
        }

        void Switch_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is BinarySwitch)
            {
                BinarySwitch CTSwitch = (BinarySwitch)sender;

                using (zvsContext context = new zvsContext())
                {
                    Device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.NodeNumber == CTSwitch.NodeID);
                    if (dev != null)
                    {
                        dev.CurrentLevelInt = e.Level;
                        dev.CurrentLevelText = e.Level > 0 ? "ON" : "OFF";
                        dev.LastHeardFrom = DateTime.Now;
                        context.SaveChanges();

                        UpdateDeviceValue(dev.Id, "BASIC", e.Level.ToString(), context);

                    }
                    else
                        log.Info("Level Changed on DEVICE NOT FOUND:" + e.Level);
                }
            }
        }

        private void AsyncSetLevel(ZWaveDevice CTDevice, byte Level)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
                {
                    CTDevice.Level = Level;
                };
            bw.RunWorkerCompleted += (s, e) =>
                {
                    if (e.Error != null)
                        log.Info("Level Changed error:" + e.Error);
                };
            bw.RunWorkerAsync();
        }

    }
}
