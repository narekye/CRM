namespace CRM.WebApi.Models.Response
{
    using System;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
    [JsonObject]
    [NotNullValidator]
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
        [RegexValidator("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        public string Email { get; set; }
        public Guid GuID { get; set; }
    }
}