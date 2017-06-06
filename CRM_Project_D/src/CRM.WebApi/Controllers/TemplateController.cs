﻿namespace CRM.WebApi.Controllers
{
    using Filters;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Net;
    using System.Net.Http;
    using InfrastructureModel.ApplicationManager;
    [ExceptionFilters]
    public class TemplateController : ApiController
    {
        private readonly ApplicationManager manager;
        public TemplateController()
        {
            manager = new ApplicationManager();
        }
        public async Task<HttpResponseMessage> GetAllTemplatesAsync()
        {
            var result = await manager.GetAllTemplatesListAsync();
            if (ReferenceEquals(result, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        public async Task<HttpResponseMessage> GetTemplateById(int? id)
        {
            if (!id.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            var result = await manager.GetTemplateByIdAsync(id);
            if (ReferenceEquals(result, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
