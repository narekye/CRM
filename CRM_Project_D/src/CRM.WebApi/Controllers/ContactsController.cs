namespace CRM.WebApi.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Models.Request;
    using Models.Response;
    using InfrastructureModel.ApplicationManager;
    public class ContactsController : ApiController
    {
        private readonly ApplicationManager _manager;
        public ContactsController()
        {
            _manager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllContactsAsync()
        {
            // TODO: login/auth check with token
            try
            {
                var data = await _manager.GetAllContactsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> GetContactByIdAsync(int? id)
        {
            // TODO: login/auth check with token
            if (!id.HasValue) return BadRequest("Set parameter.");
            try
            {
                var contact = await _manager.GetContactByIdAsync(id);
                if (ReferenceEquals(contact, null)) return NotFound();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> GetContactByGuidAsync([FromUri] Guid? guid)
        {
            try
            {
                var contact = await _manager.GetContactByGuidAsync(guid);
                if (ReferenceEquals(contact, null)) return NotFound();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IHttpActionResult> PutContactAsync([FromBody] ViewContact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            try
            {
                if (await _manager.UpdateConactAsync(c)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        public async Task<IHttpActionResult> PostContactAsync([FromBody] ViewContact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            try
            {
                if (await _manager.AddContactAsync(c)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
        }
        [Route("api/contacts/filter")]
        public async Task<IHttpActionResult> PostFilterOrderBy([FromBody] RequestQuery request)
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
        public async Task<IHttpActionResult> DeleteContactByGuIdAsync([FromUri] Guid? guid)
        {
            // TODO: login/auth check with token
            if (!guid.HasValue) return BadRequest();
            try
            {
                if (await _manager.DeleteContactAsync(guid.Value)) return Ok();
                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/upload")]
        public async Task<IHttpActionResult> PostContactByteArrayAsync([FromBody] byte[] array)
        {
            try
            {
                if (await _manager.AddToDatabaseFromBytes(array)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("api/contacts/count")]
        public async Task<int> GetContactsPageCountAsync()
        {
            return await _manager.PageCountAsync();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
