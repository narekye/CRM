namespace CRM.WebApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    using AutoMapper;
    public class EmailListsModel
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public Dictionary<string, string> Contacts { get; set; }

        public static EmailListsModel GetEmailListsModel(EmailList emailList)
        {
            int i = 0;
            return new EmailListsModel
            {
                Contacts = (from d in emailList.Contacts
                            select new ViewContact()
                            {
                                FullName = d.GuID.ToString(),
                                Email = d.Email
                            }).ToDictionary(item => item.FullName, item => item.Email),
                EmailListName = emailList.EmailListName,
                EmailListId = emailList.EmailListID
            };
        }

        public static List<EmailListsModel> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<EmailListsModel>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetEmailListsModel(emailList));
            }
            return result;
        }
    }
}