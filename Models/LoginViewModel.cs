using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{


    /// <summary>
    /// ViewModel utilizado para o formulário de login de utilizadores.
    /// Contém os campos necessários para autenticação.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Endereço de email do utilizador. Obrigatório e validado como email.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do utilizador. Obrigatória e tratada como campo de password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o utilizador pretende manter a sessão iniciada.
        /// </summary>
        [Display(Name = "Guardar?")]
        public bool RememberMe { get; set; } = false;
    }
}
