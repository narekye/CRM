using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using Newtonsoft.Json;

namespace CRM.WebApi.Models.Request
{
    public class RequestEmailList
    {
        
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        
        public List<Guid> Guids { get; set; }
    }
}