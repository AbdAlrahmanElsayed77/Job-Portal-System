using System;
using System.Collections.Generic;

namespace PortalSystemProject.Models.Admin
{
    public class JobPostListViewModel
    {
        public List<JobPostItemViewModel> JobPosts { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public string? StatusFilter { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class JobPostItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string JobTypeName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "Active" : "Inactive";
        public string StatusClass => IsActive ? "success" : "secondary";
        public int ApplicationsCount { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.Now;
    }

    public class JobPostDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public string? CompanyWebsite { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string JobTypeName { get; set; } = string.Empty;

        public string? Location { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryRange => MinSalary.HasValue && MaxSalary.HasValue
            ? $"${MinSalary:N0} - ${MaxSalary:N0}"
            : "Not specified";

        public bool IsActive { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.Now;

        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }

        public string PostedBy { get; set; } = string.Empty;
    }
}