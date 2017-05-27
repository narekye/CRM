namespace CRM.WebApi.Models
{
    using System.Collections.Generic;

    public class ViewEmailList
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ViewContactLess> Contacts { get; set; }
    }
}