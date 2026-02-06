using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesMedidasCautelares
    {
        
        public int id { get; set; }
        public int id_imputado { get; set; }
        public int id_medida {get; set; } 
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;
        public bool activo { get; set; }

    }
}