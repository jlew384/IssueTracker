﻿namespace IssueTracker.ViewModels
{
    public class IssueIndexViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string Filter { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
    }
}
