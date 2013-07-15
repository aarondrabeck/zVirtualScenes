//using System.Linq;
//using System;

//using System.ComponentModel;
//using zvs.Entities;

//namespace zvs.Processor
//{
//    public abstract class zvsPlugin
//    {
//        public string UniqueIdentifier { get; private set; }
//        public string Name { get; private set; }
//        public string Description { get; private set; }
//        public Core Core { get; set; }

//        Logging.ILog log = Logging.LogManager.GetLogger<zvsPlugin>();

//        public bool IsRunning { get; private set; }
//        public bool IsReady { get; protected set; }
//        public bool ListenForStateChanges { get; protected set; }

//        protected zvsPlugin(string uniqueIdentifier, string name, string description)
//        {
//            this.UniqueIdentifier = uniqueIdentifier;
//            this.Name = name;
//            this.Description = description;

//            using (var context = new zvsContext())
//            {
//                Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);

//                if (pl != null)
//                {
//                    pl.Name = Name;
//                    pl.Description = Description;
//                }
//                else
//                {
//                    pl = new Adapter
//                    {
//                        UniqueIdentifier = uniqueIdentifier,
//                        Name = name,
//                        Description = description
//                    };
//                    context.Plugins.Add(pl);
//                }
//                string SaveError = string.Empty;
//                if (!context.TrySaveChanges(out SaveError))
//                    log.Error(SaveError);
//            }
//        }

//        public void Start()
//        {
//            if (Enabled)
//            {
//                BackgroundWorker bw = new BackgroundWorker();
//                bw.DoWork += (s, a) =>
//                {
//                    StartPlugin();
//                    IsRunning = true;
//                };
//                bw.RunWorkerAsync();
//            }
//        }

//        public void Stop()
//        {
//            BackgroundWorker bw = new BackgroundWorker();
//            bw.DoWork += (s, a) =>
//            {
//                StopPlugin();
//                IsRunning = false;
//            };
//            bw.RunWorkerAsync();
//        }

//        public bool Enabled
//        {
//            get
//            {
//                using (var context = new zvsContext())
//                {
//                    Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);

//                    if (pl != null)
//                        return pl.isEnabled;
//                    else
//                        return false;
//                }
//            }
//            set
//            {
//                using (var context = new zvsContext())
//                {
//                    Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);

//                    if (pl != null)
//                    {
//                        pl.isEnabled = value;
//                        string SaveError = string.Empty;
//                        if (!context.TrySaveChanges(out SaveError))
//                            log.Error(SaveError);
//                    }
//                }
//            }
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        public void UpdateDeviceValue(int deviceId, string UniqueIdentifier, string Value, zvsContext context)
//        {
//            Device d = context.Devices.FirstOrDefault(o => o.Id == deviceId);
//            if (d != null)
//            {
//                DeviceValue existing_dv = d.Values.FirstOrDefault(o => o.UniqueIdentifier == UniqueIdentifier);
//                if (existing_dv != null)
//                {
//                    DeviceValue changed_dv = new DeviceValue();
//                    changed_dv.Device = existing_dv.Device;
//                    changed_dv.ValueType = existing_dv.ValueType;
//                    changed_dv.CommandClass = existing_dv.CommandClass;
//                    changed_dv.Description = existing_dv.Description;
//                    changed_dv.Name = existing_dv.Name;
//                    changed_dv.Genre = existing_dv.Genre;
//                    changed_dv.Index = existing_dv.Index;
//                    changed_dv.isReadOnly = existing_dv.isReadOnly;
//                    changed_dv.CustomData1 = existing_dv.CustomData1;
//                    changed_dv.CustomData2 = existing_dv.CustomData2;

//                    changed_dv.Value = Value;
//                    DefineOrUpdateDeviceValue(changed_dv, context);
//                }
//            }
//        }

//        public void DefineOrUpdateDeviceValue(DeviceValue dv, zvsContext context, bool IgnoreValueChange = false)
//        {
//            Device d = context.Devices.FirstOrDefault(o => o.Id == dv.Device.Id);
//            if (d != null)
//            {
//                DeviceValue existing_dv = d.Values.FirstOrDefault(o => o.UniqueIdentifier == dv.UniqueIdentifier);
//                string prev_value = string.Empty;

//                if (existing_dv == null)
//                {
//                    //NEW VALUE
//                    d.Values.Add(dv);
//                    string SaveError = string.Empty;
//                    if (!context.TrySaveChanges(out SaveError))
//                        log.Error(SaveError);

//                    //Call Event
//                    dv.DeviceValueAdded(new System.EventArgs());
//                }
//                else
//                {

//                    //CHANGED VALUE
//                    prev_value = existing_dv.Value;

//                    //values come in blank sometimes.  If they are blank, keep the DB value. 
//                    if (!IgnoreValueChange && !string.IsNullOrEmpty(dv.Value))
//                        existing_dv.Value = dv.Value;

//                    existing_dv.ValueType = dv.ValueType;
//                    existing_dv.CommandClass = dv.CommandClass;
//                    existing_dv.Description = dv.Description;
//                    existing_dv.Name = dv.Name;
//                    existing_dv.Genre = dv.Genre;
//                    existing_dv.Index = dv.Index;
//                    existing_dv.isReadOnly = dv.isReadOnly;
//                    existing_dv.CustomData1 = dv.CustomData1;
//                    existing_dv.CustomData2 = dv.CustomData2;
//                    string SaveError = string.Empty;
//                    if (!context.TrySaveChanges(out SaveError))
//                        log.Error(SaveError);

//                    if (!IgnoreValueChange && !string.IsNullOrEmpty(dv.Value) && (string.IsNullOrEmpty(prev_value) || !prev_value.Equals(dv.Value)))
//                    {
//                        //LOG IT
//                        string device_name = "Unknown";
//                        if (String.IsNullOrEmpty(d.Name))
//                            device_name = "Device #" + d.Id;
//                        else
//                            device_name = d.Name;

//                        this.Core.Dispatcher.Invoke(new Action(() =>
//                        {
//                            if (!String.IsNullOrEmpty(prev_value))
//                                log.InfoFormat("{0} {1} changed from {2} to {3}.", device_name, dv.Name, prev_value, dv.Value);//event
//                            else
//                                log.InfoFormat("{0} {1} changed to {2}.", device_name, dv.Name, dv.Value); //event
//                        }));

//                        //Call Event
//                        dv.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(existing_dv.Id,  dv.Value, prev_value));
//                    }
//                }
//            }
//            else
//            {
//                this.Core.Dispatcher.Invoke(new Action(() =>
//                {
//                    log.InfoFormat("Device value change event on '{0}' occurred but could not find a device value with id {1} in database.", dv.Name, dv.Id);//, "EVENT"); 
//                }));
//            }

//        }

//       

//        public void SetSetting(string settingUniqueIdentifier, string settingValue, zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                AdapterSetting ps = pl.Settings.FirstOrDefault(p => p.UniqueIdentifier == settingUniqueIdentifier);
//                if (ps != null)
//                {
//                    ps.Value = settingValue;
//                    string SaveError = string.Empty;
//                    if (!context.TrySaveChanges(out SaveError))
//                        log.Error(SaveError);
//                }
//            }
//        }

//        public string GetSettingValue(string settingUniqueIdentifier, zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                AdapterSetting ps = pl.Settings.FirstOrDefault(p => p.UniqueIdentifier == settingUniqueIdentifier);
//                if (ps != null)
//                {
//                    return ps.Value;
//                }
//            }
//            return string.Empty;
//        }

//        public IQueryable<Device> GetMyPluginsDevices(zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                return context.Devices.Where(d => d.Type.Plugin.UniqueIdentifier == pl.UniqueIdentifier);
//            }
//            return null;
//        }

//        public void DefineOrUpdateDeviceType(DeviceType dt, zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                //Does device type exist? 
//                DeviceType existing_dt = pl.DeviceTypes.FirstOrDefault(d => d.UniqueIdentifier == dt.UniqueIdentifier);

//                if (existing_dt == null)
//                {
//                    pl.DeviceTypes.Add(dt);
//                }
//                else
//                {
//                    existing_dt.Name = dt.Name;
//                    existing_dt.ShowInList = dt.ShowInList;

//                    foreach (DeviceTypeCommand dtc in dt.Commands)
//                    {
//                        DeviceTypeCommand existing_dtc = existing_dt.Commands.SingleOrDefault(d => d.UniqueIdentifier == dtc.UniqueIdentifier);

//                        if (existing_dtc == null)
//                        {
//                            existing_dt.Commands.Add(dtc);
//                        }
//                        else
//                        {
//                            existing_dtc.Name = dtc.Name;
//                            existing_dtc.Help = dtc.Help;
//                            existing_dtc.CustomData1 = dtc.CustomData1;
//                            existing_dtc.CustomData2 = dtc.CustomData2;
//                            existing_dtc.ArgumentType = dtc.ArgumentType;
//                            existing_dtc.Description = dtc.Description;

//                            existing_dtc.Options.ToList().ForEach(o =>
//                            {
//                                context.CommandOptions.Remove(o);
//                            });
//                            existing_dtc.Options.Clear();
//                            dtc.Options.ToList().ForEach(o => existing_dtc.Options.Add(o));
//                        }
//                    }
//                }
//                string SaveError = string.Empty;
//                if (!context.TrySaveChanges(out SaveError))
//                    log.Error(SaveError);
//            }
//        }

//        public IQueryable<Device> GetDeviceInGroup(int GroupID, zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                Group g = context.Groups.FirstOrDefault(gr => gr.Id == GroupID);
//                if (g != null)
//                {
//                    return g.Devices.Where(o => o.Type.Plugin == pl).AsQueryable();
//                }
//            }
//            return null;
//        }

//        public DeviceType GetDeviceType(string DeviceTypeUniqueIdentifier, zvsContext context)
//        {
//            Adapter pl = context.Plugins.FirstOrDefault(p => p.UniqueIdentifier == this.UniqueIdentifier);
//            if (pl != null)
//            {
//                return pl.DeviceTypes.FirstOrDefault(dt => dt.UniqueIdentifier == DeviceTypeUniqueIdentifier);
//            }
//            return null;

//        }

//        public void DefineOrUpdateDeviceCommand(DeviceCommand dc, zvsContext context)
//        {
//            Device d = context.Devices.FirstOrDefault(o => o.Id == dc.Device.Id);
//            if (d != null)
//            {
//                //Does device type exist? 
//                DeviceCommand existing_dc = d.Commands.FirstOrDefault(c => c.UniqueIdentifier == dc.UniqueIdentifier);

//                if (existing_dc == null)
//                {
//                    d.Commands.Add(dc);
//                }
//                else
//                {
//                    existing_dc.ArgumentType = dc.ArgumentType;
//                    existing_dc.CustomData1 = dc.CustomData1;
//                    existing_dc.CustomData2 = dc.CustomData2;
//                    existing_dc.Description = dc.Description;
//                    existing_dc.Name = dc.Name;
//                    existing_dc.Help = dc.Help;
//                    existing_dc.SortOrder = dc.SortOrder;

//                    existing_dc.Options.ToList().ForEach(o =>
//                    {
//                        context.CommandOptions.Remove(o);
//                    });
//                    existing_dc.Options.Clear();

//                    dc.Options.ToList().ForEach(o => existing_dc.Options.Add(o));
//                }
//                string SaveError = string.Empty;
//                if (!context.TrySaveChanges(out SaveError))
//                   log.Error(SaveError);
//            }
//        }

//        public void SettingsChange(AdapterSetting ps)
//        {
//            BackgroundWorker bw = new BackgroundWorker();
//            bw.DoWork += (s, a) =>
//            {
//                SettingChanged(ps.UniqueIdentifier, ps.Value);
//            };
//            bw.RunWorkerAsync();
//        }

//        // Abstract functions
//        protected abstract void StartPlugin();
//        protected abstract void StopPlugin();
//        protected abstract void SettingChanged(string settingsettingUniqueIdentifier, string settingValue);
//        public abstract void Initialize();
//        public abstract void ProcessCommand(int queuedCommandId);
//        public abstract void Repoll(Device device);
//        public abstract void ActivateGroup(int groupID);
//        public abstract void DeactivateGroup(int groupID);

//    }
//}
