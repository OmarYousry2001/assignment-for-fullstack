using Domains.AppMetaData;
using Domains.Entities;
using Domains.Entities.Identity;

using Domains.Identity;
using EcommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DAL.ApplicationContext
{
    public class ContextConfigurations
    {
        private static readonly string seedAdminEmail = "admin123@gamil.com";
        private static readonly string seedAdminPassword = "Admin-123";

        public static async Task SeedDataAsync(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<Role> roleManager)
        {
            // Seed user first to get admin user ID
            var adminUserId = await SeedUserAsync(userManager, roleManager);

            // Seed E-commerce data
            await SeedProductDataAsync(context, adminUserId);
        }

        private static async Task SeedProductDataAsync(ApplicationDbContext context, Guid adminUserId)
        {
            // 1. Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
        {
            new Product
            {
                Id              = Guid.NewGuid(),
                Category        = "Electronics",
                ProductCode     = "P01",
                Name            = "Smartphone",
                ImagePath       = "images/products/smartphone.webp",
                Price           = 15000,
                MinimumQuantity = 1,
                DiscountRate    = 0.10,
                CurrentState    = 1,
                CreatedBy       = adminUserId,
                CreatedDateUtc  = DateTime.UtcNow
            },
            new Product
            {
                Id              = Guid.NewGuid(),
                Category        = "Electronics",
                ProductCode     = "P02",
                Name            = "Laptop",
                ImagePath       = "images/products/laptop.webp",
                Price           = 25000,
                MinimumQuantity = 1,
                DiscountRate    = 0.15,
                CurrentState    = 1,
                CreatedBy       = adminUserId,
                CreatedDateUtc  = DateTime.UtcNow
            },
            new Product
            {
                Id              = Guid.NewGuid(),
                Category        = "Fashion",
                ProductCode     = "P03",
                Name            = "T-Shirt",
                ImagePath       = "images/products/tshirt.webp",
                Price           = 300,
                MinimumQuantity = 5,
                DiscountRate    = 0.05,
                CurrentState    = 1,
                CreatedBy       = adminUserId,
                CreatedDateUtc  = DateTime.UtcNow
            }
        };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }


        private static async Task<Guid> SeedUserAsync(UserManager<ApplicationUser> userManager,
                   RoleManager<Role> roleManager)
        {
            // Ensure roles exist
            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.User });
            }
            // Ensure roles exist
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.Admin });
            }

            // Ensure admin user exists
            var adminEmail = seedAdminEmail;
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var id = Guid.NewGuid().ToString();
                adminUser = new ApplicationUser
                {
                    Id = id,
                    UserName = "Admin000",
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(adminUser, seedAdminPassword);
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }

            // Convert adminUser.Id from string to Guid
            return Guid.Parse(adminUser.Id);  // Convert to Guid
        }
    }
}
