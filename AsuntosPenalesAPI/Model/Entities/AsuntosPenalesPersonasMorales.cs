using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesPersonasMorales
    {
    public int id { get; set; }
    public int id_asunto_penal { get; set; }
    public string rfc { get; set; } = null!;
    public bool contribuyente { get; set; }
    public string nombre { get; set; } = null!;
    public DateTime fecha_creacion { get; set; }
    public string usuario_creacion { get; set; } = null!;
    public bool activo { get; set; }
    }
}