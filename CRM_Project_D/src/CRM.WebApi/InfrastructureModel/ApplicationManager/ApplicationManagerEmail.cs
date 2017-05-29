namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Entities;
    using Models.Response;

    public partial class ApplicationManager
    {
        public async Task<List<ViewEmailListLess>> GetAllEmailListsAsync()
        {
            try
            {
                List<EmailList> list = await _database.EmailLists.ToListAsync();
                if (ReferenceEquals(list, null)) return null;
                List<ViewEmailListLess> result = _factory.GetEmailListsLessList(list);
                if (ReferenceEquals(result, null)) return null;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ViewEmailList> GetEmailListById(int? id)
        {
            if (!id.HasValue) return null;
            try
            {
                EmailList data = await _database.EmailLists.Include(p => p.Contacts)
                    .FirstOrDefaultAsync(p => p.EmailListID == id.Value);
                if (ReferenceEquals(data, null)) return null;
                var result = _factory.GetViewEmailList(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddEmailList(ViewEmailList emailList)
        {
            using (DbContextTransaction transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    EmailList entity = await _factory.EntityCreateEmailList(emailList, true);
                    List<ViewContact> viewcontacts = _factory.GetViewContactListFromLessList(emailList.Contacts);
                    List<Contact> entitycontacts = _factory.EntityGetContactListFromViewContactList(viewcontacts, true);
                    entity.Contacts = entitycontacts;
                    _database.EmailLists.Add(entity);
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

        // update emallist, need to be done...
        public async Task<bool> UpdateEmailListAsync(ViewEmailList emaillists)
        {
            EmailList original = await _database.EmailLists.Include(z => z.Contacts)
                .FirstOrDefaultAsync(p => p.EmailListID == emaillists.EmailListId);
            var viewcontacts = _factory.GetViewContactListFromLessList(emaillists.Contacts);
            var contacts = _factory.EntityGetContactListFromViewContactList(viewcontacts, true);
            var replace = _factory.EntityCreateEmailList(emaillists, true, contacts);
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
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
        public async Task<bool> DeleteEmailListByIdAsync(int? id)
        {
            if (!id.HasValue) return false;
            using (DbContextTransaction transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    EmailList emaillist = await _database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == id.Value);
                    _database.EmailLists.Remove(emaillist);
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
    }
}