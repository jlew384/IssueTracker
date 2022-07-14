using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class CreateProjectViewModel
    {
        public Project Project { get; set; }
        public string ProjectManagerId { get; set; }
        public IEnumerable<ApplicationUser> AssignableProjectManagers { get; set; }

    }
}
