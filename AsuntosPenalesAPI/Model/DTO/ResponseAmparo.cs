using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAmparo
    {
        public int id { get; set; }	   
        public int idImputado { get; set; }	
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? tipoAmparo { get; set; }
        public int? idTipoAmparo {get; set;}
        public string fechaPresentacion { get; set; } = null!;
        public string? numeroJuicio {get; set; } = null!;
        public string? tipoOrganoJurisdiccional { get; set; }
        public string? juzgado { get; set; }      
        public string? tipoResolucion { get; set; }
        public int? idTipoResolucion {get; set; }
        public string fechaResolucion { get; set; } = null!;
        public string? descripcionResolucion { get; set; }
        public string? fechaNotificacion {get; set; } = null!;
        public bool alegatos {get; set; } 
        public string? numeroOficio {get; set; } = null!;
        public string? fechaOficio {get; set; } = null!;


    }
}