using Resources;

using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    public class ValidYearAttribute : ValidationAttribute
    {
        public int MinYear { get; set; } = 2100; 

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                if (year < MinYear)
                {
                    return new ValidationResult(
                        string.Format(ValidationResources.ValueMustBeBetween, MinYear, DateTime.Now.Year));
                }

                if (year > DateTime.Now.Year)
                {
                    return new ValidationResult(
                        string.Format(ValidationResources.ValueMustBeBetween, MinYear, DateTime.Now.Year));
                }

                return ValidationResult.Success;
            }

            return new ValidationResult(ValidationResources.InvalidData);
        }
    }
}
