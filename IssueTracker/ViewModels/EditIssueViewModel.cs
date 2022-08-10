using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class EditIssueViewModel
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string? AssignedUserId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public string? ProjectTitle { get; set; }

        public int? ProjectId { get; set; }

        public IEnumerable<ApplicationUser>? AssignableUsers { get; set; }
    }
}
