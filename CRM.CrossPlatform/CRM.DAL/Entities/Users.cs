using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class Users
    {
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
