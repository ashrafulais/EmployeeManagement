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
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminController> logger;

        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
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

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id: {id} can't be found.";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id: {model.Id} can't be found.";
                return View("NotFound");
            }

            user.Email = model.Email;
            user.UserName = model.UserName;
            var result = await userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            //on error, update the page
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id: {id} can't be found.";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                //on error, update the page
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View("ListUsers");
            }
        }

        //Role deletion
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id: {id} can't be found.";
                return View("NotFound");
            }
            else
            {
                try
                {
                    //throw new Exception("Demo Role deletion exception");

                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    //on error, update the page
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role: {ex.Message}");
                    ViewBag.ErrorTitle = $"{role.Name} role contains users.";
                    ViewBag.ErrorMessage = $"{role.Name} role can not be " +
                        $"removed right now. To delete a role, please " +
                        $"change the user roles under this role first.";

                    return View("Error");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);
            ViewBag.userName = user.UserName;

            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} can't be found.";
                return View("NotFound");
            }
            else
            {
                var model = new List<ManageUserRolesViewModel>();

                foreach (var role in await roleManager.Roles.ToListAsync())
                {
                    var manageUserRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };

                    manageUserRolesViewModel.IsSelected = await userManager
                        .IsInRoleAsync(user, role.Name) ?
                         true : false;

                    model.Add(manageUserRolesViewModel);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesViewModel> model, string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);
            
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} can't be found.";
                return View("NotFound");
            }

            var findroles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, findroles);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, 
                model.Where(x => x.IsSelected).Select(y=>y.RoleName)
                );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't add selected roles to this user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }
    }
}