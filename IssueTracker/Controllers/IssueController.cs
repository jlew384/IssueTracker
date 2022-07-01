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
    public class IssueController : Controller
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public IssueController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            model.Project = project;
            model.AssignableUsers = project.Users;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(CreateIssueViewModel model)
        {
            model.Issue.CreatorUserId = _userManager.GetUserId(this.User);
            _context.Issues.Add(model.Issue);
            _context.SaveChanges();

            
            return RedirectToAction("Index", new {pid = model.Issue.ProjectId});
        }

        public IActionResult Edit(int? id)
        {
            EditIssueViewModel model = new EditIssueViewModel();
            model.Issue = _context.Issues.Find(id);
            model.AssignableUsers = model.Issue.Project.Users;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditIssueViewModel model)
        {
            model.Issue.Modified = DateTime.Now;
            _context.Issues.Update(model.Issue);
            _context.SaveChanges();
            return RedirectToAction("Details", "Project", new {pid = model.Issue.ProjectId});
        }

        public IActionResult Delete(int id)
        {
            
            var issue = _context.Issues.Find(id);
            ViewBag.Project = issue.Project;
            return View(issue);
        }

        [HttpPost]
        public IActionResult Delete(Issue obj, int pid)
        {
            _context.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index", new {pid = pid});
        }

        public IActionResult Details(int id)
        {
            var issue = _context.Issues.Find(id);
            ViewBag.Project = issue.Project;
            return View(issue);
        }

        public async Task<IActionResult> MyIssues()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            MyIssuesViewModel model = new MyIssuesViewModel();
            model.IssuesCreated = _context.Issues.Where(issue => issue.CreatorUserId == user.Id && issue.Status != IssueStatus.DONE);
            model.IssuesAssigned = _context.Issues.Where(issue => issue.AssignedUserId == user.Id && issue.CreatorUserId != user.Id && issue.Status != IssueStatus.DONE);

            return View(model);
        }
    }
}
