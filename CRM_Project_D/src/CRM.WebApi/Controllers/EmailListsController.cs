namespace CRM.WebApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using InfrastructureModel.ApplicationManager;
    using InfrastructureModel;
    using Models.Request;
    public class EmailListsController : ApiController
    {
        private readonly ApplicationManager _manager;
        private readonly LoggerManager _logger;
        public EmailListsController()
        {
            _logger = new LoggerManager();
            _manager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllEmailListsAsync()
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.GetAllEmailListsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> GetEmailListByIdAsync([FromUri] int? id)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var result = await _manager.GetEmailListById(id);
                if (ReferenceEquals(result, null)) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,Request.Method, Request.RequestUri);
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> PostEmailListAsync([FromBody] RequestEmailList model)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.AddNewEmailList(model)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> PutEmailListAsync([FromBody] RequestEmailList emaillist)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                var data = await _manager.UpdateEmailListAsync(emaillist);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
                return BadRequest();
            }
        }
        public async Task<IHttpActionResult> DeleteEmailListById([FromUri] int? id)
        {
            try
            {
                _logger.LogInfo(Request.Method, Request.RequestUri);
                if (await _manager.DeleteEmailListByIdAsync(id)) return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.Method, Request.RequestUri);
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
