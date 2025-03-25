using System.ComponentModel.DataAnnotations;

namespace motasAlcoa.Models
{
    ///
    /// <summary>
    /// Regista os clientes da oficina
    /// </summary>
    public class Clientes
    {
        /// <summary>
        /// Identificador do cliente
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string Nome { get; set; }  

        /// <summary>
        /// Email associado ao cliente
        /// </summary>
        public string Email { get; set; }  

        /// <summary>
        /// Contacto telefônico do cliente
        /// </summary>
        public string Telemovel { get; set; }

        /// <summary>
        /// Respetiva morada do cliente
        /// </summary>
        public string Morada { get; set; }


        /// <summary>
        /// Lista dos motociclos que estão associados a um cliente
        /// </summary>
        public ICollection<Motocicletas> ListaMotociclos { get; set; }
    }
}
