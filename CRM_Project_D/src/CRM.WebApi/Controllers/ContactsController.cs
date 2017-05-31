namespace CRM.WebApi.Controllers
{
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

    public class ContactsController : ApiController
    {
        private readonly ApplicationManager _manager;
        private readonly LoggerManager _logger;
        public ContactsController()
        {
            _logger = new LoggerManager();
            _manager = new ApplicationManager();
        }
        public async Task<HttpResponseMessage> GetAllContactsAsync()
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.GetAllContactsAsync();
                if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway, "Set parameter");
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var contact = await _manager.GetContactByIdAsync(id);
                if (ReferenceEquals(contact, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> GetContactByGuidAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var contact = await _manager.GetContactByGuidAsync(guid);
                if (ReferenceEquals(contact, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        public async Task<HttpResponseMessage> PutContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadGateway);
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.UpdateContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Accepted);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> PostContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.AddContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Created);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.DeleteContactAsync(guid.Value))
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        [Route("api/contacts/filter")]
        public async Task<IHttpActionResult> PostFilterOrderBy([FromBody] RequestContact request)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.FilterOrderByRequestAsync(request);
                if (ReferenceEquals(data, null)) return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/upload")] // TODO: TEST
        public async Task<IHttpActionResult> PostContactByteArrayAsync([FromBody] string base64)
        {
            byte[] array = Convert.FromBase64String(base64);
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.AddToDatabaseFromBytes(array)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            _logger.LogInfo(Request.Method, Request.RequestUri);
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
