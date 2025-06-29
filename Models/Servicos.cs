using System.ComponentModel.DataAnnotations;
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
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do motociclo ao qual vai ser feito o serviço
        /// </summary>
        [ForeignKey(nameof(Motocicleta))]
        public int? MotocicletaId { get; set; }

        /// <summary>
        /// Identificador do cliente ao qual o serviço pertence
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        public int? ClienteId { get; set; }

        /// <summary>
        /// Descrição do que é necessário ser feito no serviço
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data em que o serviço é criado
        /// </summary>
        [Display(Name = "Data")]
        [DataType(DataType.Date)] // transforma o atributo, na BD, em 'Date'
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
                     ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Valor total que será pago pelo serviço
        /// </summary>
        public decimal CustoTotal { get; set; } = decimal.Zero;

        /// <summary>
        /// Estado do serviço. Representa o ciclo de vida do serviço
        /// </summary>
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public ServicoEstado Status { get; set; } = ServicoEstado.Pendente;

        /// <summary>
        /// Enumeração que representa os estados possíveis de um serviço.
        /// </summary>
        public enum ServicoEstado
        {
            Pendente,      // Serviço criado, aguardando execução
            Concluido,     // Serviço finalizado, peças subtraídas
            Cancelado      // Serviço cancelado, não subtrai peças
        }

        // Relacionamento: Um serviço pertence a uma motocicleta
        public Motocicletas? Motocicleta { get; set; }

        // Relacionamento: Um serviço pertence a um cliente
        public Clientes? Cliente { get; set; }

        // Relacionamento: Um serviço pode usar várias peças
        public ICollection<ServicoPecas> ServicoPecas { get; set; } = [];


    }
}
