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
        private static readonly ApplicationManager Manager = new ApplicationManager();
        public async Task<IHttpActionResult> GetAllEmailListsAsync()
        {
            try
            {
                var data = await Manager.GetAllEmailListsAsync();
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
            try
            {
                var result = await Manager.GetEmailListById(id);
                if (ReferenceEquals(result, null)) return NotFound();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
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
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
