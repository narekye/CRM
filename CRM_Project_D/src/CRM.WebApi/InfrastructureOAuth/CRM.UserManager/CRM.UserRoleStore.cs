using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace CRM.WebApi.InfrastructureOAuth.CRM.UserManager
{
    public partial class UserStore : IUserRoleStore<User>
    {
        public Task AddToRoleAsync(User user, string roleName)
        {
            user.Roles.Add(this.db.Roles.SingleOrDefault(p => p.Name == roleName));
            return this.db.SaveChangesAsync();
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null || string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException();
            Role role = user.Roles.SingleOrDefault(p => p.Name == roleName);
            if (!ReferenceEquals(role, null))
            {
                user.Roles.Remove(role);
            }
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            IList<string> list = new List<string>();
            user.Roles.ForEach(p => list.Add(p.Name));
            return Task.FromResult(list);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            bool result = user.Roles.Any(p => p.Name == roleName);
            return Task.FromResult(result);
        }
    }
}