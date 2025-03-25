namespace motasAlcoa.Models
{
    ///
    /// <summary>
    /// Representa um pedido de peças realizado pela oficina. 
    /// Cada encomenda possui um identificador, uma data e um estado (Pendente ou Entregue).
    /// </summary>
    public class Encomendas
    {

        /// <summary>
        /// Identificador da encomenda
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data em que foi feito o pedido
        /// </summary>
        public DateTime DataPedido { get; set; }


        /// <summary>
        /// O estado em que se encontra o pedido
        /// </summary>
        public Estados Estado { get; set; }


        /// <summary>
        /// Estado do pedido. Representa um conjunto de valores pre-determinados
        /// que representam a evolução do 'pedido'
        /// </summary>
        public enum Estados
        {
            Pendente,
            Entregue

        }
    }
}
