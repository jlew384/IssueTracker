using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class DeleteIssueModelView
    {
        public Issue Issue { get; set; }
        public string RefererUrl { get; set; }
    }
}
