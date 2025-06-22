using System.ComponentModel.DataAnnotations;

namespace MotasAlcoafinal.Models
{
    /// <summary>
    /// ViewModel utilizada para o processo de recuperação de password.
    /// Contém o endereço de e-mail do utilizador que solicita a redefinição da sua palavra-passe.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Endereço de e-mail associado à conta do utilizador para envio do link de recuperação.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
