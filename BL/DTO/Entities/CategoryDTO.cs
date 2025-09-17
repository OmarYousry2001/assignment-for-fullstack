
using Resources;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class CategoryDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Name { get; set; } = null!;
      
    }
}
