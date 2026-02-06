using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponsePersonasMorales
    {
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool contribuyente { get; set; } 

    }
}