using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IssueTracker.Components
{
    public class UserListViewComponent : ViewComponent
    {
        public static class Type
        {
            public const string DEFAULT = "default";
            public const string NOT_IN_PROJECT = "not_in_project";
            public const string IN_PROJECT = "in_project";
        }


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PAGE_SIZE = 10;


        public UserListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type, string? sortOrder, string? searchString, int? pageIndex, string roleFilter, int? pid, string userId)
        {
            List<UserViewModel> users = await InitializeUsersAsync(type, userId, pid);

            users = FilterRole(users, roleFilter);
            users = Search(users, searchString);
            users = Sort(users, sortOrder);

            PaginatedList<UserViewModel> paginatedList = new PaginatedList<UserViewModel>(users, users.Count, pageIndex ?? 1, PAGE_SIZE);


            return View(new UserListViewModel
            {
                Type = type,
                SortOrder = sortOrder,
                SearchString = searchString,
                PageIndex = pageIndex ?? 1,
                RoleFilter = roleFilter,
                ProjectId = pid,
                UserId = userId,
                Users = paginatedList
            });
        }

        private List<UserViewModel> FilterRole(List<UserViewModel> users, string roleFilter)
        {
            if (!String.IsNullOrEmpty(roleFilter))
            {

                return users.Where(x => x.Role.Contains(roleFilter)).ToList();
            }
            else
            {
                return users;
            }

        }
        private async Task<List<UserViewModel>> InitializeUsersAsync(string type, string userId, int? pid)
        {
            List<UserViewModel> usersList = null;
            IQueryable<ApplicationUser> users;
            switch (type)
            {
                case Type.IN_PROJECT:
                    usersList = _context.Projects
                        .Where(x => x.Id == pid)
                        .Select(x => x.Users)
                        .SelectMany(x => x)
                        .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur})
                        .Join(_context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                        .Select(c => new UserViewModel()
                        {
                            Id = c.ur.u.Id,
                            Name = c.ur.u.UserName,
                            Email = c.ur.u.Email,
                            Role = c.r.Name
                        }).ToList().GroupBy(uv => new { uv.Id, uv.Name, uv.Email }).Select(r => new UserViewModel()
                        {
                            Id = r.Key.Id,
                            Name = r.Key.Name,
                            Email = r.Key.Email,
                            Role = string.Join(",", r.Select(c => c.Role).ToArray())
                        }).ToList();
                    break;
                case Type.NOT_IN_PROJECT:
                    users= _context.Projects
                        .Where(x => x.Id == pid)
                        .Select(x => x.Users)
                        .SelectMany(x => x);

                    usersList = _userManager.Users
                        .Except(users)
                        .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                        .Join(_context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                        .Select(c => new UserViewModel()
                        {
                            Id = c.ur.u.Id,
                            Name = c.ur.u.UserName,
                            Email = c.ur.u.Email,
                            Role = c.r.Name
                        }).ToList().GroupBy(uv => new { uv.Id, uv.Name, uv.Email }).Select(r => new UserViewModel()
                        {
                            Id = r.Key.Id,
                            Name = r.Key.Name,
                            Email = r.Key.Email,
                            Role = string.Join(",", r.Select(c => c.Role).ToArray())
                        }).ToList();

                    break;
                default:
                    usersList = _userManager.Users
                        .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                        .Join(_context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                        .Select(c => new UserViewModel()
                        {
                            Id = c.ur.u.Id,
                            Name = c.ur.u.UserName,
                            Email = c.ur.u.Email,
                            Role = c.r.Name
                        }).ToList().GroupBy(uv => new { uv.Id, uv.Name, uv.Email }).Select(r => new UserViewModel()
                        {
                            Id = r.Key.Id,
                            Name = r.Key.Name,
                            Email = r.Key.Email,
                            Role = string.Join(",", r.Select(c => c.Role).ToArray())
                        }).ToList();
                    break;
            }
            

            return usersList;
        }

        private List<UserViewModel> Sort(List<UserViewModel> users, string sortOrder)
        {
            switch (sortOrder)
            {
                case UserSortOrder.USER_NAME:
                    return users.OrderBy(x => x.Name).ToList();
                case UserSortOrder.USER_NAME_DESC:
                    return users.OrderByDescending(x => x.Name).ToList();
                case UserSortOrder.EMAIL:
                    return users.OrderBy(x => x.Email).ToList();
                case UserSortOrder.EMAIL_DESC:
                    return users.OrderByDescending(x => x.Email).ToList();
                case UserSortOrder.ROLES:
                    return users.OrderBy(x => x.Role).ToList();
                case UserSortOrder.ROLES_DESC:
                    return users.OrderByDescending(x => x.Role).ToList();
                default:
                    return users.OrderBy(x => x.Name).ToList();
            }
        }

        private List<UserViewModel> Search(List<UserViewModel> users, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                return users.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            else
            {
                return users;
            }
        }
    }
}
