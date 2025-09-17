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

            // Get the current entity's ID to exclude it from the uniqueness check
            var instance = validationContext.ObjectInstance;
            var productIdProperty = instance.GetType().GetProperty("Id");
            Guid productId = Guid.Empty;
            if (productIdProperty != null)
            {
                var idValue = productIdProperty.GetValue(instance);
                if (idValue is Guid guid)
                    productId = guid;
            }

            // Check if any other product has the same ProductCode
            var exists = dbContext.Products
                                  .AsNoTracking()
                                  .Any(p => p.ProductCode == productCode && p.Id != productId);

            if (exists)
            {
                var fieldName = validationContext.DisplayName ?? "Product code";
                return new ValidationResult(SystemResources.ProductCodeExists);
            }

            return ValidationResult.Success;
        }
    }
}
