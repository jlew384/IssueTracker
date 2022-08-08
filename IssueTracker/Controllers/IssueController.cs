using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
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

        [HttpGet]
        public IActionResult Index(int? pid, string sortBy)
        {
            Project? project = _context.Projects.Find(pid);
            if (pid == null)
            {
                return NotFound();
            }

            List<Issue> issues;

            if(sortBy == null)
            {
                issues = _context.Issues.Where(x => x.ProjectId == pid).ToList();
            }
            else
            {
                issues = _context.Issues.Where(x => x.ProjectId == pid).OrderBy(x => x.Title).ToList();
            }

            IssueIndexViewModel model = new IssueIndexViewModel()
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                Issues = issues
            };

            return View(model);
        }

        [HttpPost]
        public string UpdateStatus(int id, string status)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Status = status;
            _context.Issues.Update(issue);
            _context.SaveChanges();
            return status;

        }
        [HttpPost]
        public string UpdatePriority(int id, string priority)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Priority = priority;
            _context.Issues.Update(issue);
            _context.SaveChanges();
            return priority;

        }
        [HttpPost]
        public string UpdateType(int id, string type)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Type = type;
            _context.Issues.Update(issue);
            _context.SaveChanges();
            return type;

        }

        [HttpGet]
        public IActionResult Create(int? pid)
        {
            Project? project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            CreateIssueViewModel model = new CreateIssueViewModel()
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title
            };
            return View(model);
        }



        [HttpPost]
        public IActionResult Create(CreateIssueViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return NotFound();
            }

            Issue issue = new Issue()
            {
                Title = model.Title,
                Desc = model.Desc,
                Status = IssueStatus.TO_DO,
                Priority = model.Priority,
                Type = model.Type,
                ProjectId = model.ProjectId,
                CreatorUserId = _userManager.GetUserId(this.User)
            };

            _context.Issues.Add(issue);
            _context.SaveChanges();

            
            return RedirectToAction("Details", "Project", new {pid = model.ProjectId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();

            IList<ApplicationUser> submitters = await _userManager.GetUsersInRoleAsync(UserRoles.SUB);
            IEnumerable<ApplicationUser> assignableUsers = issue.Project.Users.Except(submitters);

            EditIssueViewModel model = new EditIssueViewModel()
            {
                Id = issue.Id,
                Title = issue.Title,
                Desc = issue.Desc,
                Priority = issue.Priority,
                Status = issue.Status,
                Type = issue.Type,
                AssignedUserId = issue.AssignedUserId,
                ProjectTitle = issue.Project.Title,
                AssignableUsers = assignableUsers
            };            

            ViewBag.BackUrl = Request.Headers["Referer"].ToString();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditIssueViewModel model, string backUrl)
        {
            Issue? issue = _context.Issues.Find(model.Id);
            if (!ModelState.IsValid || issue == null)
            {
                return NotFound();
            }

            issue.Title = model.Title;
            issue.Desc = model.Desc;
            issue.AssignedUser = _context.ApplicationUsers.Find(model.AssignedUserId);
            issue.Status = model.Status;
            issue.Priority = model.Priority;
            issue.Type = model.Type;
            issue.Modified = DateTime.UtcNow;

            _context.Issues.Update(issue);
            _context.SaveChanges();
            return Redirect(backUrl);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();

            ViewBag.BackUrl = Request.Headers["Referer"].ToString();
            return View(issue);
        }

        [HttpPost]
        public IActionResult Delete(int? id, string backUrl)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();

            _context.Issues.Remove(issue);
            _context.SaveChanges();

            return Redirect(backUrl);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();
            ViewBag.BackUrl = Request.Headers["Referer"].ToString();
            return View(issue);
        }

        [HttpGet]
        public async Task<IActionResult> MyIssues()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            MyIssuesViewModel model = new MyIssuesViewModel { UserId = user.Id };
            
            return View(model);
        }
    }
}
