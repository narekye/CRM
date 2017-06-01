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
    [ExceptionFilters]
    public class SendEmailController : ApiController
    {
        private readonly MailManager _manager;
        public SendEmailController()
        {
            _manager = new MailManager();
        }
        public async Task<HttpResponseMessage> PostSendToList([FromUri] int templateid, [FromBody] List<Guid> guids)
        {
            var list = await _manager.GetListOfEmailsByGuids(guids);
            await _manager.SendEmailToList(list, templateid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
