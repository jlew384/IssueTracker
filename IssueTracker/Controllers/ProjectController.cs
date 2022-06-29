using IssueTracker.Authorization;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Authorization;
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

        
        public IActionResult Index()
        {
            return View(_context.Projects.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            MyProjectsViewModel model = new MyProjectsViewModel();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            var claims = await _userManager.GetClaimsAsync(user);
            List<int> ownedProjectIds = claims
                .Where(c => c.Type == UserClaimTypes.PROJECT_OWNER)
                .Select(c => Int32.Parse(c.Value))
                .ToList();
            
            foreach(Project project in user.Projects)
            {
                if(ownedProjectIds.Contains(project.Id))
                {
                    model.ManagedProjects.Add(project);
                }
                else
                {
                    model.AssignedProjects.Add(project);
                }
            }
            return View(model);
        }


        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            project.Users.Add(user);
            project.DateCreated = DateTime.Now;
            _context.Projects.Add(project);
            _context.SaveChanges();

            await _userManager.AddClaimAsync(user, new Claim(UserClaimTypes.PROJECT_OWNER, project.Id.ToString()));

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var project = _context.Projects.Find(id);

            if(project == null)
            {
                return NotFound();
            }

            IList<ApplicationUser> projectManagers = await _userManager.GetUsersInRoleAsync(UserRoles.ADMIN);
            IList<ApplicationUser> admins = await _userManager.GetUsersInRoleAsync(UserRoles.PROJ_MNGR);

            ViewBag.ProjectManagerSelectList = new SelectList(admins.Concat(projectManagers), "Id", "UserName");
            return View(project);
        }

        
        [HttpPost]
        public IActionResult Edit(Project obj)
        {
            obj.DateModified = DateTime.Now;
            _context.Projects.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var project = _context.Projects.Find(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        
        [HttpPost]
        public IActionResult Delete(Project obj)
        {
            _context.Projects.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var project = _context.Projects.Find(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
    }
}
