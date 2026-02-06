using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesHistoricoImputados
    {
        public int? id { get; set; } 
        public int? id_imputado { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int? id_estado_procesal { get; set; }

    }
}
