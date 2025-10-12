using Domains;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DbContext
{
    public class PortalContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
    {

        public PortalContext() { }

        public PortalContext(DbContextOptions<PortalContext> options) : base(options) { }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public virtual DbSet<JobSeekerProfile> JobSeekerProfiles { get; set; }
        public virtual DbSet<CVFile> CVFiles { get; set; }
        public virtual DbSet<JobCategory> JobCategories { get; set; }
        public virtual DbSet<JobType> JobTypes { get; set; }
        public virtual DbSet<JobPost> JobPosts { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<SavedJob> SavedJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)

        {
            base.OnModelCreating(builder);
            // ------------ ApplicationUser ------------
            builder.Entity<ApplicationUser>(e =>
            {
                e.HasIndex(p => p.UserName);
            });

            // ---------------- Company ----------------
            builder.Entity<Company>(e =>
            {
                e.HasIndex(x => x.Name);
            });

            // ------------- EmployerProfile -----------
            builder.Entity<EmployerProfile>(e =>
            {
                e.HasIndex(x => new { x.CompanyId });

                e.HasOne(x => x.User)
                 .WithOne(u => u.EmployerProfile)
                 .HasForeignKey<EmployerProfile>(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Company)
                 .WithMany(c => c.EmployerProfiles)
                 .HasForeignKey(x => x.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------- JobSeekerProfile ----------
            builder.Entity<JobSeekerProfile>(e =>
            {

                e.HasOne(x => x.User)
                 .WithOne(u => u.JobSeekerProfile)
                 .HasForeignKey<JobSeekerProfile>(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- CVFile -----------------
            builder.Entity<CVFile>(e =>
            {
                e.HasIndex(x => x.JobSeekerId);

                e.HasOne(x => x.JobSeeker)
                 .WithMany(js => js.CVFiles)
                 .HasForeignKey(x => x.JobSeekerId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Applications)
                 .WithOne(a => a.CVFile!)
                 .HasForeignKey(a => a.CVFileId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // -------------- JobCategory --------------
            builder.Entity<JobCategory>(e =>
            {
                e.HasIndex(x => x.Name).IsUnique();
            });

            // ---------------- JobType ----------------
            builder.Entity<JobType>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
            });

            // ---------------- JobPost ----------------
            builder.Entity<JobPost>(e =>
            {
                e.HasIndex(x => x.Title);
                e.HasIndex(x => new { x.JobCategoryId, x.JobTypeId, x.City });

                e.Property(x => x.MinSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.MaxSalary).HasColumnType("decimal(18,2)");

                e.HasOne(x => x.Company)
                 .WithMany(c => c.JobPosts)
                 .HasForeignKey(x => x.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CreatedByUser)
                 .WithMany(u => u.CreatedJobPosts)
                 .HasForeignKey(x => x.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.JobCategory)
                 .WithMany(c => c.JobPosts)
                 .HasForeignKey(x => x.JobCategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.JobType)
                 .WithMany(t => t.JobPosts)
                 .HasForeignKey(x => x.JobTypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------- Application -------------
            builder.Entity<Application>(e =>
            {
                e.HasIndex(x => x.JobPostId);
                e.HasIndex(x => x.JobSeekerId);
                e.HasIndex(x => x.ApplicantUserId);

                // Unique: one application per job per seeker
                e.HasIndex(x => new { x.JobPostId, x.JobSeekerId }).IsUnique();

                e.HasOne(x => x.JobPost)
                 .WithMany(j => j.Applications)
                 .HasForeignKey(x => x.JobPostId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.JobSeeker)
                 .WithMany(js => js.Applications)
                 .HasForeignKey(x => x.JobSeekerId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.ApplicantUser)
                 .WithMany(u => u.Applications)
                 .HasForeignKey(x => x.ApplicantUserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CVFile)
                 .WithMany(cv => cv.Applications)
                 .HasForeignKey(x => x.CVFileId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------------- SavedJob ----------------
            builder.Entity<SavedJob>(e =>
            {
                e.HasKey(x => new { x.JobSeekerId, x.JobPostId });

                e.HasOne(x => x.JobSeeker)
                 .WithMany(js => js.SavedJobs)
                 .HasForeignKey(x => x.JobSeekerId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.JobPost)
                 .WithMany(j => j.SavedBy)
                 .HasForeignKey(x => x.JobPostId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
