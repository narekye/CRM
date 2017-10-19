namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Entities;
    using Models.Response;
    using System.Linq;
    using Models.Request;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
    public partial class ApplicationManager
    {
        public async Task<List<ViewEmailListLess>> GetAllEmailListsAsync()
        {
            var list = await _database.EmailLists.ToListAsync();
            if (ReferenceEquals(list, null)) return null;
            var result = new List<ViewEmailListLess>();
            AutoMapper.Mapper.Map(list, result);
            return result;
        }
        public async Task<ViewEmailList> GetEmailListByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            EmailList data = await _database.EmailLists.Include(p => p.Contacts)
                .FirstOrDefaultAsync(p => p.EmailListID == id.Value);
            if (ReferenceEquals(data, null)) return null;
            ViewEmailList result = new ViewEmailList();
            result.Contacts = new List<ViewContactModel>();
            var contacts = data.Contacts.ToList();
            var contactless = new List<ViewContactModel>();
            AutoMapper.Mapper.Map(data, result);
            AutoMapper.Mapper.Map(contacts, contactless);
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
                    AutoMapper.Mapper.Map(model, original);
                    if (!ReferenceEquals(model.Guids, null))
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
        public async Task<bool> UpdateEmailListAsync(RequestEmailList model)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original =
                        await
                            _database.EmailLists.Include(p => p.Contacts)
                                .FirstOrDefaultAsync(p => p.EmailListID == model.EmailListID);
                    if (!ReferenceEquals(model.EmailListName, null)) original.EmailListName = model.EmailListName;
                    if (!ReferenceEquals(model.Guids, null))
                    {
                        var contacts = new List<Contact>();
                        original.Contacts.Clear();
                        foreach (Guid emaillistGuid in model.Guids)
                            contacts.Add(await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == emaillistGuid));
                        original.Contacts = contacts;
                    }
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
        public async Task<bool> AddContactsToExsistingList(RequestEmailList model)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original = await _database.EmailLists.FirstOrDefaultAsync(p => model.EmailListID == p.EmailListID);
                    var list = new List<Contact>();
                    foreach (Guid modelGuid in model.Guids)
                    {
                        var item = await _database.Contacts.FirstOrDefaultAsync(z => z.GuID == modelGuid);
                        list.Add(item);
                    }
                    original.Contacts.ForEach(z => { if (list.Contains(z)) list.Remove(z); });
                    list.ForEach(p => original.Contacts.Add(p));
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
        public async Task<bool> DeleteContactsFromEmailListAsync(RequestEmailList model)
        {
            var emails =
                    _database.EmailLists.Include(p => p.Contacts);
            var email = await emails.FirstOrDefaultAsync(p => p.EmailListID == model.EmailListID);
            if (!email.Contacts.Any()) return false;
            var list = new List<Contact>();
            model.Guids.ForEach(p => list.Add(email.Contacts.SingleOrDefault(z => z.GuID == p)));
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    this._database.Configuration.LazyLoadingEnabled = false;
                    list.ForEach(p => email.Contacts.Remove(p));
                    _database.Entry(email).State = EntityState.Modified;
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