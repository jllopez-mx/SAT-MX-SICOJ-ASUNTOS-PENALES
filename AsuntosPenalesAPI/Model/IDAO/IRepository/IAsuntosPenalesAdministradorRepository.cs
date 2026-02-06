using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesAdministradorRepository
    {
        Task<List<ResponseAsuntosPenalesAdministradorByFilters>> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral, int? idAdministracion);
        Task<int?> GetBandejaCountAsync(int? idAdminCentral, int? idAdministracion);

        Task<ResponseAsuntosPenalesByIdAdmin> GetAdminAsuntoPenalById(int id_asuntopenal);
        Task<ResultTransaction> UpdateAsuntosPenalesAdminAsync(AsuntosPenales entity);
        Task<ResultTransaction> AsignarAsuntosPenalesAsync(AsuntosPenales entity, AsuntosPenalesAbogado entityAbogado);
        Task<List<ResponseAsuntosPenalesAdministradorByFilters>> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool v, string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad);
        Task<int?> GetHistoricoCountAsync(string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad);
        Task<ResponseAsuntosPenalesReasignados> GetAsuntosPenalesReasignadosCountAsync(int[] idasuntopenal, string? funcionario_reasignado);
        Task<ResultTransaction> RemitirAsync(AsuntosPenales entity, AsuntosPenalesRemision entityRemision, ArchivosAsuntosPenales? entityDocumento, DataFile? dataFile);

        Task<ResultTransaction> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales? entityDocumento,
            DataFile? dataFile
        );
        Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdDisconnectedAnalisisAsync(
            int id
        );
        Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id);
        Task<ResultTransaction> UpdateAnalisisAsync(AsuntosPenalesAnalisis entityAnalisis);
        Task<ResultTransaction> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdDisconnectedProcedibilidadAsync(
            int id
        );
        Task<List<AsuntosPenalesImputados>> GetImputados(int idAsuntosPenales);
        Task<ResultTransaction> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );
        Task<ResultTransaction> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        );
        Task<List<ResponsePersonasMorales>> GetPersonasMoralesDisconnected(int idAsuntosPenales);
        Task<ResultTransaction> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos);
        Task<AsuntosPenalesDelitos> GetByIdDelitosAsync(int id);
        Task<ResultTransaction> DeleteDelitos(AsuntosPenalesDelitos entity);
        Task<List<ResponseDelitos>> GetDelitosDisconnected(int idAsuntosPenales);
        Task<ResponseImputadosEtapaInicial> GetImputadoByIdDisconnectedEtapaInicial(int id);
        Task<List<ResponseAcuerdoReparatorio>> GetAcuerdoReparatorioByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ResponseCriterioOportunidad>> GetCriterioOportunidadByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<ResultTransaction> UpdateImputadosEtapaInicialAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<ResponseImputadosEtapaComplementaria> GetImputadoByIdDisconnectedEtapaComplementaria(
            int id
        );

        Task<List<ResponseSentencia>> GetSentenciaByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<List<ResponseSobreseimiento>> GetSobreseimientoByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<List<ResponseSuspencionCondicional>> GetSuspencionCondicionalByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );

        Task<ResultTransaction> UpdateImputadosEtapaComplementariaAsync(
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

        Task<ResponseImputadosEtapaIntermedia> GetImputadoByIdDisconnectedEtapaIntermedia(int id);

        Task<ResultTransaction> UpdateImputadosEtapaIntermediaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<ResponseImputadosEtapaJuicio> GetImputadoByIdDisconnectedEtapaJuicio(int id);

        Task<ResultTransaction> UpdateImputadosEtapaJuicioAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<ResultTransaction> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        );

        Task<List<ResponseMedidasCautelares>> GetMedidasCautelaresDisconnected(int idImputado);
        Task<AsuntosPenalesMedidasCautelares> GetByIdMedidasCautelaresAsync(int id);
        Task<ResultTransaction> DeleteMedidasCautelares(AsuntosPenalesMedidasCautelares entity);
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

        #region Solicitud-Transparencia

        Task<ResultTransaction> AddAsyncSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile);
        Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaRepository(int id);
        Task<ResultTransaction> UpdateSolicitudTransparenciaRepository(SolicitudTransparencia entity);
        Task<ResponseSolicitudTransparenciaList> GetByIdSolicitudTransparenciaRepositorys(int id);
        Task<int?> GetTablaSolicitudTransparenciaCountRepository(int idAsunto);
        Task<List<ResponseSolicitudTransparenciaList>> GetTablaSolicitudTransparenciaRepository(int idAsunto);
        Task<ResultTransaction> DeleteSolicitudTransparenciaRepository(int id);

        #endregion
    }
}