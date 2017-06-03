using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CRM.Entities;
using Microsoft.AspNet.Identity;
namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        public AccountController()
        {
            var db = new CRMContext();
            var manager = new UserManager<AspNetUser, string>(new UserStore.UserStore(db));
        }
    }
}
