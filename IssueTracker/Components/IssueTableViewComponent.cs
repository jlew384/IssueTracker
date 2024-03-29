﻿using IssueTracker.Constants;
using IssueTracker.Data;
using IssueTracker.Helpers;
using IssueTracker.Models;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Components
{
    public class IssueTableViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        private string? _filter;
        private string? _userId;
        private int? _projectId;
        private string? _sortOrder;
        private string? _sortDirection;
        private bool _isDashboard;

        public IssueTableViewComponent(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, string filter, int projectId, string sortField, string sortDirection, string searchString, int? pageIndex, bool isDashboard)
        {
            _userId = userId;
            _filter = filter;
            _projectId = projectId;
            _sortOrder = sortField + "_" + sortDirection;
            _sortDirection = sortDirection;
            _isDashboard = isDashboard;


            IQueryable<Issue> issues = InitializeIssues();

            

            issues = Sort(issues, _sortOrder);
            issues = Search(issues, searchString);

            PaginatedList<Issue> paginatedList = await PaginatedList<Issue>.CreateAsync(issues, pageIndex ?? 1, 10);

            


            

            if(isDashboard)
            {
                IssueTableComponentViewModel model = new IssueTableComponentViewModel()
                {
                    Issues = paginatedList,
                    SortDirection = sortDirection,
                    SortField = sortField,
                    Filter = filter,
                };
                return View("Dashboard", model);
            }
            else
            {
                Project proj = _context.Projects.Where(x => x.Id == projectId).FirstOrDefault();
                List<string> userIds = proj.Users.Select(x => x.Id).ToList();

                IssueTableComponentViewModel model = new IssueTableComponentViewModel()
                {
                    Issues = paginatedList,
                    ProjectTitle = _context.Projects.Find(projectId).Title,
                    ProjectId = projectId,
                    SortDirection = sortDirection,
                    SortField = sortField,
                    Filter = filter,
                    ProjectMemberIds = userIds
                };
                return View(model);
            }
            
        }

        private IQueryable<Issue> InitializeIssues()
        {
            switch (_filter)
            {
                case IssueFilter.PROJECT:
                    return _context.Issues.Where(x => x.ProjectId == _projectId);
                case IssueFilter.ASSIGNEE:
                    if(_isDashboard)
                    {
                        return _context.Issues.Where(x => x.AssignedUserId == _userId);
                    }
                    else
                    {
                        return _context.Issues.Where(x => x.ProjectId == _projectId).Where(x => x.AssignedUserId == _userId);
                    }
                    
                case IssueFilter.CREATOR:
                    if(_isDashboard)
                    {
                        return _context.Issues.Where(x => x.CreatorUserId == _userId);
                    }
                    else
                    {
                        return _context.Issues.Where(x => x.ProjectId == _projectId).Where(x => x.CreatorUserId == _userId);
                    }
                    
                case IssueFilter.ACTIVE:
                    if(_isDashboard)
                    {
                        return _context.Issues.Where(x => x.AssignedUserId == _userId).Where(x => x.Status != IssueStatus.DONE);
                    }
                    else
                    {
                        return _context.Issues.Where(x => x.ProjectId == _projectId).Where(x => x.Status != IssueStatus.DONE);
                    }
                    
                case IssueFilter.INACTIVE:
                    if (_isDashboard)
                    {
                        return _context.Issues.Where(x => x.AssignedUserId == _userId).Where(x => x.Status == IssueStatus.DONE);
                    }
                    else
                    {
                        return _context.Issues.Where(x => x.ProjectId == _projectId).Where(x => x.Status == IssueStatus.DONE);
                    }
                    
                default:
                    if (_isDashboard)
                    {
                        return _context.Issues.Where(x => x.AssignedUserId == _userId);
                    }
                    else
                    {
                        return _context.Issues.Where(x => x.ProjectId == _projectId);
                    }
                    
            }
        }        

        private IQueryable<Issue> Sort(IQueryable<Issue> issues, string sortOrder)
        {
            switch (sortOrder)
            {
                case IssueSortOrder.TITLE_ASC:
                    return issues.OrderBy(x => x.Title);
                case IssueSortOrder.TITLE_DESC:
                    return issues.OrderByDescending(x => x.Title);
                case IssueSortOrder.STATUS_ASC:
                    return issues.OrderByDescending(x => x.Status);
                case IssueSortOrder.STATUS_DESC:
                    return issues.OrderBy(x => x.Status);
                case IssueSortOrder.PRIORITY_ASC:
                    return issues
                        .Where(x => x.Priority == IssuePriority.LOW)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.HIGH));
                case IssueSortOrder.PRIORITY_DESC:
                    return issues
                        .Where(x => x.Priority == IssuePriority.HIGH)
                        .Concat(issues.Where(x => x.Priority == IssuePriority.MEDIUM))
                        .Concat(issues.Where(x => x.Priority == IssuePriority.LOW));
                case IssueSortOrder.TYPE_ASC:
                    return issues.OrderBy(x => x.Type);
                case IssueSortOrder.TYPE_DESC:
                    return issues.OrderByDescending(x => x.Type);
                case IssueSortOrder.CREATOR_ASC:
                    return issues.OrderBy(x => x.CreatorUser.UserName);
                case IssueSortOrder.CREATOR_DESC:
                    return issues.OrderByDescending(x => x.CreatorUser.UserName);
                case IssueSortOrder.ASSIGNED_USER_NAME_ASC:
                    return issues.OrderBy(x => x.AssignedUser.UserName);
                case IssueSortOrder.ASSIGNED_USER_NAME_DESC:
                    return issues.OrderByDescending(x => x.AssignedUser.UserName);
                case IssueSortOrder.CREATED_DATE_ASC:
                    return issues.OrderByDescending(x => x.Created);
                case IssueSortOrder.CREATED_DATE_DESC:                    
                    return issues.OrderBy(x => x.Created);
                default:
                    return issues.OrderByDescending(x => x.Created);
            }
        }

        private IQueryable<Issue> Search(IQueryable<Issue> issues, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                return issues.Where(x => x.Title.ToLower().Contains(searchString.ToLower()));
            }
            else
            {
                return issues;
            }
        }
    }
}
