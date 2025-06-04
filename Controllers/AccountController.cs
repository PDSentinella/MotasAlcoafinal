using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using MotasAlcoafinal.Services;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotasAlcoafinal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager; 

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, EmailService emailService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Exibe o formulário de registo
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Processa o registo de um novo utilizador
        /// </summary>
        /// <param name="model">Dados do registo</param>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    var message = $"<h3>Confirme o seu e-mail</h3>" +
                                  $"<p>Clique no link para confirmar: <a href='{confirmationLink}'>Confirmar E-mail</a></p>";

                    await _emailService.SendEmailAsync(model.Email, "Confirmação de E-mail", message);

                    return View("Info"); // Crie uma view que diga: "verifique seu e-mail"
                }

                foreach (var error in result.Errors)
                {
                    var errorMsg = error.Description;
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Confirma o e-mail do utilizador
        /// </summary>
        /// <param name="userId">ID do utilizador</param>
        /// <param name="token">Token de confirmação</param>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("Parâmetros inválidos.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Utilizador não encontrado.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? View("ConfirmEmailSuccess") : View("Error");
        }

        /// <summary>
        /// Exibe o formulário de login
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Processa o login do utilizador
        /// </summary>
        /// <param name="model">Dados do login</param>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,              // agora usa o email do formulário
                model.Password,           // e a password que o utilizador introduziu
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
            return View(model);
        }

        /// <summary>
        /// Exibe o formulário para recuperação da password
        /// </summary>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Processa a recuperação da password
        /// </summary>
        /// <param name="model">E-mail para recuperação</param>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return View("ForgotPasswordConfirmation"); // View informando que, se o email existir, o link foi enviado
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { token, email = model.Email }, protocol: HttpContext.Request.Scheme);

            var message = $"<p>Para redefinir sua senha, clique no link abaixo:</p>" +
                          $"<p><a href='{callbackUrl}'>Redefinir senha</a></p>";

            await _emailService.SendEmailAsync(model.Email, "Recuperação de Password", message);

            return View("ForgotPasswordConfirmation");
        }

        /// <summary>
        /// Exibe o formulário para redefinir a password
        /// </summary>
        /// <param name="token">Token de reset</param>
        /// <param name="email">E-mail do utilizador</param>
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Index", "Home");

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        /// <summary>
        /// Processa a redefinição da password
        /// </summary>
        /// <param name="model">Dados para reset</param>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        /// <summary>
        /// Exibe a confirmação de reset de password
        /// </summary>
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View(); 
        }

        /// <summary>
        /// Atribui uma role a um utilizador
        /// </summary>
        /// <param name="userId">ID do utilizador</param>
        /// <param name="role">Role a atribuir</param>
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("A role não existe.");
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return Ok($"Role '{role}' atribuída ao utilizador '{user.UserName}' com sucesso.");
            }

            return BadRequest("Erro ao atribuir a role.");
        }

        /// <summary>
        /// Faz logout do utilizador
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}