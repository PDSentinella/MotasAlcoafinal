using System.ComponentModel.DataAnnotations;
using motasAlcoafinal.Models;

namespace motasAlcoafinal.Models
{
    ///
    /// <summary>
    /// Regista os clientes da oficina
    /// </summary>
    public class Clientes
    {
        public int Id { get; set; }
        public string ? Nome { get; set; }
        public string ? Telefone { get; set; }
        public string ? Email { get; set; }
        public string ? Endereco { get; set; }

        // Relacionamento: Um cliente pode ter várias motocicletas
        public ICollection<Motocicletas>? Motocicletas { get; set; }
    }
}
