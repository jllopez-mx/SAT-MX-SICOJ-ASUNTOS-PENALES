using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestCreateControlDocumental
    {
        public string NumeroAsuntoPenal { get; set; } = null!;
        [Required]
        public string fecha_recepcion_solicitud { get; set; } = null!;

        [Required]
        public string oficio_solicitud { get; set; } = null!;

        public int? id_unidad_realiza_solicitud { get; set; }
        
    }
}