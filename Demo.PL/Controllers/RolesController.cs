using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{

    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ILogger<RolesController> logger;
        private readonly UserManager<ApplicationUser> userManager;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            ILogger<RolesController> logger,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();

            return View(roles);
        }


        public IActionResult Create()
        { 
            return View(new ApplicationRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            if (ModelState.IsValid)
            { 
                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach(var error in result.Errors)
                {
                    logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return View(role);
        }

        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return NotFound();

            var user = await roleManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            return View(viewName, user);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationRole applicationRole)
        {
            if (id != applicationRole.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await roleManager.FindByIdAsync(id);

                    Role.Name = applicationRole.Name;
                    Role.NormalizedName = applicationRole.Name.ToUpper();

                    var result = await roleManager.UpdateAsync(Role);

                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
            return View(applicationRole);
        }


        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await roleManager.FindByIdAsync(id);

                if (user is null)
                    return NotFound();

                var result = await roleManager.DeleteAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        { 
            var role = await roleManager.FindByIdAsync(roleId);

            if(role is null)
                return NotFound();

            ViewBag.RoleId = roleId;

            var usersInRole = new List<UserInRoleViewModel>();

            var users = await userManager.Users.ToListAsync();

            foreach (var user in users) 
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;

                usersInRole.Add(userInRole);
            }
            return View(usersInRole);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await userManager.FindByIdAsync(user.UserId);

                    if (appUser != null)
                    { 
                        if(user.IsSelected && !await userManager.IsInRoleAsync(appUser, role.Name))
                            await userManager.AddToRoleAsync(appUser, role.Name);
                        else if (!user.IsSelected && await userManager.IsInRoleAsync(appUser, role.Name))
                            await userManager.RemoveFromRoleAsync(appUser, role.Name);
                    }
                }

            return RedirectToAction("Update", new { id = roleId });
            }

            return View(users);

        }



    }
}
