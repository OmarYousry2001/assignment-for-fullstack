using DAL.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.CustomValidation
{
    /// <summary>
    /// Custom validation attribute to ensure ProductCode is unique.
    /// </summary>
    public class UniqueProductCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var productCode = value as string;

            if (string.IsNullOrWhiteSpace(productCode))
                return ValidationResult.Success; 

            // Get DbContext from DI
            var dbContext = validationContext.GetService<ApplicationDbContext>();
            if (dbContext == null)
                throw new InvalidOperationException(SystemResources.DatabaseNotAvailable);

            var exists = dbContext.Products
                                  .AsNoTracking()
                                  .Any(p => p.ProductCode == productCode);

            if (exists)
            {
                var fieldName = validationContext.DisplayName ?? "Product code";
                return new ValidationResult(SystemResources.ProductCodeExists);
            }

            return ValidationResult.Success;
        }
    }
}
