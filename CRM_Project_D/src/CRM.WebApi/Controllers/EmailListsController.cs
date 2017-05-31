namespace CRM.WebApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel.ApplicationManager;
    using InfrastructureModel;
    using Models.Request;
    using System.Net.Http;
    using System.Net;

    public class EmailListsController : ApiController
    {
        private readonly ApplicationManager _manager;
        private readonly LoggerManager _logger;
        public EmailListsController()
        {
            _logger = new LoggerManager();
            _manager = new ApplicationManager();
        }
        public async Task<HttpResponseMessage> GetAllEmailListsAsync()
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.GetAllEmailListsAsync();
                if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> GetEmailListByIdAsync([FromUri] int? id)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.GetEmailListById(id);
                if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> PostEmailListAsync([FromBody] RequestEmailList model)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.AddNewEmailList(model)) return Request.CreateResponse(HttpStatusCode.Created);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> PutEmailListAsync([FromBody] RequestEmailList model)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.UpdateEmailListAsync(model))
                    return Request.CreateResponse(HttpStatusCode.OK);
                return Request.CreateResponse(HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
        public async Task<HttpResponseMessage> DeleteEmailListById([FromUri] int? id)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.DeleteEmailListByIdAsync(id)) return Request.CreateResponse(HttpStatusCode.OK);
                return Request.CreateResponse(HttpStatusCode.BadGateway);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return Request.CreateResponse(HttpStatusCode.Conflict);
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
