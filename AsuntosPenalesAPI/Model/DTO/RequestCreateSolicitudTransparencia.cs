using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestCreateSolicitudTransparencia
    {
        [Required]
        public int idAsunto { get; set; }

        [Required]
        public string noSolicitud { get; set; }   =null!;     

       [Required]
        public string fechaSolicitud { get; set; } =null!;
        public IFormFile? documento { get; set; }
        public string noFolio { get; set; } =null!;
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
}