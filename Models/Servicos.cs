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
        public int Id { get; set; }
        public int? MotocicletaId { get; set; }
        public string? Descricao { get; set; }
        public DateTime? Data { get; set; }
        public decimal? CustoTotal { get; set; }

        // Relacionamento: Um serviço pertence a uma motocicleta
        public Motocicletas? Motocicleta { get; set; }

        // Relacionamento: Um serviço pode usar várias peças
        public ICollection<ServicoPecas>? ServicoPecas { get; set; }


    }
}
