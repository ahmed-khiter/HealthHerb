using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthHerb.Authorization;
using HealthHerb.Models;
using HealthHerb.Models.User;
using HealthHerb.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthHerb.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<BaseUser> userManager;
        private readonly SignInManager<BaseUser> signInManager;

        public AccountController
        (
            UserManager<BaseUser> userManager,
            SignInManager<BaseUser> signInManager
        )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
       
        [HttpGet]
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return Redirect("/");
            }

            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (signInManager.IsSignedIn(User))
            {
                return Redirect("/");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await userManager.FindByEmailAsync(model.Email);

            if (currentUser != null)
            {
                ModelState.AddModelError("Email", "Email is already exists");

                return View(model);
            }

            var user = new BaseUser
            {
                AccountType = Enum.AccountType.Client,
                FullName = model.FirstName + " " +model.LastName,
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName, 
                LastName = model.LastName ,
                EmailConfirmed = true,
                Address = model.Address
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                await userManager.AddToRoleAsync(user, Role.Consumer);

                ViewData["Success"] = "Success register, welcome to Health Herb";

                return Redirect("/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await userManager.FindByEmailAsync(model.Email);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Your email or password is wrong");

                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(currentUser, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Your email or password is wrong");

                return View(model);
            }
            ViewData["Success"] = "Welcome to health herb";
            return Redirect("/");
        }

        public async Task<IActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
