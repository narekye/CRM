using System;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CRM.Entities;
using System.Web.Http;
using CRM.WebApi.InfrastructureModel;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;
using CRM.WebApi.Models.Identity;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.Controllers
{
    internal enum Role
    {
        SuperAdmin,
        User
    }

    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private CrmUserManager manager;
        private MailManager mailmanager;

        public AccountController()
        {
            var db = new CRMContext();
            manager = CrmUserManager.UserManager;
            this.mailmanager = new MailManager();
        }

        [Authorize]
        [Route("admin/users")]
        public async Task<HttpResponseMessage> GetAllUsersAsync()
        {
            if (!this.IsInRole(Role.SuperAdmin)) return Request.CreateResponse(HttpStatusCode.NotFound);
            var data = await this.manager.Users.ToListAsync();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [Authorize]
        [Route("admin/delete")]
        public async Task<HttpResponseMessage> DeleteUserAsync([FromBody] Guid guid)
        {
            var user = await this.manager.FindByIdAsync(guid.ToString());
            IdentityResult result = await this.manager.DeleteAsync(user);
            if (!result.Succeeded) return Request.CreateResponse(HttpStatusCode.NotModified, result.Errors);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Authorize]
        [Route("admin/change")]
        public async Task<HttpResponseMessage> PutUserAsync(User user)
        {
            var result = await this.manager.UpdateAsync(user);
            if (!result.Succeeded) return Request.CreateResponse(HttpStatusCode.NotModified);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // register user
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostRegisterUser(RegisterUserModel model)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
            };
            IdentityResult identity = await this.manager.CreateAsync(user, model.Password);
            if (!identity.Succeeded) return Request.CreateResponse(HttpStatusCode.NotAcceptable, identity.Errors);
            var codef = await this.manager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callback = new Uri(Url.Link("ConfirmEmailRoute", new {userid = user.Id, code = codef}));
            if (await this.mailmanager.SendConfirmationEmail(user.Email, callback.ToString()))
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Email sended.");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // email confirmation
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<HttpResponseMessage> GetConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("Err", "User Id and Code are required");
                return Request.CreateResponse(HttpStatusCode.NotFound, ModelState);
            }
            IdentityResult result = await this.manager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "You have successfully confirmed your email.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.manager.Dispose();
            base.Dispose(disposing);
        }

        private bool IsInRole(Role role)
        {
            return RequestContext.Principal.IsInRole(role.ToString());
        }

        [HttpGet, Route("reset")]
        public HttpResponseMessage ResetDatabaseToStock()
        {
            var context = new CRMContext();
            context.RESETDATATODEFAULT();
            return Request.CreateResponse(HttpStatusCode.OK, "Reseted to defaults");
        }
    }
}