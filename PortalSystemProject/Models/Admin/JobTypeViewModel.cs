using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalSystemProject.Models.Admin
{
    public class JobTypeListViewModel
    {
        public List<JobTypeItemViewModel> JobTypes { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class JobTypeItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int JobCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateJobTypeViewModel
    {
        [Required(ErrorMessage = "Job type name is required")]
        [StringLength(60, ErrorMessage = "Job type name cannot exceed 60 characters")]
        [Display(Name = "Job Type Name")]
        public string Name { get; set; } = string.Empty;
    }

    public class EditJobTypeViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Job type name is required")]
        [StringLength(60, ErrorMessage = "Job type name cannot exceed 60 characters")]
        [Display(Name = "Job Type Name")]
        public string Name { get; set; } = string.Empty;

        public int JobCount { get; set; }
    }
}