using System.ComponentModel.DataAnnotations.Schema;

namespace motasAlcoa.Models
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
        /// Descrição do que é necessário ser feito no serviço
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor total que será pago pelo serviço
        /// </summary>
        public decimal CustoTotal { get; set; }

        /// <summary>
        /// Data em que o serviço é criado
        /// </summary>
        public DateTime DataServico { get; set; }

        /// <summary>
        /// Identificador do motociclo ao qual vai ser feito o serviço
        /// </summary>
        [ForeignKey(nameof(Motociclo))]
        public int MotocicloFK { get; set; }

        /// <summary>
        /// FK para os motociclos
        /// </summary>
        public Motocicletas? Motociclo { get; set; }


    }
}
