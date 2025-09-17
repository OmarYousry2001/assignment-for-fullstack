using Resources;
using Resources.Data.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.User
{
    /// <summary>
    /// Data transfer object for initiating a password reset.
    /// </summary>
    public class PasswordResetDto
    {
        /// <summary>
        /// Gets or sets the email of the user requesting the password reset.
        /// </summary>
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailFormat", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the new password for the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string NewPassword { get; set; } = null!;
    }
}
