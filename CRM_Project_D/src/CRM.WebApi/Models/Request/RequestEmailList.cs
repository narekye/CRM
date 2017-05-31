
namespace CRM.WebApi.Models.Request
{
    using System;
    using System.Collections.Generic;
    public class RequestEmailList
    {
        public int EmailListID { get; set; }
        public List<Guid> Guids { get; set; }
    }
}