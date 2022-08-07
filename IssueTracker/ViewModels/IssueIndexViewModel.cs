using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueIndexViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public List<Issue> Issues { get; set; }
    }
}
