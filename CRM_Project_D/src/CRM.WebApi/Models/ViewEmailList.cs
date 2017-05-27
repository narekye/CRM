using System.Data.Entity;
using System.Threading.Tasks;

namespace CRM.WebApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    public class ViewEmailList
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ViewContactLess> Contacts { get; set; }

        public static ViewEmailList GetEmailListsModel(EmailList emailList)
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

        public static List<ViewEmailList> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<ViewEmailList>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetEmailListsModel(emailList));
            }
            return result;
        }

        public static async Task<EmailList> CreateEmailList(ViewEmailList viewEmailList, bool flag, List<Contact> contacts = null)
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
    }
}