using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueDetailsModelView
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }

        public Issue Issue { get; set; }
    }
}
