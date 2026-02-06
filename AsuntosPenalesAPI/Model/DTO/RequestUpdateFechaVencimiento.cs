using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateFechaVencimiento
    {
                public int idAsuntoPenal { get; set; }
                public string fechaVencimiento { get; set; } = null!; 

    }
    
}
