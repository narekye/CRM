using CRM.Entities;
using CRM.WebApi.InfrastructureOAuth;
using System.Web.Http;
namespace CRM.WebApi.Controllers
{
    [Authorize]
    public class AccountController : ApiController
    {
        private CrmUserManager manager;
        public AccountController()
        {
            var db = new CRMContext();
            manager = new CrmUserManager(new UserStore(db));
        }

        [Authorize]
        [Route("api/account/getusers")]
        public IHttpActionResult GetUsers()
        {
            return BadRequest();
        }
    }
}
