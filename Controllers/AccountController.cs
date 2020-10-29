﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginModel, 
                                                string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager
                    .PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }

                    /*logger.LogInformation($"return url: {returnUrl}");
                    return RedirectToAction("index", "home");*/
                }
                //validaion errors displays in the validation summary of the register view
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(loginModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel regModel)
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = regModel.Email,
                    Email = regModel.Email
                };

               var result = await userManager.CreateAsync(user, regModel.Password);

                if(result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                //validaion errors displays in the validation summary of the register view
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(regModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
    }
}