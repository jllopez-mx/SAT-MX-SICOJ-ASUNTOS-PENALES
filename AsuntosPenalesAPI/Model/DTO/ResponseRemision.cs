using Sicoj.Utils.ViewModels; 

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseRemision
    {
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string? TipoAutoridadRemision { get; set; }
        public int? idTipoAutoridadRemision {get; set;}
        public string? UnidadAdministrativaRemite { get; set; }
        public int? idUnidadAdministrativaRemite {get; set;}
        public string? UnidadAdministrativaRecibe { get; set; }
        public int? idUnidadAdministrativaRecibe {get; set;}
        public string? UnidadAdministrativaExterna { get; set; }
        public int? idUnidadAdministrativaExterna {get; set;}
        public string FechaOficio { get; set; } = null!;
        public string NumeroOficio { get; set; } = null!;
    }
    
} 