using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Entities;

namespace CRM.WebApi.Models
{
    public class ViewEmailListLess
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public static ViewEmailListLess GetEmailListsLess(EmailList emailList)
        {
            return new ViewEmailListLess
            {
                EmailListName = emailList.EmailListName,
                EmailListId = emailList.EmailListID
            };
        }

        public static List<ViewEmailListLess> GetEmailListsLessList(List<EmailList> list)
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
    }
}