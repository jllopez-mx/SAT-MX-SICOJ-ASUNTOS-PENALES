using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateAsuntosPenalesAdministrador
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string oficio_solicitud { get; set; } = null!;
        [Required]
        public string fecha_recepcion { get; set; } = null!;
        public string fecha_vencimiento { get; set; } = null!;
        [Required]
        public string numero_expediente_cadido {get; set;} = null!;
        [Required]
        public int id_unidad_realiza_solicitud {get; set; }
        public int id_unidad_administrativa {get; set;}
        public int id_subadministracion {get; set;}
        [Required]
        public bool interno {get; set;}
        public string nombre_abogado {get; set;} = null!;        
        public int id_tipo_edicion { get; set; }
    }
}