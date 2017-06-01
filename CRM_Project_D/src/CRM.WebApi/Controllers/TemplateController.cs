namespace CRM.WebApi.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Net;
    using System.Net.Http;
    using InfrastructureModel.ApplicationManager;
    using InfrastructureModel;
    public class TemplateController : ApiController
    {
        private readonly ApplicationManager _manager;
        private readonly ModelFactory _factory;
        private readonly LoggerManager _logger;
        public TemplateController()
        {
            _logger = new LoggerManager();
            _manager = new ApplicationManager();
            _factory = new ModelFactory();
        }
        public async Task<HttpResponseMessage> GetAllTemplatesAsync()
        {
            var data = await _manager.GetAllTemplatesListAsync();
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            var result = _factory.GetViewTemplates(data);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        public async Task<HttpResponseMessage> GetTemplateById(int? id)
        {
            if (!id.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            var data = await _manager.GetTemplateByIdAsync(id);
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            var result = _factory.GetViewTemplate(data);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
                _manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
