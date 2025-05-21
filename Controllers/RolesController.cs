using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers
{
    [Authorize(Roles = "Root")]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View(users);
        }

        [HttpPost]
        public async Task <IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Falha ao atribuir role.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Utilizador não encontrado.");
            }
            return View("Index", _userManager.Users.ToList());
        }
    }
}