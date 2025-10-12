using BL.Contracts;
using BL.Mapping;
using BL.Services;
using DAL.Contracts;
using DAL.DbContext;
using DAL.Repositories;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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


            // Configure Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
                    tableName: "Log",
                    autoCreateSqlTable: true)
                .CreateLogger();
            builder.Host.UseSerilog();



            //builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });




            // register services 
            builder.Services.AddScoped(typeof(ITableRepository<>), typeof(TableRepository<>));
            //builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationService>();
            builder.Services.AddScoped<ICompanyRepository, CompanyService>();
            builder.Services.AddScoped<IEmployerProfileRepository, EmployerProfileService>();
            builder.Services.AddScoped<ICVFileRepository, CVFileService>();
            builder.Services.AddScoped<IJobCategoryRepository, JobCategoryService>();
            builder.Services.AddScoped<IJobPostRepository, JobPostService>();
            builder.Services.AddScoped<IJobSeekerProfileRepository, JobSeekerProfileService>();
            builder.Services.AddScoped<IJobTypeRepository, JobTypeService>();
            builder.Services.AddScoped<ISavedJobRepository, SavedJobService>();
            //builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserService>();
        }
    }
}
