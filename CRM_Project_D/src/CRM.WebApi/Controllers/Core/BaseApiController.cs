using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CRM.WebApi.Controllers.Core
{
    public class BaseApiController : ApiController
    {
        protected virtual HttpResponseMessage Create200<T>(T data = null) where T : class
        {
            HttpResponseMessage message;
            if (data == null)
            {
                message = Request.CreateResponse(HttpStatusCode.OK);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, data);
            return message;
        }

        protected virtual HttpResponseMessage Create201()
        {
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        protected virtual HttpResponseMessage Create202()
        {
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        protected virtual HttpResponseMessage Create400()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        protected virtual HttpResponseMessage Create404()
        {
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        protected virtual HttpResponseMessage Create502()
        {
            return Request.CreateResponse(HttpStatusCode.BadGateway);
        }

        protected virtual HttpResponseMessage CreateHtml(string html)
        {
            var response = new HttpResponseMessage { Content = new StringContent(html) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}