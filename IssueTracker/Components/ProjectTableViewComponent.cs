using IssueTracker.Data;
using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class ProjectTableViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PAGE_SIZE = 10;


        public ProjectTableViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<Project> projects = _context.Projects;


            //PaginatedList<Project> paginatedList = await PaginatedList<Project>.CreateAsync(projects, pageIndex ?? 1, PAGE_SIZE);


            return View(projects.ToList());
        }
    }
}
