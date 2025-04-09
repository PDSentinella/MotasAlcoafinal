using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
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
        public int? ClienteId { get; set; }

        /// <summary>
        /// A marca do motociclo
        /// </summary>
        public string ?  Marca { get; set; }

        /// <summary>
        /// Modelo do motociclo
        /// </summary>
        public string ? Modelo { get; set; }

        /// <summary>
        /// Ano de fabricação do motociclo
        /// </summary>
        public int ?  Ano { get; set; }

        /// <summary>
        /// Placa de identificação do motociclo
        /// </summary>
        public string ? Placa { get; set; }

        // Relacionamento: Uma motocicleta pertence a um cliente
        public Clientes? Cliente { get; set; }

        // Relacionamento: Uma motocicleta pode ter vários serviços
        /// <summary>
        /// Lista dos serviços que estão associados a um motociclo
        /// </summary>
        public ICollection<Servicos>? Servicos { get; set; }
    }
}
