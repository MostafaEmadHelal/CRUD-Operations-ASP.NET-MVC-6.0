using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger,
            SignInManager<ApplicationUser> signInManager) 
        {
            this.userManager = userManager;
            this.logger = logger;
            this.signInManager = signInManager;
        }
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel input)
        { 
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = input.Email,
                    UserName = input.Email.Split("@")[0],
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                    return RedirectToAction("Login");
                foreach (var error in result.Errors)
                    //logger.LogError(error.Description);
                ModelState.AddModelError("", error.Description);
            }

            return View(input);
        }

        public IActionResult Login()
        {
            return View(new SignInViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);

                if (user is null)
                    ModelState.AddModelError("", "Email Does not Exist");

                if(user != null && await userManager.CheckPasswordAsync(user, input.Password))
                {
                    var result = await signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);

                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
            }
            return View(input);
        }

        public async Task<IActionResult> SignOut()
        { 
            await signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult ForgetPassword()
        {
            return View(new ForgetPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);

                if (user is null)
                    ModelState.AddModelError("", "Email Does not Exist");
                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = input.Email, Token = token }, Request.Scheme);

                    var email = new Email
                    {
                        Title = "Reset Password",
                        Body = resetPasswordLink,
                        To = input.Email
                    };

                    // Send Email
                    EmailSettings.SendEmail(email);

                    return RedirectToAction("CompleteForgetPassword");
                }

            }
                return View(input);
        }

        public IActionResult ResetPassword(string email, string token) 
        {
           return View (new ResetPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);

                if (user is null)
                    ModelState.AddModelError("", "Email Does not Exist");
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, input.Token, input.Password);
                    if (result.Succeeded)
                        return RedirectToAction("Login");
                    foreach (var error in result.Errors)
                        //logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                }

            }

            return View(input);

        }

        public IActionResult AccessDenied()
        { 
            return View();
        }

    }
}
