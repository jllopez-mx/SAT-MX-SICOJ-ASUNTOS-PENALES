using AsuntosPenalesAPI.Model.DAO;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.ViewModels;
using Polly.Timeout;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesAbogadoRepository
    {
        Task<List<ResponseAsuntosPenalesAbogadoByFilters>> GetBandejaAsync(
            int Fetch,
            int Page,
            string? OrderByColumn,
            bool OrderDesc,
            int? idAdminCentral,
            string? rfc_abogado
        );

        Task<List<ResponseAsuntosPenalesAbogadoByFilters>> GetHistoricoAsync(
            int fetch,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string numeroasuntopenal,
            DateTime? fecharecepcion_desde,
            DateTime? fecharecepcion_hasta,
            DateTime? fechavencimiento_desde,
            DateTime? fechavencimiento_hasta,
            string oficiosolicitud,
            string numeroexpedientecadido,
            int? id_unidadrealizasolicitud,
            int? admin_control,
            int? id_subadministracion,
            string? nombre_abogado,
            int? id_estadotarea,
            int? id_estadoprocesal,
            int? id_administracion_adscrita,
            bool? tipoUnidad
        );

        Task<int?> GetHistoricoCountAsync(
            string numeroasuntopenal,
            DateTime? fecharecepcion_desde,
            DateTime? fecharecepcion_hasta,
            DateTime? fechavencimiento_desde,
            DateTime? fechavencimiento_hasta,
            string oficiosolicitud,
            string numeroexpedientecadido,
            int? id_unidadrealizasolicitud,
            int? id_admin_controla,
            int? id_subadministracion,
            string? nombre_abogado,
            int? id_estadotarea,
            int? id_estadoprocesal,
            int? id_administracion_adsc,
            bool? tipoUnidad
        );

        Task<int?> GetBandejaCountAsync(int? idAdminCentral, string? rfc_abogado);
        Task<ResponseAsuntosPenalesByIdAdmin> GetAbogadoAsuntoPenalById(
            int id_asuntopenal,
            string? rfc_abogado
        );
        Task<ResultTransaction> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales? entityDocumento,
            DataFile? dataFile
        );
        Task<ResultTransaction> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<ResultTransaction> UpdateProcedibilidadAsync(
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<ResultTransaction> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        );
        Task<ResultTransaction> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        );
        Task<ResultTransaction> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos);
        Task<List<ResponsePersonasMorales>> GetPersonasMoralesDisconnected(int idAsuntosPenales);
        Task<List<ResponseDelitos>> GetDelitosDisconnected(int idAsuntosPenales);
        Task<List<ResponseMedidasCautelares>> GetMedidasCautelaresDisconnected(int idImputado);
        Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdDisconnectedProcedibilidadAsync(
            int id
        );
        Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdDisconnectedAnalisisAsync(
            int id
        );
        Task<AsuntosPenalesProcedibilidad> GetAsuntosPenalesByIdProcedibilidad(int id);
        Task<ResultTransaction> UpdateAnalisisAsync(AsuntosPenalesAnalisis entityAnalisis);
        Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id);
        Task<List<AsuntosPenalesAnalisis>> GetAsuntosPenalesAnalisisByIdAsuntoPenal(int idAsuntoPenal);        
        Task<AsuntosPenalesDelitos> GetByIdDelitosAsync(int id);
        Task<AsuntosPenalesMedidasCautelares> GetByIdMedidasCautelaresAsync(int id);
        Task<ResultTransaction> DeleteDelitos(AsuntosPenalesDelitos entity);
        Task<ResultTransaction> DeleteMedidasCautelares(AsuntosPenalesMedidasCautelares entity);
        Task<ResultTransaction> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );
        Task<List<ResponseImputados>> GetImputadosByIdAsuntoPenalDisconnected(int idAsuntosPenales);
        Task<List<AsuntosPenalesImputados>> GetImputados(int idAsuntosPenales);

        //Task<ResultTransaction> UpdateImputadosAsync(AsuntosPenales entity, AsuntosPenalesImputados entityImputados, ArchivosAsuntosPenales entityDocumento, DataFile? dataFile);
        Task<ResponseImputados> GetImputadoByIdDisconnected(int id);
        Task<ResponseImputadosEtapaInicial> GetImputadoByIdDisconnectedEtapaInicial(int id);

        Task<ResponseImputadosEtapaComplementaria> GetImputadoByIdDisconnectedEtapaComplementaria(
            int id
        );

        Task<ResponseImputadosEtapaIntermedia> GetImputadoByIdDisconnectedEtapaIntermedia(int id);
        Task<ResponseImputadosEtapaJuicio> GetImputadoByIdDisconnectedEtapaJuicio(int id);

        Task<AsuntosPenalesImputados> GetImputadoById(int id);

        // Task<ResultTransaction> UpdateImputadosAsync(AsuntosPenalesImputados entityImputados, AsuntosPenales entity);
        Task<ResultTransaction> UpdateImputadosEtapaInicialAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ResponseAcuerdoReparatorio>> GetAcuerdoReparatorioByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ResponseCriterioOportunidad>> GetCriterioOportunidadByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesSentencia>> GetSentenciaByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ResponseSentencia>> GetSentenciaByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesSobreseimiento>> GetSobreseimientoByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<ResponseSobreseimiento>> GetSobreseimientoByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        );
        Task<List<AsuntosPenalesSuspensionCondicional>> GetSuspencionCondicionalByImputado(
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

        Task<ResultTransaction> UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );

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

        Task<ResultTransaction> ApelacionImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityAcuerdoApelacion,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );
        Task<ResultTransaction> ApelacionImputadosUpdateAsync(
            AsuntosPenalesApelacion entityApelacion
        );

        Task<List<ResponseApelacion>> GetApelacionByImputadoDisconnected(int idImputado);
        Task<ResponseApelacion> GetApelacionByImputadoDisconnectedById(int id);
        Task<AsuntosPenalesApelacion> GetApelacionByImputadoById(int id);
        Task<ResultTransaction> DeleteApelacion(AsuntosPenalesApelacion entity);
        Task<ResultTransaction> AmparoImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAcuerdoAmparo,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<ResponseAmparo> GetAmparoByImputadoDisconnectedById(int id);
        Task<List<ResponseAmparo>> GetAmparoByImputadoDisconnected(int idImputado);
        Task<ResultTransaction> AmparoImputadosUpdateAsync(
            AsuntosPenalesAmparo entityAmparo
        );       
        Task<AsuntosPenalesAmparo> GetAmparoByImputadoById(int id);
        Task<ResultTransaction> DeleteAmparo(AsuntosPenalesAmparo entity);
        Task<AsuntosPenalesModificacion> GetModificacionByIdAsunto(int idAsuntoPenal);
        Task<List<ResponseAcuseConclusion>> ExportarPDFAsyncRepository(string no_asunto);

        
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
