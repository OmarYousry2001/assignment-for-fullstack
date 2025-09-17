

using Resources;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class CommentDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Name { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Message { get; set; } = null!;
    }
}
