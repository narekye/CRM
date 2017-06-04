using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRM.Entities;
using CRM.WebApi.InfrastructureOAuth;
using System.Web.Http;
using System.Workflow.ComponentModel.Design;
using CRM.WebApi.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private CrmUserManager manager;
        public AccountController()
        {
            var db = new CRMContext();
            manager = new CrmUserManager(new UserStore(db));
        }

        public async Task<IHttpActionResult> PostRegisterUser(RegisterUserModel model)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.FirstName + model.LastName,
                // Id = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                ConfirmedEmail = false,
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                LockoutEnabled = false
            };
            IdentityResult identity = await this.manager.CreateAsync(user);
            if (identity.Succeeded) return Ok();
            return this.Ok(identity.Errors.ToList());
        }
    }
}
