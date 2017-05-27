namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Models;
    using Entities;
    public partial class ApplicationManager
    {
        public async Task<List<ViewEmailListLess>> GetAllEmailListsAsync()
        {
            try
            {
                var list = await _database.EmailLists.ToListAsync();
                if (ReferenceEquals(list, null)) return null;
                var result = _factory.GetEmailListsLessList(list);
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
                var data = await _database.EmailLists.Include(p => p.Contacts)
                    .FirstOrDefaultAsync(p => p.EmailListID == id.Value);
                if (ReferenceEquals(data, null)) return null;
                var result = _factory.GetEmailListsModel(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // test not completed yet.
        public async Task<bool> AddEmailList(ViewEmailList emailList)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var data = await _factory.CreateEmailList(emailList, true);
                    _database.EmailLists.Add(data);
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

        // update emallist
        public async Task<EmailList> UpdateEmailList(ViewEmailList emaillists)
        {
            var emaillist = await _database.EmailLists.Include(z => z.Contacts).FirstOrDefaultAsync(p => p.EmailListID == emaillists.EmailListId);
            return emaillist;
        }
        // test
        public async Task<bool> DeleteEmailListById(int? id)
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
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}