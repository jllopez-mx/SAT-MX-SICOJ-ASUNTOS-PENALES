using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseFechaVencimientoSeccion
    {
        public int idModulo { get; set; }
        public int idSeccion { get; set; }
        public int idAsunto { get; set; }
        public string? fecha_vencimiento { get; set; }
        public int? idSeccionRenglon { get; set; }
        public int mesesCalendario { get; set; }
        
    }
}