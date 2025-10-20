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
        public static async Task Main(string[] args)
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
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Production error handling
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // ✅ Handle HTTP status codes (404, 403, etc.)
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 403)
                {
                    context.Request.Path = "/Error/AccessDenied";
                    await next();
                }
            });
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
