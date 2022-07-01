﻿using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Authorization;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IssueTracker.Controllers
{
    [Authorize(Roles = UserRoles.ADMIN)]
    public class AdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<UserViewModel> modelList = new List<UserViewModel>() { };
            foreach(var user in _userManager.Users.Where(x => x.Id != _userManager.GetUserId(User)))
            {
                UserViewModel model = new UserViewModel();
                model.Id = user.Id;
                model.Name = user.UserName;
                model.Email = user.Email;
                model.Roles = await _userManager.GetRolesAsync(user);
                modelList.Add(model);
            }

            return View(modelList);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRole(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return NotFound();
            }

            EditUserRoleViewModel model = new EditUserRoleViewModel();
            model.Id = user.Id;
            model.Name = user.UserName;
            model.Email = user.Email;
            model.IsAdmin = await _userManager.IsInRoleAsync(user, UserRoles.ADMIN);
            model.IsProjectManager = await _userManager.IsInRoleAsync(user, UserRoles.PROJ_MNGR);
            model.IsDeveloper = await _userManager.IsInRoleAsync(user, UserRoles.DEV);
            model.IsSubmitter = await _userManager.IsInRoleAsync(user, UserRoles.SUB);


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);

            List<string> rolesIn = new List<string>();
            if (model.IsAdmin) rolesIn.Add(UserRoles.ADMIN);
            if (model.IsProjectManager) rolesIn.Add(UserRoles.PROJ_MNGR);
            if (model.IsDeveloper) rolesIn.Add(UserRoles.DEV);
            if (model.IsSubmitter) rolesIn.Add(UserRoles.SUB);

            IList<string> roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, rolesIn);

            return RedirectToAction("Index");
        }




        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };

            foreach(var user  in _userManager.Users)
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.RoleId} cannot be found";
                return NotFound();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NotFound();
            }

            var model = new List<UserRoleViewModel>();

            foreach(var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NotFound();
            }

            for(int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name)) 
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if(result.Succeeded)
                {
                    if(i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });
                    }
                }
            }


            return RedirectToAction("EditRole", new { id = roleId });
        }

    }
}