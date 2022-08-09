namespace IssueTracker.Constants
{
    public class IssueFilter
    {
        public const string PROJECT = "PROJECT";
        public const string ASSIGNEE = "ASSIGNEE";
        public const string CREATOR = "CREATOR";
        public const string ACTIVE = "ACTIVE";
        public const string INACTIVE = "INACTIVE";

        public static List<string> LIST = new List<string> { PROJECT, ASSIGNEE, CREATOR, ACTIVE, INACTIVE };
    }
}
