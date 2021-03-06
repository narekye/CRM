﻿using CRM.Entities;
using CRM.WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;
using Microsoft.Owin.Security.DataProtection;

namespace CRM.WebApi
{
    public partial class Startup
    {
        private void ConfigureOAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();  // current

            app.CreatePerOwinContext(CRMContext.Create);
            app.CreatePerOwinContext<CrmUserManager>(CrmUserManager.Create);
            
            var options = new OAuthAuthorizationServerOptions
            {
                
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new ApplicationOAuthProvider()
            };
            
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication
            (
                new OAuthBearerAuthenticationOptions
                {
                    Provider = new OAuthBearerAuthenticationProvider()
                }
            );
        }
    }
}