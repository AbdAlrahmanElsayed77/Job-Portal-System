using BL.Contracts;
using BL.Mapping;
using BL.Seeders;
using BL.Services;
using DAL.Contracts;
using DAL.DbContext;
using DAL.Repositories;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace PortalSystemProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();
            RegisterServciesHelper.RegisteredServices(builder);

            var app = builder.Build();
            // 🔧 Auto apply migrations + seed roles/admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // 1️⃣ Apply pending migrations (safe for dev/test)
                var db = services.GetRequiredService<PortalContext>();
                db.Database.Migrate();

                // 2️⃣ Seed roles and admin user
                IdentitySeeder.SeedRolesAndAdminAsync(services).GetAwaiter().GetResult();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=AllJops}/{id?}");


            app.Run();
        }
    }
}
