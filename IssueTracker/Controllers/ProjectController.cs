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

        [HttpGet]
        public IActionResult Index()
        {
            return View(new MyProjectsViewModel { UserId = _userManager.GetUserAsync(User).Result.Id});
        }

        [HttpGet]
        public IActionResult ProjectList(string type, string sortOrder, string searchString, int? pageIndex, string userId)
        {
            return ViewComponent("ProjectList",
                new
                {
                    type = type,
                    sortOrder = sortOrder,
                    searchString = searchString,
                    pageIndex = pageIndex,
                    userId = userId
                });
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
            model.UserId = user.Id;
            return View(model);
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]        
        public async Task<IActionResult> Create()
        {
            CreateProjectViewModel model = new CreateProjectViewModel();
            model.RefererUrl = Request.Headers["Referer"].ToString();
            return View(model);
        }


        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            model.Project.Users.Add(currentUser);
            _context.Projects.Add(model.Project);
            _context.SaveChanges();
            await AddProjectOwnerClaim(currentUser, model.Project.Id);
            return RedirectToAction("ManageUsersInProject", "Administration", new {pid = model.Project.Id } );
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

            EditProjectViewModel model = new EditProjectViewModel();
            model.Id = project.Id;
            model.Title = project.Title;
            model.Desc = project.Desc;
            model.RefererUrl = Request.Headers["Referer"].ToString();
            return View(model);
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Edit(EditProjectViewModel model)
        {
            if (User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(model.Id))
            {
                return NotFound();
            }
            
            Project project = _context.Projects.Find(model.Id);            

            project.Title = model.Title;
            project.Desc = model.Desc;

            project.DateModified = DateTime.UtcNow;
            _context.Projects.Update(project);
            _context.SaveChanges();
            return Redirect(model.RefererUrl);
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]
        public async Task<IActionResult> Delete(int? pid, string? refererUrl)
        {
            if (pid == null || pid == 0)
            {
                return NotFound();
            }

            if (User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(pid))
            {
                return NotFound();
            }

            var project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }
            
            return View(new DeleteProjectViewModel
            {
                Project = project,
                RefererUrl = Request.Headers["Referer"].ToString()
            });
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteProjectViewModel model)
        {
            if (User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(model.Project.Id))

            {
                return NotFound();
            }

            _context.Projects.Remove(model.Project);
            _context.SaveChanges();
            await RemoveProjectOwnerClaim(model.Project.Id);
            return Redirect(model.RefererUrl);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? pid)
        {
            if (pid == null || pid == 0)
            {
                return NotFound();
            }
            var project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);

            return View(new ProjectIssuesViewModel { 
                Project = project, 
                UserId = user.Id, 
                RefererUrl = Request.Headers["Referer"].ToString() 
            });
        }
    }
}
