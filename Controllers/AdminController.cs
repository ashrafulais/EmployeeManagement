using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var identityResult = await roleManager.CreateAsync(identityRole);

                if(identityResult.Succeeded)
                {
                    return RedirectToAction("listroles", "admin");
                }

                foreach(IdentityError err in identityResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role is null)
            {
                ViewBag.ErrorMessage = $"Role with id: {id} can't be found.";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var user in await userManager.GetUsersInRoleAsync(role.Name))
            {
                model.Users.Add(user.UserName);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id: {model.Id} can't be found.";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if(result.Succeeded)
                {
                    return RedirectToAction("listroles");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.roleName = role.Name;

            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} can't be found.";
                return View("NotFound");
            }
            else
            {
                var model = new List<UserRoleViewModel>();

                foreach(var user in await userManager.Users.ToListAsync() )
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    userRoleViewModel.IsSelected = await userManager
                        .IsInRoleAsync(user, role.Name) ?
                         true : false;

                    model.Add(userRoleViewModel);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} can't be found.";
                return View("NotFound");
            }

            foreach(var user in model)
            {
                var finduser = await userManager.FindByIdAsync(user.UserId);

                IdentityResult result = null;
                //if the user is selected and not in the role
                if(user.IsSelected && ! await userManager.IsInRoleAsync(finduser, role.Name) )
                {
                    result = await userManager.AddToRoleAsync(finduser, role.Name);
                }
                else if(!user.IsSelected && await userManager.IsInRoleAsync(finduser, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(finduser, role.Name);

                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }
}