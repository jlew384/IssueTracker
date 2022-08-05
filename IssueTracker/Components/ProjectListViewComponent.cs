using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IssueTracker.Components
{
    public class ProjectListViewComponent : ViewComponent
    {
        public static class Type
        {
            public const string DEFAULT = "default";
            public const string MANAGED = "managed";
            public const string ASSIGNED = "assigned";
            public const string ASSIGNED_ONLY = "assigned_only";
        }


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PAGE_SIZE = 10;


        public ProjectListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type, string? sortOrder, string? searchString, int? pageIndex, string userId)
        {
            IQueryable<Project> projects = InitializeProjectsAsync(type, userId);
            projects = Search(projects, searchString);
            projects = Sort(projects, sortOrder);


            PaginatedList<Project> paginatedList = await PaginatedList<Project>.CreateAsync(projects, pageIndex ?? 1, PAGE_SIZE);


            return View(new ProjectListViewModel
            {
                Type = type,
                SortOrder = sortOrder,
                SearchString = searchString,
                PageIndex = pageIndex ?? 1,
                UserId = userId,
                Projects = paginatedList
            });
        }


        private IQueryable<Project> InitializeProjectsAsync(string type, string userId)
        {
            if(!String.IsNullOrEmpty(userId))
            {
                ApplicationUser user = _userManager.FindByIdAsync(userId).Result;
                switch (type)
                {
                    case Type.MANAGED:
                        return _context.Projects
                            .Where(x => x.Users.Select(u => u.Id).Contains(userId))
                            .Where(x => FindOwnedProjectIds(user).Contains(x.Id));
                    case Type.ASSIGNED:
                        return _context.Projects.Where(x => x.Users.Select(u => u.Id).Contains(userId));
                    case Type.ASSIGNED_ONLY:
                        return _context.Projects
                            .Where(x => x.Users.Select(u => u.Id).Contains(userId))
                            .Where(x => !FindOwnedProjectIds(user).Contains(x.Id));
                }
            }
            return _context.Projects;            
        }

        private List<int> FindOwnedProjectIds(ApplicationUser user)
        {
            var claims = _userManager.GetClaimsAsync(user).Result;
            return claims
                .Where(c => c.Type == UserClaimTypes.PROJECT_OWNER)
                .Select(c => Int32.Parse(c.Value))
                .ToList();
        }

        private IQueryable<Project> Sort(IQueryable<Project> projects, string sortOrder)
        {
            switch (sortOrder)
            {
                case ProjectSortOrder.TITLE:
                    return projects.OrderBy(x => x.Title);
                case ProjectSortOrder.TITLE_DESC:
                    return projects.OrderByDescending(x => x.Title);
                case ProjectSortOrder.CREATED_DATE:
                    return projects.OrderBy(x => x.DateCreated);
                case ProjectSortOrder.CREATED_DATE_DESC:
                    return projects.OrderByDescending(x => x.DateCreated);
                case ProjectSortOrder.MODIFIED_DATE:
                    return projects.OrderBy(x => x.DateCreated);
                case ProjectSortOrder.MODIFIED_DATE_DESC:
                    return projects.OrderByDescending(x => x.DateModified);
                default:
                    return projects.OrderByDescending(x => x.DateModified);
            }
        }

        private IQueryable<Project> Search(IQueryable<Project> projects, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                return projects.Where(x => x.Title.ToLower().Contains(searchString.ToLower()));
            }
            else
            {
                return projects;
            }
        }
    }
}
