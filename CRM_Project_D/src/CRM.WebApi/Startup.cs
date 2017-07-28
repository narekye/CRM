using CRM.Entities;
using CRM.WebApi.Models.Request;
using CRM.WebApi.Models.Response;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.DataProtection;

[assembly: OwinStartup(typeof(CRM.WebApi.Startup))]
namespace CRM.WebApi
{
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var config = new HttpConfiguration();
            ConfigureWebApi(config);
            InitMapper();
            app.UseWebApi(config);
        }
        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.MapHttpAttributeRoutes();
            // var cors = new EnableCorsAttribute("http://localhost:3000", "*", "*");
            config.EnableCors(/*cors*/); //
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        private void InitMapper()
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
