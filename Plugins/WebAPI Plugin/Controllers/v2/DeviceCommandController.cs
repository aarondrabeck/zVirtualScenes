using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation(typeof (DeviceCommand), "v2/DeviceCommand", 2.1, "Execute device commands.")]
    public class DeviceCommandController : zvsEntityController<DeviceCommand>
    {

        public DeviceCommandController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin)
        {
        }

        [HttpPost]
        public new async Task<HttpResponseMessage> Post(DeviceCommandParameters deviceCommand)
        {
            return await Execute(deviceCommand);
        }

        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new async Task<IQueryable<DeviceCommand>> Get()
        {
            return await base.Get();
        }

        private async Task<DeviceCommandParameters> ResolveIds(DeviceCommandParameters deviceCommand)
        {
            if (deviceCommand.DeviceId <= 0 && !string.IsNullOrEmpty(deviceCommand.DeviceName))
            {

                using (zvsContext context = new zvsContext())
                {
                    Device d = await context.Devices.Where(o => o.Name == deviceCommand.DeviceName).FirstOrDefaultAsync();
                    if (d != null) deviceCommand.DeviceId = d.Id;
                }
            }
            if (deviceCommand.CommandId <= 0 && !string.IsNullOrEmpty(deviceCommand.CommandName))
            {

                using (zvsContext context = new zvsContext())
                {
                    DeviceCommand d = await context.DeviceCommands.Where(o => o.Name == deviceCommand.CommandName && o.DeviceId == deviceCommand.DeviceId).FirstOrDefaultAsync();
                    if (d != null) deviceCommand.CommandId = d.Id;
                }
            }
            return deviceCommand;
        }

        private async Task<HttpResponseMessage> Execute(DeviceCommandParameters deviceCommand)
        {
            try
            {

                DenyUnauthorized();

                deviceCommand = await ResolveIds(deviceCommand);

                DeviceCommand dc =
                    await
                        BaseQueryable.Where(o => o.DeviceId == deviceCommand.DeviceId && o.Id == deviceCommand.CommandId)
                            .SingleOrDefaultAsync();

                if (dc != null)
                {
                    CommandProcessor cp = new CommandProcessor(this.WebAPIPlugin.Core);
                    var result = await Task.Run(async () => await cp.RunCommandAsync(this, dc, deviceCommand.Value));
                    if (result.HasErrors)
                    {
                        return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, result.Message);
                    }
                    else
                    {
                        return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, result.Message);
                    }
                }
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest,
                    "Invalid device or command id.");
            }
            catch (Exception e)
            {
                log.Fatal(e);
                throw;
            }
        }

    }

    public class DeviceCommandParameters
    {
        public int DeviceId { get; set; }
        public int CommandId { get; set; }
        public string Value { get; set; }
        public string DeviceName { get; set; }
        public string CommandName { get; set; }
    }
}