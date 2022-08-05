using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Helpers
{
    public class UserRoles
    {
        public const string ADMIN = "Admin";
        public const string PROJ_MNGR = "Project Manager";
        public const string DEV = "Developer";
        public const string SUB = "Submitter";

        public static readonly string DEFAULT_ROLE = SUB;
        public static readonly string[] ROLES = new string[] { ADMIN, PROJ_MNGR, DEV, SUB };

        readonly RoleManager<IdentityRole> _roleManager;
        readonly UserManager<ApplicationUser> _userManager;

        public UserRoles(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task CreateRolesAsync()
        {
            foreach (string role in ROLES)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task AddToDefaultRoleAsync(ApplicationUser user)
        {
            await _userManager.AddToRoleAsync(user, DEFAULT_ROLE);
        }
    }
}
