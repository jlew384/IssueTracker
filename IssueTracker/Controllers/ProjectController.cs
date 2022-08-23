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
            return View();
        }


        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Create(Project model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            model.Users.Add(currentUser);
            model.OwnerId = currentUser.Id;
            _context.Projects.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? pid)
        {
            var project = _context.Projects.Find(pid);

            if(project == null)
            {
                return NotFound();
            }


            ApplicationUser user = await _userManager.GetUserAsync(User);
            

            if(User.IsInRole(UserRoles.ADMIN) || (User.IsInRole(UserRoles.PROJ_MNGR) && project.OwnerId == user.Id))
            {
                return View("Edit", project);
            }
            else
            {
                return View(project);
            }
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Delete(int pid)
        {
            Project project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User);

            if (User.IsInRole(UserRoles.PROJ_MNGR) && userId != project.OwnerId)
            {
                return BadRequest();
            }

            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
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

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
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

        [Authorize(Roles = UserRoles.ADMIN)]
        [HttpPost]
        public string UpdateProjectOwner(int pid, string userId)
        {
            Project? project = _context.Projects.Find(pid);
            if(project == null)
            {
                return "";
            }
            project.OwnerId = userId;
            _context.Projects.Update(project);
            _context.SaveChanges();

            return "success";
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<int> RemoveUsersFromProject(int pid, List<string> userIds)
        {
            Project? project = _context.Projects.Find(pid);

            foreach(string userId in userIds)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                project.Users.Remove(user);
            }

            _context.SaveChanges();
            return pid;
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<int> AddUsersToProject(int pid, List<string> userIds)
        {
            Project? project = _context.Projects.Find(pid);

            foreach (string userId in userIds)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                project.Users.Add(user);
            }

            _context.SaveChanges();
            return pid;
        }
    }
}
