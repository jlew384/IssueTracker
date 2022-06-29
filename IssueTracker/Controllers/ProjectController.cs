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
            model.ManagedProjects = _context.Projects.Where(p => p.ProjectManagerId == user.Id).ToList();

            if (user.Projects != null)
            {
                foreach (var row in user.Projects)
                {
                    if(row.Project.ProjectManagerId != user.Id)
                    {
                        model.AssignedProjects.Add(row.Project);
                    }
                    
                }
            }            
            return View(model);
        }


        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Create(Project obj)
        {
            obj.DateCreated = DateTime.Now;
            _context.Projects.Add(obj);
            _context.SaveChanges();

            string userId = _userManager.GetUserId(this.User);
            ApplicationUserProject applicationUserProject = new ApplicationUserProject();
            applicationUserProject.ApplicationUserId = userId;
            applicationUserProject.ProjectId = obj.Id;

            _context.ApplicationUserProjects.Add(applicationUserProject);
            _context.SaveChanges();
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
