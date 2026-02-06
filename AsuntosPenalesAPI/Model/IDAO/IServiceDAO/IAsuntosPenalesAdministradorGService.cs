using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IServiceDAO
{
    public interface IAsuntosPenalesAdministradorGService
    {

        Task<ResultOperation> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool v, string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad, UserInformationView userInformationView);
        Task<ResultOperation<ResponseReactivar>> ReactivarAsync(AsuntosPenales entity, AsuntosPenalesAbogado entityAbogado);
        Task<List<AsuntosPenalesHistoricoImputados>> GetHistoricoImputados(int id_imputado);
        Task<AsuntosPenales> GetAsuntoPenalById(int id);
        Task<ResultOperation> GetAdminAsuntoPenalById(int id);
        Task<ResultOperation> UpdateFechaVencimientoAsync(AsuntosPenales entity);
        Task<ResultOperation> GetRegistroFechasAsync(int fetch, int page, string? orderByColumn, bool v,string? numeroasuntopenal, int? admin_control,  UserInformationView userInformationView);

        Task<List<ResponseReporteGeneral>> ExportarReporteGeneral
        (
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