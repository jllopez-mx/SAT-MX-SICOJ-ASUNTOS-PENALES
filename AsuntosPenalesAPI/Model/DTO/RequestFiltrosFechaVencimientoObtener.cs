using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestFiltrosFechaVencimientoObtener
    {
        public string fecha_inicial { get; set; }= null!;
        public string fecha_final { get; set; } = null!;
        public List<int> Secciones { get; set; } = null!;
    }
}