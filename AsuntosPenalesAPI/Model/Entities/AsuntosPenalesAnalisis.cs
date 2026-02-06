
namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesAnalisis
    {
    
    public int id { get; set; }
    public int id_asunto_penal { get; set; }
    public int id_estado_procesal {get; set; }
    public bool requerimiento { get; set; } 
    public string? oficio_requerimiento { get; set; } 
    public DateTime? fecha_requerimiento { get; set; }
    public bool? requerimiento_atendido{ get; set; }
    public string? oficio_atencion { get; set; }
    public DateTime? fecha_atencion { get; set; }
    public int id_determinacion_asunto_penal { get; set; }
    public DateTime? fecha_determinacion { get; set; }
    public DateTime fecha_creacion { get; set; }
    public string usuario_creacion { get; set; } = null!;
    public bool activo { get; set; }
    public string usuario_modificacion { get; set; } = null!;
    public DateTime fecha_modificacion { get; set; }

    }
}