using IssueTracker.Helpers;
using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class ProjectListViewModel
    {
        public string Type { get; set; }
        public string SortOrder { get; set; }
        public string SearchString { get; set; }
        public int PageIndex { get; set; }
        public string UserId { get; set; }
        public PaginatedList<Project> Projects { get; set; }
        public Project Project { get; }
    }
}
