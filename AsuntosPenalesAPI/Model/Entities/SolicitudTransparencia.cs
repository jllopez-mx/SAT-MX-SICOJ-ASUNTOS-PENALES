using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class SolicitudTransparencia
    {
        public int id { get; set; }
        public int id_rol { get; set; }
        public string? nombre_rol { get; set; }
        public string? noSolicitud { get; set; }
        public int id_asunto {get; set;}      
        
        public DateTime fechaSolicitud { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime fecha_modificacion { get; set; }
    }
}