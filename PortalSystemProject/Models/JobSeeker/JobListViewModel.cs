using BL.Dtos;

namespace PortalSystemProject.Models.JobSeeker
{

    public class JobListViewModel
    {
        public List<JobCardViewModel> Jobs { get; set; } = new();

        public List<CategoryFilterItem> Categories { get; set; } = new();
        public List<JobTypeFilterItem> JobTypes { get; set; } = new();
        public List<string> Countries { get; set; } = new();
        public List<string> Cities { get; set; } = new();

        public JobFilterModel Filters { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalJobs { get; set; }
        public int PageSize { get; set; } = 10;

        public List<JobCategoryDto> AllCategories { get; set; } = new();
        public List<JobTypeDto> AllJobTypes { get; set; } = new();
        public Dictionary<Guid, CompanyDto> CompanyDictionary { get; set; } = new();

    }


    public class JobCardViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string JobCategory { get; set; } = string.Empty;
        public string? JobType { get; set; }
        public string? SalaryRange { get; set; }
        public string? ExperienceRange { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool IsSaved { get; set; } 
        public bool HasApplied { get; set; } 
    }

    public class JobFilterModel
    {
        public string? SearchKeyword { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? JobTypeId { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public byte? MinExperience { get; set; }
        public byte? MaxExperience { get; set; }
        public string SortBy { get; set; } = "recent"; 
    }

    public class CategoryFilterItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int JobCount { get; set; }
    }

    public class JobTypeFilterItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int JobCount { get; set; }
    }
}