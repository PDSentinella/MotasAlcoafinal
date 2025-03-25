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
        public int Id { get; set; }
        public int?  ServicoId { get; set; }
        public int? PecaId { get; set; }
        public int? QuantidadeUsada { get; set; }

        // Relacionamento: Um ServiçoPeca pertence a um Serviço
        public Servicos? Servico { get; set; }

        // Relacionamento: Um ServiçoPeca pertence a uma Peça
        public Pecas? Peca { get; set; }
    }
}
