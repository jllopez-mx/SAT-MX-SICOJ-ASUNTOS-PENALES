using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseDelitos
    {       
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public int id_delito { get; set; }
        public string codigo { get; set; } = null!;
        public string delito { get; set; } = null!;
    }
}