using System;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CRM.Entities;
using System.Web.Http;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;
using CRM.WebApi.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

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
        public AccountController()
        {
            var db = new CRMContext();
            db.Configuration.LazyLoadingEnabled = false;
            manager = new CrmUserManager(new UserStore(db));
            manager.UserTokenProvider = new DataProtectorTokenProvider<User>(new DpapiDataProtectionProvider().Create("EmailConfirm"));
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
                UserName = $"{model.FirstName}{model.LastName}",
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
            };
            IdentityResult identity = await this.manager.CreateAsync(user, model.Password);
            if (!identity.Succeeded) return Request.CreateResponse(HttpStatusCode.Accepted, user);


            var message = await this.manager.GenerateEmailConfirmationTokenAsync(this.manager.FindAsync(user.UserName, model.Password).Result.Id);
            var callback = new Uri(Url.Link("ConfirmEmailRoute", new { userid = user.Id, message }));

            await this.manager.SendEmailAsync(user.Id, "", "");

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.manager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.ToString());
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
    }
}
