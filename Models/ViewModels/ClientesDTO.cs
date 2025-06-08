using motasAlcoafinal.Models;
using System.ComponentModel.DataAnnotations;

namespace MotasAlcoafinal.Models.ViewModels
{
    public class ClientesDTO
    {
        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string Nome { get; set; } = string.Empty;


        /// <summary>
        /// Email associado ao cliente
        /// </summary>
        public string Email { get; set; } = string.Empty;


        ///Relacionamento: Um cliente pode ter várias motocicletas
        /// <summary>
        /// Lista de motocicletas que são propriedade do 
        /// cliente
        /// </summary>
        public ICollection<MotocicletasDTO> Motocicletas { get; set; } = [];
    }
}
