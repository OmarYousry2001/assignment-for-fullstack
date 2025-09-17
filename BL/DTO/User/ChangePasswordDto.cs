using Resources;
using Resources.Data.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Compare(nameof(NewPassword), ErrorMessageResourceName = "PasswordConfirmation_Mismatch", ErrorMessageResourceType = typeof(UserResources))]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; } = null!;
    }
}
