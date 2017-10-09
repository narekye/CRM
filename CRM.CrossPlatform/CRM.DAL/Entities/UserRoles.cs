using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class UserRoles
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public Roles Role { get; set; }
        public Users User { get; set; }
    }
}
