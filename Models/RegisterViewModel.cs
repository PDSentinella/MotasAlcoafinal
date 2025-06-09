using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{

    /// <summary>
    /// ViewModel para o registo de utilizadores
    /// /// </summary>
    /// using System.ComponentModel.DataAnnotations;
    /// using System.ComponentModel.DataAnnotations.Schema;
    /// using System.ComponentModel.DataAnnotations;
    /// using System.ComponentModel.DataAnnotations.Schema;
    /// using System.ComponentModel.DataAnnotations.Schema;
    ///     
    /// /// <remarks>
    /// Esta classe é usada para capturar os dados necessários para o registo de um novo utilizador.
    /// </remarks>
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
