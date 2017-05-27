namespace CRM.WebApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    public class ViewEmailLists
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ViewContact> Contacts { get; set; }

        public static ViewEmailLists GetEmailListsModel(EmailList emailList)
        {
            return new ViewEmailLists
            {
                Contacts = (from d in emailList.Contacts
                            select new ViewContact
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

        public static List<ViewEmailLists> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<ViewEmailLists>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetEmailListsModel(emailList));
            }
            return result;
        }

    }
}