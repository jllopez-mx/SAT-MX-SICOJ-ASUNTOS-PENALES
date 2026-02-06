
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;
using AsuntosPenalesAPI.Model.DTO;

namespace AsuntosPenalesAPI.Model.IDAO.IServiceDAO
{
    public interface IAsuntosPenalesAdministradorService
    {
        Task<ResultOperation> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral, int? idAdministracion);
        Task<ResultOperation> GetAdminAsuntoPenalById(int id);
        Task<AsuntosPenales> GetAsuntoPenalById(int id);
        Task<ResultOperation<int>> UpdateAsuntoPenalAdministrador(AsuntosPenales entity);
        Task<ResultOperation<ResponseAsignar>> UpdateAsignarAsuntosPenales(AsuntosPenales entity, AsuntosPenalesAbogado entityAbogado);
        Task<ResultOperation> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool v, string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad);
        Task<ResultOperation> RemitirAsync(AsuntosPenales entity, AsuntosPenalesRemision entityRemision, ArchivosAsuntosPenales entityDocumento, DataFile dataFile);
        Task<ResultOperation<int>> AddArchivosAsyncService(ArchivosAsuntosPenales entity, DataFile dataFile);
        Task<ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(int id_asuntospenales);
        Task<ArchivosAsuntosPenales> GetByIdArchivoService(int id);
        Task<ArchivosAsuntosPenales> GetByIdArchivoDeleteService(int id);
        Task<ResultOperation<List<ResponseRemision>>> GetAllRemision(int idAsuntoPenal);
        Task<ResultOperation> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales entityDocumento,
            DataFile dataFile
        );
        Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdAnalisisDisconnected(int id);
        Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id);
        Task<ResultOperation> UpdateAnalisis(AsuntosPenalesAnalisis entityAnalisis);
        Task<ResultOperation> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdProcedibilidadDisconnected(
            int id
        );
        Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal);
        Task<ResultOperation> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );
        Task<ResultOperation> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        );
        Task<ResultOperation<List<ResponsePersonasMorales>>> GetAllPersonasMorales(
            int idAsuntoPenal
        );
        Task<ResultOperation> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos);
        Task<AsuntosPenalesDelitos> GetDelitoById(int id);
        Task<ResultOperation> DeleteDelitos(AsuntosPenalesDelitos entity);
        Task<ResultOperation<List<ResponseDelitos>>> GetAllDelitos(int idAsuntoPenal);
        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaInicial(int id);
        Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ArchivosAsuntosPenales>> GetArchivosByIdRenglonSeccion_Async_Repository(
            int id_asuntospenales,
            int id_seccion,
            int id_renglonseccion,
            int id_tipo_documento
        );
        Task<ResultOperation> UpdateImputadosEtapaInicialAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaComplementaria(int id);

        Task<ResultOperation> UpdateImputadosEtapaComplementariaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<List<AsuntosPenalesSentencia>> GetSentenciaByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<List<AsuntosPenalesSobreseimiento>> GetSobreseimientoByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<List<AsuntosPenalesSuspensionCondicional>> GetSuspencionCondicionalByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaIntermedia(int id);

        Task<ResultOperation> UpdateImputadosEtapaIntermediaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );
        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaJuicio(int id);

        Task<ResultOperation> UpdateImputadosEtapaJuicioAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );
        Task<ResultOperation> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        );

        Task<ResultOperation<List<ResponseMedidasCautelares>>> GetAllMedidasCautelares(
            int idImputado
        );

        Task<AsuntosPenalesMedidasCautelares> GetMedidaCautelarById(int id);
        Task<ResultOperation> DeleteMedidasCautelares(AsuntosPenalesMedidasCautelares entity);

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

        Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile);
        Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaService(int id);
        Task<ResultOperation> UpdateSolicitudTransparenciaService(SolicitudTransparencia entity);
        Task<ResultOperation> GetByIdSolicitudTransparenciaServices(int id);
        Task<ResultOperation> GetTablaSolicitudTransparenciaService(int idAsunto);
        Task<ResultOperation> DeleteSolicitudTransparenciaService(SolicitudTransparencia entity);
    }
}