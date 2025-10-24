using System;
using System.Collections.Generic;

namespace PortalSystemProject.Models.Admin
{
    public class DashboardViewModel
    {
        // Statistics
        public int TotalUsers { get; set; }
        public int TotalJobSeekers { get; set; }
        public int TotalEmployers { get; set; }
        public int TotalAdmins { get; set; }

        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int InactiveJobs { get; set; }

        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }

        public int TotalCompanies { get; set; }
        public int PendingCompanies { get; set; }
        public int ApprovedCompanies { get; set; }

        public int TotalCategories { get; set; }
        public int TotalJobTypes { get; set; }

        // Recent Activities
        public List<RecentActivityItem> RecentActivities { get; set; } = new();

        // Charts Data
        public Dictionary<string, int> JobsByCategory { get; set; } = new();
        public Dictionary<string, int> ApplicationsByMonth { get; set; } = new();
    }

    public class RecentActivityItem
    {
        public string Action { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = "bi-circle-fill";
        public string Color { get; set; } = "primary";
    }
}