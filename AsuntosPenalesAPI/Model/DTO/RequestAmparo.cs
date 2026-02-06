using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestAmparo
    {
        public int idAsuntoPenal { get; set; }
        public int idImputado { get; set; }	   
        public int idTipoAmparo { get; set; }	   
        public int idEstadoProcesal { get; set; }	 
        public string? fechaPresentacion { get; set; }
        public string? numeroJuicioAmparo { get; set; }
        public string? TipoOrganoJurisdiccional { get; set; }	 
        public string? juzgado { get; set; }
        public int idTipoResolucion { get; set; }
        public string? fechaResolucion { get; set; }
        public string? descripcionResolucion { get; set; }
        public string? fechaNotificacion { get; set; }
        public bool alegatos { get; set; }
        public string? numeroOficio { get; set; }
	    public string? fechaOficio { get; set; }

        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }

    }
}