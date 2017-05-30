using System.Web.Http;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(CRM.WebApi.Startup))]
namespace CRM.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWelcomePage("/");
            var config = new HttpConfiguration();
            ConfigureWebApi(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
