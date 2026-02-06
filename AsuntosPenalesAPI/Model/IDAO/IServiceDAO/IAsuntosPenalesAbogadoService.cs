using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IServiceDAO
{
    public interface IAsuntosPenalesAbogadoService
    {
        Task<ResultOperation> GetBandejaAsync(
            int Fetch,
            int Page,
            string? OrderByColumn,
            bool OrderDesc,
            int? idAdminCentral,
            string? rfc_abogado
        );
        Task<ResultOperation> GetHistoricoAsync(
            int Fetch,
            int Page,
            string? OrderByColumn,
            bool OrderDesc,
            string? numeroasuntopenal,
            DateTime? fecharecepcion_desde,
            DateTime? fecharecepcion_hasta,
            DateTime? fechavencimiento_desde,
            DateTime? fechavencimiento_hasta,
            string? oficiosolicitud,
            string? numeroexpedientecadido,
            int? id_unidadrealizasolicitud,
            int? admin_control,
            int? id_subadministracion,
            string? nombre_abogado,
            int? id_estadotarea,
            int? id_estadoprocesal,
            int? id_administracion_adscrita,
            bool? tipoUnidad
        );
        Task<ResultOperation> GetAbogadoAsuntoPenalById(int id, string? rfc_abogado);
        Task<AsuntosPenales> GetAsuntoPenalById(int id);
        Task<ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(
            int id_asuntospenales
        );
        Task<ResultOperation<List<ResponseRemision>>> GetAllRemision(int idAsuntoPenal);
        Task<ResultOperation> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales entityDocumento,
            DataFile dataFile
        );
        Task<ResultOperation> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<ArchivosAsuntosPenales> GetByIdArchivoDeleteService(int id);
        Task<ArchivosAsuntosPenales> GetByIdArchivoService(int id);
        Task<ResultOperation<int>> AddArchivosAsyncService(
            ArchivosAsuntosPenales entity,
            DataFile dataFile
        );
        Task<ResultOperation> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        );
        Task<ResultOperation> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        );
        Task<ResultOperation> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos);
        Task<ResultOperation<List<ResponsePersonasMorales>>> GetAllPersonasMorales(
            int idAsuntoPenal
        );
        Task<ResultOperation<List<ResponseDelitos>>> GetAllDelitos(int idAsuntoPenal);
        Task<ResultOperation<List<ResponseMedidasCautelares>>> GetAllMedidasCautelares(
            int idImputado
        );
        Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdProcedibilidadDisconnected(
            int id
        );
        Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdAnalisisDisconnected(int id);
        Task<AsuntosPenalesProcedibilidad> GetAsuntosPenalesByIdProcedibilidad(int id);
        Task<ResultOperation> UpdateProcedibilidad(
            AsuntosPenalesProcedibilidad entityProcedibilidad
        );
        Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id);
        Task<List<AsuntosPenalesAnalisis>> GetAsuntosPenalesAnalisisByIdAsuntoPenal(int idAsuntoPenal);
        Task<AsuntosPenalesImputados> GetImputadoById(int id);
        Task<ResultOperation> GetImputadoByIdDisconnected(int id);
        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaInicial(int id);

        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaComplementaria(int id);

        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaIntermedia(int id);
        Task<ResultOperation> GetImputadoByIdDisconnectedEtapaJuicio(int id);
        Task<ResultOperation> UpdateAnalisis(AsuntosPenalesAnalisis entityAnalisis);
        Task<AsuntosPenalesDelitos> GetDelitoById(int id);
        Task<AsuntosPenalesMedidasCautelares> GetMedidaCautelarById(int id);
        Task<ResultOperation> DeleteDelitos(AsuntosPenalesDelitos entity);
        Task<ResultOperation> DeleteMedidasCautelares(AsuntosPenalesMedidasCautelares entity);
        Task<ResultOperation> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );
        Task<ResultOperation<List<ResponseImputados>>> GetAllImputadosDisconnected(
            int idAsuntoPenal
        );
        Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal);
        Task<ResultOperation> UpdateImputadosEtapaInicialAsync(
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


        Task<List<ArchivosAsuntosPenales>> GetArchivosByIdRenglonSeccion_Async_Repository(
            int id_asuntospenales,
            int id_seccion,
            int id_renglonseccion,
            int id_tipo_documento
        );

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

        Task<ResultOperation> UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        );

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

        Task<ResultOperation> ApelacionImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityApelacion,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );

        Task<AsuntosPenalesApelacion> GetApelacionById(int id_asuntospenales);

        Task<ResultOperation<List<ResponseApelacion>>> GetApelacionDisconnected(
            int idAsuntoPenal
        );

        Task<ResultOperation> GetApelacionDisconnectedById(int id);
        Task<ResultOperation> ApelacionImputadosUpdateAsync(
            AsuntosPenales entityAsuntoPenal,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityApelacion
        );
        Task<ResultOperation> DeleteApelacion(AsuntosPenalesApelacion entity);
        Task<ResultOperation> AmparoImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAmparo,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        );
        Task<ResultOperation> GetAmparoDisconnectedById(int id);

        Task<ResultOperation<List<ResponseAmparo>>> GetAmparoDisconnected(
            int idAsuntoPenal
        );
        Task<AsuntosPenalesAmparo> GetAmparoById(int id_asuntospenales);
        Task<ResultOperation> AmparoImputadosUpdateAsync(
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAmparo
        );
        Task<ResultOperation> DeleteAmparo(AsuntosPenalesAmparo entity);
        Task<AsuntosPenalesModificacion> GetModificacionByIdAsunto(int idAsuntoPenal);
        Task<List<ResponseAcuseConclusion>> Exporta_PDF(string noAsunto);
        
        Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile);
        Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaService(int id);
        Task<ResultOperation> UpdateSolicitudTransparenciaService(SolicitudTransparencia entity);
        Task<ResultOperation> GetByIdSolicitudTransparenciaServices(int id);
        Task<ResultOperation> GetTablaSolicitudTransparenciaService(int idAsunto);
        Task<ResultOperation> DeleteSolicitudTransparenciaService(SolicitudTransparencia entity);

    }
}
           
           