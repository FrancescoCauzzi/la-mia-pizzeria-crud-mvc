using la_mia_pizzeria_crud_mvc.CustomLoggers;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace la_mia_pizzeria_crud_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Dependency Injection
            builder.Services.AddScoped<ICustomLogger, CustomFileLogger>();

            var app = builder.Build();

            var defaultDateCulture = "en-US";
            var ci = new CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo>
            {
                ci,
            },
                SupportedUICultures = new List<CultureInfo>
            {
                ci,
            }
            });
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "PizzasIndex",
                pattern: "Our-Pizzas",
                defaults: new { controller = "Pizza", action = "Index" });

            app.MapControllerRoute(
                name: "PizzaDetails",
                pattern: "Our-Pizzas/{*name}",
                defaults: new { controller = "Pizza", action = "Details" });

            
            app.MapControllerRoute(
                name: "PizzaEdit",
                pattern: "Edit/{*name}",
                defaults: new { controller = "Pizza", action = "Update" });
            

            /*
            app.MapControllerRoute(
                name: "PizzaCreate",
                pattern: "Our-Pizzas/Create",
                defaults: new { controller = "Pizza", action = "Create" });
            */

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");          
            

            app.Run();
        }
    }
}