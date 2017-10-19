using System.ComponentModel.DataAnnotations;
namespace CRM.WebApi.Models.Request
{
    using System.Collections.Generic;
    using Response;
    public class RequestContact
    {
        [Required]
        public ViewContactModel FilterBy { get; set; }
        [Required]
        public Dictionary<string,string> SortBy { get; set; }
    }
}