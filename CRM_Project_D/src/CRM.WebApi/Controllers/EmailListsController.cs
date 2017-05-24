namespace CRM.WebApi.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Entities;

    public class EmailListsController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        public IHttpActionResult GetAllEmailLists()
        {
            int data = 0;
            return Ok(data);
        }



        [Route("api/EmailLists/count")]
        public int GetEmailListsPageCount()
        {
            return _database.Contacts.Count() > 10 ? _database.Contacts.Count() / 10 : 1;
        }
    }
}
