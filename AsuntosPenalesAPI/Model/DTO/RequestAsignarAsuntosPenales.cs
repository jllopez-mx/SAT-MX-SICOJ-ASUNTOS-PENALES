using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestAsignarAsuntosPenales
    {
        public int id {get; set;}

        public string rfc_abogado {get; set;} = null!;


    }
}