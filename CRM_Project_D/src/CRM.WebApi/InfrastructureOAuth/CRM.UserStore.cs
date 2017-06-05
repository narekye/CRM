using CRM.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CRM.WebApi.Filters;

namespace CRM.WebApi.InfrastructureOAuth
{
    [ExceptionFilters]
    public partial class UserStore : IUserStore<User>, IUserPasswordStore<User>,
        IQueryableUserStore<User>, IUserRoleStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>
    {
        private CRMContext db;

        public UserStore()
        {
            db = new CRMContext();
            this.db.Configuration.LazyLoadingEnabled = false;
        }

        public IQueryable<User> Users
        {
            get { return db.Users.AsQueryable(); }
        }

        public UserStore(CRMContext db)
        {
            if (this.db != null)
                this.db = db;
        }

        public Task CreateAsync(User user)
        {
            db.Users.Add(user);
            var error = db.GetValidationErrors();
            if (error.Any())
            {
                throw new EntityException();
            }
            return db.SaveChangesAsync();
        }

        public Task UpdateAsync(User user)
        {
            this.db.Entry(user).State = EntityState.Modified;
            return db.SaveChangesAsync();
        }

        public Task DeleteAsync(User user)
        {
            db.Users.Remove(user);
            return db.SaveChangesAsync();
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(p => p.Id == userId);
            if (ReferenceEquals(user, null)) return null;
            return user;
        }

        public Task<User> FindByNameAsync(string userName)
        {
            var user = db.Users.FirstOrDefaultAsync(p => p.UserName == userName);
            return user;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            var us = db.Users.FirstOrDefaultAsync(p => p.Id == user.Id).Result;
            return Task.FromResult<string>(us.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            if (user.PasswordHash != null) return Task.FromResult(true);
            return Task.FromResult(false);
        }

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

        public void Dispose()
        {
            this.db?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task SetEmailAsync(User user, string email)
        {
            user.Email = email;
            this.db.Entry(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public Task<string> GetEmailAsync(User user)
        {
            var email = this.db.Users.SingleOrDefault(p => p.Id == user.Id)?.Email;
            return Task.FromResult(email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            var flag = this.db.Users.SingleOrDefault(p => p.Id == user.Id)?.ConfirmedEmail;
            if (flag.HasValue) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed)
        {
            throw new NotImplementedException();
        }
    }
}