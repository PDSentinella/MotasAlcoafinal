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
        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public string ?  Marca { get; set; }
        public string ? Modelo { get; set; }
        public int ?  Ano { get; set; }
        public string ? Placa { get; set; }

        // Relacionamento: Uma motocicleta pertence a um cliente
        public Clientes? Cliente { get; set; }

        // Relacionamento: Uma motocicleta pode ter vários serviços
        public ICollection<Servicos>? Servicos { get; set; }
    }
}
