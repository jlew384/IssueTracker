using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueTableComponentViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public List<Issue> Issues { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public string Filter { get; set; }
    }
}
