using IssueTracker.Helpers;
using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueIndexViewModel
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string Filter { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public string SearchString { get; set; }
        public int PageIndex { get; set; }
    }
}
