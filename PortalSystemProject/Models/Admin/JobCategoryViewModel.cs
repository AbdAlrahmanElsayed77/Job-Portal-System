using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalSystemProject.Models.Admin
{
    public class JobCategoryListViewModel
    {
        public List<JobCategoryItemViewModel> Categories { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class JobCategoryItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int JobCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateJobCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(120, ErrorMessage = "Category name cannot exceed 120 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
    }

    public class EditJobCategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(120, ErrorMessage = "Category name cannot exceed 120 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        public int JobCount { get; set; }
    }
}