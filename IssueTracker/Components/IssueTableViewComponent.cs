using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueTableViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        private string? _filter;
        private string? _userId;
        private int? _projectId;
        private string? _sortOrder;
        private string? _sortDirection;

        public IssueTableViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string filter, int projectId, string sortField, string sortDirection)
        {
            _filter = filter;
            _projectId = projectId;
            _sortOrder = sortField + "_" + sortDirection;
            _sortDirection = sortDirection;


            IQueryable<Issue> issues = InitializeIssues();

            string projectTitle = _context.Projects.Find(projectId).Title;

            issues = Sort(issues, _sortOrder);



            IssueTableComponentViewModel model = new IssueTableComponentViewModel()
            {
                Issues = issues.ToList(),
                ProjectTitle = projectTitle,
                ProjectId = projectId,
                SortDirection = sortDirection,
                SortField = sortField,
                Filter = filter
            };
            return View(model);
        }

        private IQueryable<Issue> InitializeIssues()
        {
            switch (_filter)
            {
                case IssueFilter.PROJECT:
                    return _context.Issues.Where(x => x.ProjectId == _projectId);
                case IssueFilter.ASSIGNEE:
                    return _context.Issues.Where(x => x.AssignedUserId == _userId);
                case IssueFilter.CREATOR:
                    return _context.Issues.Where(x => x.CreatorUserId == _userId);
                case IssueFilter.ACTIVE:
                    return _context.Issues.Where(x => x.ProjectId == _projectId);
                case IssueFilter.INACTIVE:
                    return _context.Issues.Where(x => x.ProjectId == _projectId);
                default:
                    return _context.Issues.Where(x => x.ProjectId == _projectId);
            }
        }




        private IQueryable<Issue> FilterStatus(IQueryable<Issue> issues, string statusFilter)
        {
            if (!String.IsNullOrEmpty(statusFilter))
            {

                return issues.Where(x => x.Status == statusFilter);
            }
            else
            {
                return issues;
            }

        }
        private IQueryable<Issue> FilterPriority(IQueryable<Issue> issues, string priorityFilter)
        {
            if (!String.IsNullOrEmpty(priorityFilter))
            {

                return issues.Where(x => x.Priority == priorityFilter);
            }
            else
            {
                return issues;
            }
        }


        private IQueryable<Issue> FilterType(IQueryable<Issue> issues, string typeFilter)
        {
            if (!String.IsNullOrEmpty(typeFilter))
            {

                return issues.Where(x => x.Type == typeFilter);
            }
            else
            {
                return issues;
            }

        }


        

        private IQueryable<Issue> Sort(IQueryable<Issue> issues, string sortOrder)
        {
            switch (sortOrder)
            {
                case IssueSortOrder.STATUS_ASC:
                    return issues.OrderByDescending(x => x.Status);
                case IssueSortOrder.STATUS_DESC:
                    return issues.OrderBy(x => x.Status);
                case IssueSortOrder.PRIORITY_ASC:
                    return issues
                        .Where(x => x.Priority == IssuePriority.LOW)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.HIGH));
                case IssueSortOrder.PRIORITY_DESC:
                    return issues
                        .Where(x => x.Priority == IssuePriority.HIGH)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.LOW));
                case IssueSortOrder.CREATED_DATE_ASC:
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
