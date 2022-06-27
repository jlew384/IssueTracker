using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.ViewModels
{
    public class EditUserRoleViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsProjectManager { get; set; }

        public bool IsDeveloper { get; set; }

        public bool IsSubmitter { get; set; }
    }
}
