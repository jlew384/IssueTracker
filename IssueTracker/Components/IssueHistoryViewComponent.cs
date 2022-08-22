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

            IQueryable<IssueHistory> histories;
            if (isDashboard)
            {
                histories = _context.IssuesHistory.OrderByDescending(x => x.Updated).Take(10);
            }
            else
            {
                histories = _context.IssuesHistory.Where(x => x.IssueId == issueId).OrderByDescending(x => x.Updated).Take(10);
            }


            if (isDashboard)
            {
                return View("Dashboard", histories.ToList());
            }

            return View(histories.ToList());
        }
    }
}
