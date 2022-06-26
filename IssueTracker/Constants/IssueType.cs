using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Constants
{
    public static class IssueType
    {
        public const string BUG = "Bug";
        public const string FEATURE = "Feature";
        public const string OTHER = "Other";
        public static List<string> List => new List<string> { BUG, FEATURE, OTHER };
    }
}
