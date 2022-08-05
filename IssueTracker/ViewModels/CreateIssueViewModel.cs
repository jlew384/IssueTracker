using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class CreateIssueViewModel
    {
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
    }
}
