﻿using IssueTracker.Helpers;
using IssueTracker.Models;

namespace IssueTracker.ViewModels
{
    public class IssueListViewModel
    {
        public string Type { get; set; }
        public string SortOrder { get; set; }
        public string SearchString { get; set; }
        public int PageIndex { get; set; }
        public string StatusFilter { get; set; }
        public string PriorityFilter { get; set; }
        public string TypeFilter { get; set; }
        public int? ProjectId { get; set; }
        public string UserId { get; set; }
        public PaginatedList<Issue> Issues { get; set; }
        public Issue Issue { get;}
    }
}