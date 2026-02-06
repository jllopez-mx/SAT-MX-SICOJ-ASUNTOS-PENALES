using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesModificacion
    {
        public int id { get; set; }
        public int id_asunto_penal { get; set; }
        public int? id_imputado { get; set; }
        public int? id_seccion { get; set; }
        public int? id_renglon_seccion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
        public int? id_estado_procesal { get; set; }
    }
}