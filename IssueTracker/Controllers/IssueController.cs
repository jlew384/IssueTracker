using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public IssueController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult IssueTable(string filter, string sortField, string sortDirection, string searchString, int? pageIndex, bool emptySearch = false)
        {

            if(pageIndex == null)
            {
                pageIndex = HttpContext.Session.GetInt32("pageIndex");
            }
            else
            {
                HttpContext.Session.SetInt32("pageIndex", (int)pageIndex);
            }

            if(emptySearch)
            {
                searchString = "";
            }

            if(searchString == null && !emptySearch)
            {
                searchString = HttpContext.Session.GetString("searchString");
            }
            else
            {
                HttpContext.Session.SetString("searchString", searchString);
            }


            if(filter == null)
            {
                filter = HttpContext.Session.GetString("filter");
            }
            else
            {
                HttpContext.Session.SetString("filter", filter);
            }

            if(sortField == null)
            {
                sortField = HttpContext.Session.GetString("sortField");
                sortDirection = HttpContext.Session.GetString("sortDirection");
            }
            else
            {
                if(sortField == HttpContext.Session.GetString("sortField"))
                {
                    if(HttpContext.Session.GetString("sortDirection") == IssueSortOrder.DESC)
                    {
                        sortDirection = IssueSortOrder.ASC;
                        HttpContext.Session.SetString("sortDirection", sortDirection);
                    }
                    else if(HttpContext.Session.GetString("sortDirection") == IssueSortOrder.ASC)
                    {
                        sortDirection = IssueSortOrder.DESC;
                        HttpContext.Session.SetString("sortDirection", sortDirection);
                    }
                }
                else
                {
                    HttpContext.Session.SetString("sortField", sortField);
                    sortDirection = IssueSortOrder.DEFAULT_DIRECTION;
                    HttpContext.Session.SetString("sortDirection", sortDirection);
                }

            }


            return ViewComponent("IssueTable",
                new
                {
                    userId = _userManager.GetUserId(this.User),
                    filter = filter,
                    projectId = HttpContext.Session.GetInt32("projectId"),
                    sortField = sortField,
                    sortDirection = sortDirection,
                    searchString = searchString,
                    pageIndex = pageIndex
                });
        }

        [HttpGet]
        public IActionResult Index(int? pid)
        {
            Project? project = _context.Projects.Find(pid);
            if (pid == null)
            {
                return NotFound();
            }

            string? filter =  HttpContext.Session.GetString("filter");
            string? sortField = HttpContext.Session.GetString("sortField");
            string? sortDirection = HttpContext.Session.GetString("sortDirection");
            string? searchString = HttpContext.Session.GetString("searchString");
            int? pageIndex = HttpContext.Session.GetInt32("pageIndex");

            HttpContext.Session.SetInt32("projectId", (int)pid);

            if(filter == null)
            {
                filter = IssueFilter.PROJECT;
                HttpContext.Session.SetString("filter", filter);
            }

            if(sortField == null)
            {
                sortField = IssueSortOrder.DEFAULT_FIELD;
                HttpContext.Session.SetString("sortField", sortField);
            }

            if(sortDirection == null)
            {
                sortDirection = IssueSortOrder.DEFAULT_DIRECTION;
                HttpContext.Session.SetString("sortDirection", sortDirection);
            }

            if(searchString == null)
            {
                searchString = "";
                HttpContext.Session.SetString("searchString", searchString);
            }

            if(pageIndex == null)
            {
                pageIndex = 1;
                HttpContext.Session.SetInt32("pageIndex", (int)pageIndex);
            }

            IssueIndexViewModel model = new IssueIndexViewModel()
            {
                UserId = _userManager.GetUserId(this.User),
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                Filter = filter,
                SortField = sortField,
                SortDirection = sortDirection,
                SearchString = searchString,
                PageIndex = (int)pageIndex
            };

            return View(model);
        }

        

        [HttpGet]
        public async Task<IActionResult> Create(int? pid)
        {
            Project? project = _context.Projects.Find(pid);

            if (project == null)
            {
                return NotFound();
            }

            IList<ApplicationUser> submitters = await _userManager.GetUsersInRoleAsync(UserRoles.SUB);
            IEnumerable<ApplicationUser> assignableUsers = project.Users.Except(submitters);

            CreateIssueViewModel model = new CreateIssueViewModel()
            {
                ProjectTitle = project.Title,
                ProjectId = project.Id,
                AssignableUsers = assignableUsers
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult Create(Issue issue)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            issue.CreatorUserId = _userManager.GetUserId(this.User);

            _context.Issues.Add(issue);
            _context.SaveChanges();
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Title = issue.Title,
                IsTitleUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Desc = issue.Desc,
                IsDescUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Status = issue.Status,
                IsStatusUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Priority = issue.Priority,
                IsPriorityUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Type = issue.Type,
                IsTypeUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                CreatorUserId = issue.CreatorUserId,
                IsCreatorUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                AssignedUserId = issue.AssignedUserId,
                IsAssignedUserUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();

            return RedirectToAction("Index", new {pid = issue.ProjectId});
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            
            
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();

            

            IList<ApplicationUser> submitters = await _userManager.GetUsersInRoleAsync(UserRoles.SUB);
            IEnumerable<ApplicationUser> assignableUsers = issue.Project.Users.Except(submitters);

            EditIssueViewModel model = new EditIssueViewModel()
            {
                Id = issue.Id,
                Title = issue.Title,
                Desc = issue.Desc,
                Priority = issue.Priority,
                Status = issue.Status,
                Type = issue.Type,
                AssignedUserId = issue.AssignedUserId,
                ProjectTitle = issue.Project.Title,
                ProjectId = issue.Project.Id,
                AssignableUsers = assignableUsers
            };            

            List<string> projMemberIds = issue.Project.Users.Select(x => x.Id).ToList();

            var userId = _userManager.GetUserId(this.User);
            if (User.IsInRole(UserRoles.ADMIN) || (projMemberIds.Contains(userId) && !User.IsInRole(UserRoles.SUB)))
            {
                return View("Edit", model);
            }
            else
            {
                return View(model);
            }
            
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null) return NotFound();

            int projectId = issue.ProjectId;

            _context.Issues.Remove(issue);
            _context.SaveChanges();

            return RedirectToAction("Index", new { pid = projectId});
        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public string UpdateStatus(int id, string status)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Status = status;
            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Status = issue.Status,
                IsStatusUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();
            return status;

        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public string UpdatePriority(int id, string priority)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Priority = priority;
            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Priority = issue.Priority,
                IsPriorityUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();
            return priority;

        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public string UpdateType(int id, string type)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Type = type;
            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Type = issue.Type,
                IsTypeUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();
            return type;

        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public async Task<string> UpdateAssignedUser(int id, string userId)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                issue.AssignedUserId = null;
            }
            else
            {
                issue.AssignedUserId = user.Id;
            }

            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                AssignedUserId = issue.AssignedUserId,
                IsAssignedUserUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();

            if (user == null)
            {
                return "Unassigned";
            }
            else
            {
                return user.UserName;
            }

        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public string UpdateTitle(int id, string title)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Title = title;
            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Title = issue.Title,
                IsTitleUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();
            return title;

        }

        [Authorize(Roles = UserRoles.ADMIN + "," + UserRoles.PROJ_MNGR + "," + UserRoles.DEV)]
        [HttpPost]
        public string UpdateDesc(int id, string desc)
        {
            Issue? issue = _context.Issues.Find(id);
            if (issue == null)
            {
                return "";
            }

            issue.Desc = desc;
            _context.Issues.Update(issue);
            _context.IssuesHistory.Add(new IssueHistory()
            {
                IssueId = issue.Id,
                Desc = issue.Desc,
                IsDescUpdated = true,
                UpdatedByUserId = _userManager.GetUserId(this.User)
            });
            _context.SaveChanges();
            return desc;

        }

        [HttpGet]
        public IActionResult IssueCommentList(int id)
        {
            return ViewComponent("IssueCommentList", new
            {
                issueId = id
            });
        }


        [HttpPost]
        public string CreateIssueComment(int id, string text)
        {
            Issue? issue = _context.Issues.Find(id);

            if (issue == null)
            {
                return "";
            }           

            IssueComment comment = new IssueComment()
            {
                IssueId = id,
                Text = text,
                OwnerId = _userManager.GetUserId(this.User)
            };

            _context.IssueComments.Add(comment);
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string DeleteIssueComment(int cid)
        {
            IssueComment comment = _context.IssueComments.Find(cid);

            if (comment == null)
            {
                return "";
            }

            _context.IssueComments.Remove(comment);
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string EditIssueComment(int cid, string text)
        {
            IssueComment comment = _context.IssueComments.Find(cid);

            if (comment == null)
            {
                return "";
            }

            comment.Text = text;
            comment.ModifiedDate = DateTime.UtcNow;

            _context.IssueComments.Update(comment);
            _context.SaveChanges();
            return "success";
        }

        [HttpGet]
        public IActionResult IssueHistory(int id)
        {
            return ViewComponent("IssueHistory", new
            {
                issueId = id
            });
        }
    }
}
