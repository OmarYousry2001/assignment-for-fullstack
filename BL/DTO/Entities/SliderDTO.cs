
using Microsoft.AspNetCore.Http;
using Resources;
using Shared.CustomValidation;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class SliderDTO : BaseDTO
    {
        public string? ImgPath { get; set; }
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5)]
        public IFormFile? Img { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, 20, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int Order { get; set; }
    }
}
