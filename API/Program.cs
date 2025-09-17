
using API.Middleware;
using API.MiddleWare;
using DAL.ApplicationContext;
using Domains.Entities.Identity;
using Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            RegisterServicesHelper.RegisteredServices(builder);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers();

            var app = builder.Build();

            #region Loaclation
            // Use resources for multi-language support
            var supportedCultures = new List<CultureInfo>
                  {
       new CultureInfo("en-US"),
      new CultureInfo("ar-EG")
};
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ar-EG"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    }
            });
            #endregion


            app.UseStaticFiles();
            // Enable GZip compression
            app.UseResponseCompression();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            #region Configure the HTTP request pipeline.
            app.UseSwagger();
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "Endpoint"; // Set this to match your API path if needed
                });

                app.MapGet("/", async context =>
                {
                    context.Response.Redirect("/Endpoint/");
                    await context.Response.CompleteAsync(); // Ensure the async Task is returned
                });
            }
            #endregion

            // Configure rate limiting middleware
            app.UseMiddleware<RateLimitingMiddleware>();

            app.MapControllers();

            #region Seeding Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                // Apply migrations
                await dbContext.Database.MigrateAsync();

                // Seed data
                await ContextConfigurations.SeedDataAsync(dbContext, userManager, roleManager);
            }
            #endregion

            app.Run();
        }
    }
}
