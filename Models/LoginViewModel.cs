using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Representa a relação entre uma encomenda e as peças associadas.
    /// Cada instância desta classe indica a quantidade de uma peça específica 
    /// dentro de uma determinada encomenda.
    /// </summary>

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = false;
    }
}
