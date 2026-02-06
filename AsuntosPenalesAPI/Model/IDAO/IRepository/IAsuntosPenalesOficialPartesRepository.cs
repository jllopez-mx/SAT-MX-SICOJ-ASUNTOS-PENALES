using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesOficialPartesRepository
    {
        Task<ResultTransaction> AddAsuntosPenalesAsync(AsuntosPenales entity);
        Task<ResultTransaction> AddControlDocumentalAsync(AsuntosPenales entity);
        Task<ResponseAsuntosPenalesById> GetAsuntosPenalesByIdDisconnectedAsync(int id);
        Task<ResponseNumeroAsunto> GetAsuntosPenalesByNumeroAsuntoAsync(string numero_asunto);
        Task<List<ResponseAsuntosPenalesOficialPartesByFilters>> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool v, string numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string oficiosolicitud, string numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, bool? tipo_unidad, int? id_administracion_central);
        Task<int?> GetHistoricoCountAsync(string numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string oficiosolicitud, string numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, bool? tipo_unidad, int? id_administracion_central);
        Task<List<ResponseAsuntosPenalesOficialPartesByFilters>> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral);
        Task<int?> GetBandejaCountAsync(int? idAdminCentral);
        Task<ResultTransaction> TurnarAsuntosPenalesAsync(AsuntosPenales entity);
        Task<ResultTransaction> UpdateAsuntosPenalesAsync(AsuntosPenales entity);
        Task<ResultTransaction> DeleteAsuntosPenalesAsync(AsuntosPenales entity);
        //Task<ArchivosAsuntosPenales> GetFileAsuntosPenalesByIdAsync(int id);    
        //Task<ArchivosAsuntosPenales> GetFileAsuntosPenalesByIdAsuntosPenalesAsync(int id_asuntospenales);    
        //  Task<ResultTransaction> AddFileAsuntosPenalesAsync(ArchivosAsuntosPenales entity);

        #region Solicitud-Transparencia

        Task<ResultTransaction> AddAsyncSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile);
        Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaRepository(int id);
        Task<ResultTransaction> UpdateSolicitudTransparenciaRepository(SolicitudTransparencia entity);
        Task<ResponseSolicitudTransparenciaList> GetByIdSolicitudTransparenciaRepositorys(int id);
        Task<int?> GetTablaSolicitudTransparenciaCountRepository(int idAsunto);
        Task<List<ResponseSolicitudTransparenciaList>> GetTablaSolicitudTransparenciaRepository(int idAsunto);
        Task<ResultTransaction> DeleteSolicitudTransparenciaRepository(int id);

        #endregion
        Task<List<ResponseFechaVencimientoSeccion>> FechasVencimientoAsync(DateTime? fecha_inicial, DateTime? fecha_final, List<int>? Secciones);
        Task<ResultTransaction> UpdateAsuntosPenalesMasivoAsync(DateTime? fecha_vencimiento, int? idAsunto, int? idModulo, int? idSeccion, int? idSeccionRenglon);
        
    }
}