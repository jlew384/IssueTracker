using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class CreateProjectViewModel
    {
        public Project Project { get; set; }
        public string ProjectManagerId { get; set; }
        public List<ApplicationUser> AssignableUsers { get; set; } = new List<ApplicationUser>();
        public IEnumerable<ApplicationUser> AssignableProjectManagers { get; set; }

    }
}
