using System.Linq;
using System.Data.Objects;
using System;
using zVirtualScenesModel;

namespace zVirtualScenes
{
    public abstract class Plugin
    {
        protected string _name;
        protected string Friendly_Name;
        protected string Description;
        public Core Core;

        public bool IsRunning { get; private set; }
        public bool IsReady { get; protected set; }
        public bool ListenForStateChanges { get; protected set; }

        protected Plugin(string Plugin_Name, string Plugin_Friendly_Name, string Plugin_Description)
        {
            _name = Plugin_Name;
            Friendly_Name = Plugin_Friendly_Name;
            Description = Plugin_Description;

            using (var context = new zvsLocalDBEntities())
            {
                plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);

                if (pl != null)
                {
                    pl.friendly_name = Friendly_Name;
                    pl.description = Description;
                }
                else
                {
                    pl = new plugin { name = _name, friendly_name = Friendly_Name, description = Description };
                    context.plugins.Add(pl);
                }
                context.SaveChanges();
            }
        }

        public void Start()
        {
            if (Enabled)
            {
                StartPlugin();
                IsRunning = true;
            }
        }

        public void Stop()
        {
            StopPlugin();
            IsRunning = false;
        }

        public bool Enabled
        {
            get
            {
                using (var context = new zvsLocalDBEntities())
                {
                    plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);

                    if (pl != null)
                        return pl.enabled;
                    else
                        return false;
                }
            }
            set
            {
                using (var context = new zvsLocalDBEntities())
                {
                    plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);

                    if (pl != null)
                    {
                        pl.enabled = value;
                        context.SaveChanges();
                    }
                }
            }
        }

        public override string ToString()
        {
            return Friendly_Name;
        }

        public string FriendlyName
        {
            get
            {
                return Friendly_Name;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public void DefineOrUpdateDeviceValue(device_values dv, zvsLocalDBEntities context, bool IgnoreValueChange = false)
        {

            device d = context.devices.FirstOrDefault(o => o.id == dv.device_id);
            if (d != null)
            {
                device_values existing_dv = d.device_values.FirstOrDefault(o => o.value_id == dv.value_id);
                string prev_value = string.Empty;

                if (existing_dv == null)
                {
                    //NEW VALUE
                    d.device_values.Add(dv);
                    context.SaveChanges();

                    //Call Event
                    dv.DeviceValueAdded(new System.EventArgs());
                }
                else
                {

                    //CHANGED VALUE
                    prev_value = existing_dv.value2;

                    //values come in blank sometimes.  If they are blank, keep the DB value. 
                    if (!IgnoreValueChange && !string.IsNullOrEmpty(dv.value2))
                        existing_dv.value2 = dv.value2;

                    existing_dv.type = dv.type;
                    existing_dv.label_name = dv.label_name;
                    existing_dv.index2 = dv.index2;
                    existing_dv.genre = dv.genre;
                    existing_dv.commandClassId = dv.commandClassId;
                    existing_dv.read_only = dv.read_only;
                    context.SaveChanges();

                    if (!IgnoreValueChange && !string.IsNullOrEmpty(dv.value2) && (string.IsNullOrEmpty(prev_value) || !prev_value.Equals(dv.value2)))
                    {
                        //LOG IT
                        string device_name = "Unknown";
                        if (String.IsNullOrEmpty(d.friendly_name))
                            device_name = "Device #" + d.id;
                        else
                            device_name = d.friendly_name;

                        this.Core.Dispatcher.Invoke(new Action(() =>
                        {
                            if (!String.IsNullOrEmpty(prev_value))
                                Core.Logger.WriteToLog(Urgency.INFO, string.Format("{0} {1} changed from {2} to {3}.", device_name, dv.label_name, prev_value, dv.value2), "EVENT");
                            else
                                Core.Logger.WriteToLog(Urgency.INFO, string.Format("{0} {1} changed to {2}.", device_name, dv.label_name, dv.value2), "EVENT");
                        }));

                        //Call Event
                        dv.DeviceValueDataChanged(new device_values.ValueDataChangedEventArgs(existing_dv.id, prev_value, dv.value2));
                    }
                }
            }
            else
            {
                this.Core.Dispatcher.Invoke(new Action(() =>
                {
                    Core.Logger.WriteToLog(Urgency.WARNING, string.Format("Device value change event on '{0}' occurred but could not find a device value with id {1} in database.", dv.label_name, dv.value_id), "EVENT");
                }));
            }

        }

        public void DefineOrUpdateSetting(plugin_settings ps, zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                plugin_settings existing_ps = pl.plugin_settings.FirstOrDefault(pls => pls.name == ps.name);
                if (existing_ps == null)
                {
                    pl.plugin_settings.Add(ps);
                }
                else
                {
                    existing_ps.description = ps.description;
                    existing_ps.friendly_name = ps.friendly_name;
                    existing_ps.name = ps.name;
                    existing_ps.value_data_type = ps.value_data_type;
                }
                context.SaveChanges();
            }
        }

        public void SetSetting(string settingName, string settingValue, zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                plugin_settings ps = pl.plugin_settings.FirstOrDefault(p => p.name == settingName);
                if (ps != null)
                {
                    ps.value = settingValue;
                    context.SaveChanges();
                }
            }
        }

        public string GetSettingValue(string settingName, zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                plugin_settings ps = pl.plugin_settings.FirstOrDefault(p => p.name == settingName);
                if (ps != null)
                {
                    return ps.value;
                }
            }
            return string.Empty;
        }

        public IQueryable<device> GetMyPluginsDevices(zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                return context.devices.Where(d => d.device_types.plugin.name == pl.name);
            }
            return null;
        }

        public void DefineOrUpdateDeviceType(device_types dt, zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                //Does device type exist? 
                device_types existing_dt = pl.device_types.FirstOrDefault(d => d.name == dt.name);

                if (existing_dt == null)
                {
                    pl.device_types.Add(dt);
                }
                else
                {
                    existing_dt.friendly_name = dt.friendly_name;
                    existing_dt.show_in_list = dt.show_in_list;

                    foreach (device_type_commands dtc in dt.device_type_commands)
                    {
                        device_type_commands exsisting_dtc = existing_dt.device_type_commands.FirstOrDefault(d => d.name == dtc.name);

                        if (exsisting_dtc == null)
                        {
                            existing_dt.device_type_commands.Add(dtc);
                        }
                        else
                        {
                            exsisting_dtc.friendly_name = dtc.friendly_name;
                            exsisting_dtc.help = dtc.help;
                            exsisting_dtc.custom_data2 = dtc.custom_data2;
                            exsisting_dtc.custom_data1 = dtc.custom_data1;
                            exsisting_dtc.arg_data_type = dtc.arg_data_type;
                            exsisting_dtc.description = dtc.description;

                            foreach (var option in context.device_type_command_options.Where(o => o.device_type_command_id == exsisting_dtc.id).ToArray())
                            {
                                context.device_type_command_options.Remove(option);
                            }

                            foreach (device_type_command_options dtco in dtc.device_type_command_options)
                                exsisting_dtc.device_type_command_options.Add(new device_type_command_options { options = dtco.options });
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public IQueryable<device> GetDeviceInGroup(int GroupID, zvsLocalDBEntities context)
        {
            plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
            if (pl != null)
            {
                group g = context.groups.FirstOrDefault(gr => gr.id == GroupID);
                if (g != null)
                {
                    return g.group_devices.Where(gd => gd.device.device_types.plugin == pl).Select(d => d.device).AsQueryable();
                }
            }
            return null;
        }

        public device_types GetDeviceType(string DeviceTypeName, zvsLocalDBEntities context)
        {
            lock (context)
            {
                plugin pl = context.plugins.FirstOrDefault(p => p.name == this._name);
                if (pl != null)
                {
                    return pl.device_types.FirstOrDefault(dt => dt.name == DeviceTypeName);
                }
                return null;
            }
        }

        public void DefineOrUpdateDeviceCommand(device_commands dc, zvsLocalDBEntities context)
        {
            device d = context.devices.FirstOrDefault(o => o.id == dc.device_id);
            if (d != null)
            {
                //Does device type exist? 
                device_commands existing_dc = d.device_commands.FirstOrDefault(c => c.name == dc.name);

                if (existing_dc == null)
                {
                    d.device_commands.Add(dc);
                }
                else
                {
                    existing_dc.help = dc.help;
                    existing_dc.friendly_name = dc.friendly_name;
                    existing_dc.description = dc.description;
                    existing_dc.custom_data2 = dc.custom_data2;
                    existing_dc.custom_data1 = dc.custom_data1;
                    existing_dc.arg_data_type = dc.arg_data_type;

                    existing_dc.device_command_options.Clear();

                    foreach (var option in context.device_command_options.Where(o => o.device_command_id == existing_dc.id).ToArray())
                    {
                        context.device_command_options.Remove(option);
                    }

                    foreach (device_command_options o in dc.device_command_options)
                        existing_dc.device_command_options.Add(new device_command_options { name = o.name });

                    existing_dc.sort_order = dc.sort_order;
                }
                context.SaveChanges();
            }
        }

        public void WriteToLog(Urgency u, string message)
        {
            this.Core.Dispatcher.Invoke(new Action(() =>
            {
                Core.Logger.WriteToLog((Urgency)u, message, this.Friendly_Name);
            }));
        }

        // Abstract functions
        protected abstract void StartPlugin();
        protected abstract void StopPlugin();
        protected abstract void SettingChanged(string settingName, string settingValue);
        public abstract void Initialize();
        public abstract bool ProcessDeviceCommand(device_command_que cmd);
        public abstract bool ProcessDeviceTypeCommand(device_type_command_que cmd);
        public abstract bool Repoll(device device);
        public abstract bool ActivateGroup(int groupID);
        public abstract bool DeactivateGroup(int groupID);

    }
}
