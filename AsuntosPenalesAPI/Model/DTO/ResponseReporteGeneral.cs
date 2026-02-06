using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseReporteGeneral
    {
        
    public string UnidadAdministrativa { get; set; }
    public string Subadministracion { get; set; }
    public string UnidadRealizaSolicitud { get; set; }
    public string EstadoProcesal { get; set; }
    public string NumeroAsunto { get; set; }
    public string FechaRecepcion { get; set; }
    public string NombreAbogado { get; set; }
    public string DeterminacionAsunto { get; set; }
    public string FechaDeterminacion { get; set; }
    public string RequisitoProcedibilidad { get; set; }
    public string FechaPresentacionRequisito { get; set; }
    public string PersonaMoral { get; set; }
    public string ContribuyentePersona { get; set; }
    public string RfcPersonaMoral { get; set; }
    public string Delito { get; set; }
    public decimal? Cuantia { get; set; }
    public string NumeroCarpetaInvestigacion { get; set; }
    public string AgenteMinisterioPublico { get; set; }
    public string NombreImputado { get; set; }
    public string ContribuyenteImputado { get; set; }
    public string RfcImputado { get; set; }
    public string FormaTerminacionInvestigacion { get; set; }
    public string FechaTerminacionInvestigacion { get; set; }
    public string SolucionAlternaIn { get; set; }

    public string CondicionesArInicial { get; set; }
    public string FechaAutorizacionArInicial { get; set; }
    public string ReparacionDanioArInicial { get; set; }
    public string FechaReparacionArInicial { get; set; }
    public string FechaConclusionArInicial { get; set; }

    public string CondicionesCo { get; set; }
    public string FechaCriterioOportunidad { get; set; }
    public string FechaConclusionCo { get; set; }

    public string FechaSolicitudAudienciaInicial { get; set; }
    public string CentroJusticia { get; set; }
    public string CausaPenal { get; set; }
    public string FechaAudienciaInicial { get; set; }
    public string FechaAutoVinculacion { get; set; }
    public string FechaOrdenAprehension { get; set; }

    public string TipoSentenciaComplementaria { get; set; }
    public string FechaEmisionSentenciaComplementaria { get; set; }
    public string ReparacionDanioSenComplementaria { get; set; }
    public string CumplimientoLibertadComplementaria { get; set; }
    public int? AniosSenComplementaria { get; set; }
    public int? MesesSenComplementaria { get; set; }
    public int? DiasSenComplementaria { get; set; }
    public string OtorgamientoBeneficiosComplementaria { get; set; }
    public string AccionesEjecucionComplementaria { get; set; }
    public string FechaEjecucionComplementaria { get; set; }
    public string ConclusionAsuntoComplementaria { get; set; }
    public string FechaConclusionSenComplementaria { get; set; }

    public string SolucionAlternaCom { get; set; }
    public string CondicionesArComplementaria { get; set; }
    public string FechaAutorizacionArComplementaria { get; set; }
    public string ReparacionDanioArComplementaria { get; set; }
    public string FechaReparacionArComplementaria { get; set; }
    public string FechaConclusionArComplementaria { get; set; }

    public string FechaDeterminacionSobreseimientoCom { get; set; }
    public string SolicitudSobreseimientoCom { get; set; }
    public string FechaConclusionSobreseimientoCom { get; set; }

    public string CondicionesSuspensionComplementariaCom { get; set; }
    public string FechaCelebracionSuspensionComplementariaCom { get; set; }
    public string ReparacionDanioSuspensionComplementariaCom { get; set; }
    public int? PlazoSuspensionComplementariaCom { get; set; }
    public string FechaCumplimientoSuspensionComplementariaCom { get; set; }
    public string FechaConclusionSuspensionComplementariaCom { get; set; }

    public string FechaEscritoAcusacionCp { get; set; }
    public string FechaAudienciaIntermedia { get; set; }
    public string FechaAperturaJuicioOral { get; set; }

    public string TipoSentenciaIntermedia { get; set; }
    public string FechaEmisionSentenciaIntermedia { get; set; }
    public string ReparacionDanioSenIntermedia { get; set; }
    public string CumplimientoLibertadIntermedia { get; set; }
    public int? AniosSenIntermedia { get; set; }
    public int? MesesSenIntermedia { get; set; }
    public int? DiasSenIntermedia { get; set; }
    public string OtorgamientoBeneficiosIntermedia { get; set; }
    public string AccionesEjecucionIntermedia { get; set; }
    public string FechaEjecucionIntermedia { get; set; }
    public string ConclusionAsuntoIntermedia { get; set; }
    public string FechaConclusionSenIntermedia { get; set; }

    public string SolucionAlternaInt { get; set; }
    public string CondicionesArIntermedia { get; set; }
    public string FechaAutorizacionArIntermedia { get; set; }
    public string ReparacionDanioArIntermedia { get; set; }
    public string FechaReparacionArIntermedia { get; set; }
    public string FechaConclusionArIntermedia { get; set; }

    public string FechaDeterminacionSobreseimientoInt { get; set; }
    public string FechaConclusionSobreseimientoInt { get; set; }

    public string CondicionesSuspensionComplementariaInt { get; set; }
    public string FechaCelebracionSuspensionComplementariaInt { get; set; }
    public string ReparacionDanioSuspensionComplementariaInt { get; set; }
    public int? PlazoSuspensionComplementariaInt { get; set; }
    public string FechaCumplimientoSuspensionComplementariaInt { get; set; }
    public string FechaConclusionSuspensionComplementariaInt { get; set; }

    public string FechaInicialAudienciaJuicio { get; set; }
    public string FechaFinalAudienciaJuicio { get; set; }
    public string TipoSentenciaJuicio { get; set; }
    public string FechaEmisionSentenciaJuicio { get; set; }
    public string ReparacionDanioSenJuicio { get; set; }
    public string CumplimientoLibertadJuicio { get; set; }
    public int? AniosSenJuicio { get; set; }
    public int? MesesSenJuicio { get; set; }
    public int? DiasSenJuicio { get; set; }
    public string OtorgamientoBeneficiosJuicio { get; set; }
    public string AccionesEjecucionJuicio { get; set; }
    public string FechaEjecucionJuicio { get; set; }
    public string ConclusionAsuntoJuicio { get; set; }
    public string FechaConclusionSenJuicio { get; set; }

    public string FechaPresentacionApelacion { get; set; }
    public string NumeroTocaPenal { get; set; }
    public string TipoOrganoJurisdiccionalApelacion { get; set; }
    public string ResolucionApelacion { get; set; }
    public string FechaResolucionApelacion { get; set; }

    public string TipoAmparo { get; set; }
    public string FechaPresentacionAmparo { get; set; }
    public string NumeroJuicioAmparo { get; set; }
    public string TipoOrganoJurisdiccionalAmparo { get; set; }
    public string Juzgado { get; set; }
    public string ResolucionAmparo { get; set; }
    public string FechaResolucionAmparo { get; set; }
    public string FechaNotificacionAmparo { get; set; }
    }
}