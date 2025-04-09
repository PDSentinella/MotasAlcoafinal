using System.ComponentModel.DataAnnotations.Schema;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Tem como função registar os serviços que são prestados
    /// </summary>
    public class Servicos
    {

        /// <summary>
        /// Identificador do servico
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do motociclo ao qual vai ser feito o serviço
        /// </summary>
        [ForeignKey(nameof(Motocicleta))]
        public int? MotocicletaId { get; set; }

        /// <summary>
        /// Descrição do que é necessário ser feito no serviço
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Data em que o serviço é criado
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        /// Valor total que será pago pelo serviço
        /// </summary>
        public decimal? CustoTotal { get; set; }

        // Relacionamento: Um serviço pertence a uma motocicleta
        public Motocicletas? Motocicleta { get; set; }

        // Relacionamento: Um serviço pode usar várias peças
        public ICollection<ServicoPecas>? ServicoPecas { get; set; }


    }
}
