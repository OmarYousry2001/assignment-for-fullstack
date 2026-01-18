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

            // Get the current model instance being validated
            var instance = validationContext.ObjectInstance;
            // Use reflection to find a property named "Id" on the current object
            var productIdProperty = instance.GetType().GetProperty("Id");
            Guid productId = Guid.Empty;
            if (productIdProperty != null)
            {
                // If the value is a Guid, assign it to productId
                var idValue = productIdProperty.GetValue(instance);
                if (idValue is Guid guid)
                    productId = guid;
            }


            // Check if there is another product with the same ProductCode
            // excluding the current one (by Id) and with CurrentState = 1 (active)
            var exists = dbContext.Products
                                  .AsNoTracking()
                                  .Any(p => p.ProductCode == productCode && p.Id != productId && p.CurrentState==1);

            if (exists)
            {
                var fieldName = validationContext.DisplayName ?? "Product code";
                return new ValidationResult(SystemResources.ProductCodeExists);
            }

            return ValidationResult.Success;
        }
    }
}
