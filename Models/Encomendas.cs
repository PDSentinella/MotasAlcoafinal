using motasAlcoafinal.Models;
using System.ComponentModel.DataAnnotations;

namespace motasAlcoafinal.Models
{

    /// <summary>
    /// Representa um pedido de peças realizado pela oficina. 
    /// Cada encomenda possui um identificador, uma data e um estado (Pendente ou Entregue).
    
    /// </summary>
    public class Encomendas
    {


        /// <summary>
        /// Identificador da encomenda
        /// </summary>
        [Key]  // PK, int, autonumber
        public int Id { get; set; }

        /// <summary>
        /// Data em que foi feito o pedido
        /// </summary>
        [Display(Name = "Data")]
        [DataType(DataType.Date)] // transforma o atributo, na BD, em 'Date'
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
                     ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        /// <summary>
        /// Estado da encomenda. Representa um conjunto de valores pre-determinados
        /// que representam a evolução da 'encomenda'
        /// </summary>
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public Estados Status { get; set; } = Estados.Pendente; // Default 'Pendente'


        /// <summary>
        /// Enumeração que representa os estados possíveis de uma encomenda.
        /// Os estados são: Pendente e Entregue.
        /// </summary>
        public enum Estados
        {
            Pendente,
            Entregue

        }

        /// <summary>
        /// Lista de peças que fazem parte da encomenda
        /// </summary>
        public ICollection <EncomendaPecas> EncomendaPecas { get; set; } = [];

    }
}
