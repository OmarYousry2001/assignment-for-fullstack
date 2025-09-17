using Resources;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.User
{
    public class LoginDTO
    {

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$",
         ErrorMessageResourceName = "PasswordComplexity", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Password { get; set; } = null!;

        //public string? ReturnUrl { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        public bool RememberMe { get; set; }
    }
}