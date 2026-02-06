using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateEtapaInvestigacionJuicio
    {
          public int id { get; set; }
        public int idAsuntoPenal { get; set; }

        public string? fechaInicialAudienciaJuicio { get; set; } 	
	    public string? fechaFinalAudienciaJuicio { get; set; }

        #region Sentencia
        public int? idSentencia { get; set; }
        public string? fechaEmisionSentencia { get; set; }
        public decimal? reparacionDañoSentencia  { get; set; }
        public bool? cumplimientoPrivadaLibertadSentencia  { get; set; }
        public int? idAnioSentencia  { get; set; }
        public int? idMesSentencia  { get; set; }
        public int? idDiaSentencia  { get; set; }
        public string? otorgamientoBeneficiosSentencia  { get; set; } = null!;
        public string? accionesEjecucionSentencia  { get; set; } = null!;
        public string? fechaEjecucionSentencia  { get; set; }        
        public bool? conclusionAsuntoSentencia  { get; set; }
        public string? fechaConclusionSentencia  { get; set; }
        #endregion

        public bool solucionAlterna { get; set; }
        public int? idTipoSolucionAlterna { get; set; }
        
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
}