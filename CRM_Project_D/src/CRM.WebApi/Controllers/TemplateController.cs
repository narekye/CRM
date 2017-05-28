namespace CRM.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using InfrastructureModel.ApplicationManager;

    public class TemplateController : ApiController
    {
        private readonly ApplicationManager _manager;

        public TemplateController()
        {
            _manager = new ApplicationManager();
        }

        public async Task<IHttpActionResult> GetAllTemplatesAsync()
        {
            try
            {
                var data = await _manager.GetAllTemplatesListAsync();
                if (ReferenceEquals(data, null)) return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IHttpActionResult> GetTemplateById(int? id)
        {
            try
            {
                var data = await _manager.GetTemplateByIdAsync(id);
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
