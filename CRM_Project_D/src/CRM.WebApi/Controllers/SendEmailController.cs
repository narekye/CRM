namespace CRM.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel;
    public class SendEmailController : ApiController
    {
        private readonly MailManager _manager;
        public SendEmailController()
        {
            _manager = new MailManager();
        }
        public async Task<IHttpActionResult> PostSendToList([FromUri] int templateid, [FromBody] List<Guid> guids)
        {
            try
            {
                var list = await _manager.GetListOfEmailsByGuids(guids);
                await _manager.SendEmailToList(list, templateid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
