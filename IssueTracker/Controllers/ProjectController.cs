using IssueTracker.Data;
using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_context.Projects.ToList());
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
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

        [Authorize]
        public IActionResult Edit(int? id)
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

            return View(project);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Project obj)
        {
            obj.DateModified = DateTime.Now;
            _context.Projects.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public IActionResult Delete(Project obj)
        {
            _context.Projects.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
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
