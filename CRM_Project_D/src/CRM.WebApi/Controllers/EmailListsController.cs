using System.Linq;
using System.Web.Http;
using CRM.Entities;

namespace CRM.WebApi.Controllers
{
    public class EmailListsController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        public IHttpActionResult GetAllEmailLists()
        {
            var data = _database.EmailLists.Select(e => new
            {
                e.EmailListName,
                Contacts = e.Contacts.Select(c => new
                {
                    c.FullName,
                    c.CompanyName,
                    c.Position,
                    c.Country,
                    c.Email,
                    c.GuID,
                    c.DateInserted,
                })
            }).ToList();
            if (!(data.Count > 0)) return NotFound();
            return Ok(data);
        }

        [Route("api/EmailLists/count")]
        public int GetEmailListsPageCount()
        {
            return _database.Contacts.Count() > 10 ? _database.Contacts.Count() / 10 : 1;
        }
    }
}
