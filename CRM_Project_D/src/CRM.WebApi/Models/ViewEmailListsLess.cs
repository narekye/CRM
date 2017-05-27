using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Entities;

namespace CRM.WebApi.Models
{
    public class ViewEmailListsLess
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public static ViewEmailListsLess GetEmailListsLess(EmailList emailList)
        {
            return new ViewEmailListsLess
            {
                EmailListName = emailList.EmailListName,
                EmailListId = emailList.EmailListID
            };
        }

        public static List<ViewEmailListsLess> GetEmailListsLessList(List<EmailList> list)
        {
            var result = new List<ViewEmailListsLess>();
            foreach (EmailList emailList in list)
            {
                result.Add(new ViewEmailListsLess()
                {
                    EmailListName = emailList.EmailListName,
                    EmailListId = emailList.EmailListID
                });
            }
            return result;
        }
    }
}