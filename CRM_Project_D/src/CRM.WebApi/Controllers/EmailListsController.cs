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
                var data = await _manager.GetAllEmailListsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IHttpActionResult> GetEmailListByIdAsync(int? id)
        {
            try
            {
                var result = await _manager.GetEmailListById(id);
                if (ReferenceEquals(result, null)) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // working, can send a contact list if you want.
        public async Task<IHttpActionResult> PostEmailListAsync([FromBody] ViewEmailList model)
        {
            try
            {
                if (await _manager.AddEmailList(model)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // TODO: CHANGE
        public async Task<IHttpActionResult> PutEmailListAsync([FromBody] ViewEmailList emaillist)
        {
            var data = await _manager.UpdateEmailListAsync(emaillist);
            return Ok(data);
        }
        public async Task<IHttpActionResult> DeleteEmailListById([FromUri] int? id)
        {
            try
            {
                if (await _manager.DeleteEmailListByIdAsync(id)) return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
