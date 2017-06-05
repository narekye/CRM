using System;
using CRM.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace CRM.WebApi.InfrastructureOAuth.CRM.UserManager
{
    public class CrmUserManager : UserManager<User>
    {
        public CrmUserManager(IUserStore<User> store) : base(store)
        {
            
        }

        public static CrmUserManager Create(IdentityFactoryOptions<CrmUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<CRMContext>();
            var appUserManager = new CrmUserManager(new UserStore(appDbContext));          
            appUserManager.UserValidator = new UserValidator<User>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("CRM BetConstruct"))
                {
                    TokenLifespan = TimeSpan.FromHours(1)
                };
            }
            return appUserManager;
        }
    }
}