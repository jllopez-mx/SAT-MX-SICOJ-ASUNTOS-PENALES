using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseApelacion
    {
        public int id { get; set; }	   
        public int idImputado { get; set; }	   
        public string fechaPresentacion { get; set; } = null!;
        public string? numeroTocaPenal { get; set; }
        public string? tipoOrganoJurisdiccional { get; set; }
        public string? tipoResolucion { get; set; }
        public int? idTipoResolucion {get; set;}
        public string fechaResolucion { get; set; } = null!;
        public string? descripcion_resolucion { get; set; }
        public string? estadoProcesal {get; set;}
        public int? idEstadoProcesal {get; set;}

    }
}