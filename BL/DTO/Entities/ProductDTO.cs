
using Microsoft.AspNetCore.Http;
using Resources;
using Shared.CustomValidation;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class ProductDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Category { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(20, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        [UniqueProductCodeAttribute]
        public string ProductCode { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, double.MaxValue, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]

        public decimal Price { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int MinimumQuantity { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, 100, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public double DiscountRate { get; set; }

        public string? ImagePath { get; set; }
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5)]
        public IFormFile? Image { get; set; }  

    }
}
