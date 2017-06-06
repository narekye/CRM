using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using CRM.Entities;
using CRM.WebApi.Filters;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.InfrastructureOAuth.CRM.UserManager
{
    [ExceptionFilters]
    public partial class UserStore : IUserStore<User>, IQueryableUserStore<User>, IUserEmailStore<User>
    {
        private readonly CRMContext db;

        public UserStore()
        {
            this.db = new CRMContext();
            this.db.Configuration.LazyLoadingEnabled = false;
        }

        public IQueryable<User> Users
        {
            get { return this.db.Users.AsQueryable(); }
        }

        public UserStore(CRMContext db)
        {
            if (this.db == null)
                this.db = db;
        }

        public Task CreateAsync(User user)
        {
            this.db.Users.Add(user);
            var error = this.db.GetValidationErrors();
            if (error.Any())
            {
                throw new EntityException();
            }
            return this.db.SaveChangesAsync();
        }

        public Task UpdateAsync(User user)
        {
            this.db.Entry(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(User user)
        {
            this.db.Users.Remove(user);
            return this.db.SaveChangesAsync();
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            var user = await this.db.Users.FirstOrDefaultAsync(p => p.Id == userId);
            if (ReferenceEquals(user, null)) return null;
            return user;
        }

        public Task<User> FindByNameAsync(string userName)
        {
            var user = this.db.Users.FirstOrDefaultAsync(p => p.UserName == userName);
            return user;
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
            var flag = this.db.Users.SingleOrDefault(p => p.Id == user.Id)?.EmailConfirmed;
            if (flag.HasValue) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;            
            this.db.Entry(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public Task<User> FindByEmailAsync(string email)
        {
            var user = this.db.Users.FirstOrDefaultAsync(p => p.Email == email);
            return user;
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            this.db.Entry(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public Task<string> GetPhoneNumberAsync(User user)
        {
            var phone = this.db.Users.SingleOrDefaultAsync(p => p.Id == user.Id);
            return Task.FromResult(phone.Result.Email);
        }
        public void Dispose()
        {
            this.db?.Dispose();
            // GC.SuppressFinalize(this);
        }
    }
}