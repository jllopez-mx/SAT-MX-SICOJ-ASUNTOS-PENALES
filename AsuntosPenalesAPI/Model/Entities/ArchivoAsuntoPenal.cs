using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class ArchivoAsuntoPenal
    {
        public int id { get; set; }
        public int id_rol { get; set; }
        public string? rol { get; set; } = null!;
        public int id_consulta { get; set; }
        public int id_remision { get; set; }
        public string? no_folio { get; set; }
        public int? id_seccion { get; set; }
        public string? file_name { get; set; } = null!;
        public string? path_file { get; set; } = null!;
        public int? id_tipo_documento { get; set; }
        public string? content_type { get; set; } = null!;
        public string? owner_name { get; set; } = null!;
        public bool estatus { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public bool activo { get; set; }
        public string? size { get; set; }
        public int id_administracion { get; set; }
        public string? usuario_creacion { get; set; } = null!;
        public string? usuario_modificacion { get; set; } = null!;
        public bool permanente { get; set; }
        public bool remplazable { get; set; }
        public int? id_documento_seccion { get; set; }
        public string? noAsunto { get; set; } = null!;
    }
}