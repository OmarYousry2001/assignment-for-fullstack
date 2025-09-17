using Resources;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.User
{
    public class RegisterDTO 
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string UserName { get; set; } = null!; 

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$",
         ErrorMessageResourceName = "PasswordComplexity", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Password { get; set; } = null!;

        [Required(ErrorMessageResourceName = "PasswordConfirmation_Mismatch", ErrorMessageResourceType = typeof(UserResources))]
        [Compare(nameof(Password), ErrorMessageResourceName = "PasswordConfirmation_Mismatch", ErrorMessageResourceType = typeof(UserResources))]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; } = null!;
    }
}
