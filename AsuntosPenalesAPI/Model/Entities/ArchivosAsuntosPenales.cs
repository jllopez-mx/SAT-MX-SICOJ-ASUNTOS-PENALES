using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class ArchivosAsuntosPenales
    {
        public int? id { get; set; }
        public int? id_tipo_documento { get; set; }
        public int id_asuntospenales { get; set; }
        public int? id_seccion { get; set; }
        public string? file_name { get; set; } = null!;
        public string path_file { get; set; } = null!;
        public string content_type { get; set; } = null!;
        public string owner_name { get; set; } = null!;
        public bool estatus { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public bool activo { get; set; }
        public string? size { get; set; }
        public string? usuario_creacion { get; set; } = null!;
        public string? usuario_modificacion { get; set; }
        public int? id_renglon_seccion { get; set; }
        public bool permanente { get; set; }
        public int id_rol { get; set; }

    
    }
}