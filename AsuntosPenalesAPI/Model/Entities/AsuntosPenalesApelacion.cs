using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesApelacion
    {
        public int id { get; set; }	   
        public int id_imputado { get; set; }	   
        public DateTime fecha_presentacion { get; set; } 
        public string? numero_toca_penal { get; set; }
        public string? tipo_organo_jurisdiccional { get; set; }
        public int? id_tipo_resolucion { get; set; }
        public DateTime fecha_resolucion { get; set; }
        public string? descripcion_resolucion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public bool activo { get; set; }
        public string? usuario_modificacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }


    }
}
