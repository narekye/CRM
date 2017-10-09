using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class Contacts
    {
        public Contacts()
        {
            EmailListContacts = new HashSet<EmailListContacts>();
        }

        public int ContactId { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuId { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime? DateModified { get; set; }

        public ICollection<EmailListContacts> EmailListContacts { get; set; }
    }
}
