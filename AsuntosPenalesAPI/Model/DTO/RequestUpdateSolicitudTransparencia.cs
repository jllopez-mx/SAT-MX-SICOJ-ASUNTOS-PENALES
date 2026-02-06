using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateSolicitudTransparencia
    {
        [Required]
        public int id { get; set; }

        [Required]
        public int idAsunto { get; set; }

        [Required]
        public string noSolicitud { get; set; }   =null!;     

       [Required]
        public string fechaSolicitud { get; set; } =null!;
    }
}