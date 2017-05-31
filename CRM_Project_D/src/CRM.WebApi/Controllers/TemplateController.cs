namespace CRM.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using InfrastructureModel.ApplicationManager;
    using InfrastructureModel;
    public class TemplateController : ApiController
    {
        private readonly ApplicationManager _manager;
        private readonly ModelFactory _factory;
        public TemplateController()
        {
            _manager = new ApplicationManager();
            _factory = new ModelFactory();
        }
        public async Task<IHttpActionResult> GetAllTemplatesAsync()
        {
            try
            {
                var data = await _manager.GetAllTemplatesListAsync();
                if (ReferenceEquals(data, null)) return BadRequest();
                var result = _factory.GetViewTemplates(data);
                return Ok(result);
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
                var result = _factory.GetViewTemplate(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
                _manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
