using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Models;
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
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Issue obj)
        {
            obj.Status = IssueStatus.TO_DO;
            obj.CreatorUserId = _userManager.GetUserId(this.User);
            _context.Issues.Add(obj);
            _context.SaveChanges();
            return RedirectToAction("Index", new {pid = obj.ProjectId});
        }

        public IActionResult Edit(int? id)
        {            
            var issue = _context.Issues.Find(id);
            var users = _userManager.Users;
            var userIds = new List<string>();
            ViewBag.AssignedUsers = new SelectList(users);
            ViewBag.StatusSelectList = new SelectList(IssueStatus.List);
            ViewBag.PrioritySelectList = new SelectList(IssuePriority.List);
            ViewBag.TypeSelectList = new SelectList(IssueType.List);
            return View(issue);
        }

        [HttpPost]
        public IActionResult Edit(Issue obj)
        {
            obj.Modified = DateTime.Now;
            _context.Issues.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index", new {pid = obj.ProjectId});
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
    }
}
