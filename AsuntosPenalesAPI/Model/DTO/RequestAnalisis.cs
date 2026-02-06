using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestAnalisis
    {
        public int idAsuntoPenal { get; set; }
        public bool requerimiento { get; set; }
        public string? oficioRequerimiento { get; set; } = null!;
        public string? fechaRequerimiento { get; set; }        
        public bool? requerimientoAtendido { get; set; }
        public string? oficioAtencion { get; set; } = null!;
        public string? fechaAtencion { get; set; }
        public int idDeterminacionAsuntoPenal { get; set; } 
        public string fechaDeterminacion { get; set;}  = null!;
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }

    }
}