using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{


    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Guardar?")]
        public bool RememberMe { get; set; } = false;
    }
}
