using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class CreateProjectViewModel
    {
        public Project Project { get; set; }
        public string ProjectManagerId { get; set; }
        public List<AssignedUser> AssignedUsers { get; set; } = new List<AssignedUser>();
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public IEnumerable<ApplicationUser> ProjectManagers { get; set; }

        public class AssignedUser
        {
            public ApplicationUser User { get; set; }
            public bool IsSelected { get; set; }   
        }
    }
}
