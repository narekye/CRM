namespace CRM.WebApi.Controllers
{
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel.ApplicationManager;
    using InfrastructureModel;
    using Models.Request;
    using System.Net.Http;
    using System.Net;
    using Filters;
    [ExceptionFilters]
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
            var data = await _manager.GetAllEmailListsAsync();
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        public async Task<HttpResponseMessage> GetEmailListByIdAsync([FromUri] int? id)
        {
            if (!id.HasValue) return null; //check
            var data = await _manager.GetEmailListById(id);
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        public async Task<HttpResponseMessage> PostEmailListAsync([FromBody] RequestEmailList model)
        {
            if (await _manager.AddNewEmailList(model)) return Request.CreateResponse(HttpStatusCode.Created);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> PutEmailListAsync([FromBody] RequestEmailList model)
        {
            if (await _manager.UpdateEmailListAsync(model))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.NotModified);
        }
        public async Task<HttpResponseMessage> DeleteEmailListById([FromUri] int? id)
        {
            if (await _manager.DeleteEmailListByIdAsync(id)) return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadGateway);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}