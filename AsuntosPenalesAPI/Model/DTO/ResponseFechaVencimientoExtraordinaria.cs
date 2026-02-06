using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseFechaVencimientoExtraordinaria
    {
        public int Id { get; set; }
        public string numeroAsunto { get; set; } = null!;
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal { get; set; }
        
        public string? administracion { get; set; }
        public int? idAdministracion {get; set;}
    }
}