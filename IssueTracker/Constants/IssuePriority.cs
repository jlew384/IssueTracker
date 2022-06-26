using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Constants
{
    public static class IssuePriority
    {
        public const string LOW = "Low";
        public const string MEDIUM = "Medium";
        public const string HIGH = "High";
        public static List<string> List => new List<string> { LOW, MEDIUM, HIGH };
    }
}
