using System.Collections.Generic;

namespace CRM.WebApi.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Models;
    using Entities;

    public class EmailListsController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        public async Task<IHttpActionResult> GetAllEmailListsAsync()
        {
            using (var database = new CRMContext())
            {
                try
                {
                    var data = EmailListsModel.GetEmailListsModels(await database.EmailLists.ToListAsync());
                    if (ReferenceEquals(data, null)) return NotFound();
                    return Ok(data);
                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        public async Task<IHttpActionResult> GetEmailListByIdAsync(int? id)
        {
            return Ok();
        }

        public async Task<IHttpActionResult> PostEmailListAsync([FromBody] EmailListsModel model)
        {
            return this.Ok();
        }
        public async Task<IHttpActionResult> PostEmailListsAsync([FromBody] List<EmailListsModel> model)
        {
            return this.Ok();
        }

        public async Task<IHttpActionResult> DeleteEmailListById([FromUri] int? id)
        {
            return this.Ok();
        }
    }
}
