using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestDelitos
    {
        public int idAsuntoPenal {get; set;}
        public List<int> id_delito {get; set;} = null!;

        
    }
}