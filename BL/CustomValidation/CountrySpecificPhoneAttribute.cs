using Resources;
using Resources.Data.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace Shared.Attributes
{
    public class CountrySpecificPhoneAttribute : ValidationAttribute
    {
        public string CountryField { get; set; } = string.Empty;
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var country = context.ObjectInstance.GetType()
                            .GetProperty(CountryField)?
                            .GetValue(context.ObjectInstance) as string;
            var phone = value as string;
            if (string.IsNullOrWhiteSpace(phone))
            {
                return new ValidationResult(Resources.ValidationResources.RequiredField);
            }
            return country?.ToUpperInvariant() switch
            {
                "EG" => ValidateEgyptianPhone(phone),
                "SA" => ValidateSaudiPhone(phone),
                "US" => ValidateUsPhone(phone),
                _ => ValidationResult.Success
            };
        }
        private ValidationResult ValidateEgyptianPhone(string phone)
        {
            if (phone.Length != 11 || !phone.StartsWith("01") || !phone.All(char.IsDigit))
            {
                return new ValidationResult(ValidationResources.InvalidPhoneNumber);
            }
            return ValidationResult.Success;
        }
        private ValidationResult ValidateSaudiPhone(string phone)
        {
            if (phone.Length != 9 || !phone.StartsWith("5") || !phone.All(char.IsDigit))
            {
                return new ValidationResult(ValidationResources.InvalidPhoneNumber);
            }
            return ValidationResult.Success;
        }
        private ValidationResult ValidateUsPhone(string phone)
        {
            var pattern = @"^\(\d{3}\)\s\d{3}-\d{4}$"; // e.g., (123) 456-7890
            if (!Regex.IsMatch(phone, pattern))
            {
                return new ValidationResult(ValidationResources.InvalidPhoneNumber);
            }
            return ValidationResult.Success;
        }
    }
}