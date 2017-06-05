using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CRM.Entities;
using CRM.WebApi.InfrastructureOAuth;
using System.Web.Http;
using CRM.WebApi.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private CrmUserManager manager;
        public AccountController()
        {
            var db = new CRMContext();
            db.Configuration.LazyLoadingEnabled = false;
            manager = new CrmUserManager(new UserStore(db));
        }

        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostRegisterUser(RegisterUserModel model)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.FirstName + model.LastName,
                Id = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                ConfirmedEmail = false,
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                LockoutEnabled = false
            };
            IdentityResult identity = await this.manager.CreateAsync(user, model.Password);
            if (!identity.Succeeded) return Request.CreateResponse(HttpStatusCode.Accepted, user);

            this.manager.UserTokenProvider = new DataProtectorTokenProvider<User>(new DpapiDataProtectionProvider().Create("EmailConfirm"));
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

    }
}
