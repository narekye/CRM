namespace CRM.WebApi.Models.Response
{
    using System.Collections.Generic;
    public class ViewEmailList
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ViewContactLess> Contacts { get; set; }
    }
}