using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesSentencia
    {
        public int id { get; set; }
        public int id_imputado { get; set; }
        public int id_tipo_etapa_investigacion { get; set; }  
        public int? id_tipo_sentencia { get; set; }
        public DateTime fecha_emision_sentencia { get; set; }
        public decimal? reparacion_daño { get; set; }
        public bool cumplimiento_privada_libertad { get; set; }
        public int? id_anio { get; set; }
        public int? id_mes { get; set; }
        public int? id_dia { get; set; }
        public string otorgamiento_beneficios { get; set; } = null!;
        public string acciones_ejecucion { get; set; } = null!;
        public DateTime fecha_ejecucion { get; set; }        
        public bool conclusion_asunto { get; set; }
        public DateTime? fecha_conclusion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;        
        public string? usuario_modificacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
    }
}