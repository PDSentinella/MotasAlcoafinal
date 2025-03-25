using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    /// <summary>
    /// Regista as peças que são adicionadas ao inventário
    /// </summary>
    public class Pecas
    {
        public int Id { get; set; }
        public string ?  Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? Preco { get; set; }
        public int? QuantidadeEstoque { get; set; }

        // Relacionamento: Uma peça pode estar em vários serviços e encomendas
        public ICollection<ServicoPecas>? ServicoPecas { get; set; }
        public ICollection<EncomendaPecas>? EncomendaPecas { get; set; }

    }
}
