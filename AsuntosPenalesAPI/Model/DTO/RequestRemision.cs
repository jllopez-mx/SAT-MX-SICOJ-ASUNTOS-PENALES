using System.ComponentModel.DataAnnotations;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestRemitir
    {
        public int idAsuntoPenal { get; set; }
        public int idTipoAutoridad { get; set; }
        public int? idUnidadAdministrativaRecibe { get; set; }
        public int? idUnidadAdministrativaExterna { get; set; }       
        public string? fechaOficio { get; set; }
        public string? numeroOficio { get; set; }



        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
    
}