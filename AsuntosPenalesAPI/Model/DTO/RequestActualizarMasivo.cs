using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestActualizarMasivo
    {
        public string? fecha_vencimiento { get; set; }
        public int? idAsunto { get; set; }
        public int? idModulo { get; set; }
        public int? idSeccion { get; set; }
        public int? idSeccionRenglon { get; set; }

    }
}