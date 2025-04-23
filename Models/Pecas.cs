using motasAlcoafinal.Models;
using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{
    /// <summary>
    /// Regista as peças que são adicionadas ao inventário
    /// </summary>
    public class Pecas
    {
        /// <summary>
        /// Identificador da peça
        /// </summary>
        [Key]  // PK, int, autonumber
        public int Id { get; set; }

        /// <summary>
        /// Nome da peça
        /// </summary>
        [Display(Name = "Nome da peça")]
        [StringLength(50)]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]   
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da peça com as suas caracteristicas
        /// </summary>
        [Display(Name = "Descrição")]
        [StringLength(200)]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Preço ao qual a peça vai ser vendida
        /// </summary>
        [Display(Name = "Preço")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [Range(0.01, 9999.99, ErrorMessage = "O {0} deve ser entre {1} e {2}")]
        public decimal Preco { get; set; } = decimal.Zero;

        /// <summary>
        /// Quantidade da peça disponível em armazém 
        /// </summary>
        [Display(Name = "Quantidade em armazém")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        public int QuantidadeEstoque { get; set; } = 0;

        // Relacionamento: Uma peça pode estar em vários serviços e encomendas
        public ICollection<ServicoPecas> ServicoPecas { get; set; } = [];
        public ICollection<EncomendaPecas> EncomendaPecas { get; set; } = [];

    }
}
