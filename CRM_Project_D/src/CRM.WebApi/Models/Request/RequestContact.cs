namespace CRM.WebApi.Models.Request
{
    using System.Collections.Generic;
    using Response;
    public class RequestContact
    {
        public ViewContactLess FilterBy { get; set; }
        public Dictionary<string,string> SortBy { get; set; }
    }
}