using System.Data.Entity;
using CRM.WebApi.Models;

namespace CRM.WebApi.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Entities;

    public class EmailListsController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        public IHttpActionResult GetEmailList()
        {
            var data = _database.EmailLists.ToList();
            return this.Unauthorized();
        }
    }
}
