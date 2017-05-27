namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Models;

    public class ModelFactory : IDisposable
    {
        private readonly CRMContext _database = new CRMContext();
        public ViewContact CreateViewModel(Contact contact)
        {
            var result = new ViewContact()
            {
                EmailLists = contact.EmailLists.Select(p => p.EmailListName).ToList(),
                FullName = contact.FullName,
                Email = contact.Email,
                DateInserted = contact.DateInserted,
                CompanyName = contact.CompanyName,
                Country = contact.Country,
                Position = contact.Position,
                GuId = contact.GuID
            };
            return result;
        }
        public List<ViewContact> GetViewModelList(List<Contact> list)
        {
            var result = new List<ViewContact>();
            foreach (Contact contact in list)
                result.Add(CreateViewModel(contact));
            return result;
        }

        public async Task<Contact> GetContactFromContactModel(ViewContact model, bool flag, List<EmailList> emailList = null)
        {
            Contact contact;
            if (flag) // true returns new object with new Guid from model without contactId
            {
                contact = new Contact()
                {
                    GuID = Guid.NewGuid(),
                    DateInserted = DateTime.UtcNow,
                    FullName = model.FullName,
                    CompanyName = model.CompanyName,
                    Country = model.Country,
                    Email = model.Email,
                    Position = model.Position,
                    EmailLists = emailList
                };
            }
            else // false returns object from database 
            {
                contact = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == model.GuId);
                contact.FullName = model.FullName;
                contact.CompanyName = model.CompanyName;
                contact.Country = model.Country;
                contact.Email = model.Email;
                contact.Position = model.Position;
                contact.EmailLists = emailList;
            }
            return contact;
        }

        #region VIEWemailMODELLESS
        public ViewEmailListLess GetEmailListsLess(EmailList emailList)
        {
            return new ViewEmailListLess
            {
                EmailListName = emailList.EmailListName,
                EmailListId = emailList.EmailListID
            };
        }

        public List<ViewEmailListLess> GetEmailListsLessList(List<EmailList> list)
        {
            var result = new List<ViewEmailListLess>();
            foreach (EmailList emailList in list)
            {
                result.Add(new ViewEmailListLess()
                {
                    EmailListName = emailList.EmailListName,
                    EmailListId = emailList.EmailListID
                });
            }
            return result;
        }


        #endregion

        #region VIEWcontactMODELLESS

        public List<ViewContactLess> CreateViewModelLess(List<Contact> contacts)
        {
            var data = new List<ViewContactLess>();
            contacts.ForEach(p =>
            {
                data.Add(new ViewContactLess()
                {
                    CompanyName = p.CompanyName,
                    FullName = p.FullName,
                    DateInserted = p.DateInserted,
                    Email = p.Email,
                    Position = p.Position,
                    GuId = p.GuID,
                    Country = p.Country
                });
            });
            return data;
        }

        #endregion

        #region VIEWEMAILLIST

        public ViewEmailList GetEmailListsModel(EmailList emailList)
        {
            return new ViewEmailList
            {
                Contacts = (from d in emailList.Contacts
                            select new ViewContactLess
                            {
                                CompanyName = d.CompanyName,
                                Country = d.Country,
                                DateInserted = d.DateInserted,
                                Email = d.Email,
                                FullName = d.FullName,
                                GuId = d.GuID,
                                Position = d.Position
                            }).ToList(),
                EmailListName = emailList.EmailListName,
                EmailListId = emailList.EmailListID
            };
        }

        public List<ViewEmailList> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<ViewEmailList>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetEmailListsModel(emailList));
            }
            return result;
        }

        public async Task<EmailList> CreateEmailList(ViewEmailList viewEmailList, bool flag, List<Contact> contacts = null)
        {
            EmailList obj;
            if (flag) // returns new object 
            {

                obj = new EmailList
                {
                    EmailListName = viewEmailList.EmailListName,
                    Contacts = contacts
                };
            }
            else // returns object from db
            {
                using (var database = new CRMContext())
                {
                    var list = await
                        database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == viewEmailList.EmailListId);
                    obj = new EmailList
                    {
                        EmailListName = viewEmailList.EmailListName,
                        Contacts = list?.Contacts
                    };
                }
            }
            return obj;
        }

        #endregion
        public void Dispose()
        {
            _database.Dispose();
        }
    }
}