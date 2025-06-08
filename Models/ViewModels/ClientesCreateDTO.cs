using System.ComponentModel.DataAnnotations;

namespace MotasAlcoafinal.Models.ViewModels
{
    public class ClientesCreateDTO
    {

        /// <summary>
        /// Nome do cliente 
        /// </summary>
        [Required]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone do cliente
        /// </summary>
        [Required]
        [RegularExpression(@"^(91|92|93|96)\d{7}$", ErrorMessage = "Número inválido.")]
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// Email do cliente
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Endereço do cliente
        /// </summary>
        [Required]
        public string Endereco { get; set; } = string.Empty;
    }

}
