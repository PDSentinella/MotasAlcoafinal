using System.ComponentModel.DataAnnotations;

namespace MotasAlcoafinal.Models
{
    /// <summary>
    /// ViewModel utilizado para o processo de redefinição de palavra-passe.
    /// Contém os campos necessários para validar e atualizar a password do utilizador.
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Token de validação para o reset da password. Obrigatório.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Endereço de email do utilizador. Obrigatório e validado como email.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Nova palavra-passe do utilizador. Obrigatória.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Confirmação da nova palavra-passe. Deve coincidir com a nova password.
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        public string ConfirmPassword { get; set; }
    }

}
