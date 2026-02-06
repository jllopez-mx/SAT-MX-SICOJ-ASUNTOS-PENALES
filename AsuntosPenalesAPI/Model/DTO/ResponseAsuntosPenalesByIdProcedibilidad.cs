using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseAsuntosPenalesByIdProcedibilidad
    {
        public int id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string? requisito_procedibilidad {get; set; }
        public int? id_requisito_procedibilidad {get; set;}
        public string numero_oficio_procedibilidad{get; set;} = null!;
        public string fecha_presentacion {get; set;} = null!;
        public decimal cuantia { get; set; }
        public string numero_carpeta_investigacion { get; set; } = null!;
        public string agente_ministerio_publico  { get; set; } = null!;
    }
}