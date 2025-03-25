using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Representa um pedido de peças realizado pela oficina. 
    /// Cada encomenda possui um identificador, uma data e um estado (Pendente ou Entregue).
    
    /// </summary>
    public class Encomendas
    {

        public int Id { get; set; }
        public DateTime ? DataPedido { get; set; }
        public Estados ? Status { get; set; } = Estados.Pendente; // Default 'Pendente'

        public enum Estados
        {
            Pendente,
            Entregue

        }
        public ICollection<EncomendaPecas>? EncomendaPecas { get; set; }

    }
}
