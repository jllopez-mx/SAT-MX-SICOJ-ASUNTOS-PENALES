using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
     public interface IArchivosAsuntosPenalesRepository
    {
    // Task<List<ResponseArchivosAsuntosPenales>> GetAllArchivosAsync();

    Task<ResultTransaction> AddFileAsyncRepository(ArchivosAsuntosPenales entity, DataFile dataFile);
    Task<List<ResponseArchivosAsuntosPenales>> GetArchivosByIdRegistroAsync_Repository( int id_asuntospenales);
    Task<ArchivosAsuntosPenales> GetByIdArchivoAsyncRepository(int id);
    Task<ResultTransaction> DeleteAsync(int[] ids, string usuarioModificacion);
    Task<List<ArchivosAsuntosPenales>> GetArchivosByIdRenglonSeccion_Async_Repository(int id_asuntospenales,int id_seccion,int id_renglonseccion, int id_tipo_documento );
    Task<List<ArchivosAsuntosPenales>> GetByIdsAsync(int[] id);
    //Task<ArchivosAsuntosPenales> GetFileAsuntosPenalesByIdAsuntosPenalesAsync(int id_asuntospenales);    
    Task<ResultTransaction> UpdateAsync(ArchivosAsuntosPenales entityDocumento, DataFile dataFile);
    Task<int?> GetDocumentosDisconnectedCount(int? idAsuntoPenal, List<int>? idSeccion, int? idRenglonSeccion, bool activo);
    Task<List<ResponseArchivosAsuntosPenales>> GetDocumentosHistoricoDisconnected(bool paginado, int? idAsuntoPenal, List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!);

    }

}