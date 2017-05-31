using Newtonsoft.Json;

namespace CRM.WebApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Models.Request;
    using Models.Response;
    using NLog;
    using InfrastructureModel.ApplicationManager;
    using System.IO;
    using NLog.Targets;

    public class ContactsController : ApiController
    {
        private readonly ApplicationManager _manager;
        public ContactsController()
        {
            _manager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllContactsAsync()
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var data = await _manager.GetAllContactsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return BadRequest("Set parameter.");
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var contact = await _manager.GetContactByIdAsync(id);
                if (ReferenceEquals(contact, null)) return NotFound();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> GetContactByGuidAsync([FromUri] Guid? guid)
        {
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                var contact = await _manager.GetContactByGuidAsync(guid);
                if (ReferenceEquals(contact, null)) return NotFound();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> PutContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.UpdateContactAsync(c)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }

        }
        public async Task<IHttpActionResult> PostContactAsync([FromBody] ViewContactLess c)
        {
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.AddContactAsync(c)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            if (!guid.HasValue) return BadRequest();
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.DeleteContactAsync(guid.Value)) return Ok();
                return BadRequest();

            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/filter")]
        public async Task<IHttpActionResult> PostFilterOrderBy([FromBody] RequestContact request)
        {
            try
            {

                var data = await _manager.FilterOrderByRequestAsync(request);
                if (ReferenceEquals(data, null)) return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/upload")] // TODO: TEST
        public async Task<IHttpActionResult> PostContactByteArrayAsync([FromBody] string base64)
        {
            byte[] array = Convert.FromBase64String(base64);
            try
            {
                ApplicationManager.Logger.Info($"Request: {Request.Method} | URL: {Request.RequestUri}");
                if (await _manager.AddToDatabaseFromBytes(array)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                ApplicationManager.Logger.Error(ex, $"Request: {Request.Method} | URL: {Request.RequestUri}");
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            return await _manager.PageCountAsync();
        }

        [Route("api/contacts/log")]
        public IHttpActionResult GetLogs()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            string fileName = fileTarget.FileName.Render(logEventInfo);
            if (!File.Exists(fileName))
                throw new Exception("Log file does not exist.");
            var data = File.ReadAllLines(fileName);
            return Ok(data);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
