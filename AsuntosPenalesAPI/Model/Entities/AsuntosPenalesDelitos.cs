using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesDelitos
    {
    public int id { get; set; }
    public int id_asunto_penal { get; set; }  
    public int id_delito {get; set;}
    public DateTime fecha_creacion { get; set; }
    public string usuario_creacion { get; set; } = null!;
    public bool activo { get; set; }   
    }
}