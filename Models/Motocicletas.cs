using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Regista os motociclos que dão entrada na oficina
    /// </summary>
    public class Motocicletas
    {

        /// <summary>
        /// Identificador do motociclo
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }

        /// <summary>
        /// A marca do motociclo
        /// </summary>
        [Display(Name = "Marca")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "A {0} só pode conter letras e números.")]
        public string Marca { get; set; } = string.Empty;

        /// <summary>
        /// Modelo do motociclo
        /// </summary>
        [Display(Name = "Modelo")]
        [StringLength(50)]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "O {0} só pode conter letras e números.")]
        public string Modelo { get; set; } = string.Empty;

        /// <summary>
        /// Ano de fabricação do motociclo
        /// </summary>
        [Display(Name = "Ano")]
        [Range(1900, 2100, ErrorMessage = "O {0} deve estar entre {1} e {2}.")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public int Ano { get; set; } = 0;

        /// <summary>
        /// Placa de identificação do motociclo
        /// </summary>
        [Display(Name = "Placa")]
        [StringLength(10)]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "A {0} só pode conter letras e números.")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        public string Placa { get; set; } = string.Empty;

        // Relacionamento: Uma motocicleta pertence a um cliente
        public Clientes? Cliente { get; set; }

        // Relacionamento: Uma motocicleta pode ter vários serviços
        /// <summary>
        /// Lista dos serviços que estão associados a um motociclo
        /// </summary>
        public ICollection<Servicos> Servicos { get; set; } = [];
    }
}
