using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace motasAlcoa.Models
{
    ///
    /// <summary>
    /// Regista os motociclos que dão entrada na oficina
    /// </summary>
    public class Motocicletas
    {

        /// <summary>
        /// Identificador do motociclo
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// A marca do motociclo
        /// </summary>
        public string Marca { get; set; }

        /// <summary>
        /// Modelo do motociclo
        /// </summary>
        public string Modelo { get; set; }  

        /// <summary>
        /// Ano de fabricação do motociclo
        /// </summary>
        public string Ano {  get; set; }

        /// <summary>
        /// Placa de identificação do motociclo
        /// </summary>
        public string Matricula { get; set; }


        /// <summary>
        /// Identificador do cliente ao qual o motociclo está associado
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        public int? ClienteFK { get; set; }


        /// <summary>
        /// FK para os clientes
        /// </summary>
        public Clientes? Cliente { get; set; }


        /// <summary>
        /// Lista dos serviços que estão associados a um motociclo
        /// </summary>
        public ICollection<Servicos> ListaServicos { get; set; }

    }
}
