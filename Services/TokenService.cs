﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotasAlcoafinal.Services
{
    /// <summary>
    /// Gerador de 'tokens' JWT
    /// </summary>
    public class TokenService
    {

        private readonly IConfiguration _config;

        /// <summary>
        /// Inicializa uma nova instância do serviço de geração de tokens JWT.
        /// </summary>
        /// <param name="config">Configuração da aplicação, utilizada para obter as definições do JWT.</param>
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Gera um token JWT para o utilizador autenticado.
        /// </summary>
        /// <param name="user">Utilizador para o qual o token será gerado.</param>
        /// <returns>Token JWT como string.</returns>
        public string GenerateToken(ApplicationUser user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
           new Claim(JwtRegisteredClaimNames.Sub, user.Id),   // User ID
           new Claim(JwtRegisteredClaimNames.Email, user.Email),  // User Email - não será nulo pq é usado como UserName
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
       };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
