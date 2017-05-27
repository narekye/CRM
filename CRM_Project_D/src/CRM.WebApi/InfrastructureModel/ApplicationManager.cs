namespace CRM.WebApi.InfrastructureModel
{
    using LinqToExcel;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using Entities;
    using System.IO;
    using System.Threading.Tasks;
    using Models;

    public class ApplicationManager : IDisposable
    {
        private static readonly CRMContext Database = new CRMContext();
        // private static readonly DbContextTransaction Transaction = Database.Database.BeginTransaction();

        static ApplicationManager()
        {
            Database.Configuration.LazyLoadingEnabled = false;
        }

        #region SendEmail

        private static Contact c;

        public static List<Contact> GetContactsFromFile(byte[] bytes)
        {
            List<Contact> contactsList = new List<Contact>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\newfile.xlsx";
            try
            {
                File.WriteAllBytes(path, bytes);
                var excel = new ExcelQueryFactory(path);
                var contacts = from c in excel.Worksheet<Row>("Sheet1")
                               select c;

                foreach (var m in contacts)
                {
                    c = new Contact();
                    c.FullName = m["fullname"];
                    c.CompanyName = m["company"];
                    c.Country = m["country"];
                    c.Position = m["position"];
                    c.Email = m["email"];
                    c.DateInserted = Convert.ToDateTime(m["datainserted"]);
                    c.GuID = Guid.NewGuid();
                    contactsList.Add(c);
                }
            }
            catch { }
            finally
            {
                File.Delete(path);
            }
            if (contactsList == null) return null;
            return contactsList;
        }

        #endregion

        // working, almost change the mapping
        // TODO: change to view model second without emaillists
        public async Task<List<ViewContactLess>> GetAllContactsAsync()
        {
            try
            {
                var list = await Database.Contacts.ToListAsync();
                var data = ViewContactLess.CreateViewModelLess(list);
                return data;
            }
            catch (Exception ex)
            {
                throw new EntityException(ex.Message);
            }
        }

        // working
        public async Task<ViewContact> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            try
            {
                var contact =
                    await Database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.ContactId == id.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = ViewContact.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // working
        public async Task<ViewContact> GetContactByGuidAsync(Guid? guid)
        {
            try
            {
                var contact =
                    await Database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.GuID == guid.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = ViewContact.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // working
        public async Task<bool> UpdateConactAsync(ViewContact contact)
        {
            if (ReferenceEquals(contact, null)) return false;
            using (var transaction = Database.Database.BeginTransaction())
            {
                try
                {
                    var original =
                        await
                            Database.Contacts.Include(p => p.EmailLists)
                                .FirstOrDefaultAsync(p => p.GuID == contact.GuId);
                    var replace = await ViewContact.GetContactFromContactModel(contact, false);
                    replace.ContactId = original.ContactId;
                    Database.Entry(original).CurrentValues.SetValues(replace);
                    await Database.SaveChangesAsync();
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

        // working
        public async Task<bool> AddContactAsync(ViewContact contact)
        {
            var cont = await ViewContact.GetContactFromContactModel(contact, true);
            using (var transaction = Database.Database.BeginTransaction())
            {
                try
                {
                    Database.Contacts.Add(cont);
                    await Database.SaveChangesAsync();
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

        // working
        public async Task<bool> DeleteContactAsync(Guid guid)
        {
            var cont = await Database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
            try
            {
                Database.Contacts.Remove(cont);
                await Database.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // public async Task<List<Contact>> 
        public async Task<int> PageCountAsync()
        {
            try
            {
                var count = await Database.Contacts.CountAsync();
                return count / 10 + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<ViewEmailListsLess>> GetAllEmailListsAsync()
        {
            try
            {
                var list = await Database.EmailLists.ToListAsync();
                if (ReferenceEquals(list, null)) return null;
                var result = ViewEmailListsLess.GetEmailListsLessList(list);
                if (ReferenceEquals(result, null)) return null;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}