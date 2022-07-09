using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private static string lastSortOrder = "";

        public IssueListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? pid, string? creatorId, string? assignedId, string? sortOrder, string? searchString)
        {
            IEnumerable<Issue> model;
            if(pid != null)
            {
                model = _context.Issues.Where(x => x.ProjectId == pid);
            }
            else
            {
                model = _context.Issues;
            }

            if(creatorId != null)
            {
                model = _context.Issues.Where(x => x.CreatorUserId == creatorId);
            }

            if(assignedId != null)
            {
                model = _context.Issues.Where(x => x.AssignedUserId == assignedId && x.CreatorUserId != assignedId);
            }

            switch (sortOrder)
            {
                case IssueSortOrder.TITLE:
                    if (lastSortOrder == IssueSortOrder.TITLE)
                    {
                        lastSortOrder = IssueSortOrder.TITLE_DESC;
                        model = model.OrderByDescending(x => x.Title);
                    }
                    else
                    {
                        lastSortOrder = IssueSortOrder.TITLE;
                        model = model.OrderBy(x => x.Title);
                    }
                    break;
                case IssueSortOrder.STATUS:
                    if (lastSortOrder == IssueSortOrder.STATUS)
                    {
                        lastSortOrder = IssueSortOrder.STATUS_DESC;
                        model = model.OrderBy(x => x.Status);
                    }
                    else
                    {
                        lastSortOrder = IssueSortOrder.STATUS;
                        model = model.OrderByDescending(x => x.Status);
                    }
                    break;
                case IssueSortOrder.PRIORITY:
                    var highs = model.Where(x => x.Priority == IssuePriority.HIGH);
                    var mediums = model.Where(x => x.Priority == IssuePriority.MEDIUM);
                    var lows = model.Where(x => x.Priority == IssuePriority.LOW);

                    if (lastSortOrder == IssueSortOrder.PRIORITY)
                    {
                        lastSortOrder = IssueSortOrder.PRIORITY_DESC;
                        model = highs.Concat(mediums).Concat(lows);
                    }
                    else
                    {
                        lastSortOrder = IssueSortOrder.PRIORITY;
                        model = lows.Concat(mediums).Concat(highs);
                    }
                    break;
                case IssueSortOrder.TYPE:
                    if (lastSortOrder == IssueSortOrder.TYPE)
                    {
                        lastSortOrder = IssueSortOrder.TYPE_DESC;
                        model = model.OrderByDescending(x => x.Type);
                    }
                    else
                    {
                        lastSortOrder = IssueSortOrder.TYPE;
                        model = model.OrderBy(x => x.Type);
                    }
                    break;
                case IssueSortOrder.ASSIGNED_USER_NAME:
                    if (lastSortOrder == IssueSortOrder.STATUS)
                    {
                        lastSortOrder = IssueSortOrder.ASSIGNED_USER_NAME_DESC;
                        model = model.OrderByDescending(x => x.AssignedUser.UserName);
                    }
                    else
                    {
                        lastSortOrder = IssueSortOrder.ASSIGNED_USER_NAME;
                        model = model.OrderBy(x => x.AssignedUser.UserName);
                    }
                    break;
                case IssueSortOrder.CREATED_DATE:
                    if (lastSortOrder == IssueSortOrder.CREATED_DATE)
                    {
                        lastSortOrder = IssueSortOrder.CREATED_DATE_DESC;
                        model = model.OrderByDescending(x => x.Created);
                    }
                    else
                    {
                        lastSortOrder= IssueSortOrder.CREATED_DATE;   
                        model = model.OrderBy(x => x.Created);
                    }
                    break;
                default:
                    lastSortOrder = IssueSortOrder.CREATED_DATE_DESC;
                    model = model.OrderByDescending(x => x.Created);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Title.Contains(searchString));
            }
            return View(model);
        }
    }
}
