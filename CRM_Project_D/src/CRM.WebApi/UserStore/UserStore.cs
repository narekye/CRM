using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRM.Entities;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.UserStore
{
    public partial class UserStore : IUserStore<AspNetUser>, IUserPasswordStore<AspNetUser>
    {
        private CRMContext _context;

        public UserStore(CRMContext db)
        {
            this._context = db;
        }
        /// <inheritdoc />
        public void Dispose() { }

        /// <inheritdoc />
        public Task CreateAsync(AspNetUser user)
        {
            return null;
        }

        /// <inheritdoc />
        public Task UpdateAsync(AspNetUser user)
        {
            return null;
        }

        /// <inheritdoc />
        public Task DeleteAsync(AspNetUser user)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<AspNetUser> FindByIdAsync(string userId)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<AspNetUser> FindByNameAsync(string userName)
        {
            return null;
        }

        /// <inheritdoc />
        public Task SetPasswordHashAsync(AspNetUser user, string passwordHash)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<string> GetPasswordHashAsync(AspNetUser user)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<bool> HasPasswordAsync(AspNetUser user)
        {
            return null;
        }
    }
}