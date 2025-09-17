

using Microsoft.AspNetCore.Http;
using Resources;
using Shared.Attributes;
using Shared.CustomValidation;
using Shared.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO.Entities
{
    public class ProjectDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Title { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Description { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string ClientName { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        //[ValidYearAttribute(MinYear = 2010, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int Year { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Location { get; set; } = null!;

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5)]
        public IFormFileCollection? Photos { get; set; } = null!;

        //public List<string>? ImageName { get; set; }
        public List<ImageDTO>? Images { get; set; }
        
        // Relations
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        public Guid CategoryId { get; set; }
    }
    public class GetProjectDTO : BaseDTO
    {
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Title { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Description { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string ClientName { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [ValidYearAttribute(MinYear = 2010, ErrorMessageResourceName = "ValueMustBeBetween", ErrorMessageResourceType = typeof(ValidationResources))]
        public int Year { get; set; }
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Location { get; set; } = null!;

        public List<ImageDTO> Images { get; set; }

        // Relations
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceName = "FieldLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string CategoryName { get; set; } = null!;

    }



}
