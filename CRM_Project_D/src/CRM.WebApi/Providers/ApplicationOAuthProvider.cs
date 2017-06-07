using System.Security.Claims;
using CRM.Entities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;

namespace CRM.WebApi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            var userManager = context.OwinContext.GetUserManager<CrmUserManager>();
            User user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("InvalidGrant", "The user name or password is incorrect.");
                return;
            }
            // check if email is confirmed
            if (!user.EmailConfirmed)
            {
                context.SetError("InvalidGrant", "User did not confirm email.");
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "SuperAdmin"));
            context.Validated(identity);
        }
    }
}