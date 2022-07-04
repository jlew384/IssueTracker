using IssueTracker.Authorization;
using IssueTracker.Constants;
using IssueTracker.Data;
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

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]        
        public async Task<IActionResult> Create()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            CreateProjectViewModel model = new CreateProjectViewModel();
            model.AssignableUsers = _userManager.Users.Where(u => u.Id != user.Id).ToList();
            IList<ApplicationUser> admins = await _userManager.GetUsersInRoleAsync(UserRoles.ADMIN);
            IList<ApplicationUser> projectManagers = await _userManager.GetUsersInRoleAsync(UserRoles.PROJ_MNGR);
            model.AssignableProjectManagers = admins.Concat(projectManagers);
            return View(model);
        }


        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {            
            for (int i = 0; i < model.AssignableUsers.Count; i++)
            {
                if(model.AssignableUsers[i].IsSelected)
                {
                    model.Project.Users.Add(await _userManager.FindByIdAsync(model.AssignableUsers[i].Id));
                }
                
            }
            ApplicationUser user = await _userManager.FindByIdAsync(model.ProjectManagerId);
            ApplicationUser projectManager = await _userManager.FindByIdAsync(model.ProjectManagerId);

            if(!model.Project.Users.Contains(user))
            {
                model.Project.Users.Add(user);
            }

            if (!model.Project.Users.Contains(user))
            {
                model.Project.Users.Add(projectManager);
            }

            model.Project.DateCreated = DateTime.Now;
            _context.Projects.Add(model.Project);
            _context.SaveChanges();
            await AddProjectOwnerClaim(user, model.Project.Id);
            return RedirectToAction("Index");
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

            ApplicationUser user = await _userManager.GetUserAsync(User);
            EditProjectViewModel model = new EditProjectViewModel();
            model.Id = project.Id;
            model.Title = project.Title;
            model.Desc = project.Desc;

            foreach (ApplicationUser assignableUser in _userManager.Users)
            {
                if(assignableUser.Id != user.Id)
                {
                    if(project.Users.Contains(assignableUser))
                    {
                        assignableUser.IsSelected = true;
                    }

                    model.AssignableUsers.Add(assignableUser);
                }                
            }

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
            foreach (var partialUser in model.AssignableUsers)
            {
                var user  = await _userManager.FindByIdAsync(partialUser.Id);
                if(partialUser.IsSelected && !project.Users.Contains(user))
                {
                    project.Users.Add(user);
                }
                else if(!partialUser.IsSelected)
                {
                    project.Users.Remove(user);
                }
                
            }

            project.DateModified = DateTime.Now;
            _context.Projects.Update(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpGet]
        public async Task<IActionResult> Delete(int? pid)
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

            await RemoveProjectOwnerClaim(pid);


            return View(project);
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR)]
        [HttpPost]
        public IActionResult Delete(Project model)
        {
            if (User.IsInRole(UserRoles.PROJ_MNGR) && !CheckForProjectOwnerClaim(model.Id))

            {
                return NotFound();
            }

            _context.Projects.Remove(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Details(int? pid)
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

            ProjectIssueViewModel model = new ProjectIssueViewModel();
            model.Project = project;
            model.IssuesNotDone = _context.Issues
                .Where(i => i.ProjectId == project.Id && i.Status != IssueStatus.DONE)
                .ToList();
            model.IssuesDone = _context.Issues
                .Where(i => i.ProjectId == project.Id && i.Status == IssueStatus.DONE)
                .ToList();

            return View(model);
        }
    }
}
