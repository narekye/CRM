using System.ComponentModel.DataAnnotations;

namespace CRM.WebApi.Models.Identity
{
    public class RegisterUserModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}