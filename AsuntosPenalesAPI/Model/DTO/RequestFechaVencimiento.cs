using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestFechaVencimiento
    {
        public string NumeroAsuntoPenal { get; set; } = null!;
        public int? idAdminControla { get; set; }

    }
}