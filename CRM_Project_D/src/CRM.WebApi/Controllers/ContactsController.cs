namespace CRM.WebApi.Controllers
{
    using Filters;
    using InfrastructureModel;
    using InfrastructureModel.ApplicationManager;
    using Models.Request;
    using Models.Response;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    [ExceptionFilters]
    public class ContactsController : ApiController
    {
        private readonly ApplicationManager manager;
        private readonly LoggerManager logger;
        public ContactsController()
        {
            logger = new LoggerManager();
            manager = new ApplicationManager();
        }

        public async Task<HttpResponseMessage> GetAllContactsAsync()
        {
            var data = await manager.GetAllContactsAsync();
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        public async Task<HttpResponseMessage> PutContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadGateway);
            if (await manager.UpdateContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Accepted);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> PostContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (await manager.AddContactAsync(c)) return Request.CreateResponse(HttpStatusCode.Created);
            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }
        public async Task<HttpResponseMessage> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Request.CreateResponse(HttpStatusCode.BadGateway);
            if (await manager.DeleteContactAsync(guid.Value))
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public async Task<HttpResponseMessage> DeleteContactsByGuidsAsync([FromBody] List<Guid> guids)
        {
            if (ReferenceEquals(guids, null)) return Request.CreateResponse(HttpStatusCode.BadGateway);
            if (await manager.DeleteContactsAsync(guids)) return Request.CreateResponse(HttpStatusCode.NoContent);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Route("api/contacts/filter")]
        public async Task<HttpResponseMessage> PostFilterOrderBy([FromBody] RequestContact model)
        {
            var data = await manager.FilterOrderByRequestAsync(model);
            if (ReferenceEquals(data, null)) return Request.CreateResponse(HttpStatusCode.BadRequest);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        [Route("api/contacts/upload")]
        public async Task<HttpResponseMessage> PostContactByteArrayAsync()
        {
            string root = HttpContext.Current.Server.MapPath("~/log");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.FileData)
            {
                var buffer = File.ReadAllBytes(file.LocalFileName);
                await manager.AddToDatabaseFromBytes(buffer);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            return await manager.PageCountAsync();
        }
        [Route("api/contacts/developer")]
        public HttpResponseMessage GetLog()
        {
            var response = new HttpResponseMessage { Content = new StringContent(logger.ReadData()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) manager.Dispose();
            base.Dispose(disposing);
        }
    }
}