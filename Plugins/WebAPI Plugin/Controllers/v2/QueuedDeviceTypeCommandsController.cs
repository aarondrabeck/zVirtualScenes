using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using WebAPI.Cors;
using WebAPI.DTO;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation("v2/QueuedDeviceTypeCommands", 2.1,
    @"All available queued device commands.

    Post to execute a command, Example: 
    {
                ""CommandId"": 35,         
                ""Argument"": ""7"",
                ""DeviceId"": ""9""
    }

    ")]
    public class QueuedDeviceTypeCommandsController : zvsEntityController<QueuedDeviceTypeCommand>
    {
        public QueuedDeviceTypeCommandsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override DbSet DBSet
        {
            get { return db.QueuedCommands; }
        }

        protected override IQueryable<QueuedDeviceTypeCommand> BaseQueryable
        {
            get
            {
                return base.BaseQueryable.OfType<QueuedDeviceTypeCommand>();
            }
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<QueuedDeviceTypeCommand> Get()
        {
            return base.Get();
        }

        [EnableCors]
        [HttpGet]
        public new HttpResponseMessage GetById(int id)
        {
            return base.GetById(id);
        }

        [EnableCors]
        [HttpPost]
        public new HttpResponseMessage Add(dynamic cmd)
        {
            DenyUnauthorized();

            if (cmd == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "No data posted");

            CommandProcessor cp = new CommandProcessor(WebAPIPlugin.Core);
            try
            {
                Task.Run(() => cp.RunDeviceTypeCommandAsync((int)cmd.CommandId,(int)cmd.DeviceId, (string)cmd.Argument));
            }
            catch (DbEntityValidationException dbEx)
            {
                List<object> ValidationErrors = new List<object>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ValidationErrors.Add(new
                        {
                            Property = validationError.PropertyName,
                            Error = validationError.ErrorMessage
                        });
                    }
                }

                Dictionary<string, object> ResponseDictionary = new Dictionary<string, object>();
                ResponseDictionary.Add("ValidationErrors", ValidationErrors);
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Bad Request", null, ResponseDictionary));
            }
            catch (Exception ex)
            {
                string ErrMsg = (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message)) ? ex.InnerException.Message : ex.Message;
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Validation Errors", new { ValidationErrors = ErrMsg }));
            }

            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.Created, "Command queued and will be processed.");
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
        public new IQueryable<object> GetNestedCollection(int parentId, string nestedCollectionName)
        {
            return base.GetNestedCollection(parentId, nestedCollectionName);
        }
    }
}
