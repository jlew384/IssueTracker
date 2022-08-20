using IssueTracker.Data;
using IssueTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueCommentListViewComponent : ViewComponent
    {
        ApplicationDbContext _context;

        public IssueCommentListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int issueId)
        {
            List<IssueComment> comments = _context.IssueComments.Where(x => x.IssueId == issueId).OrderByDescending(x => x.CreatedDate).ToList();
            return View(comments);
        }
    }
}
