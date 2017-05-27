namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Models;

    public enum SortBy
    {
        NoSort,
        Ascending,
        Descending,
    }
    public partial class ApplicationManager : IDisposable
    {
        private readonly CRMContext _database;
        private readonly ModelFactory _factory;
        public ApplicationManager()
        {
            _factory = new ModelFactory();
            _database = new CRMContext();
            _database.Configuration.LazyLoadingEnabled = false;
        }
        // working, almost change the mapping
        public async Task<List<ViewContactLess>> GetAllContactsAsync()
        {
            try
            {
                var list = await _database.Contacts.ToListAsync();
                var data = _factory.CreateViewModelLess(list);
                return data;
            }
            catch (Exception ex)
            {
                throw new EntityException(ex.Message);
            }
        }
        public async Task<ViewContact> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            try
            {
                var contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.ContactId == id.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = _factory.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ViewContact> GetContactByGuidAsync(Guid? guid)
        {
            try
            {
                var contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.GuID == guid.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = _factory.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateConactAsync(ViewContact contact)
        {
            if (ReferenceEquals(contact, null)) return false;
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original =
                        await
                            _database.Contacts.Include(p => p.EmailLists)
                                .FirstOrDefaultAsync(p => p.GuID == contact.GuId);
                    var replace = await _factory.GetContactFromContactModel(contact, false);
                    replace.ContactId = original.ContactId;
                    _database.Entry(original).CurrentValues.SetValues(replace);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<bool> AddContactAsync(ViewContact contact)
        {
            var cont = await _factory.GetContactFromContactModel(contact, true);
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    _database.Contacts.Add(cont);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<bool> DeleteContactAsync(Guid guid)
        {
            var cont = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
            try
            {
                _database.Contacts.Remove(cont);
                await _database.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<int> PageCountAsync()
        {
            try
            {
                var count = await _database.Contacts.CountAsync();
                return count / 10 + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<ViewContactLess>> BigPostRequest(List<string> values, SortBy select)
        {
            var data = new List<string>();
            if (values.Contains("FullName"))
            {
                data = await _database.Contacts.Select(p => p.FullName).ToListAsync();
            }
            if (values.Contains("Position"))
            {
                data = await _database.Contacts.Select(p => p.Position).ToListAsync();

            }

            if (select == SortBy.Ascending)
            {
                // sort by ascending
                data.Sort();
            }
            if (select == SortBy.Descending)
            {
                // sort by descending
            }
            if (select == SortBy.NoSort)
            {
                // only return;
            }
            return null;
        }
        public void Dispose()
        {
            _database.Dispose();
        }
    }
}