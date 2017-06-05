using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRM.Entities;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.InfrastructureOAuth.CRM.UserManager
{
    public partial class UserStore : IUserPasswordStore<User>
    {
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            var us = this.db.Users.FirstOrDefaultAsync(p => p.Id == user.Id).Result;
            return Task.FromResult<string>(us.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            if (user.PasswordHash != null) return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}