using IssueTracker.Helpers;

namespace IssueTracker.ViewModels
{
    public class UserListViewModel
    {
        public string Type { get; set; }
        public string SortOrder { get; set; }
        public string SearchString { get; set; }
        public string RoleFilter { get; set; }
        public int PageIndex { get; set; }
        public string UserId { get; set; }
        public int? ProjectId { get; set; }
        public PaginatedList<UserViewModel> Users { get; set; }
        public UserViewModel User { get; }
    }
}
