using BL.Contracts;
using BL.Mapping;
using BL.Services;
using DAL.Contracts;
using DAL.DbContext;
using DAL.Repositories;
using Domains;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

namespace PortalSystemProject
{
    public class RegisterServciesHelper
    {
        public static void RegisteredServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<PortalContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 🟢 تسجيل الـ Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<PortalContext>()
                .AddDefaultTokenProviders();

            //// Configure Serilog for logging
            //if (Process.GetCurrentProcess().ProcessName != "dotnet")
            //{
            //    Log.Logger = new LoggerConfiguration()
            //        .WriteTo.Console()
            //        .WriteTo.MSSqlServer(
            //            connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
            //            tableName: "Log",
            //            autoCreateSqlTable: true)
            //        .CreateLogger();

            //    builder.Host.UseSerilog();
            //}

            //builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));



            // register services 
            builder.Services.AddScoped(typeof(ITableRepository<>), typeof(TableRepository<>));
            //builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationService>();
            builder.Services.AddScoped<ICompanyRepository, CompanyService>();
            builder.Services.AddScoped<IEmployerProfileService, EmployerProfileService>();
            builder.Services.AddScoped<ICVFileRepository, CVFileService>();
            builder.Services.AddScoped<IJobCategoryRepository, JobCategoryService>();
            builder.Services.AddScoped<IJobPostRepository, JobPostService>();
            builder.Services.AddScoped<IJobTypeRepository, JobTypeService>();
            builder.Services.AddScoped<ISavedJobRepository, SavedJobService>();
            builder.Services.AddScoped<IJobSeekerProfileRepository, JobSeekerProfileService>();
            builder.Services.AddScoped<IJobSeekerProfileService, JobSeekerProfileServices>();

            //builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //external services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();

            //builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserService>();
        }
    }
}