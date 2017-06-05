using System;
using System.Data.Entity;
using System.Threading.Tasks;
using CRM.Entities;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.InfrastructureOAuth
{
    public class RoleStore : IRoleStore<Role>
    {
        private CRMContext db;

        public RoleStore(CRMContext db)
        {
            this.db = db;
        }

        public void Dispose()
        {
            this.db.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task CreateAsync(Role role)
        {
            this.db.Roles.Add(role);
            return this.db.SaveChangesAsync();
        }

        public Task UpdateAsync(Role role)
        {
            this.db.Entry(role).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(Role role)
        {
            this.db.Roles.Remove(role);
            return this.db.SaveChangesAsync();
        }

        public Task<Role> FindByIdAsync(string roleId)
        {
            var role = this.db.Roles.SingleOrDefaultAsync(p => p.Id == roleId);
            return role;
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            var role = this.db.Roles.SingleOrDefaultAsync(p => p.Name == roleName);
            return role;
        }
    }
}