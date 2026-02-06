using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateEtapaInvestigacionComplementaria
    {
        public int id { get; set; }
        public int idAsuntoPenal { get; set; }


        public string? fechaPlazoInvestigacionComplementaria { get; set; }
        public bool procedimientoAbreviado { get; set; }
        public bool escritoAcusacion { get; set; }
        public string? fechaEscritoAcusacion { get; set; }

        #region Sentencia
        public int? idTipoSentencia { get; set; }
        public string? fechaEmisionSentencia { get; set; }
        public decimal? reparacionDañoSentencia  { get; set; }
        public bool? cumplimientoPrivadaLibertadSentencia  { get; set; }
        public int? idAnioSentencia  { get; set; }
        public int? idMesSentencia  { get; set; }
        public int? idDiaSentencia  { get; set; }
        public string? otorgamientoBeneficiosSentencia  { get; set; }
        public string? accionesEjecucionSentencia  { get; set; }
        public string? fechaEjecucionSentencia  { get; set; }        
        public bool? conclusionAsuntoSentencia  { get; set; }
        public string? fechaConclusionSentencia  { get; set; }
        #endregion

        public bool? solucionAlterna { get; set; }
        public int? idTipoSolucionAlterna { get; set; }

        #region Acuerdo Reparatorio
        public string? condicionesAcuerdoReparatorio { get; set; }
        public string? fechaAutorizacionAcuerdoReparatorio { get; set; }
        public decimal? reparacionDañoAcuerdoReparatorio { get; set; }
        public string? fechaCelebracionAcuerdoReparatorio { get; set; }
        public bool? conclusionAsuntoAcuerdoReparatorio { get; set; }
        public string? fechaConclusionAcuerdoReparatorio { get; set; }
        #endregion

        #region Sobreseimineto
        public bool? solicitudSobreseimiento { get; set; }
        public string? fechaDeterminacionSobreseimiento { get; set; }
        public bool? conclusionAsuntoSobreseimiento  { get; set; }
        public string? fechaConclusionSobreseimiento  { get; set; }
        #endregion

        #region Suspension
        public string? condicionesSuspension { get; set; }
        public string? fechaCelebracionSuspension { get; set; }
        public decimal? reparacionDañoSuspension { get; set; }
        public string? fechaPlazoSuspension { get; set; }
        public string? fechaCumplimientoSuspension { get; set; }        
        public bool? conclusionAsuntoSuspension { get; set; }
        public string? fechaConclusionSuspension { get; set; }

        #endregion
        
        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
}