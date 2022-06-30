using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class ProjectIssueViewModel
    {
        public Project Project { get; set; } = null!;
        public IEnumerable<Issue> Issues { get; set; } = new List<Issue>();
    }
}
