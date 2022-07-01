using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class MyIssuesViewModel
    {
        public IEnumerable<Issue> IssuesCreated { get; set; } = new List<Issue>();
        public IEnumerable<Issue> IssuesAssigned { get; set; } = new List<Issue>();
    }
}
