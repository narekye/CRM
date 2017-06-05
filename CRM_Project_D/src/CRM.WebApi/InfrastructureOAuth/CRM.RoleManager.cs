using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace CRM.WebApi.InfrastructureOAuth
{
    public class CrmRoleManager : RoleManager<Role>
    {
        public CrmRoleManager(IRoleStore<Role, string> store) : base(store)
        {
        }

        public static CrmRoleManager Create(IdentityFactoryOptions<CrmRoleManager> options, IOwinContext context)
        {
            var crmRolemanager = new CrmRoleManager(new RoleStore(context.Get<CRMContext>()));
            return crmRolemanager;
        }
    }
}