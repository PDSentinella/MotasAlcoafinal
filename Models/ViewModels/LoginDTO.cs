﻿namespace MotasAlcoafinal.Models.ViewModels
{
    /// <summary>
    /// dados da pessoa para gerar uma autenticação
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// 'username' da pessoa que se quer autenticar
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        ///  Password da pessoa que se quer autenticar
        /// </summary>
        public string Password { get; set; } = string.Empty;

    }
}
