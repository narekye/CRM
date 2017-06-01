namespace CRM.WebApi.Models.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class RequestEmailList
    {
        [Required(ErrorMessage = "EmaillistId is required")]
        public int EmailListID { get; set; }
        [Required(ErrorMessage = "List of guids required")]
        public List<Guid> Guids { get; set; }
    }
}