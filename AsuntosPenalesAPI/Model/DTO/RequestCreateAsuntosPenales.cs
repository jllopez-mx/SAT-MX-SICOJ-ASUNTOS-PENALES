using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestCreateAsuntosPenales
    {
        [Required]
        public string fecha_recepcion { get; set; } = null!; 
        
        [Required]
        public string fecha_vencimiento { get; set; } = null!; 

        [Required]
        public string oficio_solicitud { get; set; } = null!; 

        [Required]
        public string? numero_expediente_cadido { get; set; }
        
        public int? id_unidad_realiza_solicitud { get; set; } //UNIDAD ADMINISTRATIVA

        [Required]
        public int? id_admin_controla  { get; set; }
        [Required]
        public bool interno { get; set; } 
  
    }
}