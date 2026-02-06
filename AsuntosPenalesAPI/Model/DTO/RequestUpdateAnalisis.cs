using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateAnalisis
    {
        public int id { get; set; }     
        public int idAsuntoPenal { get; set; }
        public bool requerimiento { get; set; }
        public string? oficioRequerimiento { get; set; } = null!;
        public string? fechaRequerimiento { get; set; } = null!;      
        public bool? requerimientoAtendido { get; set; }
        public string? oficioAtencion { get; set; } = null!;
        public string? fechaAtencion { get; set; }  = null!;
        public int idDeterminacionAsuntoPenal { get; set; } 
        public string fechaDeterminacion { get; set;}  = null!;

    }
}

