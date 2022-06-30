using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class EditIssueViewModel
    {
        public Issue Issue { get; set; }
        public IEnumerable<ApplicationUser> AssignableUsers { get; set; }
    }
}
