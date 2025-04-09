using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Representa a relação entre uma encomenda e as peças associadas.
    /// Cada instância desta classe indica a quantidade de uma peça específica 
    /// dentro de uma determinada encomenda.
    /// </summary>

    public class EncomendaPecas
    {
        public int Id { get; set; }

        /// <summary>
        /// Identificador da encomenda
        /// </summary>
        [ForeignKey(nameof(Encomenda))]
        public int ? EncomendaId { get; set; }

        /// <summary>
        /// Identificador da peça
        /// </summary>
        [ForeignKey(nameof(Peca))]
        public int ? PecaId { get; set; }

        /// <summary>
        /// Número de unidades da peça solicitada no pedido
        /// </summary>
        public int ? Quantidade { get; set; }

        // Relacionamento: Uma EncomendaPeca pertence a uma Encomenda
        public Encomendas? Encomenda { get; set; }

        // Relacionamento: Uma EncomendaPeca pertence a uma Peça
        public Pecas? Peca { get; set; }

    }
}
