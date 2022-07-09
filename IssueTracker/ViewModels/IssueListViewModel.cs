using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueListViewModel
    {
        public int? ProjectId { get; set; }
        public string? CreatorId { get; set; }

        public string? AssignedUserId { get; set; }
        public string Title { get; set; }        
        public Issue Issue { get;}
        public string SortOrder { get; set; }
        public string SearchString { get; set; }
    }
}
