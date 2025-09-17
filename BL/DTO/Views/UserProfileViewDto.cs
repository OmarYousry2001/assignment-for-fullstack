using Resources;
using Resources.Data.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Views
{
    public class UserProfileViewDto
    {
        public string Id { get; set; } = null!;
        [Display(Name = "FirstName", ResourceType = typeof(FormResources))]
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "LastName", ResourceType = typeof(FormResources))]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailFormat", ErrorMessageResourceType = typeof(ValidationResources))]
        public string City { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [EmailAddress(ErrorMessageResourceName = "EmailFormat", ErrorMessageResourceType = typeof(ValidationResources))]
        public string Email { get; set; } = null!;
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "PhoneNumber", ResourceType = typeof(FormResources))]
        public string PhoneNumber { get; set; } = null!;
    }
}
