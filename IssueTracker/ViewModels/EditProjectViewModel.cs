using IssueTracker.Models;
using System.ComponentModel;

namespace IssueTracker.ViewModels
{
    public class EditProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        [DisplayName("Description")]
        public string Desc { get; set; } = null!;

        public string RefererUrl { get; set; }
    }
}
