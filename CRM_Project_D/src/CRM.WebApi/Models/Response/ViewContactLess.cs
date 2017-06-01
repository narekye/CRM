namespace CRM.WebApi.Models.Response
{
    using System;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    [JsonObject]
    public class ViewContactLess
    {
        [JsonProperty("Full Name")]
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public Guid GuID { get; set; }
    }
}