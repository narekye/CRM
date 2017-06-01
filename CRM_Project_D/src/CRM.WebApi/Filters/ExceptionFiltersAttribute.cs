using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using CRM.WebApi.InfrastructureModel;

namespace CRM.WebApi.Filters
{
    public class ExceptionFiltersAttribute : ExceptionFilterAttribute
    {
        private readonly LoggerManager _logger = new LoggerManager();
        public override Task OnExceptionAsync(HttpActionExecutedContext action, CancellationToken cancellationToken)
        {
            _logger.LogError(action.Exception, action.Request.Method, action.Request.RequestUri);
            if (action.Exception is NotImplementedException)
            {
                action.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            // entity exception
            if (action.Exception is DbUpdateException)
            {
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Entity update exception\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.Conflict,
                    ReasonPhrase = "Entity update exception"
                };
            }
            // at the end
            //if (action.Exception is Exception)
            //{
            //    action.Response = new HttpResponseMessage
            //    {
            //        StatusCode = HttpStatusCode.GatewayTimeout,
            //        ReasonPhrase = "Server error."
            //    };
            //}
            return base.OnExceptionAsync(action, cancellationToken);
        }
    }
}