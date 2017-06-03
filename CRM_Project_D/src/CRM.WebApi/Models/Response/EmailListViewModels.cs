namespace CRM.WebApi.Models.Response
{
    using System.Collections.Generic;
    public class ViewEmailList
    {
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public List<ViewContactLess> Contacts { get; set; }
    }
    public class ViewEmailListLess
    {
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    }
    public class ViewTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
    }
}