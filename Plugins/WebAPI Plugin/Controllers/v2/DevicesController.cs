using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using WebAPI.Cors;
using zvs.Entities;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation("v2/Devices", 2.1, "All devices.")]
    public class DevicesController : zvsControllerBase<Device>
    {
        ILog log = LogManager.GetLogger<DevicesController>();

        protected override DbSet DBSet
        {
            get { return db.Devices; }
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<Device> Get()
        {
            //Check authorization
            bool isAuthorized = base.isAuthorized;

            List<Device> devices = new List<Device>();
            foreach (Device d in db.Devices.OrderBy(o => o.Name))
            {
                bool show = true;
                bool.TryParse(DevicePropertyValue.GetDevicePropertyValue(db, d, "HTTPAPI_SHOW"), out show);

                if (show) devices.Add(d);
            }
            return devices.AsQueryable();
        }

        [EnableCors]
        [HttpGet]
        public new HttpResponseMessage GetById(int id)
        {
            return base.GetById(id);
        }

        [EnableCors]
        [HttpPost]
        public new HttpResponseMessage Add(Device tEntityPost)
        {
            return base.Add(tEntityPost);
        }

        [EnableCors]
        [HttpPatch]
        [HttpPut]
        public new HttpResponseMessage Update(int id, Delta<Device> tEntityPatch)
        {
            return base.Update(id, tEntityPatch);
        }

        [EnableCors]
        [HttpDelete]
        public new HttpResponseMessage Remove(int id)
        {
            return base.Remove(id);
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<object> GetNestedCollections(int parentId, string nestedCollectionName)
        {
            return base.GetNestedCollections(parentId, nestedCollectionName);
        }


        //public object Get(string name)
        //{
        //    base.Log(log, "name=", name);
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        using (zvsContext context = new zvsContext())
        //        {
        //            Device d = context.Devices.FirstOrDefault(o => o.Name == name);

        //            if (d != null)
        //            {
        //                return Get(d.Id);
        //            }
        //        }
        //    }
        //    return null;
        //}
        //public object Get(int id)
        //{
        //    base.Log(log, "id=", id);
        //    if (id > 0)
        //    {
        //        using (zvsContext context = new zvsContext())
        //        {
        //            Device d = context.Devices.FirstOrDefault(o => o.Id == id);

        //            if (d != null)
        //            {
        //                List<object> values = new List<object>();
        //                foreach (DeviceValue v in d.Values)
        //                {
        //                    values.Add(new
        //                    {
        //                        value_id = v.UniqueIdentifier,
        //                        value = v.Value,
        //                        grene = v.Genre,
        //                        index2 = v.Index,
        //                        read_only = v.isReadOnly,
        //                        label_name = v.Name,
        //                        type = v.ValueType,
        //                        id = v.Id
        //                    });
        //                }

        //                return new { success = true, values = values.ToArray() };
        //            }
        //        }
        //    }
        //    return new { success = false, reason = "Device not found." };
        //}

        //public object Get(int id, string action)
        //{
        //    base.Log(log, "id=", id, "action=", action);

        //    if (id > 0 && action.ToLower() == "values")
        //    {
        //        using (zvsContext context = new zvsContext())
        //        {
        //            Device d = context.Devices.FirstOrDefault(o => o.Id == id);

        //            if (d != null)
        //            {
        //                double level = 0;

        //                if (d.CurrentLevelInt.HasValue)
        //                    level = d.CurrentLevelInt.Value;

        //                string on_off = string.Empty;
        //                if (level == 0)
        //                    on_off = "OFF";
        //                else if (level > 98)
        //                    on_off = "ON";
        //                else
        //                    on_off = "DIM";

        //                StringBuilder sb = new StringBuilder();
        //                d.Groups.ToList().ForEach((o) => sb.Append(o.Name + " "));

        //                var details = new
        //                {
        //                    id = d.Id,
        //                    name = d.Name,
        //                    on_off = on_off,
        //                    level = d.CurrentLevelInt,
        //                    level_txt = d.CurrentLevelText,
        //                    type = d.Type.UniqueIdentifier,
        //                    type_txt = d.Type.Name,
        //                    last_heard_from = d.LastHeardFrom.HasValue ? d.LastHeardFrom.Value.ToString() : "",
        //                    groups = sb.ToString(),
        //                    mode = d.Values.FirstOrDefault(o => o.Name == "Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Mode").Value,
        //                    fan_mode = d.Values.FirstOrDefault(o => o.Name == "Fan Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan Mode").Value,
        //                    op_state = d.Values.FirstOrDefault(o => o.Name == "Operating State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Operating State").Value,
        //                    fan_state = d.Values.FirstOrDefault(o => o.Name == "Fan State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan State").Value,
        //                    heat_p = d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1").Value,
        //                    cool_p = d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1").Value,
        //                    esm = d.Values.FirstOrDefault(o => o.Name == "SetBack Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "SetBack Mode").Value,
        //                    plugin_name = d.Type.Plugin.UniqueIdentifier
        //                };
        //                return new { success = true, details = details };
        //            }
        //        }
        //    }
        //    return new { success = false, reason = "Device not found." };

        //}
    }
}
