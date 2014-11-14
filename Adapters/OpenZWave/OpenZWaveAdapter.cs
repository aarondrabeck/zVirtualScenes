using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenZWaveDotNet;
using OpenZWavePlugin;
using OpenZWavePlugin.Forms;
using zvs.DataModel;
using zvs.Processor;

namespace OpenZWaveAdapter
{
    [Export(typeof(ZvsAdapter))]
    public class OpenZWaveAdapter : ZvsAdapter
    {
        private async void OpenZWaveAdapter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsEnabled) return;
            await StopOpenzwaveAsync();
            await StartOpenzwaveAsync();
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

        private int ControllerTypeId { get; set; }
        private int SwitchTypeId { get; set; }
        private int DimmerTypeId { get; set; }
        private int ThermoTypeId { get; set; }
        private int UnknownTypeId { get; set; }
        private int LockTypeId { get; set; }
        private int SensorTypeId { get; set; }

        public override async Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
            //Controller Type Devices
            var controllerDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Controller.ToString(),
                Name = "OpenZWave Controller",
                ShowInList = true
            };
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "RESET",
                Name = "Reset Controller",
                ArgumentType = DataType.NONE,
                Description = "Erases all Z-Wave network settings from your controller. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "ADDDEVICE",
                Name = "Add Device to Network",
                ArgumentType = DataType.NONE,
                Description = "Adds a ZWave Device to your network. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "AddController",
                Name = "Add Controller to Network",
                ArgumentType = DataType.NONE,
                Description = "Adds a ZWave Controller to your network. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "CreateNewPrimary",
                Name = "Create New Primary",
                ArgumentType = DataType.NONE,
                Description = "Puts the target controller into receive configuration mode. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "ReceiveConfiguration",
                Name = "Receive Configuration",
                ArgumentType = DataType.NONE,
                Description = "Receives the network configuration from another controller. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "RemoveController",
                Name = "Remove Controller",
                ArgumentType = DataType.NONE,
                Description = "Removes a Controller from your network. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "RemoveDevice",
                Name = "Remove Device",
                ArgumentType = DataType.NONE,
                Description = "Removes a Device from your network. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "TransferPrimaryRole",
                Name = "Transfer Primary Role",
                ArgumentType = DataType.NONE,
                Description = "Transfers the primary role to another controller. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "HasNodeFailed",
                Name = "Has Node Failed",
                ArgumentType = DataType.NONE,
                Description = "Tests whether a node has failed. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "RemoveFailedNode",
                Name = "Remove Failed Node",
                ArgumentType = DataType.NONE,
                Description = "Removes the failed node from the controller's list. Argument2 = DeviceId."
            });
            controllerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "ReplaceFailedNode",
                Name = "Replace Failed Node",
                ArgumentType = DataType.NONE,
                Description = "Tests the failed node. Argument2 = DeviceId."
            });
            var result = await deviceTypeBuilder.RegisterAsync(AdapterGuid, controllerDt, CancellationToken);
            if (result.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave controller device type. {0}", result.Message);


            //Switch Type Devices
            var switchDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Switch.ToString(),
                Name = "OpenZWave Binary",
                ShowInList = true
            };
            switchDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "TURNON",
                Name = "Turn On",
                ArgumentType = DataType.NONE,
                Description = "Activates a switch. Argument2 = DeviceId."
            });
            switchDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "TURNOFF",
                Name = "Turn Off",
                ArgumentType = DataType.NONE,
                Description = "Deactivates a switch. Argument2 = DeviceId."
            });
            switchDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "MOMENTARY",
                Name = "Turn On for X milliseconds",
                ArgumentType = DataType.INTEGER,
                Description =
                    "Turns a device on for the specified number of milliseconds and then turns the device back off. Argument2 = DeviceId."
            });
            var switchSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, switchDt, CancellationToken);
            if (switchSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave switch device type. {0}",
                        switchSaveResult.Message);

            //Dimmer Type Devices
            var dimmerDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Dimmer.ToString(),
                Name = "OpenZWave Dimmer",
                ShowInList = true
            };
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "TURNON",
                Name = "Turn On",
                ArgumentType = DataType.NONE,
                Description = "Activates a dimmer. Argument2 = DeviceId."
            });
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "TURNOFF",
                Name = "Turn Off",
                ArgumentType = DataType.NONE,
                Description = "Deactivates a dimmer. Argument2 = DeviceId."
            });

            var dimmerPresetCmd = new DeviceTypeCommand
            {
                UniqueIdentifier = "SETPRESETLEVEL",
                Name = "Set Level",
                ArgumentType = DataType.LIST,
                Description = "Sets a dimmer to a preset level. Argument2 = DeviceId."
            };
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "0%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "20%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "40%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "60%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "80%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "100%" });
            dimmerPresetCmd.Options.Add(new CommandOption { Name = "255" });
            dimmerDt.Commands.Add(dimmerPresetCmd);
            var dimmerSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, dimmerDt, CancellationToken);
            if (dimmerSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave dimmer device type. {0}",
                        dimmerSaveResult.Message);

            //Thermostat Type Devices
            var thermoDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Thermostat.ToString(),
                Name = "OpenZWave Thermostat",
                ShowInList = true
            };
            thermoDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "SETENERGYMODE",
                Name = "Set Energy Mode",
                ArgumentType = DataType.NONE,
                Description = "Set thermostat to Energy Mode. Argument2 = DeviceId."
            });
            thermoDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "SETCONFORTMODE",
                Name = "Set Comfort Mode",
                ArgumentType = DataType.NONE,
                Description = "Set thermostat to Comfort Mode. (Run) Argument2 = DeviceId."
            });
            var thermoSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, thermoDt, CancellationToken);
            if (thermoSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave thermostat device type. {0}",
                        thermoSaveResult.Message);

            var unknwonDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Unknown.ToString(),
                Name = "OpenZWave Unknown",
                ShowInList = true
            };
            var unknownSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, unknwonDt, CancellationToken);
            if (unknownSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave unknwon device type. {0}",
                        unknownSaveResult.Message);

            //Door Lock Type Devices
            var lockDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Doorlock.ToString(),
                Name = "OpenZWave Door lock",
                ShowInList = true
            };
            var lockSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, lockDt, CancellationToken);
            if (lockSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave door lock device type. {0}",
                        lockSaveResult.Message);

            //Sensors
            var sensorDt = new DeviceType
            {
                UniqueIdentifier = OpenzWaveDeviceTypes.Sensor.ToString(),
                Name = "OpenZWave Sensor",
                ShowInList = true
            };
            var sensorSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, sensorDt, CancellationToken);
            if (sensorSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave sensor device type. {0}",
                        sensorSaveResult.Message);
            using (var context = new ZvsContext(EntityContextConnection))
            {
                ControllerTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Controller.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                SwitchTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Switch.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                DimmerTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Dimmer.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                ThermoTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Thermostat.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                UnknownTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Unknown.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                LockTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Doorlock.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
                SensorTypeId = await context.DeviceTypes.Where(o => o.UniqueIdentifier == OpenzWaveDeviceTypes.Sensor.ToString()).Select(o => o.Id).FirstOrDefaultAsync();
            }
        }

        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var comSetting = new AdapterSetting
             {
                 Name = "Com Port",
                 Value = (3).ToString(CultureInfo.InvariantCulture),
                 ValueType = DataType.COMPORT,
                 Description = "The COM port that your z-wave controller is assigned to."
             };

            var pollIntSetting = new AdapterSetting
           {
               Name = "Polling interval",
               Value = (360).ToString(CultureInfo.InvariantCulture),
               ValueType = DataType.INTEGER,
               Description = "The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network traffic. "
           };

            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(comSetting, o => o.ComportSetting);
            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(pollIntSetting, o => o.PollingIntervalSetting);

            await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
            {
                UniqueIdentifier = OpenzWaveDeviceTypeSettings.DefaultDimmerOnLevel.ToString(),
                DeviceTypeId = DimmerTypeId,
                Name = "Default Level",
                Description = "Level that an device is set to when using the 'ON' command.",
                Value = "99",//default value
                ValueType = DataType.BYTE
            });

            await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
            {
                UniqueIdentifier = OpenzWaveDeviceTypeSettings.EnableRepollOnLevelChange.ToString(),
                DeviceTypeId = DimmerTypeId,
                Name = "Enable re-poll on level change",
                Description = "Re-poll dimmers 3 seconds after a level change is received?",
                Value = true.ToString(), //default value
                ValueType = DataType.BOOL
            });

            await settingBuilder.RegisterDeviceTypeSettingAsync(new DeviceTypeSetting
            {
                UniqueIdentifier = OpenzWaveDeviceTypeSettings.RepollingEnabled.ToString(),
                DeviceTypeId = DimmerTypeId,
                Name = "Enable polling for this device",
                Description = "Toggles automatic polling for a device.",
                Value = false.ToString(), //default value
                ValueType = DataType.BOOL
            });

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
        private bool _useHidSetting;
        public bool UseHidSetting
        {
            get { return _useHidSetting; }
            set
            {
                if (value == _useHidSetting) return;
                _useHidSetting = value;
                NotifyPropertyChanged();
            }
        }

        private string _comportSetting = "10";
        public string ComportSetting
        {
            get { return _comportSetting; }
            set
            {
                if (value == _comportSetting) return;
                _comportSetting = value;
                NotifyPropertyChanged();
            }
        }

        private int _pollingIntervalSetting;
        public int PollingIntervalSetting
        {
            get { return _pollingIntervalSetting; }
            set
            {
                if (value == _pollingIntervalSetting) return;
                _pollingIntervalSetting = value;
                NotifyPropertyChanged();
            }
        }

        private bool InitialPollingComplete { get; set; }

        //OpenzWave Data
        private ZWManager MManager { get; set; }
        private ZWOptions MOptions { get; set; }
        ZWNotification MNotification { get; set; }
        UInt32 MHomeId { get; set; }
        readonly List<Node> _mNodeList = new List<Node>();
        private const string LastEventNameValueId = "LEN1";

        private readonly HashSet<byte> _nodesReady = new HashSet<byte>();

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

        public string LogPath
        {
            get
            {
                var path = Path.Combine(Utils.AppDataPath, @"openzwave\");
                if (Directory.Exists(path)) return path + "\\";
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.ReportErrorFormatAsync(CancellationToken, "An error occured while trying to create Openzwave log directory. {0}", ex.Message).Wait();
                }

                return path + "\\";
            }
        }

        private async Task StartOpenzwaveAsync()
        {
            if (CancellationToken.IsCancellationRequested)
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "{0} driver cannot start because it is still shutting down", Name);
                return;
            }

            PropertyChanged += OpenZWaveAdapter_PropertyChanged;
            try
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "OpenZwave driver starting on {0}", UseHidSetting ? "HID" : "COM" + ComportSetting);

                // Environment.CurrentDirectory returns wrong directory in Service environment so we have to make a trick
                var directoryName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                // Create the Options                
                MOptions = new ZWOptions();
                MOptions.Create(directoryName + @"\config\",
                                        LogPath,
                                        @"");
                MOptions.Lock();
                MManager = new ZWManager();


                MManager.Create();
                MManager.OnNotification += NotificationHandler;

                if (!UseHidSetting)
                {
                    if (ComportSetting != "0")
                    {
                        MManager.AddDriver(@"\\.\COM" + ComportSetting);
                    }
                }
                else
                {
                    MManager.AddDriver("HID Controller", ZWControllerInterface.Hid);
                }


                if (PollingIntervalSetting != 0)
                {
                    MManager.SetPollInterval(PollingIntervalSetting, true);
                }
            }
            catch (Exception e)
            {
                Log.ReportErrorFormatAsync(CancellationToken, "Error initializing Openzwave {0}", e.Message).Wait();
            }
        }

        private async Task StopOpenzwaveAsync()
        {
            if (!CancellationToken.IsCancellationRequested)
            {
                PropertyChanged -= OpenZWaveAdapter_PropertyChanged;
                InitialPollingComplete = false;


                //EKK this is blocking and can be slow
                if (MManager != null)
                {
                    MManager.OnNotification -= NotificationHandler;
                    MManager.RemoveDriver(@"\\.\COM" + ComportSetting);
                    await Task.Run(() =>
                    {
                        MManager.Destroy();
                        MManager = null;
                    });
                }

                if (MOptions != null)
                {
                    await Task.Run(() =>
                    {
                        MOptions.Destroy();
                        MOptions = null;
                    });
                }

                await Log.ReportInfoAsync("OpenZwave driver stopped", CancellationToken);
            }
        }
        public override async Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument)
        {
            var nodeNumber = Convert.ToByte(device.NodeNumber);
            if (!IsNodeReady(nodeNumber))
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "Failed to issue command on {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                return;
            }

            if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.Controller.ToString())
            {
                #region Controller Commands
                switch (command.UniqueIdentifier)
                {
                    case "RESET":
                        {
                            MManager.ResetController(MHomeId);
                            break;
                        }
                    case "ADDDEVICE":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.AddDevice, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }

                    case "CreateNewPrimary":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.CreateNewPrimary, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReceiveConfiguration":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.ReceiveConfiguration, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }

                    case "RemoveDevice":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.RemoveDevice, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "TransferPrimaryRole":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.TransferPrimaryRole, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "HasNodeFailed":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.HasNodeFailed, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "RemoveFailedNode":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.RemoveFailedNode, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                    case "ReplaceFailedNode":
                        {
                            var dlg = new ControllerCommandDlg(MManager, MHomeId, ZWControllerCommand.ReplaceFailedNode, (byte)device.NodeNumber);
                            dlg.ShowDialog();
                            dlg.Dispose();
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.Switch.ToString())
            {
                #region Switch command handeling
                switch (command.UniqueIdentifier)
                {
                    case "MOMENTARY":
                        {
                            int delay;
                            int.TryParse(argument, out delay);
                            var nodeId = (byte)device.NodeNumber;

                            MManager.SetNodeOn(MHomeId, nodeId);
                            await Task.Delay(delay);
                            MManager.SetNodeOff(MHomeId, nodeId);

                            break;

                        }
                    case "TURNON":
                        {
                            MManager.SetNodeOn(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "TURNOFF":
                        {
                            MManager.SetNodeOff(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.Dimmer.ToString())
            {
                #region Dimmer command handling
                switch (command.UniqueIdentifier)
                {
                    case "TURNON":
                        {
                            using (var context = new ZvsContext(EntityContextConnection))
                            {
                                var value = await device.GetDeviceTypeValueAsync(OpenzWaveDeviceTypeSettings.DefaultDimmerOnLevel.ToString(), context);

                                if (value != null)
                                {
                                    byte bValue = byte.TryParse(value, out bValue) ? bValue : (byte)99;
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, bValue);
                                    break;
                                }
                            }

                            MManager.SetNodeOn(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "TURNOFF":
                        {
                            MManager.SetNodeOff(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "SETPRESETLEVEL":
                        {
                            switch (argument)
                            {
                                case "0%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(0));
                                    break;
                                case "20%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(20));
                                    break;
                                case "40%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(40));
                                    break;
                                case "60%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(60));
                                    break;
                                case "80%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(80));
                                    break;
                                case "100%":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(100));
                                    break;
                                case "255":
                                    MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, Convert.ToByte(255));
                                    break;
                            }
                            break;
                        }
                }
                #endregion
            }
            else if (deviceType.UniqueIdentifier == OpenzWaveDeviceTypes.Thermostat.ToString())
            {
                #region Thermostat Command Handling
                switch (command.UniqueIdentifier)
                {
                    case "SETENERGYMODE":
                        {
                            MManager.SetNodeOff(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                    case "SETCONFORTMODE":
                        {
                            MManager.SetNodeOn(MHomeId, (byte)device.NodeNumber);
                            break;
                        }
                }
                #endregion
            }
        }

        public override async Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2)
        {
            if (!command.UniqueIdentifier.Contains("DYNAMIC_CMD_")) return;
            var nodeNumber = Convert.ToByte(device.NodeNumber);

            //Get more info from this Node from OpenZWave
            var node = GetNode(MHomeId, nodeNumber);

            if (!IsNodeReady(nodeNumber))
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "Failed to issue command on {0}, node {1}. Node not ready.", device.Name, nodeNumber);
                return;
            }

            switch (command.ArgumentType)
            {
                case DataType.BYTE:
                    {
                        byte b;
                        byte.TryParse(argument, out b);

                        var value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString(CultureInfo.InvariantCulture).Equals(command.CustomData2));
                        if (value != null)
                            MManager.SetValue(value.ValueID, b);
                        break;
                    }
                case DataType.BOOL:
                    {
                        bool b;
                        bool.TryParse(argument, out b);

                        var value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString(CultureInfo.InvariantCulture).Equals(command.CustomData2));
                        if (value != null)
                            MManager.SetValue(value.ValueID, b);
                        break;
                    }
                case DataType.DECIMAL:
                    {
                        var f = Convert.ToSingle(argument);

                        var value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString(CultureInfo.InvariantCulture).Equals(command.CustomData2));
                        if (value != null)
                            MManager.SetValue(value.ValueID, f);
                        break;
                    }
                case DataType.LIST:
                case DataType.STRING:
                    {
                        var value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString(CultureInfo.InvariantCulture).Equals(command.CustomData2));
                        if (value != null)
                            MManager.SetValue(value.ValueID, argument);
                        break;
                    }
                case DataType.INTEGER:
                    {
                        int i;
                        int.TryParse(argument, out i);

                        var value = node.Values.FirstOrDefault(o => o.ValueID.GetId().ToString(CultureInfo.InvariantCulture).Equals(command.CustomData2));
                        if (value != null)
                            MManager.SetValue(value.ValueID, i);
                        break;
                    }
            }
        }

        public override async Task RepollAsync(Device device)
        {
            var nodeNumber = Convert.ToByte(device.NodeNumber);

            if (!IsNodeReady(nodeNumber))
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "Re-poll node {0} failed, node not ready.", nodeNumber);
                return;
            }

            MManager.RequestNodeState(MHomeId, nodeNumber);
        }

        public override async Task ActivateGroupAsync(Group group)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var devices = await context.Devices
                    .Include(d => d.Type)
                    .Where(o => o.Type.Adapter.AdapterGuid == AdapterGuid)
                    .Where(o => o.Groups.Any(g => g.Id == group.Id))
                    .ToListAsync();

                foreach (var device in devices)
                {
                    var nodeNumber = Convert.ToByte(device.NodeNumber);
                    if (!IsNodeReady(nodeNumber))
                    {
                        await
                            Log.ReportInfoFormatAsync(CancellationToken,
                                "Failed to activate group member {0}, node {1}. Node not ready.", device.Name,
                                nodeNumber);
                        continue;
                    }

                    if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.Dimmer.ToString())
                    {
                        var value =
                            await
                                device.GetDeviceTypeValueAsync(
                                    OpenzWaveDeviceTypeSettings.DefaultDimmerOnLevel.ToString(), context);

                        if (value != null)
                        {
                            byte bValue = byte.TryParse(value, out bValue) ? bValue : (byte)99;
                            MManager.SetNodeLevel(MHomeId, (byte)device.NodeNumber, bValue);
                            continue;
                        }
                    }

                    MManager.SetNodeOn(MHomeId, nodeNumber);
                }
            }
        }

        public override async Task DeactivateGroupAsync(Group group)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var devices = await context.Devices
                    .Where(o => o.Type.Adapter.AdapterGuid == AdapterGuid)
                    .Where(o => o.Groups.Any(g => g.Id == group.Id))
                    .ToListAsync();

                foreach (var device in devices)
                {
                    var nodeNumber = Convert.ToByte(device.NodeNumber);
                    if (!IsNodeReady(nodeNumber))
                    {
                        await
                            Log.ReportInfoFormatAsync(CancellationToken,
                                "Failed to deactivate group member {0}, node {1}. Node not ready.", device.Name,
                                nodeNumber);
                        continue;
                    }

                    MManager.SetNodeOff(MHomeId, nodeNumber);
                }
            }
        }

        private bool IsNodeReady(byte nodeId)
        {
            return _nodesReady.Contains(nodeId);
        }

        private async Task AddNewDeviceToDatabase(byte nodeId)
        {
            #region Add device to database

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var ozwDevice = await context.Devices
                    .FirstOrDefaultAsync(d => d.Type.Adapter.AdapterGuid == AdapterGuid &&
                        d.NodeNumber == nodeId);

                //If already have the device, don't install a duplicate
                if (ozwDevice != null)
                    return;

                ozwDevice = new Device
                {
                    NodeNumber = nodeId,
                    DeviceTypeId = UnknownTypeId,
                    Name = "Unknown OpenZwave Device",
                    CurrentLevelInt = 0,
                    CurrentLevelText = ""
                };

                context.Devices.Add(ozwDevice);

                var result = await context.TrySaveChangesAsync(CancellationToken);
                if (result.HasError)
                    await Log.ReportErrorFormatAsync(CancellationToken, "Failed to save new device. {0}", result.Message);
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

        private int EvaluateOrder(string genre)
        {
            switch (genre)
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
            MNotification = notification;
            NotificationHandler();
            MNotification = null;
        }

        private readonly HashSet<int> _nodeValuesRepolling = new HashSet<int>();
        private async void NotificationHandler()
        {
            switch (MNotification.GetType())
            {

                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        #region NodeProtocolInfo

                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());

                        if (node != null)
                        {
                            node.Label = MManager.GetNodeType(MHomeId, node.ID);
                            Debug.WriteLine("[Node Protocol Info] " + node.Label);

                            //Find device type
                            var deviceTypeId = FindDeviceTypeId(node.Label);
                            if (deviceTypeId == UnknownTypeId)
                                await Log.ReportWarningFormatAsync(CancellationToken, "[Unknown Node Label] {0}", node.Label);

                            using (var context = new ZvsContext(EntityContextConnection))
                            {
                                var ozwDevice = await context.Devices
                                    .FirstOrDefaultAsync(d => d.Type.Adapter.AdapterGuid == AdapterGuid &&
                                        d.NodeNumber == node.ID);

                                //If we don't already have the device
                                if (ozwDevice == null)
                                    break;

                                if (ozwDevice.DeviceTypeId != deviceTypeId)
                                {
                                    ozwDevice.DeviceTypeId = deviceTypeId;

                                    var result = await context.TrySaveChangesAsync(CancellationToken);
                                    if (result.HasError)
                                        await Log.ReportErrorFormatAsync(CancellationToken, "Failed to change device type. {0}", result.Message);
                                }

                                #region Last Event Value Storage
                                //Node event value placeholder 
                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = ozwDevice.Id,
                                    UniqueIdentifier = LastEventNameValueId,
                                    Name = "Last Node Event Value",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.BYTE,
                                    CommandClass = "0",
                                    Value = "0",
                                    IsReadOnly = true
                                }, ozwDevice, CancellationToken);
                                #endregion
                            }
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.ValueAdded:
                    {
                        #region ValueAdded

                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                        var vid = MNotification.GetValueID();
                        var value = new Value
                        {
                            ValueID = vid,
                            Label = MManager.GetValueLabel(vid),
                            Genre = vid.GetGenre().ToString(),
                            Index = vid.GetIndex().ToString(CultureInfo.InvariantCulture),
                            Type = vid.GetType().ToString(),
                            CommandClassID = vid.GetCommandClassId().ToString(CultureInfo.InvariantCulture),
                            Help = MManager.GetValueHelp(vid)
                        };
                        var readOnly = MManager.IsValueReadOnly(vid);
                        node.AddValue(value);
                        var vIdString = vid.GetId().ToString(CultureInfo.InvariantCulture);

#if DEBUG
                        var sw = new Stopwatch();
                        sw.Start();
#endif

                        string data;
                        var b = MManager.GetValueAsString(vid, out data);

                        Debug.WriteLine("[ValueAdded] Node: {0}, Label: {1}, Data: {2}, result: {3}",
                            node.ID,
                            value.Label,
                            data,
                            b);

                        using (var context = new ZvsContext(EntityContextConnection))
                        {
                            var d = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == AdapterGuid &&
                                o.NodeNumber == node.ID);

                            if (d == null)
                            {
                                await Log.ReportWarningAsync("ValueAdded called on a node id that was not found in the database", CancellationToken);
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
                                IsReadOnly = readOnly
                            }, d, CancellationToken);

                            #region Install Dynamic Commands

                            if (!readOnly || !string.IsNullOrEmpty(value.Label))
                            {
                                var pType = TranslateDataType(vid.GetType());

                                var dynamicDc = new DeviceCommand
                                {
                                    Device = d,
                                    DeviceId = d.Id,
                                    UniqueIdentifier = string.Format("DYNAMIC_CMD_{0}_{1}", value.Label.ToUpper(), vid.GetId()),
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
                                    if (MManager.GetValueListItems(vid, out options))
                                        foreach (var option in options)
                                            dynamicDc.Options.Add(new CommandOption { Name = option });
                                }

                                await DeviceCommandBuilder.RegisterAsync(d.Id, dynamicDc, CancellationToken);

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
                            var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                            var vid = MNotification.GetValueID();
                            var val = node.GetValue(vid);

                            Debug.WriteLine("[ValueRemoved] Node:" + node.ID + ",Label:" + MManager.GetValueLabel(vid));

                            node.RemoveValue(val);
                            //TODO: Remove from values and command table
                        }
                        catch (Exception ex)
                        {
                            Log.ReportErrorFormatAsync(CancellationToken, "Value removed error. {0}", ex.Message).Wait();
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.ValueChanged:
                    {
                        #region ValueChanged
                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                        var vid = MNotification.GetValueID();
                        var value = new Value
                        {
                            ValueID = vid,
                            Label = MManager.GetValueLabel(vid),
                            Genre = vid.GetGenre().ToString(),
                            Index = vid.GetIndex().ToString(CultureInfo.InvariantCulture),
                            Type = vid.GetType().ToString(),
                            CommandClassID = vid.GetCommandClassId().ToString(CultureInfo.InvariantCulture),
                            Help = MManager.GetValueHelp(vid)
                        };
                        var readOnly = MManager.IsValueReadOnly(vid);

                        var data = GetValue(vid);
                        //m_manager.GetValueAsString(vid, out data);                          

                        Debug.WriteLine("[ValueChanged] Node:" + node.ID + ", Label:" + value.Label + ", Data:" + data);

                        using (var context = new ZvsContext(EntityContextConnection))
                        {
                            var device = await context.Devices
                                .Include(o => o.Type)
                                .FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == AdapterGuid &&
                                    o.NodeNumber == node.ID);

                            if (device == null)
                            {
                                await Log.ReportWarningAsync("ValueChanged called on a node id that was not found in the database", CancellationToken);
                                break;
                            }

                            //Update device value
                            await DeviceValueBuilder.RegisterAsync(new DeviceValue
                            {
                                DeviceId = device.Id,
                                UniqueIdentifier = vid.GetId().ToString(CultureInfo.InvariantCulture),
                                Name = value.Label,
                                Genre = value.Genre,
                                Index = value.Index,
                                CommandClass = value.CommandClassID,
                                Value = data,
                                ValueType = ConvertType(vid),
                                IsReadOnly = readOnly
                            }, device, CancellationToken);

                            #region Update Device Status Properties
                            //Update Current Status Field
                            var changed = false;
                            if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.Thermostat.ToString())
                            {
                                if (value.Label == "Temperature")
                                {
                                    double level;
                                    double.TryParse(data, out level);
                                    var levelTxt = string.Format("{0}° F", level);

                                    if (!device.CurrentLevelInt.Equals(level))
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
                            else if (device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.Switch.ToString())
                            {
                                if (value.Label == "Basic")
                                {
                                    double level;
                                    if (double.TryParse(data, out level))
                                    {
                                        var levelOnOff = level > 0 ? 100 : 0;
                                        var leveltxt = level > 0 ? "On" : "Off";

                                        if (!device.CurrentLevelInt.Equals(levelOnOff))
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
                                    bool state;
                                    if (bool.TryParse(data, out state))
                                    {
                                        var levelOnOff = state ? 100 : 0;
                                        var leveltxt = state ? "On" : "Off";

                                        if (!device.CurrentLevelInt .Equals(levelOnOff))
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
                                    double level;
                                    double.TryParse(data, out level);
                                    var levelInt = (int)level;
                                    var levelTxt = level + "%";

                                    if (!device.CurrentLevelInt.Equals(levelInt))
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
                                var result = await context.TrySaveChangesAsync(CancellationToken);
                                if (result.HasError)
                                    await Log.ReportErrorFormatAsync(CancellationToken, "Failed update device level. {0}", result.Message);
                            }
                            #endregion

                            #region Update Device Commands
                            if (!readOnly)
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

                                var vidId = vid.GetId().ToString(CultureInfo.InvariantCulture);
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
                            bool enableDimmerRepoll = bool.TryParse(await
                                device.GetDeviceSettingAsync(OpenzWaveDeviceTypeSettings.EnableRepollOnLevelChange.ToString(), context), out enableDimmerRepoll) && enableDimmerRepoll;

                            if (InitialPollingComplete &&
                                enableDimmerRepoll &&
                                device.Type.UniqueIdentifier == OpenzWaveDeviceTypes.Dimmer.ToString() &&
                                value.Label == "Basic")
                            {
                                //only allow each device to re-poll 1 time.
                                if (!_nodeValuesRepolling.Contains(device.NodeNumber))
                                {
                                    _nodeValuesRepolling.Add(device.NodeNumber);

                                    await Task.Delay(3500);
                                    MManager.RefreshValue(vid);
                                    Debug.WriteLine("Node {0} value re-polled", device.NodeNumber);

                                    //Do not allow another re-poll for 10 seconds
                                    await Task.Delay(10000);
                                    _nodeValuesRepolling.Remove(device.NodeNumber);
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
                        Debug.WriteLine("[Group]");
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
                        var node = new Node { ID = MNotification.GetNodeId(), HomeID = MNotification.GetHomeId() };
                        _mNodeList.Add(node);

                        Debug.WriteLine("[NodeAdded] ID:" + node.ID + " Added");
                        await AddNewDeviceToDatabase(node.ID);

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeNew:
                    {
                        #region NodeNew
                        // Add the new node to our list (and flag as uninitialized)
                        var node = new Node { ID = MNotification.GetNodeId(), HomeID = MNotification.GetHomeId() };
                        _mNodeList.Add(node);

                        Debug.WriteLine("[NodeNew] ID:" + node.ID + " Added");
                        await AddNewDeviceToDatabase(node.ID);

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeRemoved:
                    {
                        #region NodeRemoved

                        foreach (var node in _mNodeList.Where(node => node.ID == MNotification.GetNodeId()))
                        {
                            Debug.WriteLine("[NodeRemoved] ID:" + node.ID);
                            _mNodeList.Remove(node);
                            break;
                        }
                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeNaming:
                    {
                        #region NodeNaming
                        const string manufacturerNameValueId = "MN1";
                        const string productNameValueId = "PN1";
                        const string nodeLocationValueId = "NL1";
                        const string nodeNameValueId = "NN1";

                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());

                        if (node != null)
                        {
                            node.Manufacturer = MManager.GetNodeManufacturerName(MHomeId, node.ID);
                            node.Product = MManager.GetNodeProductName(MHomeId, node.ID);
                            node.Location = MManager.GetNodeLocation(MHomeId, node.ID);
                            node.Name = MManager.GetNodeName(MHomeId, node.ID);

                            Debug.WriteLine("[NodeNaming] Node:" + node.ID + ", Product:" + node.Product + ", Manufacturer:" + node.Manufacturer + ")");

                            using (var context = new ZvsContext(EntityContextConnection))
                            {
                                var device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == AdapterGuid &&
                                    o.NodeNumber == node.ID);

                                if (device == null)
                                {
                                    await Log.ReportWarningAsync("NodeNaming called on a node id that was not found in the database", CancellationToken);
                                    break;
                                }

                                //lets store the manufacturer name and product name in the values table.   
                                //Giving ManufacturerName a random value_id 9999058723211334120                                                           
                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = manufacturerNameValueId,
                                    Name = "Manufacturer Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Manufacturer,
                                    IsReadOnly = true
                                }, device, CancellationToken);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = productNameValueId,
                                    Name = "Product Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Product,
                                    IsReadOnly = true
                                }, device, CancellationToken);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = nodeLocationValueId,
                                    Name = "Node Location",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Location,
                                    IsReadOnly = true
                                }, device, CancellationToken);

                                await DeviceValueBuilder.RegisterAsync(new DeviceValue
                                {
                                    DeviceId = device.Id,
                                    UniqueIdentifier = nodeNameValueId,
                                    Name = "Node Name",
                                    Genre = "Custom",
                                    Index = "0",
                                    ValueType = DataType.STRING,
                                    CommandClass = "0",
                                    Value = node.Name,
                                    IsReadOnly = true
                                }, device, CancellationToken);
                            }
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeEvent:
                    {
                        #region NodeEvent
                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                        var gevent = MNotification.GetEvent();

                        if (node == null)
                            break;

                        await Log.ReportInfoFormatAsync(CancellationToken, "[NodeEvent] Node: {0}, Event Byte: {1}", node.ID, gevent);

                        using (var context = new ZvsContext(EntityContextConnection))
                        {
                            var device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == AdapterGuid &&
                                    o.NodeNumber == node.ID);

                            if (device == null)
                            {
                                await Log.ReportWarningAsync("NodeNaming called on a node id that was not found in the database", CancellationToken);
                                break;
                            }

                            var dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == device.Id &&
                                o.UniqueIdentifier == LastEventNameValueId);

                            //Node event value placeholder
                            if (dv == null)
                                break;

                            dv.Value = gevent.ToString(CultureInfo.InvariantCulture);

                            var result = await context.TrySaveChangesAsync(CancellationToken);
                            if (result.HasError)
                                await Log.ReportErrorFormatAsync(CancellationToken, "Failed to update device value. {0}", result.Message);

                            //Since open wave events are differently than values changes, we need to fire the value change event every time we receive the 
                            //event regardless if it is the same value or not.
                            //TODO:
                            //dv.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(dv.Id, dv.Value, string.Empty));
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.DriverReady:
                    {
                        #region DriverReady
                        _nodesReady.Clear();

                        MHomeId = MNotification.GetHomeId();
                        await Log.ReportInfoFormatAsync(CancellationToken, "Initializing...driver with Home ID 0x{0} is ready.", MHomeId.ToString("X8"));

                        break;
                        #endregion
                    }
                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        #region NodeQueriesComplete
                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                        if (node != null)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Initializing...node {0} queries complete", node.ID);

                            if (!_nodesReady.Contains(node.ID))
                                _nodesReady.Add(node.ID);

                            //await UpdateLastHeardFrom(node.ID);
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                    {
                        #region EssentialNodeQueriesComplete
                        var node = GetNode(MNotification.GetHomeId(), MNotification.GetNodeId());
                        if (node != null)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Initializing...node {0} essential queries complete", node.ID);

                            if (!_nodesReady.Contains(node.ID))
                                _nodesReady.Add(node.ID);

                            //await UpdateLastHeardFrom(node.ID);
                        }

                        break;
                        #endregion
                    }
                case ZWNotification.Type.AllNodesQueried:
                    {
                        #region AllNodesQueried
                        //This is an important message to see.  It tells you that you can start issuing commands
                        await Log.ReportInfoAsync("Ready:  All nodes queried", CancellationToken);
                        InitialPollingComplete = true;
                        MManager.WriteConfig(MNotification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.AllNodesQueriedSomeDead:
                    {
                        #region AllNodesQueriedSomeDead
                        //This is an important message to see.  It tells you that you can start issuing commands
                        await Log.ReportInfoAsync("Ready:  All nodes queried but some are dead.", CancellationToken);
                        InitialPollingComplete = true;
                        MManager.WriteConfig(MNotification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        #region AwakeNodesQueried
                        await Log.ReportInfoAsync("Ready:  Awake nodes queried (but not some sleeping nodes)", CancellationToken);
                        InitialPollingComplete = true;
                        MManager.WriteConfig(MNotification.GetHomeId());
                        await EnablePollingOnDevices();
                        break;
                        #endregion
                    }
                case ZWNotification.Type.PollingDisabled:
                    {
                        #region PollingDisabled
                        await Log.ReportInfoAsync("Polling disabled notification", CancellationToken);
                        break;
                        #endregion
                    }
                case ZWNotification.Type.PollingEnabled:
                    {
                        #region PollingEnabled
                        await Log.ReportInfoAsync("Polling enabled notification", CancellationToken);
                        break;
                        #endregion
                    }
                case ZWNotification.Type.SceneEvent:
                    {
                        #region SceneEvent
                        await Log.ReportInfoAsync("Scene event notification received", CancellationToken);
                        break;
                        #endregion
                    }
            }
        }

        private async Task EnablePollingOnDevices()
        {
            foreach (var n in _mNodeList)
                using (var context = new ZvsContext(EntityContextConnection))
                {
                    var n1 = n;
                    var device = await context.Devices.FirstOrDefaultAsync(o => o.Type.Adapter.AdapterGuid == AdapterGuid &&
                                    o.NodeNumber == n1.ID);

                    if (device == null)
                    {
                        await Log.ReportWarningAsync("EnablePollingOnDevices called on a node id that was not found in the database", CancellationToken);
                        continue;
                    }

                    bool enableRepoll = bool.TryParse(await device.GetDeviceSettingAsync(OpenzWaveDeviceTypeSettings.RepollingEnabled.ToString(), context), out enableRepoll) && enableRepoll;

                    if (enableRepoll)
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
        //            zvsEngine.log.Error(result.Message);
        //    }

        //}

        private static DataType ConvertType(ZWValueID zwValueId)
        {
            var dataType = DataType.NONE;
            var openZwaveVType = zwValueId.GetType();

            switch (openZwaveVType)
            {
                case ZWValueID.ValueType.Bool:
                    dataType = DataType.BOOL;
                    break;
                case ZWValueID.ValueType.Button:
                    dataType = DataType.STRING;
                    break;
                case ZWValueID.ValueType.Byte:
                    dataType = DataType.BYTE;
                    break;
                case ZWValueID.ValueType.Decimal:
                    dataType = DataType.DECIMAL;
                    break;
                case ZWValueID.ValueType.Int:
                    dataType = DataType.INTEGER;
                    break;
                case ZWValueID.ValueType.List:
                    dataType = DataType.LIST;
                    break;
                case ZWValueID.ValueType.Schedule:
                    dataType = DataType.STRING;
                    break;
                case ZWValueID.ValueType.Short:
                    dataType = DataType.SHORT;
                    break;
                case ZWValueID.ValueType.String:
                    dataType = DataType.STRING;
                    break;
            }
            return dataType;
        }

        private string GetValue(ZWValueID zValueId)
        {
            switch (zValueId.GetType())
            {
                case ZWValueID.ValueType.Bool:
                    bool r1;
                    MManager.GetValueAsBool(zValueId, out r1);
                    return r1.ToString();
                case ZWValueID.ValueType.Byte:
                    byte r2;
                    MManager.GetValueAsByte(zValueId, out r2);
                    return r2.ToString(CultureInfo.InvariantCulture);
                case ZWValueID.ValueType.Decimal:
                    decimal r3;
                    MManager.GetValueAsDecimal(zValueId, out r3);
                    return r3.ToString(CultureInfo.InvariantCulture);
                case ZWValueID.ValueType.Int:
                    int r4;
                    MManager.GetValueAsInt(zValueId, out r4);
                    return r4.ToString(CultureInfo.InvariantCulture);
                case ZWValueID.ValueType.List:
                    string r6;
                    MManager.GetValueListSelection(zValueId, out r6);
                    return r6;
                case ZWValueID.ValueType.Schedule:
                    return "Schedule";
                case ZWValueID.ValueType.Short:
                    short r7;
                    MManager.GetValueAsShort(zValueId, out r7);
                    return r7.ToString(CultureInfo.InvariantCulture);
                case ZWValueID.ValueType.String:
                    string r8;
                    MManager.GetValueAsString(zValueId, out r8);
                    return r8;
                default:
                    return "";
            }
        }

        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (var node in _mNodeList.Where(node => (node.ID == nodeId) && (node.HomeID == homeId)))
                return node;

            return new Node();
        }

        private void EnablePolling(byte nid)
        {
            try
            {
                var n = GetNode(MHomeId, nid);
                ZWValueID zv = null;
                switch (n.Label)
                {
                    case "Toggle Switch":
                    case "Binary Toggle Switch":
                    case "Binary Switch":
                    case "Binary Power Switch":
                    case "Binary Scene Switch":
                    case "Binary Toggle Remote Switch":
                        foreach (var v in n.Values.Where(v => v.Label == "Switch"))
                            zv = v.ValueID;
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
                        foreach (var v in n.Values.Where(v => v.Genre == "User" && v.Label == "Level"))
                            zv = v.ValueID;
                        break;
                    case "General Thermostat V2":
                    case "Heating Thermostat":
                    case "General Thermostat":
                    case "Setback Schedule Thermostat":
                    case "Setpoint Thermostat":
                    case "Setback Thermostat":
                        foreach (var v in n.Values.Where(v => v.Label == "Temperature"))
                            zv = v.ValueID;
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
                        foreach (var v in n.Values.Where(v => v.Genre == "User" && v.Label == "Basic"))
                            zv = v.ValueID;
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
                        foreach (var v in n.Values.Where(v => v.Genre == "User" && v.Label == "Basic"))
                            zv = v.ValueID;
                        break;
                }
                if (zv != null)
                    MManager.EnablePoll(zv);
            }
            catch (Exception ex)
            {
                Log.ReportErrorFormatAsync(CancellationToken, "Error attempting to enable polling: {0}", ex.Message).Wait();
            }
        }

        #endregion




    }
}
