using System;
using System.Collections.Generic;

namespace PortalSystemProject.Models.Home
{
    public class HomeViewModel
    {
        public List<JobCardItem> RecentJobs { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public int TotalJobs { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalCategories { get; set; }
    }

    public class JobCardItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? JobType { get; set; }
        public string? SalaryRange { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class CategoryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int JobCount { get; set; }
    }
}