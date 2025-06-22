using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{

    /// <summary>
    /// ViewModel utilizado para o registo de novos utilizadores.
    /// Contém os campos necessários para criar uma nova conta.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Endereço de email do utilizador. 
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do utilizador. 
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmação da palavra-passe. Deve coincidir com a palavra-passe.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "A password e a password de confimação devem coincidir.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
