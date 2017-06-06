namespace CRM.WebApi.Controllers
{
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel.ApplicationManager;
    using Models.Request;
    using System.Net.Http;
    using System.Net;
    using Filters;
    [ExceptionFilters]
    public class EmailListsController : ApiController
    {
        private readonly ApplicationManager manager;
        public EmailListsController()
        {
            manager = new ApplicationManager();
        }
        public async Task<HttpResponseMessage> GetAllEmailListsAsync()
        {
            var data = await manager.GetAllEmailListsAsync();
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        public async Task<HttpResponseMessage> GetEmailListByIdAsync([FromUri] int? id)
        {
            if (!id.HasValue) return null;
            var data = await manager.GetEmailListByIdAsync(id);
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        public async Task<HttpResponseMessage> PostEmailListAsync([FromBody] RequestEmailList model)
        {
            if (await manager.AddNewEmailList(model))
                return Request.CreateResponse(HttpStatusCode.Created);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> PutEmailListAsync([FromBody] RequestEmailList model)
        {
            if (await manager.UpdateEmailListAsync(model))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.NotModified);
        }
        [Route("api/emaillists/add")]
        public async Task<HttpResponseMessage> PutAddToExsistingEmailListAsync([FromBody] RequestEmailList model)
        {
            if (await manager.AddContactsToExsistingList(model)) return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> DeleteEmailListContactsAsync([FromBody] RequestEmailList model)
        {
            if (await manager.DeleteContactsFromEmailListAsync(model))
                return Request.CreateResponse(HttpStatusCode.Accepted);
            return Request.CreateResponse(HttpStatusCode.NotModified);
        }
        public async Task<HttpResponseMessage> DeleteEmailListById([FromUri] int? id)
        {
            if (await manager.DeleteEmailListByIdAsync(id)) return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadGateway);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                manager.Dispose();
            base.Dispose(disposing);
        }
    }
}