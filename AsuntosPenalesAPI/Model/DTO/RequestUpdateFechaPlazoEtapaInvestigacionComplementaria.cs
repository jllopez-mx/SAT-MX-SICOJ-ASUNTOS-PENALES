using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateFechaPlazoEtapaInvestigacionComplementaria
    {
        public int id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string? fechaPlazoInvestigacionComplementaria { get; set; }

        
    }
}