using Microsoft.AspNetCore.Http;
using Resources;
using Shared.CustomValidation;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class SettingsDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Location { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string InteriorLink { get; set; } = null!;// or WebsiteLink
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string AboutMe { get; set; } = null!;

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string CopyrightText { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, 10000, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int ProjectsCompleted { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, 100, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int YearsExperience { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Range(1, 10000, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int HappyClients { get; set; }
        public string? LogoName { get; set; }
        public string? PersonPhotoName { get; set; }
        public string? VideoName { get; set; }

        


        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5)]
        public IFormFile? Photo { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5)]
        public IFormFile? PersonPhoto { get; set; }

        public IFormFile? Video { get; set; }

    }
}
