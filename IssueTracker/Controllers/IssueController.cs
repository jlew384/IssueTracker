using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public IssueController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult IssueList(string type, string sortOrder, string searchString, int? pageIndex, string statusFilter, string priorityFilter, string typeFilter, int? pid, string userId)
        {
            return ViewComponent("IssueList",
                new
                {
                    type = type,
                    sortOrder = sortOrder,
                    searchString = searchString,
                    pageIndex = pageIndex,
                    statusFilter = statusFilter,
                    priorityFilter = priorityFilter,
                    typeFilter = typeFilter,
                    pid = pid,
                    userId = userId
                });
        }


        public IActionResult Index(int? pid)
        {
         
            Project project = _context.Projects.Find(pid);
            if (pid == null)
            {
                return NotFound();
            }
            ViewBag.Project = project;
            IEnumerable<Issue> issues = _context.Issues.Where(x => x.ProjectId == pid).ToList();
            return View(issues);
        }

        [HttpGet]
        public IActionResult Create(int? pid)
        {
            Project project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.PrioritySelectList = new SelectList(IssuePriority.List);
            ViewBag.TypeSelectList = new SelectList(IssueType.List);
            ViewBag.Project = project;

            CreateIssueViewModel model = new CreateIssueViewModel();
            model.ProjectId = project.Id;
            model.ProjectTitle = project.Title;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateIssueViewModel model)
        {
            model.Issue.CreatorUserId = _userManager.GetUserId(this.User);
            _context.Issues.Add(model.Issue);
            _context.SaveChanges();

            
            return RedirectToAction("Details", "Project", new {pid = model.Issue.ProjectId});
        }

        public IActionResult Edit(int? id)
        {
            EditIssueViewModel model = new EditIssueViewModel();
            model.Issue = _context.Issues.Find(id);
            model.AssignableUsers = model.Issue.Project.Users;


            model.RefererUrl = Request.Headers["Referer"].ToString();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditIssueViewModel model)
        {
            model.Issue.Modified = DateTime.Now;
            _context.Issues.Update(model.Issue);
            _context.SaveChanges();
            return Redirect(model.RefererUrl);
        }

        public IActionResult Delete(int id)
        {
            return View(new DeleteIssueModelView { 
                Issue = _context.Issues.Find(id),
                RefererUrl = Request.Headers["Referer"].ToString() 
            });
        }

        [HttpPost]
        public IActionResult Delete(DeleteIssueModelView model)
        {
            _context.Issues.Remove(model.Issue);
            _context.SaveChanges();
            return Redirect(model.RefererUrl);
        }

        public IActionResult Details(int id)
        {
            var issue = _context.Issues.Find(id);
            IssueDetailsModelView model = new IssueDetailsModelView();
            model.ProjectId = issue.ProjectId;
            model.ProjectTitle = issue.Project.Title;
            model.Issue = issue;
            model.RefererUrl = Request.Headers["Referer"].ToString();
            return View(model);
        }

        public async Task<IActionResult> MyIssues()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            MyIssuesViewModel model = new MyIssuesViewModel { UserId = user.Id };
            
            return View(model);
        }
    }
}
