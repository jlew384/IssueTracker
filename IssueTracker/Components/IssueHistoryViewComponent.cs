using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using IssueTracker.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueHistoryViewComponent : ViewComponent
    {
        public ApplicationDbContext _context;

        public IssueHistoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int issueId, bool isDashboard)
        {
            List<IssueHistoryViewModel> model = new List<IssueHistoryViewModel>();
            IQueryable<IssueHistory> histories;
            if (isDashboard)
            {
                histories = _context.IssuesHistory.OrderByDescending(x => x.Updated);
            }
            else
            {
                histories = _context.IssuesHistory.Where(x => x.IssueId == issueId).OrderByDescending(x => x.Updated);
            }

            foreach(IssueHistory item in histories)
            {
                //typeof(Class1).GetProperty("Name")
                //Attribute.GetCustomAttribute(MemberInfo, Type)

                IssueHistoryViewModel modelItem;

                if(item.IsAssignedUserUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Assigned",
                        FieldValue = item.AssignedUser == null ? "Unassigned" : item.AssignedUser.UserName,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if(item.IsCreatorUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Creator",
                        FieldValue = item.CreatorUser.UserName,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if(item.IsDescUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Description",
                        FieldValue = item.Desc,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if(item.IsPriorityUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Priority",
                        FieldValue = item.Priority,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if(item.IsStatusUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Status",
                        FieldValue = item.Status,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if(item.IsTypeUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Type",
                        FieldValue = item.Type,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
                else if (item.IsTitleUpdated)
                {
                    modelItem = new IssueHistoryViewModel()
                    {
                        FieldName = "Title",
                        FieldValue = item.Title,
                        Updated = DateTimeHelpers.GetSimpleElapsedTime(item.Updated),
                        UpdatedBy = item.UpdatedBy.UserName,
                        IssueTitle = item.Issue.Title
                    };
                    model.Add(modelItem);
                }
            }

            if(isDashboard)
            {
                return View("Dashboard", model);
            }

            return View(model);
        }
    }
}
