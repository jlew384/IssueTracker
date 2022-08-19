namespace IssueTracker.ViewModels
{
    public class UserTableComponentViewModel
    {
        public string Filter { get; set; }
        public int ProjectId { get; set; }
        public List<UserViewModel> Users { get; set; }

        public UserViewModel? User { get; }

        public bool IsSelectable { get; set; }
    }
}
