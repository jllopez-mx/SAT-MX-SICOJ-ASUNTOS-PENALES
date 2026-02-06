using AmparoDirectoAPI.Model.ViewModels;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAsuntosPenalesAbogadoByFilters
    {
        public int id { get; set; }
        public string? numero_asunto_penal { get; set;}
        public DateTime? fecha_recepcion {get; set; }
        public DateTime? fecha_vencimiento { get; set;}
        public string? oficio_solicitud {get; set; }
        public string? numero_expediente_cadido { get; set;}
        public string? unidadRealizaSolicitud {get; set; }
        public int? idUnidadRealizaSolicitud {get; set;}
        public string? adminControla {get; set;}
        public int? idAdminControla {get; set;}
        public string? subAdministracion { get; set; }
        public int? idSubAdministracion {get; set;}
        public string? nombre_abogado {get; set;}
        public string? idAbogado {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public bool interno {get; set;}
        public int conteoImputados {get; set;}
    }
}