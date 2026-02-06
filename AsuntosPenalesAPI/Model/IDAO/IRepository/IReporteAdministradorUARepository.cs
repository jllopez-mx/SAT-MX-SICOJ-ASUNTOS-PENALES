using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IReporteAdministradorUARepository
    {
        Task<List<ResponseReporteGeneral>> ExportarReporteGeneralAsyncRepository(
           string? noAsunto,
                DateTime? fechaRecepcionDesde,
                DateTime? fechaRecepcionHasta,
                DateTime? fechaVencimientoDesde,
                DateTime? fechaVencimientoHasta,
                string? oficioSolicitud,
                string? NoExpedienteCadido,
                List<int>? UnidadRealizaSolicitud,
                int? idAdministracion,
                int? idSubadministracion,
                string? idAbogadoAsigno,
                List<int>? EstadoTarea,
                List<int>? EstadoProcesal,
                List<int>? TipoConclusion,
                DateTime? fechaConclusionDesde,
                DateTime? fechaConclusionHasta,
                List<int>? DeterminacionAsunto,
                List<int>? RequisitosProcedibilidad,
                List<int>? Delito,
                int? TipoSolucionAlterna,
                DateTime? FechaPresentacionRequisito,
                DateTime? FechaDelAutoVinculacion,
                DateTime? FechaEmisionSentencia

           );
    }
}