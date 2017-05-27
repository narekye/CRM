using System.Collections.Generic;
using CRM.WebApi.InfrastructureModel;

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
        private readonly ApplicationManager manager = new ApplicationManager();
        public async Task<IHttpActionResult> GetAllEmailListsAsync()
        {
            try
            {
                var data = await manager.GetAllEmailListsAsync();
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public async Task<IHttpActionResult> GetEmailListByIdAsync(int? id)
        {
            if (!id.HasValue) return BadRequest();
            using (var database = new CRMContext())
            {
                var emaillist = await database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == id.Value);
                if (ReferenceEquals(emaillist, null)) return NotFound();
                return this.Ok(emaillist);
            }
        }

        //public async Task<IHttpActionResult> PostEmailListAsync([FromBody] EmailListsModel model)
        //{
        //    return this.Ok();
        //}
        //public async Task<IHttpActionResult> PostEmailListsAsync([FromBody] List<EmailListsModel> model)
        //{
        //    return this.Ok();
        //}

        //public async Task<IHttpActionResult> DeleteEmailListById([FromUri] int? id)
        //{
        //    return this.Ok();
        //}
    }
}
