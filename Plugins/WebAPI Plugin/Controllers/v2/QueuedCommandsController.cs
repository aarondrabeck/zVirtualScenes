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

using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation("v2/QueuedCommands", 2.1,
    @"All available queued command.

        POST to execute commands: 

            Example DeviceCommand where CommandId 27 = Set Basic, Argument 7 = desired level
            POST:        
            {
                CommandId: 27,
                Argument: ""7""
            }

            Example DeviceTypeCommand where CommandId 19 = Turn Off, Argument2 2 = device with Id of 2
            POST:        
            {
                CommandId: 19,
                Argument2: ""2""
            }

            Example Built-in Command where CommandId 2 = Built-in Command REPOLL_ALL
            POST:        
            {
                CommandId: 2
            }
    ")]
    public class QueuedCommandsController : zvsEntityController<QueuedCommand>
    {
        public QueuedCommandsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override DbSet DBSet
        {
            get { return db.QueuedCommands; }
        }

        
        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new IQueryable<QueuedCommand> Get()
        {
            return base.Get();
        }

        
        [HttpGet]
        public new HttpResponseMessage GetById(int id)
        {
            return base.GetById(id);
        }

        
        [HttpPost]
        public new HttpResponseMessage Add(QueuedCommand cmd)
        {
            DenyUnauthorized();

            if (cmd == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "No data posted");

            CommandProcessor cp = new CommandProcessor(WebAPIPlugin.Core);
            try
            {
                Task.Run(() => cp.RunCommandAsync(this, cmd.CommandId.Value, cmd.Argument, cmd.Argument2));
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

        
        [HttpDelete]
        public new HttpResponseMessage Remove(int id)
        {
            return base.Remove(id);
        }

        
        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new IQueryable<object> GetNestedCollection(int parentId, string nestedCollectionName)
        {
            return base.GetNestedCollection(parentId, nestedCollectionName);
        }
    }
}
