using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

namespace IssueTracker.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ProjectTable()
        {
            return ViewComponent("ProjectTable");
        }

        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }        

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]        
        public async Task<IActionResult> Create()
        {
            ViewBag.BackUrl = Request.Headers["Referer"].ToString();
            return View();
        }


        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Create(Project model)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            model.Users.Add(currentUser);
            _context.Projects.Add(model);
            _context.SaveChanges();
            await AddProjectOwnerClaim(currentUser, model.Id);
            return RedirectToAction("Index", "Project");
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]
        public async Task<IActionResult> Edit(int? pid)
        {
            var project = _context.Projects.Find(pid);

            if(project == null)
            {
                return NotFound();
            }
            if(User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(pid))
            {
                return NotFound();
            }

            ViewBag.BackUrl = Request.Headers["Referer"].ToString();
            return View(project);
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Delete(int pid)
        {
            if (User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(pid))

            {
                return NotFound();
            }

            Project project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            _context.SaveChanges();
            await RemoveProjectOwnerClaim(pid);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string UpdateTitle(int pid, string title)
        {
            Project? project = _context.Projects.Find(pid);
            if (project == null)
            {
                return "";
            }

            project.Title = title;
            _context.Projects.Update(project);
            _context.SaveChanges();
            return title;
        }

        [HttpPost]
        public string UpdateDesc(int pid, string desc)
        {
            Project? project = _context.Projects.Find(pid);
            if (project == null)
            {
                return "";
            }

            project.Desc = desc;
            _context.Projects.Update(project);
            _context.SaveChanges();
            return desc;

        }

        private async Task AddProjectOwnerClaim(ApplicationUser projectOwner, int? projectId)
        {
            Claim projectOwnerClaim = new Claim(UserClaimTypes.PROJECT_OWNER, projectId.ToString());
            await RemoveProjectOwnerClaim(projectOwnerClaim);
            await _userManager.AddClaimAsync(projectOwner, projectOwnerClaim);
        }

        private async Task RemoveProjectOwnerClaim(int? projectId)
        {
            Claim projectOwnerClaim = new Claim(UserClaimTypes.PROJECT_OWNER, projectId.ToString());
            await RemoveProjectOwnerClaim(projectOwnerClaim);
        }

        private async Task RemoveProjectOwnerClaim(Claim projectOwnerClaim)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersForClaimAsync(projectOwnerClaim);
            foreach (ApplicationUser user in users)
            {
                await _userManager.RemoveClaimAsync(user, projectOwnerClaim);
            }
        }

        private bool CheckForProjectOwnerClaim(int? projectId)
        {
            return User.HasClaim(x => x.Type == UserClaimTypes.PROJECT_OWNER && x.Value == projectId.ToString());
        }
    }
}
