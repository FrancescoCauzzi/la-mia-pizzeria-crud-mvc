namespace la_mia_pizzeria_crud_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.MapControllerRoute(name: "PizzasIndex",
                pattern: "Our-Pizzas",
                defaults: new { controller = "Pizza", action = "Index" });

            app.MapControllerRoute(
                name: "PizzaDetails",
                pattern: "Our-Pizzas/{*name}",
                defaults: new { controller = "Pizza", action = "Details" });
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