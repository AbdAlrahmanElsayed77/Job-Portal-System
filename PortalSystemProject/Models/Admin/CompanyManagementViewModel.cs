using System;
using System.Collections.Generic;

namespace PortalSystemProject.Models.Admin
{
    public class CompanyListViewModel
    {
        public List<CompanyItemViewModel> Companies { get; set; } = new();
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public string? StatusFilter { get; set; }
    }

    public class CompanyItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? LogoUrl { get; set; }
        public byte Status { get; set; } // 0=Pending, 1=Approved
        public string StatusText => Status == 1 ? "Approved" : "Pending";
        public string StatusClass => Status == 1 ? "success" : "warning";
        public int JobPostsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CompanyDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalJobPosts { get; set; }
        public int ActiveJobPosts { get; set; }
        public List<string> EmployerEmails { get; set; } = new();
    }
}