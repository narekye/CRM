using System.Collections.Generic;
using System.Web.Http;
using CRM.Entities;
using CRM.WebApi.Models.Request;
using CRM.WebApi.Models.Response;
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
            Mapper();
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
        private void Mapper()
        {
            AutoMapper.Mapper.Initialize(p => p.CreateProfile("ModelsToViewModels", z =>
            {
                z.CreateMap(typeof(Contact), typeof(ViewContact));
                z.CreateMap(typeof(Contact), typeof(ViewContactLess));
                z.CreateMap(typeof(ViewContactLess), typeof(Contact));
                z.CreateMap(typeof(List<Contact>), typeof(List<ViewContact>));
                z.CreateMap(typeof(EmailList), typeof(ViewEmailList));
                z.CreateMap(typeof(EmailList), typeof(ViewEmailListLess));
                z.CreateMap(typeof(RequestEmailList), typeof(EmailList));
                z.CreateMap(typeof(Template), typeof(ViewTemplate));
                z.CreateMap(typeof(List<Template>), typeof(List<ViewTemplate>));
            }));
        }
    }
}
