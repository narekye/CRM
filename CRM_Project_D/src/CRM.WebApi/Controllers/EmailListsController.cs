using CRM.WebApi.Models.Request;

namespace CRM.WebApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel.ApplicationManager;
    using Models.Response;
    public class EmailListsController : ApiController
    {
        private readonly ApplicationManager _manager;
        public EmailListsController()
        {
            _manager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllEmailListsAsync()
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var data = await _manager.GetAllEmailListsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> GetEmailListByIdAsync([FromUri] int? id)
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var result = await _manager.GetEmailListById(id);
                if (ReferenceEquals(result, null)) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> PostEmailListAsync([FromBody] RequestEmailList model)
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.AddNewEmailList(model)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> PutEmailListAsync([FromBody] RequestEmailList emaillist)
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var data = await _manager.UpdateEmailListAsync(emaillist);
                return Ok(data);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> DeleteEmailListById([FromUri] int? id)
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.DeleteEmailListByIdAsync(id)) return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
