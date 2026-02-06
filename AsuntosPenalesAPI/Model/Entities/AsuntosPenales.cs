using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenales
    {
        public int id { get; set; }
        public string? numero_asunto_penal { get; set;} = null!;
        public DateTime fecha_recepcion {get; set; }
        public DateTime fecha_vencimiento { get; set;}
        public string oficio_solicitud {get; set; } = null!;
        public string? numero_expediente_cadido { get; set;}
        public int? id_unidad_realiza_solicitud {get; set; }
        public int? id_estado_tarea {get; set; }
        public int? id_estado_procesal {get; set; }
        public int? id_admin_controla {get; set; }
        public int? id_administracion_central {get; set;}
        public DateTime? fecha_turnado {get; set; }
        public string? nombre_abogado {get;set;} = null!;
        public bool activo { get; set; }
        public string? numero_empleado { get; set;}
        public string? numero_empleado_adm {get; set;} = null!;
        public DateTime fecha_asignacion {get; set;}
        public bool turnado { get; set; }
        public string? usuario_modificacion {get;set;} = null!;
        public AsuntosPenalesAbogado abogado { get; set; } = null!;

        public bool? interno { get; set; }
        public int? id_subadministracion { get; set; }
        public DateTime? fecha_control_solicitudes { get; set; }
        public bool? requerimiento { get; set; }


    }
}