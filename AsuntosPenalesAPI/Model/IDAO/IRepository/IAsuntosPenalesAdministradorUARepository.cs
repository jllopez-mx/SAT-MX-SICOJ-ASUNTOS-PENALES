using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesAdministradorUARepository
    {
        Task<List<ResponseAsuntosPenalesAdministradorUAByFilters>> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool v, string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad);
        Task<int?> GetHistoricoCountAsync(string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad);
        Task<List<AsuntosPenales>> GetListByIdsAsync(int[] ids);
        Task<AsuntosPenales> GetByIdAsync(int id);
        Task<ResponseAsuntosPenalesByIdAdminUA> GetAdministradorUAsuntoPenalById(
            int id_asuntopenal
        );
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

        Task<ResultTransaction> RemitirAsync(
            AsuntosPenales entity,
            AsuntosPenalesRemision entityRemision,
            ArchivosAsuntosPenales? entityDocumento,
            DataFile? dataFile
        );
    }
}