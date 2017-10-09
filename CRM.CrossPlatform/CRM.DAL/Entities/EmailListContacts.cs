using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class EmailListContacts
    {
        public int EmailListId { get; set; }
        public int ContactId { get; set; }

        public Contacts Contact { get; set; }
        public EmailLists EmailList { get; set; }
    }
}
