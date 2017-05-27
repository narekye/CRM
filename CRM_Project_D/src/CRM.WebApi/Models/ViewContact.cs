namespace CRM.WebApi.Models
{
    using System;
    using System.Collections.Generic;

    public class ViewContact
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuId { get; set; }
        public DateTime DateInserted { get; set; }
        public List<string> EmailLists { get; set; }
    }
}