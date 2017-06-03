namespace CRM.WebApi.Models.Response
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
    using Newtonsoft.Json;
    [JsonObject]
    public class ViewContact
    {
        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public List<string> EmailLists { get; set; }
    }
    [JsonObject]
    [NotNullValidator]
    public class ViewContactLess
    {
        [JsonProperty("Full Name"), Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The full name must be specified.")]
        public string FullName { get; set; }
        [JsonProperty("Company Name"), Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Company name can't be empty.")]
        public string CompanyName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Position can't be empty.")]
        public string Position { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Country can't be empty.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegexValidator("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        [StringLength(100, MinimumLength = 1)]
        public string Email { get; set; }
        public Guid GuID { get; set; }
    }
}