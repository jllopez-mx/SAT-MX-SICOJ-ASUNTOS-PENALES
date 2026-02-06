using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesCriterioOportunidad
    {
        
    public int id { get; set; }
    public int id_imputado { get; set; }
    public int id_tipo_etapa_investigacion { get; set; }  
    public string condiciones { get; set; } = null!;
    public DateTime fecha_criterio_oportunidad { get; set; }
    public bool conclusion_asunto {get; set;}
    public DateTime? fecha_conclusion { get; set; }
    public DateTime fecha_creacion { get; set; }
    public string usuario_creacion { get; set; } = null!;
    public bool activo { get; set; }
    public string? usuario_modificacion { get; set; } = null!;
    public DateTime? fecha_modificacion { get; set; }

    }
}