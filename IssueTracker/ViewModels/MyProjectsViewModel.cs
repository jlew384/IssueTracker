using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class MyProjectsViewModel
    {
        public List<Project> ManagedProjects { get; set; } = new List<Project>();
        public List<Project> AssignedProjects { get; set; } = new List<Project>();

        public string UserId { get; set; }
    }
}
