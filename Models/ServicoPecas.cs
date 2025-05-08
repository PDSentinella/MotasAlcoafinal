using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Representa a relação entre os serviços realizados e as peças utilizadas.
    /// Cada registro nesta tabela indica quantas unidades de uma peça foram usadas
    /// num serviço específico.
    /// </summary>
    public class ServicoPecas
    {
        /// <summary>
        /// Identificador do relacionamento entre serviços e peças
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do serviço que está associado 
        /// ao relacionamento entre serviços e peças
        /// </summary>

        [ForeignKey(nameof(Servico))]
        public int? ServicoId { get; set; }

        /// <summary>
        /// Identificador da peça que está associada    
        /// ao relacionamento entre serviços e peças
        /// </summary>

        [ForeignKey(nameof(Peca))]
        public int? PecaId { get; set; }

        /// <summary>
        /// Quantidade da peça usada no serviço
        /// </summary>
        [Required]
        public int QuantidadeUsada { get; set; } = 0;

        // Relacionamento: Um ServiçoPeca pertence a um Serviço
        public Servicos? Servico { get; set; }

        // Relacionamento: Um ServiçoPeca pertence a uma Peça
        public Pecas? Peca { get; set; }
    }
}