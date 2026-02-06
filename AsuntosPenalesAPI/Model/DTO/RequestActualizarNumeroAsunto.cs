using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestActualizarNumeroAsunto
    {
        public int id { get; set; }
        public string numeroAsunto { get; set; } = null!;
        public int idModulo { get; set; }
        public int idTipoAsunto { get; set; }
    }
}