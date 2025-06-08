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

        /// <summary>
        /// Carrega dados para ViewBags com utilizadores e roles
        /// </summary>
        private async Task CarregarViewBagsAsync()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                userRoles[user.Id] = role ?? "Sem Role";
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = roles;
        }

        /// <summary>
        /// Exibe a lista de utilizadores e as suas roles
        /// </summary>
        public async Task<IActionResult> Index()
        {
            await CarregarViewBagsAsync();
            return View(_userManager.Users.ToList());
        }

        /// <summary>
        /// Exibe o formulário para atribuir roles
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            await CarregarViewBagsAsync();
            return View("Index", _userManager.Users.ToList());
        }

        /// <summary>
        /// Processa a atribuição de uma role a um utilizador
        /// </summary>
        /// <param name="userId">ID do utilizador</param>
        /// <param name="role">Role a atribuir</param>
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null && user.UserName == User.Identity.Name)
            {
                ModelState.AddModelError("", "Você não pode alterar a sua própria role.");
                await CarregarViewBagsAsync();
                return View("Index", _userManager.Users.ToList());
            }
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Falha ao atribuir role.");
            }
            else
            {
                ModelState.AddModelError("", "Utilizador não encontrado.");
            }

            await CarregarViewBagsAsync();
            return View("Index", _userManager.Users.ToList());
        }

        /// <summary>
        /// Remove um utilizador do sistema
        /// </summary>
        /// <param name="userId">ID do utilizador a remover</param>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user != null && user.UserName == User.Identity.Name)
            {
                ModelState.AddModelError("", "Você não se pode apagar a si mesmo.");
                await CarregarViewBagsAsync();
                return View("Index", _userManager.Users.ToList());
            }

            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                    return(RedirectToAction("Index"));  

                }
                ModelState.AddModelError("", "Erro ao apagar utilizador");
            }
            else
            {
                ModelState.AddModelError("", "Utilizador não encontrado");
            }
            await CarregarViewBagsAsync();
            return View("Index",_userManager.Users.ToList());
        }
    }
}