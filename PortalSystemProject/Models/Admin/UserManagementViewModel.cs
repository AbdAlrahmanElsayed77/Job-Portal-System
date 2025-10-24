using System;
using System.Collections.Generic;

namespace PortalSystemProject.Models.Admin
{
    public class UserManagementViewModel
    {
        public List<UserItemViewModel> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int JobSeekersCount { get; set; }
        public int EmployersCount { get; set; }
        public int AdminsCount { get; set; }

        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
    }

    public class UserItemViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();

        public string? CompanyName { get; set; }
        public int? ApplicationsCount { get; set; }
        public int? JobPostsCount { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
