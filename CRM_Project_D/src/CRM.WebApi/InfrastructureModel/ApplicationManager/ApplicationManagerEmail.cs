namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Entities;
    using Models.Response;
    using System.Linq;
    using Converter;
    using Models.Request;

    public partial class ApplicationManager
    {
        public async Task<List<ViewEmailListLess>> GetAllEmailListsAsync()
        {
            var list = await _database.EmailLists.ToListAsync();
            if (ReferenceEquals(list, null)) return null;
            var result = new List<ViewEmailListLess>();
            await list.ConvertToList(result);
            return result;
        }
        public async Task<ViewEmailList> GetEmailListById(int? id)
        {
            if (!id.HasValue) return null;
            EmailList data = await _database.EmailLists.Include(p => p.Contacts)
                .FirstOrDefaultAsync(p => p.EmailListID == id.Value);
            if (ReferenceEquals(data, null)) return null;
            ViewEmailList result = new ViewEmailList();
            data.ConvertTo(result);
            result.Contacts = new List<ViewContactLess>();
            var contacts = data.Contacts.ToList();
            var contactless = _factory.CreateViewContactLessList(contacts);
            result.Contacts = contactless;
            return result;
        }
        public async Task<bool> AddNewEmailList(RequestEmailList model)
        {

            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original = new EmailList();
                    model.ConvertTo(original);
                    foreach (Guid modelGuid in model.Guids)
                        original.Contacts.Add(await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == modelGuid));
                    _database.EmailLists.Add(original);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> UpdateEmailListAsync(RequestEmailList emaillist)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original =
                        await
                            _database.EmailLists.Include(p => p.Contacts)
                                .FirstOrDefaultAsync(p => p.EmailListID == emaillist.EmailListID);
                    original.Contacts.Clear();
                    var contacts = new List<Contact>();
                    foreach (Guid emaillistGuid in emaillist.Guids)
                        contacts.Add(await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == emaillistGuid));
                    original.Contacts = contacts;
                    _database.Entry(original).State = EntityState.Modified;
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> DeleteEmailListByIdAsync(int? id)
        {
            if (!id.HasValue) return false;
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var emaillist = await _database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == id.Value);
                    _database.EmailLists.Remove(emaillist);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}