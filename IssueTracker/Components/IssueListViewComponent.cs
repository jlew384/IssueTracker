using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueListViewComponent : ViewComponent
    {
        public static class Type
        {
            public const string DEFAULT = "default";
            public const string CREATED = "created";
            public const string ASSIGNED = "assigned";
            public const string ASSIGNED_ONLY = "assigned_only";
        }


        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 5;


        public IssueListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type, string? sortOrder, string? searchString, int? pageIndex, int? pid, string userId)
        {
            IQueryable<Issue> issues = InitializeIssues(type, userId, pid);           

            issues = Sort(issues, sortOrder);
            issues = Search(issues, searchString);

            PaginatedList<Issue> paginatedList = await PaginatedList<Issue>.CreateAsync(issues, pageIndex ?? 1, PAGE_SIZE);


            return View(new IssueListViewModel
                {
                    Type = type,
                    SortOrder = sortOrder,
                    SearchString = searchString,
                    PageIndex = pageIndex ?? 1,
                    ProjectId = pid,
                    UserId = userId,
                    Issues = paginatedList
                });
        }

        private IQueryable<Issue> InitializeIssues(string type, string userId, int? pid)
        {
            switch (type)
            {
                case Type.CREATED:
                    return _context.Issues.Where(x => x.CreatorUserId == userId);
                case Type.ASSIGNED:
                    return _context.Issues.Where(x => x.AssignedUserId == userId);
                case Type.ASSIGNED_ONLY:
                    return _context.Issues.Where(x => x.AssignedUserId == userId && x.CreatorUserId != userId);
                default:
                    return _context.Issues.Where(x => x.ProjectId == pid);
            }
        }

        private IQueryable<Issue> Sort(IQueryable<Issue> issues, string sortOrder)
        {
            switch (sortOrder)
            {
                case IssueSortOrder.TITLE:
                    return issues.OrderBy(x => x.Title);
                case IssueSortOrder.TITLE_DESC:
                    return issues.OrderByDescending(x => x.Title);
                case IssueSortOrder.STATUS:
                    return issues.OrderByDescending(x => x.Status);
                case IssueSortOrder.STATUS_DESC:
                    return issues.OrderBy(x => x.Status);
                case IssueSortOrder.PRIORITY:
                    return issues
                        .Where(x => x.Priority == IssuePriority.LOW)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.HIGH));
                case IssueSortOrder.PRIORITY_DESC:
                    return issues
                        .Where(x => x.Priority == IssuePriority.HIGH)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.LOW));
                case IssueSortOrder.TYPE:
                    return issues.OrderBy(x => x.Type);
                case IssueSortOrder.TYPE_DESC:
                    return issues.OrderByDescending(x => x.Type);
                case IssueSortOrder.ASSIGNED_USER_NAME:
                    return issues.OrderBy(x => x.AssignedUser.UserName);
                case IssueSortOrder.ASSIGNED_USER_NAME_DESC:
                    return issues.OrderByDescending(x => x.AssignedUser.UserName);
                case IssueSortOrder.CREATED_DATE:
                    return issues.OrderBy(x => x.Created);
                case IssueSortOrder.CREATED_DATE_DESC:
                    return issues.OrderByDescending(x => x.Created);
                default:
                    return issues.OrderByDescending(x => x.Created);
            }
        }

        private IQueryable<Issue> Search(IQueryable<Issue> issues, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                return issues.Where(x => x.Title.ToLower().Contains(searchString.ToLower()));
            }
            else
            {
                return issues;
            }
        }
    }
}
