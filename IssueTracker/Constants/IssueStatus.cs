using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IssueTracker.Constants
{
    public static class IssueStatus
    {
        public const string TO_DO = "To Do";
        public const string IN_PROGRESS = "In Progress";
        public const string DONE = "Done";
        public static List<string> List => new List<string> { TO_DO, IN_PROGRESS, DONE };
    }
}
