using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateApelacion
    {
        public int idAsuntoPenal { get; set; }
        public int id {get; set; }
        public int idImputado { get; set; }	   
        public string? fechaPresentacion { get; set; }
        public string? numeroTocaPenal { get; set; }
        public string? TipoOrganoJurisdiccional { get; set; }
        public int idTipoResolucion { get; set; }
        public string? fechaResolucion { get; set; }
        public string? descripcionResolucion { get; set; }



    }
}