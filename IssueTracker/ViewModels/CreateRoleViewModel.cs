using System.ComponentModel.DataAnnotations;

namespace IssueTracker.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
