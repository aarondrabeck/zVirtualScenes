using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ControlThink.ZWave;
using ControlThink.ZWave.Devices;
using ControlThink.ZWave.Devices.Specific;
using zVirtualScenes;
using zVirtualScenesModel;

namespace ThinkStickHIDPlugin
{
    [Export(typeof(Plugin))]
    public class ThinkStickPlugin : Plugin
    {
        private readonly ZWaveController CTController = new ZWaveController();
        private List<ZWaveDevice> _CTDevices = new List<ZWaveDevice>();
        private Window CommandDialogWindow = null;

        public ThinkStickPlugin()
            : base("THINKSTICK",
               "ThinkStick HID Plugin",
                "This plug-in interfaces zVirtualScenes with OpenZwave using the ThinkStick HID .net library."
                ) { }

        public override void Initialize()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                device_types generic_dt = new device_types { name = "GENERIC", friendly_name = "ControlThink Device", show_in_list = true };
                DefineOrUpdateDeviceType(generic_dt, context);

                device_types controller_dt = new device_types { name = "CONTROLLER", friendly_name = "ControlThink Controller", show_in_list = true };
                DefineOrUpdateDeviceType(controller_dt, context);

                device_types switch_dt = new device_types { name = "SWITCH", friendly_name = "ControlThink Binary", show_in_list = true };
                switch_dt.device_type_commands.Add(new device_type_commands { name = "TURNON", friendly_name = "Turn On", arg_data_type = (int)Data_Types.NONE, description = "Activates a switch." });
                switch_dt.device_type_commands.Add(new device_type_commands { name = "TURNOFF", friendly_name = "Turn Off", arg_data_type = (int)Data_Types.NONE, description = "Deactivates a switch." });
                switch_dt.device_type_commands.Add(new device_type_commands { name = "MOMENTARY", friendly_name = "Turn On for X milliseconds", arg_data_type = (int)Data_Types.INTEGER, description = "Turns a device on for the specified number of milliseconds and then turns the device back off." });
                DefineOrUpdateDeviceType(switch_dt, context);

                device_types dimmer_dt = new device_types { name = "DIMMER", friendly_name = "ControlThink Dimmer", show_in_list = true };
                dimmer_dt.device_type_commands.Add(new device_type_commands { name = "TURNON", friendly_name = "Turn On", arg_data_type = (int)Data_Types.NONE, description = "Activates a dimmer." });
                dimmer_dt.device_type_commands.Add(new device_type_commands { name = "TURNOFF", friendly_name = "Turn Off", arg_data_type = (int)Data_Types.NONE, description = "Deactivates a dimmer." });

                device_type_commands dimmer_preset_cmd = new device_type_commands { name = "SETPRESETLEVEL", friendly_name = "Set Basic", arg_data_type = (int)Data_Types.LIST, description = "Sets a dimmer to a preset level." };
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "0%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "20%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "40%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "60%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "80%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "100%" });
                dimmer_preset_cmd.device_type_command_options.Add(new device_type_command_options { options = "255" });
                dimmer_dt.device_type_commands.Add(dimmer_preset_cmd);
                DefineOrUpdateDeviceType(dimmer_dt, context);

                device_types thermo_dt = new device_types { name = "THERMOSTAT", friendly_name = "ControlThink Thermostat", show_in_list = true };
                thermo_dt.device_type_commands.Add(new device_type_commands { name = "SETENERGYMODE", friendly_name = "Set Energy Mode", arg_data_type = (int)Data_Types.NONE, description = "Set thermosat to Energy Mode." });
                thermo_dt.device_type_commands.Add(new device_type_commands { name = "SETCONFORTMODE", friendly_name = "Set Comfort Mode", arg_data_type = (int)Data_Types.NONE, description = "Set thermosat to Confort Mode. (Run)" });
                DefineOrUpdateDeviceType(thermo_dt, context);

                device_types sensor_dt = new device_types { name = "SENSOR", friendly_name = "ControlThink Sensor", show_in_list = true };
                DefineOrUpdateDeviceType(sensor_dt, context);

                device_propertys.AddOrEdit(new device_propertys
                {
                    name = "ENABLEREPOLLONLEVELCHANGE",
                    friendly_name = "Repoll dimmers 3 seconds after a basic change is received?",
                    default_value = true.ToString(),
                    value_data_type = (int)Data_Types.BOOL
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
                WriteToLog(Urgency.ERROR, string.Format("Failed to connect. {0}", e.Message));
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
                WriteToLog(Urgency.ERROR, string.Format("Failed to disconnect. {0}", e.Message));
                IsReady = false;
            }
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            return;
        }

        public override bool ProcessDeviceCommand(zVirtualScenesModel.device_command_que cmd)
        {
            try
            {
                ZWaveDevice CTDevice = GetCTDevice((byte)cmd.device.node_id);
                if (CTDevice != null)
                {
                    if (cmd.device.device_types.name == "CONTROLLER")
                    {
                        if (CTDevice.NodeID == CTController.NodeID)
                        {
                            if (CommandDialogWindow == null)
                            {
                                string cmdName = cmd.device_commands.name;
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
                    else if (cmd.device.device_types.name == "THERMOSTAT")
                    {
                        GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;

                        switch (cmd.device_commands.name)
                        {
                            case "FAN_MODE":
                                {
                                    ThermostatFanMode mode = ThermostatFanMode.AutoLow;
                                    if (Enum.TryParse(cmd.arg, out mode))
                                    {
                                        CTThermostat.ThermostatFanMode = mode;
                                    }
                                    break;
                                }
                            case "MODE":
                                {
                                    ThermostatMode mode = ThermostatMode.Off;
                                    if (Enum.TryParse(cmd.arg, out mode))
                                    {
                                        CTThermostat.ThermostatMode = mode;
                                    }
                                    break;
                                }
                        }

                        //Dynamic SP's
                        if (cmd.device_commands.name.StartsWith("DYNAMIC_SP_R207_"))
                        {
                            string spTypeStr = cmd.device_commands.name.Replace("DYNAMIC_SP_R207_", "");
                            ThermostatSetpointType setPointType = ThermostatSetpointType.Cooling1;
                            decimal temp = 0;

                            if (Enum.TryParse(spTypeStr, out setPointType) && decimal.TryParse(cmd.arg, out temp))
                            {
                                CTThermostat.ThermostatSetpoints[setPointType].Temperature = new Temperature(temp, TemperatureScale.Fahrenheit);
                            }
                        }
                    }
                    else if (cmd.device.device_types.name == "SWITCH" || cmd.device.device_types.name == "DIMMER")
                    {
                        if (cmd.device_commands.name == "BASIC")
                        {
                            byte level = 0;
                            byte.TryParse(cmd.arg, out level);
                            CTDevice.Level = level;
                        }
                    }

                    //Polling all devices
                    if (cmd.device_commands.name == "REPOLLINT")
                    {
                        int PollingInt = 0;

                        if (int.TryParse(cmd.arg, out PollingInt))
                        {
                            //Set the Polling Interval
                            CTDevice.PollEnabled = PollingInt > 0;
                            CTDevice.PollInterval = TimeSpan.FromSeconds(PollingInt);

                            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                            {
                                UpdateDeviceValue(cmd.device.id, "REPOLLINT", PollingInt.ToString(), context);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error sending command. " + ex.Message);
                return false;
            }
            return true;
        }

        public override bool ProcessDeviceTypeCommand(zVirtualScenesModel.device_type_command_que cmd)
        {
            try
            {
                ZWaveDevice CTDevice = GetCTDevice((byte)cmd.device.node_id);
                if (CTDevice != null)
                {
                    if (cmd.device.device_types.name == "CONTROLLER")
                    {

                    }
                    else if (cmd.device.device_types.name == "SWITCH")
                    {
                        switch (cmd.device_type_commands.name)
                        {
                            case "MOMENTARY":
                                {

                                    int delay = 1000;
                                    int.TryParse(cmd.arg, out delay);
                                    byte nodeID = (byte)cmd.device.node_id;

                                    CTDevice.Level = 255;
                                    System.Timers.Timer t = new System.Timers.Timer();
                                    t.Interval = delay;
                                    t.Elapsed += (sender, e) =>
                                    {
                                        t.Stop();
                                        CTDevice.Level = 0;
                                        t.Dispose();
                                    };
                                    t.Start();

                                    break;
                                }
                            case "TURNON":
                                {

                                    CTDevice.Level = 255;

                                    break;
                                }
                            case "TURNOFF":
                                {

                                    CTDevice.Level = 0;
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
                                    using (zvsLocalDBEntities Context = new zvsLocalDBEntities())
                                    {
                                        byte defaultonlevel = 99;
                                        byte.TryParse(device_property_values.GetDevicePropertyValue(Context, cmd.device_id, "DEFAULONLEVEL"), out defaultonlevel);


                                        CTDevice.Level = defaultonlevel;
                                    }
                                    break;
                                }
                            case "TURNOFF":
                                {

                                    CTDevice.Level = 0;
                                    return true;
                                }
                            case "SETPRESETLEVEL":
                                {

                                    switch (cmd.arg)
                                    {
                                        case "0%":
                                            CTDevice.Level = 0;
                                            break;
                                        case "20%":
                                            CTDevice.Level = 20;
                                            break;
                                        case "40%":
                                            CTDevice.Level = 40;
                                            break;
                                        case "60%":
                                            CTDevice.Level = 60;
                                            break;
                                        case "80%":
                                            CTDevice.Level = 80;
                                            break;
                                        case "100%":
                                            CTDevice.Level = 100;
                                            break;
                                        case "255":
                                            CTDevice.Level = 255;
                                            break;

                                    }
                                    break;

                                }

                        }
                    }
                    else if (cmd.device.device_types.name == "THERMOSTAT")
                    {

                        GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;

                        switch (cmd.device_type_commands.name)
                        {
                            case "SETENERGYMODE":
                                {
                                    CTThermostat.Level = 0;
                                    return true;
                                }
                            case "SETCONFORTMODE":
                                {
                                    CTThermostat.Level = 255;
                                    return true;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error sending command. " + ex.Message);
                return false;
            }

            return true;
        }

        public override bool Repoll(zVirtualScenesModel.device device)
        {
            ZWaveDevice CTDevice = GetCTDevice((byte)device.node_id);
            if (CTDevice != null)
            {
                ManuallyPollDevice(CTDevice);
            }

            return true;
        }

        public override bool ActivateGroup(int groupID)
        {
            using (zvsLocalDBEntities Context = new zvsLocalDBEntities())
            {
                IQueryable<device> devices = GetDeviceInGroup(groupID, Context);
                if (devices != null)
                {
                    foreach (device d in devices)
                    {
                        ZWaveDevice CTDevice = GetCTDevice(Convert.ToByte(d.node_id));
                        if (CTDevice != null)
                        {
                            try
                            {
                                switch (d.device_types.name)
                                {
                                    case "SWITCH":
                                        CTDevice.Level = 255;
                                        break;
                                    case "DIMMER":
                                        byte defaultonlevel = 99;
                                        byte.TryParse(device_property_values.GetDevicePropertyValue(Context, d.id, "DEFAULONLEVEL"), out defaultonlevel);
                                        CTDevice.Level = defaultonlevel;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteToLog(Urgency.ERROR, "Error sending command. " + ex.Message);
                                return false;
                            }

                        }

                    }
                }
            }
            return true;
        }

        public override bool DeactivateGroup(int groupID)
        {
            using (zvsLocalDBEntities Context = new zvsLocalDBEntities())
            {
                IQueryable<device> devices = GetDeviceInGroup(groupID, Context);
                if (devices != null)
                {
                    foreach (device d in devices)
                    {
                        ZWaveDevice CTDevice = GetCTDevice(Convert.ToByte(d.node_id));
                        if (CTDevice != null)
                        {
                            try
                            {
                                switch (d.device_types.name)
                                {
                                    case "DIMMER":
                                    case "SWITCH":
                                        CTDevice.Level = 0;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteToLog(Urgency.ERROR, "Error sending command. " + ex.Message);
                                return false;
                            }
                        }

                    }
                }
            }
            return true;
        }

        private ZWaveDevice GetCTDevice(byte NodeID)
        {
            foreach (ZWaveDevice CTDevice in CTController.Devices)
                if (CTDevice.NodeID == NodeID)
                    return CTDevice;

            return null;
        }

        private void CTController_LevelChanged(object sender, ZWaveController.LevelChangedEventArgs e)
        {
            WriteToLog(Urgency.INFO, "Level changed global: " + e.Level + e.OriginDevice.NodeID);
        }

        private void CTController_ControllerNotResponding(object sender, EventArgs e)
        {
            WriteToLog(Urgency.INFO, "ControlThink HID USB Controller not responding.  Attempting to disconnect...");
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
            WriteToLog(Urgency.INFO, "Initializing: Driver with Home ID 0x" + CTController.HomeID + "...");
            WriteToLog(Urgency.INFO, "Initializing: Getting devices...");
            DiscoverDevices();
            WriteToLog(Urgency.INFO, "Initializing: Subscribing to events...");
            SubscribeEvents();
            WriteToLog(Urgency.INFO, "Initializing: Setting polling intervals...");
            SetPollingIntervals();
            IsReady = true;
            WriteToLog(Urgency.INFO, "Initializing Complete. Plugin Ready.");
            
            WriteToLog(Urgency.INFO, "Polling each device...");
            ManuallyPollDevices();
            WriteToLog(Urgency.INFO, "Polling Complete.");
        }

        private void CTController_Disconnected(object sender, EventArgs e)
        {
            IsReady = false;
            UnSubscribeEvents();
            WriteToLog(Urgency.INFO, "Disconnected: Plug-in shutdown.");
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
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    #region Discover Devices
                    foreach (ZWaveDevice CTDevice in CTController.Devices)
                    {
                        //Look for this device in the DB
                        device existingDevice = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTDevice.NodeID);
                        if (existingDevice == null)
                        {
                            existingDevice = new device();
                            existingDevice.device_types = GetDeviceType("GENERIC", context);
                            existingDevice.node_id = CTDevice.NodeID;
                            existingDevice.friendly_name = "ControlThink OpenZwave Device";
                            existingDevice.current_level_int = 0;
                            existingDevice.current_level_txt = "";
                            context.devices.Add(existingDevice);
                            context.SaveChanges();
                        }

                        //Type
                        if (CTDevice is Controller)
                        {
                            existingDevice.device_types = GetDeviceType("CONTROLLER", context);

                            if (existingDevice.friendly_name == "ControlThink OpenZwave Device")
                                existingDevice.friendly_name = "My ControlThink Controller";

                            #region Controller Commands
                            if (CTController.NodeID == CTDevice.NodeID)
                            {
                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginAddController",
                                    friendly_name = "Add Controller",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Adds controller to network",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginAddDevice",
                                    friendly_name = "Add Device",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Adds device to network",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginCreateNewPrimaryController",
                                    friendly_name = "Create New Primary Controller",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Creates New Primary Controller",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginReceiveConfiguration",
                                    friendly_name = "Receive Configuration",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Receives Configuration",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginRemoveController",
                                    friendly_name = "Remove Controller",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Removes Controller",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                                {
                                    device_id = existingDevice.id,
                                    name = "BeginRemoveDevice",
                                    friendly_name = "Remove Device",
                                    arg_data_type = (int)Data_Types.NONE,
                                    help = "Removes Device",
                                    custom_data1 = "",
                                    custom_data2 = "",
                                    sort_order = 1
                                }, context);

                                DefineOrUpdateDeviceCommand(new device_commands
                               {
                                   device_id = existingDevice.id,
                                   name = "BeginTransferPrimaryRole",
                                   friendly_name = "Transfer Primary Role",
                                   arg_data_type = (int)Data_Types.NONE,
                                   help = "Transfer Primary Role",
                                   custom_data1 = "",
                                   custom_data2 = "",
                                   sort_order = 1
                               }, context);
                            #endregion
                            }
                        }
                        else if (CTDevice is BinarySwitch)
                        {
                            existingDevice.device_types = GetDeviceType("SWITCH", context);

                            if (existingDevice.friendly_name == "ControlThink OpenZwave Device")
                                existingDevice.friendly_name = "My ControlThink Switch";

                            #region BinarySwitch Level Value and Command
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "BASIC",
                                label_name = "Basic",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "0",
                                read_only = false
                            }, context, true);

                            device_commands dc = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "BASIC",
                                friendly_name = "Basic",
                                arg_data_type = (int)Data_Types.BYTE,
                                help = "Changes the Switch Level",
                                custom_data1 = "",
                                custom_data2 = "BASIC",
                                sort_order = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                        else if (CTDevice is MultilevelSwitch)
                        {
                            existingDevice.device_types = GetDeviceType("DIMMER", context);

                            if (existingDevice.friendly_name == "ControlThink OpenZwave Device")
                                existingDevice.friendly_name = "My ControlThink Dimmer";

                            #region DimmerSwtich Level Value and Command
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "BASIC",
                                label_name = "Basic",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "0",
                                read_only = false
                            }, context, true);

                            device_commands dc = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "BASIC",
                                friendly_name = "Basic",
                                arg_data_type = (int)Data_Types.BYTE,
                                help = "Changes the Dimmer Level",
                                custom_data1 = "",
                                custom_data2 = "BASIC",
                                sort_order = 1
                            };

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion
                        }
                        else if (CTDevice is GeneralThermostatV2)
                        {
                            GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)CTDevice;
                            existingDevice.device_types = GetDeviceType("THERMOSTAT", context);

                            if (existingDevice.friendly_name == "ControlThink OpenZwave Device")
                                existingDevice.friendly_name = "My ControlThink Thermostat";

                            #region Temp
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "TEMPERATURE",
                                label_name = "Temperature",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = true
                            }, context, true);
                            #endregion

                            #region Thermostat SetBack Mode
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "SETBACK",
                                label_name = "SetBack Mode",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = false
                            }, context, true);
                            #endregion

                            #region Thermostat Fan Mode
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "FAN_MODE",
                                label_name = "Fan Mode",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = false
                            }, context, true);
                            device_commands dc = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "FAN_MODE",
                                friendly_name = "Fan Mode",
                                arg_data_type = (int)Data_Types.LIST,
                                help = "Changes the Thermostat Fan Mode",
                                custom_data1 = "",
                                custom_data2 = "FAN_MODE",
                                sort_order = 1
                            };


                            try
                            {
                                foreach (ThermostatFanMode mode in CTThermostat.SupportedThermostatFanModes)
                                    dc.device_command_options.Add(new device_command_options { name = mode.ToString() });
                            }
                            catch (Exception e)
                            {
                                WriteToLog(Urgency.ERROR, "Error getting supported thermostat fan modes. " + e.Message);
                            }

                            DefineOrUpdateDeviceCommand(dc, context);
                            #endregion

                            #region Thermostat Mode
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "MODE",
                                label_name = "Mode",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = false
                            }, context, true);

                            device_commands dc2 = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "MODE",
                                friendly_name = "Mode",
                                arg_data_type = (int)Data_Types.LIST,
                                help = "Changes the Thermostat Mode",
                                custom_data1 = "",
                                custom_data2 = "MODE",
                                sort_order = 2
                            };

                            try
                            {
                                foreach (ThermostatMode mode in CTThermostat.SupportedThermostatModes)
                                    dc2.device_command_options.Add(new device_command_options { name = mode.ToString() });
                            }
                            catch (Exception e)
                            {
                                WriteToLog(Urgency.ERROR, "Error getting supported thermostat modes. " + e.Message);
                            }

                            DefineOrUpdateDeviceCommand(dc2, context);
                            #endregion

                            #region Thermostat Fan State
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "FAN_STATE",
                                label_name = "Fan State",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = true
                            }, context, true);
                            #endregion

                            #region Thermostat Operating State
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "OPERATING_STATE",
                                label_name = "Operating State",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = true
                            }, context, true);
                            #endregion

                            #region Thermostat Setpoint Collection
                            try
                            {
                                foreach (ThermostatSetpointType type in CTThermostat.SupportedThermostatSetpoints)
                                {
                                    ThermostatSetpoint sp = CTThermostat.ThermostatSetpoints[type];
                                    DefineOrUpdateDeviceValue(new device_values
                                    {
                                        device_id = existingDevice.id,
                                        value_id = "DYNAMIC_SP_R207_" + type.ToString(),
                                        label_name = type.ToString(),
                                        genre = "",
                                        index2 = "",
                                        type = "",
                                        commandClassId = "",
                                        value2 = "",
                                        read_only = false
                                    }, context, true);

                                    device_commands dc3 = new device_commands
                                    {
                                        device_id = existingDevice.id,
                                        name = "DYNAMIC_SP_R207_" + type.ToString(),
                                        friendly_name = "Set " + type.ToString(),
                                        arg_data_type = (int)Data_Types.INTEGER,
                                        help = "Changes the Thermostat " + type.ToString(),
                                        custom_data1 = "",
                                        custom_data2 = "DYNAMIC_SP_R207_" + type.ToString(),
                                        sort_order = 1
                                    };
                                    DefineOrUpdateDeviceCommand(dc3, context);
                                }
                            }
                            catch (Exception e)
                            {
                                WriteToLog(Urgency.ERROR, "Error getting supported thermostat setpoints. " + e.Message);
                            }
                            #endregion
                        }
                        else if (CTDevice is BinarySensor || CTDevice is MultilevelSensor)
                        {
                            existingDevice.device_types = GetDeviceType("SENSOR", context);

                            if (existingDevice.friendly_name == "ControlThink OpenZwave Device")
                                existingDevice.friendly_name = "My ControlThink Sensor";

                            #region Sensor Level Value and Command
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "BASIC",
                                label_name = "Basic",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = "",
                                read_only = false
                            }, context, true);

                            device_commands dc = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "BASIC",
                                friendly_name = "Basic",
                                arg_data_type = (int)Data_Types.BYTE,
                                help = "Changes the Dimmer Level",
                                custom_data1 = "",
                                custom_data2 = "BASIC",
                                sort_order = 1
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
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    foreach (ZWaveDevice CTDevice in CTController.Devices)
                    {
                        //Look for this device in the DB
                        device existingDevice = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTDevice.NodeID);
                        if (existingDevice != null)
                        {
                            int DefaultPollingInterval = 600;

                            //Get the repoll interval saved previously if is there.
                            device_values existingDV = existingDevice.device_values.FirstOrDefault(o => o.value_id == "REPOLLINT");
                            if (existingDV != null)
                                int.TryParse(existingDV.value2, out DefaultPollingInterval);

                            try
                            {
                                //Set the Polling Interval
                                CTDevice.PollEnabled = DefaultPollingInterval > 0;
                                CTDevice.PollInterval = TimeSpan.FromSeconds(DefaultPollingInterval);
                            }
                            catch (Exception ex)
                            {
                                WriteToLog(Urgency.ERROR, "Error setting polling. " + ex.Message);
                            }

                            #region Repoll Value and Command
                            DefineOrUpdateDeviceValue(new device_values
                            {
                                device_id = existingDevice.id,
                                value_id = "REPOLLINT",
                                label_name = "Repoll Interval (Seconds). 0 = No Polling.",
                                genre = "",
                                index2 = "",
                                type = "",
                                commandClassId = "",
                                value2 = DefaultPollingInterval.ToString(),
                                read_only = false
                            }, context, true);

                            device_commands dc = new device_commands
                            {
                                device_id = existingDevice.id,
                                name = "REPOLLINT",
                                friendly_name = "Repoll Interval (Seconds)",
                                arg_data_type = (int)Data_Types.INTEGER,
                                help = "Repoll Interval (Seconds).  0 = No Polling.",
                                custom_data1 = "",
                                custom_data2 = "REPOLLINT",
                                sort_order = 1
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
                        WriteToLog(Urgency.ERROR, string.Format("Node {0} threw an error. {1}", CTDevice.NodeID, e.Message));
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
                        WriteToLog(Urgency.ERROR, string.Format("Node {0} threw an error. {1}", CTDevice.NodeID, e.Message));
                    }
                }
            }

        }

        void CTThermostat_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "SETBACK", e.Level > 0 ? "Confort Mode" : "Energy Mode", context);
                    else
                        WriteToLog(Urgency.ERROR, "Thermostat set back Changed on DEVICE NOT FOUND:" + e.Level.ToString());
                }
            }
        }

        void CTThermostat_ThermostatSetpointChanged(object sender, ThermostatSetpointChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "DYNAMIC_SP_R207_" + e.ThermostatSetpointType.ToString(), e.Temperature.ToFahrenheit().ToString(), context);
                    else
                        WriteToLog(Urgency.ERROR, "ThermostatSetpoint Changed on DEVICE NOT FOUND:" + e.ThermostatSetpointType.ToString());
                }
            }
        }

        void CTThermostat_ThermostatTemperatureChanged(object sender, ThermostatTemperatureChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                    {
                        dev.current_level_int = (int)e.ThermostatTemperature.ToFahrenheit();
                        dev.current_level_txt = e.ThermostatTemperature.ToFahrenheit().ToString() + "° F"; ;

                        UpdateDeviceValue(dev.id, "TEMPERATURE", e.ThermostatTemperature.ToFahrenheit().ToString(), context);
                    }
                    else
                        WriteToLog(Urgency.ERROR, "TEMPERATURE Changed on DEVICE NOT FOUND:" + e.ThermostatTemperature.ToFahrenheit());
                }
            }
        }

        void CTThermostat_ThermostatOperatingStateChanged(object sender, ThermostatOperatingStateChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "OPERATING_STATE", e.ThermostatOperatingState.ToString(), context);
                    else
                        WriteToLog(Urgency.ERROR, "OPERATING_STATE Changed on DEVICE NOT FOUND:" + e.ThermostatOperatingState);
                }
            }
        }

        void CTThermostat_ThermostatModeChanged(object sender, ThermostatModeChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "MODE", e.ThermostatMode.ToString(), context);
                    else
                        WriteToLog(Urgency.ERROR, "MODE Changed on DEVICE NOT FOUND:" + e.ThermostatMode);
                }
            }
        }

        void CTThermostat_ThermostatFanStateChanged(object sender, ThermostatFanStateChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "FAN_STATE", e.ThermostatFanState.ToString(), context);
                    else
                        WriteToLog(Urgency.ERROR, "FAN_STATE Changed on DEVICE NOT FOUND:" + e.ThermostatFanState);
                }
            }
        }

        void CTThermostat_ThermostatFanModeChanged(object sender, ThermostatFanModeChangedEventArgs e)
        {
            if (sender is GeneralThermostatV2)
            {
                GeneralThermostatV2 CTThermostat = (GeneralThermostatV2)sender;
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTThermostat.NodeID);
                    if (dev != null)
                        UpdateDeviceValue(dev.id, "FAN_MODE", e.ThermostatFanMode.ToString(), context);
                    else
                        WriteToLog(Urgency.ERROR, "ThermostatFanMode Changed on DEVICE NOT FOUND:" + e.ThermostatFanMode);
                }
            }
        }

        void Dimmer_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is MultilevelSwitch)
            {
                MultilevelSwitch CTDimmer = (MultilevelSwitch)sender;

                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTDimmer.NodeID);
                    if (dev != null)
                    {
                        dev.current_level_int = e.Level;
                        dev.current_level_txt = e.Level + "%";

                        UpdateDeviceValue(dev.id, "BASIC", e.Level.ToString(), context);

                        //Some dimmers take x number of seconds to dim to desired level.  Therefor the level recieved here initially is a 
                        //level between old level and new level. (if going from 0 to 100 we get 84 here).
                        //To get the real level repoll the device a second or two after a level change was recieved.     
                        bool EnableDimmerRepoll = false;
                        bool.TryParse(device_property_values.GetDevicePropertyValue(context, dev.id, "ENABLEREPOLLONLEVELCHANGE"), out EnableDimmerRepoll);

                        if (IsReady)
                        {
                            System.Timers.Timer t = new System.Timers.Timer();
                            t.Interval = 2000;
                            t.Elapsed += (s, args) =>
                            {
                                Console.WriteLine(string.Format("Timer {0} Elapsed.", dev.node_id));
                                ManuallyPollDevice(CTDimmer);
                                t.Stop();
                            };
                            t.Start();
                            Console.WriteLine(string.Format("Timer {0} started.", dev.node_id));
                        }
                    }
                    else
                        WriteToLog(Urgency.ERROR, "Level Changed on DEVICE NOT FOUND:" + e.Level);
                }
            }
        }

        void Switch_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (sender is BinarySwitch)
            {
                BinarySwitch CTSwitch = (BinarySwitch)sender;

                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device dev = GetMyPluginsDevices(context).FirstOrDefault(o => o.node_id == CTSwitch.NodeID);
                    if (dev != null)
                    {
                        dev.current_level_int = e.Level;
                        dev.current_level_txt = e.Level > 0 ? "ON" : "OFF";

                        UpdateDeviceValue(dev.id, "BASIC", e.Level.ToString(), context);
                    }
                    else
                        WriteToLog(Urgency.INFO, "Level Changed on DEVICE NOT FOUND:" + e.Level);
                }
            }
        }
    }
}
