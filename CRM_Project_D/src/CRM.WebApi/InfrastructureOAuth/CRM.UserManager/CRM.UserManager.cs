using System;
using System.Web;
using CRM.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace CRM.WebApi.InfrastructureOAuth.CRM.UserManager
{
    public class CrmUserManager : UserManager<User>
    {
        public CrmUserManager(IUserStore<User> store) : base(store) { }
        public static CrmUserManager UserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<CrmUserManager>(); }
        }
        public static CrmUserManager Create(IdentityFactoryOptions<CrmUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<CRMContext>();
            var manager = new CrmUserManager(new UserStore(appDbContext));
            var protection = Startup.DataProtectionProvider;
            manager.UserTokenProvider =
            new DataProtectorTokenProvider<User>(protection.Create("Identity"))
            {
                TokenLifespan = TimeSpan.FromHours(2)
            };
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                // RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
            };
            return manager;
        }
    }
}