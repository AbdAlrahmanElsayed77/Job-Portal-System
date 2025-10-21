namespace BL.Dtos.Dashboard
{
    public class DashboardSummaryDto
    {
        public int UserCount { get; set; }
        public int CompanyCount { get; set; }
        public int JobCount { get; set; }
        public int CategoryCount { get; set; }

        public List<string> CategoryLabels { get; set; } = new();
        public List<int> CategoryJobCounts { get; set; } = new();

        public List<string> CompanyLabels { get; set; } = new();
        public List<int> CompanyJobCounts { get; set; } = new();
    }
}
