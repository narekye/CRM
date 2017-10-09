using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class EmailLists
    {
        public EmailLists()
        {
            EmailListContacts = new HashSet<EmailListContacts>();
        }

        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public ICollection<EmailListContacts> EmailListContacts { get; set; }
    }
}
