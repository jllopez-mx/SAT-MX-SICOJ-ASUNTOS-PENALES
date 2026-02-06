using AmparoDirectoAPI.Model.ViewModels;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAsuntosPenalesById : PortadorClass
    {
        public string? numero_asunto_penal { get; set;}
        public DateTime? fecha_recepcion {get; set; }
        public DateTime? fecha_vencimiento { get; set;}
        public string? oficio_solicitud {get; set; }
        public string? numero_expediente_cadido { get; set;}
        public string? unidadRealizaSolicitud {get; set; }
        public int? idUnidadRealizaSolicitud {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public bool activo  { get; set; }= new();
        public bool interno {get; set;}
        public string? adminControla {get; set;}
        public int? idAdminControla {get; set;}
        public bool turnado { get; set; }

    }   

}