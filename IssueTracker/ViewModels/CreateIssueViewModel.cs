using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class CreateIssueViewModel
    {
        public Project Project { get; set; }
        public Issue Issue { get; set; }
        public IEnumerable<ApplicationUser> AssignableUsers { get; set; }
    }
}
