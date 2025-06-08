using Microsoft.AspNetCore.SignalR.Protocol;

namespace MotasAlcoafinal.Models.ViewModels
{
    /// <summary>
    /// dados de uma motocicleta, para serem usados na API
    /// </summary>
    public class MotocicletasDTO
    {
        /// <summary>
        /// Marca da motocicleta
        /// </summary>
        public string Marca {  get; set; }=string.Empty;

        /// <summary>
        /// Modelo da motocicleta
        /// </summary>
        public string? Modelo {  get; set; }

        /// <summary>
        /// Ano de fabricação da motocicleta
        /// </summary>
        public int Ano {  get; set; }

    }
}
