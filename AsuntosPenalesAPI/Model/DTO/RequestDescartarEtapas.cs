using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestDescartarEtapas
    {
        public int idAsuntoPenal { get; set; }
        public int idImputado { get; set; }

    }
}