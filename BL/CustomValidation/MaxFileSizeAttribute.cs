using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.CustomValidation
{
    /// <summary>
    /// Custom validation attribute to restrict the maximum allowed file size.
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInBytes;

        /// <summary>
        /// Constructor to set the maximum file size.
        /// </summary>
        /// <param name="maxSizeInMB">Maximum file size in megabytes.</param>
        public MaxFileSizeAttribute(int maxSizeInMB)
        {
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024; // Convert MB to Bytes
        }

        /// <summary>
        /// Validates if the uploaded file size does not exceed the limit.
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            // Skip validation if the file is null (allowing optional fields)
            if (file == null) return ValidationResult.Success;

            if (file.Length > _maxSizeInBytes)
            {
                var fieldName = validationContext.DisplayName ?? "File";
                return new ValidationResult($"{fieldName} size must not exceed {_maxSizeInBytes / (1024 * 1024)}MB.");
            }

            return ValidationResult.Success;
        }
    }
}
