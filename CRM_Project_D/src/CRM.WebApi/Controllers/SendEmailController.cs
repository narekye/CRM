namespace CRM.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Filters;
    using InfrastructureModel;
    using System.Net.Http;
    using System.Net;
    [HandleExceptions]
    public class SendEmailController : ApiController
    {
        private readonly MailManager manager;
        public SendEmailController()
        {
            manager = new MailManager();
        }
        public async Task<HttpResponseMessage> PostSendToList([FromUri] int templateid, [FromBody] List<Guid> guids)
        {
            var list = await manager.GetListOfEmailsByGuids(guids);
            await manager.SendEmailToList(list, templateid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [Route("api/sendemail/list")]
        public async Task<HttpResponseMessage> PostSendToEmailList([FromUri] int id, [FromUri] int template)
        {
            await manager.SendEmailToEmailList(id, template);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
