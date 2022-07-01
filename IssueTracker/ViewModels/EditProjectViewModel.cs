using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class EditProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public List<ApplicationUser> AssignableUsers { get; set; } = new List<ApplicationUser>();
    }
}
