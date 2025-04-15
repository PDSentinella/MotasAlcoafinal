using System.ComponentModel.DataAnnotations;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Regista os clientes da oficina
    /// </summary>
    public class Clientes
    {

        /// <summary>
        /// Identificador do cliente
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        [Display(Name = "Nome")]
        [StringLength(50)]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public string ? Nome { get; set; }

        /// <summary>
        /// Contacto telefônico do cliente
        /// </summary>
        [Display(Name = "Telefone")]
        [StringLength(9)]
        [RegularExpression(@"^(91|92|93|96)\d{7}$", ErrorMessage = "Número de {0} inválido. Deve começar com 91, 92, 93 ou 96 e ter 9 dígitos no total.")]
        public string ? Telefone { get; set; }


        /// <summary>
        /// Email associado ao cliente
        /// </summary>
        [Display(Name = "Email")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O {0} não é válido.")]
        public string ? Email { get; set; }


        /// <summary>
        /// Respetiva morada do cliente
        /// </summary>
        [Display(Name = "Endereço")]
        [StringLength(50)]
        public string ? Endereco { get; set; }

        ///Relacionamento: Um cliente pode ter várias motocicletas
        /// <summary>
        /// Lista de motocicletas que são propriedade do 
        /// cliente
        /// </summary>
        public ICollection<Motocicletas>? Motocicletas { get; set; }
    }
}
