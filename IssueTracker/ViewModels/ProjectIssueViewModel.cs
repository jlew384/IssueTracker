using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class ProjectIssueViewModel
    {
        public Project Project { get; set; } = null!;
        public IEnumerable<Issue> IssuesNotDone { get; set; } = new List<Issue>();
        public IEnumerable<Issue> IssuesDone { get; set; } = new List<Issue>();
    }
}
