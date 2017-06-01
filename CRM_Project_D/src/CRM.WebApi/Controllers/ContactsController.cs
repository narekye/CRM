namespace CRM.WebApi.Controllers
{
    using Filters;
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Models.Request;
    using Models.Response;
    using InfrastructureModel;
    using InfrastructureModel.ApplicationManager;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net;

    [ExceptionFilters]
    public class ContactsController : ApiController
    {
        private readonly ApplicationManager _manager;
        public ContactsController()
        {
            _logger = new LoggerManager();
            _manager = new ApplicationManager();
        }

        public async Task<HttpResponseMessage> GetAllContactsAsync()
        {
            var data = await _manager.GetAllContactsAsync();
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        public async Task<HttpResponseMessage> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway, "Set ID parameter.");
            var contact = await _manager.GetContactByIdAsync(id);
            if (ReferenceEquals(contact, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, contact);
        }
        public async Task<HttpResponseMessage> GetContactByGuidAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            var contact = await _manager.GetContactByGuidAsync(guid);
            if (ReferenceEquals(contact, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, contact);
        }
        public async Task<HttpResponseMessage> PutContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadGateway);
            if (!_manager.CheckEmailAddress(c.Email)) return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid email address detected.");
            if (await _manager.UpdateContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Accepted);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> PostContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (!_manager.CheckEmailAddress(c.Email)) return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid email address detected.");
            if (await _manager.AddContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Created);
            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }
        public async Task<HttpResponseMessage> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            if (await _manager.DeleteContactAsync(guid.Value))
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Route("api/contacts/filter")]
        public async Task<IHttpActionResult> PostFilterOrderBy([FromBody] RequestContact request)
        {
            var data = await _manager.FilterOrderByRequestAsync(request);
            if (ReferenceEquals(data, null)) return BadRequest();
            return Ok(data);
        }
        [Route("api/contacts/upload")] // TODO: TEST
        public async Task<IHttpActionResult> PostContactByteArrayAsync([FromBody] string base64)
        {
            byte[] array = Convert.FromBase64String(base64);
            if (await _manager.AddToDatabaseFromBytes(array)) return Ok();
            return BadRequest();
        }
        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            return await _manager.PageCountAsync();
        }
        [Route("api/contacts/developer")]
        public HttpResponseMessage GetLog()
        {
            var response = new HttpResponseMessage { Content = new StringContent(_logger.ReadData()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}