using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseReasignarAdministrador
    {
        public int ReasignacionesExitosas { get; set; }
        public int ReasignacionesIncorrectas { get; set; }
        public string administrador { get; set; } = null!;
    }
}
