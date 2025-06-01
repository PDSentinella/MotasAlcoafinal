using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using motasAlcoafinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MotasAlcoafinal.Models.ViewModels;

namespace MotasAlcoafinal.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(MotasAlcoaContext context,
           UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager,
           IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }


        /// <summary>
        /// Realiza a autenticação do utilizador e gera um token JWT para ter acesso à API.
        /// </summary>
        /// <param name="login"> Um objeto que contém as credenciais do utilizador (username/email e password).</param>
        /// <returns>Retorna um token JWT válido para autenticação, ou Unauthorized em caso de falha.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            // procurar pelo 'username' na base de dados,
            // para determinar se o utilizador existe
            var user = await _userManager.FindByEmailAsync(login.UserName);
            if (user == null) return Unauthorized();

            // se chego aqui, é porque o 'username' existe
            // mas, a password é válida?
            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded) return Unauthorized();

            // houve sucesso na autenticação
            // vou gerar o 'token', associado ao utilizador
            var token = await GenerateJwtToken(user);

            //devolvo o 'token'
            return Ok(new { token });
        }

        /// <summary>
        ///  Gerador de tokens para fazer a autenticação na API
        /// </summary>
        /// <param name="username">Nome do utilizador associado ao token</param>
        /// <returns>Retorna o token como uma string</returns>
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            // Claims básicas: nome e ID do token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),

        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var roles = await _userManager.GetRolesAsync(user);

            // Adicionar roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Criar a chave de segurança
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            // Definir as credenciais do token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Criar o token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            // Retornar token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }


}
