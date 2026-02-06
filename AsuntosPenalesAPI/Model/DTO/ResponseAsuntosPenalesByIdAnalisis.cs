using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAsuntosPenalesByIdAnalisis
    {
        public int id { get; set; }
        public int idAsuntoPenal { get; set; }
        public bool requerimiento { get; set; }
        public string oficio_requerimiento {get; set;} = null!;
        public string fecha_requerimiento {get; set;} = null!;
        public bool requerimiento_atendido { get; set; }
        public string oficio_atencion {get; set;}  = null!;
        public string fecha_atencion {get; set;} = null!;
        public string? determinacion_asunto_penal {get; set;}
        public int? id_determinacion_asunto {get; set;}
        public string fecha_determinacion {get; set;} = null!; 
        public string? estadoTarea {get; set;}
        public int ? idEstadoTarea {get; set;}
        public string? estadoProcesal {get; set;}
        public int? idEstadoProcesal {get; set;}





    }
}




