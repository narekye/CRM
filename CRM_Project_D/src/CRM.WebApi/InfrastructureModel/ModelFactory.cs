namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Models.Response;
    using Converter;
    public class ModelFactory : IDisposable
    {
        private readonly CRMContext _database;
        public ModelFactory()
        {
            _database = new CRMContext();
        }
        public ViewContact CreateViewContact(Contact contact)
        {
            ViewContact result = new ViewContact();
            contact.ConvertTo(result);
            result.EmailLists = contact.EmailLists.Select(p => p.EmailListName).ToList();
            return result;
        }
        public List<ViewContact> GetViewModelList(List<Contact> list)
        {
            var result = new List<ViewContact>();
            foreach (Contact contact in list)
                result.Add(CreateViewContact(contact));
            return result;
        }
        public ViewContact ViewContactGet(ViewContactLess less)
        {
            ViewContact contact = new ViewContact();
            less.ConvertTo(contact);
            return contact;
        }
        public List<ViewContact> GetViewContactListFromLessList(List<ViewContactLess> less)
        {
            var list = new List<ViewContact>();
            less.ForEach(i => list.Add(ViewContactGet(i)));
            return list;
        }
        #region RETURNS contact

        public async Task<Contact> GetContactFromViewContact(ViewContact model, bool flag, List<EmailList> emailList = null)
        {
            Contact contact = new Contact();
            model.ConvertTo(contact);
            if (flag) // true returns new object with new Guid from model without contactId and emaillist |
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
                contact = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == model.GuID);
                contact.FullName = model.FullName;
                contact.CompanyName = model.CompanyName;
                contact.Country = model.Country;
                contact.Email = model.Email;
                contact.Position = model.Position;
                contact.EmailLists = emailList;
            }
            return contact;
        }

        public List<Contact> EntityGetContactListFromViewContactList(List<ViewContact> list, bool flag, List<EmailList> emailLists = null)
        {
            var result = new List<Contact>();
            if (flag) list.ForEach(async i => result.Add(await GetContactFromViewContact(i, true, emailLists)));
            else
            {
                list.ForEach(async z => result.Add(await GetContactFromViewContact(z, false)));
            }
            return result;
        }
        #endregion
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

        public List<ViewContactLess> CreateViewContactLessList(List<Contact> contacts)
        {
            var data = new List<ViewContactLess>();
            contacts.ForEach(p =>
            {
                data.Add(new ViewContactLess()
                {
                    CompanyName = p.CompanyName,
                    FullName = p.FullName,
                    Email = p.Email,
                    Position = p.Position,
                    GuID = p.GuID,
                    Country = p.Country
                });
            });
            return data;
        }

        #endregion
        #region VIEWEMAILLIST

        public ViewEmailList GetViewEmailList(EmailList emailList)
        {
            return new ViewEmailList
            {
                Contacts = (from d in emailList.Contacts
                            select new ViewContactLess
                            {
                                CompanyName = d.CompanyName,
                                Country = d.Country,
                                Email = d.Email,
                                FullName = d.FullName,
                                GuID = d.GuID,
                                Position = d.Position
                            }).ToList(),
                EmailListName = emailList.EmailListName,
                EmailListID = emailList.EmailListID
            };
        }

        public List<ViewEmailList> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<ViewEmailList>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetViewEmailList(emailList));
            }
            return result;
        }

        public async Task<EmailList> EntityCreateEmailList(ViewEmailList viewEmailList, bool flag, ICollection<Contact> contacts = null)
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
                var list = await
                    _database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == viewEmailList.EmailListID);
                obj = new EmailList
                {
                    EmailListName = viewEmailList.EmailListName,
                    Contacts = list?.Contacts
                };
            }
            return obj;
        }

        #endregion
        public void Dispose()
        {
            _database.Dispose();
        }
        public ViewTemplate GetViewTemplate(Template template)
        {
            ViewTemplate up = new ViewTemplate();
            template.ConvertTo(up);
            return up;
        }
        public List<ViewTemplate> GetViewTemplates(List<Template> templates)
        {
            if (ReferenceEquals(templates, null)) return null;
            var list = new List<ViewTemplate>();
            templates.ForEach(i => list.Add(GetViewTemplate(i)));
            return list;
        }
    }
}