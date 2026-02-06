using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAcuseConclusion
    {
        public string? no_asunto { get; set; } = null!;
        public string? numero_expediente_cadido { get; set; } = null!;
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal { get; set; }
        public string? unidadRealizaSolicitud { get; set; }
        public int? idUnidadRealizaSolicitud { get; set; }
        public string? subAdministracion { get; set; }
        public int? idSubAdministracion {get; set;}
        public string? adminControla { get; set; }
        public int? idAdminControla { get; set; }
        public string abogado { get; set; } = null!;


        






    }
}