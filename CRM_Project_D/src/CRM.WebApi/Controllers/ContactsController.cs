using System.Linq;
using CRM.Entities;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.Controllers
{
    using Filters;
    using InfrastructureModel;
    using InfrastructureModel.ApplicationManager;
    using Models.Request;
    using Models.Response;
    using Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    [HandleExceptions]
    public class ContactsController : BaseApiController
    {
        private readonly ApplicationManager manager;
        private readonly LoggerManager logger;
        private CrmUserManager managers;
        public ContactsController()
        {
            logger = new LoggerManager();
            manager = new ApplicationManager();
            managers = CrmUserManager.UserManager;
        }

        public async Task<HttpResponseMessage> GetAllContactsAsync()
        {
            var data = await manager.GetAllContactsAsync();
            if (ReferenceEquals(data, null)) return Create404();
            // ===
            User user = new User()
            {
                Email = "jjjj@mail.ru",
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumber = "789754654",
                UserName = "narekye",
            };
            IdentityResult result = await managers.CreateAsync(user, "Anlegala88+");
            if (result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Created");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, result.Errors);
            }
            // === 
            // return Create200(managers.Users.ToList());
        }

        [Authorize]
        public async Task<HttpResponseMessage> PutContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return Create502();
            if (await manager.UpdateContactAsync(c)) return Create202();
            return Create400();
        }

        [Authorize]
        public async Task<HttpResponseMessage> PostContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid)
                return Create400();
            if (await manager.AddContactAsync(c)) return Create201();
            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }

        [Authorize]
        public async Task<HttpResponseMessage> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return Create502();
            if (await manager.DeleteContactAsync(guid.Value))
                return Request.CreateResponse(HttpStatusCode.NoContent);
            return Create400();
        }

        [Authorize]
        public async Task<HttpResponseMessage> DeleteContactsByGuidsAsync([FromBody] List<Guid> guids)
        {
            if (ReferenceEquals(guids, null)) return Create502();
            if (await manager.DeleteContactsAsync(guids)) return Request.CreateResponse(HttpStatusCode.NoContent);
            return Create400();
        }

        // [Route("api/contacts/filter")]
        public async Task<HttpResponseMessage> PostFilterOrderBy([FromBody] RequestContact model)
        {
            var data = await manager.FilterOrderByRequestAsync(model);
            if (ReferenceEquals(data, null)) return Create400();
            return Create200(data);
        }

        [Authorize]
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
            return Create202();
        }

        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            return await manager.PageCountAsync();
        }

        [Route("api/contacts/developer")]
        public HttpResponseMessage GetLog()
        {
            return CreateHtml(logger.ReadData());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) manager.Dispose();
            base.Dispose(disposing);
        }
    }
}