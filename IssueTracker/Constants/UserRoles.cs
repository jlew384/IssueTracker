namespace IssueTracker.Constants
{
    public static class UserRoles
    {
        public const string ADMIN = "Admin";
        public const string PROJ_MNGR = "Project Manager";
        public const string DEV = "Developer";
        public const string SUB = "Submitter";

        public static readonly string[] ROLES = new string[] { ADMIN, PROJ_MNGR, DEV, SUB };
    }
}
