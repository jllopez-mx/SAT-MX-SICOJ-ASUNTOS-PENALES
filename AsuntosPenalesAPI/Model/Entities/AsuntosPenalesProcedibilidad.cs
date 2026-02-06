using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesProcedibilidad
    {
    public int id { get; set; }
    public int id_asunto_penal { get; set; }
    public int id_requisito_procedibilidad {get; set; }
    public string numero_oficio_procedibilidad {get; set; } = null!;
    public DateTime fecha_presentacion {get; set; }
    public decimal cuantia {get; set;}
    public string numero_carpeta_investigacion {get; set;} = null!;
    public string  agente_ministerio_publico {get; set;} = null!;
    public DateTime fecha_creacion { get; set; }
    public string usuario_creacion { get; set; } = null!;
    public bool activo { get; set; }
    public string usuario_modificacion { get; set; } = null!;
    public DateTime fecha_modificacion { get; set; }
    
    public int? id_estado_procesal { get; set; }

    }
}