using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesReasignar
    {
        public int id {get; set;}

        public int id_asunto_penal_reasignado{get; set;}
        public DateTime fecha_reasignacion {get; set;} 
        public string rfc_funcionario_reasignador {get; set;} = null!;
        public string rfc_funcionario_retirado {get; set;} = null!;
        public string rfc_funcionario_reasignado {get; set;} = null!;
        public int id_unidad_administrativa_reasignador { get; set; }
        public int id_subadministracion_reasignador { get; set; }
        public int id_unidad_administrativa_reasignado { get; set; }
        public int id_subadministracion_reasignado { get; set; }
        public int? id_estado_procesal_previo { get; set; }
        public int? id_estado_procesal_nuevo {get; set;}
        public int? id_estado_tarea_nuevo {get; set;}
        public string tipo_movimiento {get; set;} = null!;
    }
}