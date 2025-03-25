namespace motasAlcoa.Models
{
    /// <summary>
    /// Regista as peças que são adicionadas ao inventário
    /// </summary>
    public class Pecas
    {
        /// <summary>
        /// Identificador da peça
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da peça
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Descrição da peça com as suas caracteristicas
        /// </summary>
        public string descricao { get; set; }

        /// <summary>
        /// Preço ao qual a peça vai ser vendida
        /// </summary>
        public Decimal Preco {  get; set; }

        /// <summary>
        /// Quantidade da peça disponível em armazém 
        /// </summary>
        public int QuantidadeStock { get; set; }    

    }
}
