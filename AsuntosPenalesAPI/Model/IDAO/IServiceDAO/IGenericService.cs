using System.Reflection.Metadata;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Models;
using Sicoj.Utils.ViewModels;


namespace AsuntosPenalesAPI.Model.IDAO.IServiceDAO
{
    public interface IGenericService
    {
        
        #region Asuntos Penales
        Task<AsuntosPenalesAbogado> GetAbogadoByIdAsuntosPenalesAsync(int id);
        Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal);
        #endregion

        #region Archivos
        Task<ResultOperation<int>> AddArchivosAsyncService(ArchivosAsuntosPenales entity, DataFile dataFile);
        Task<ResultOperation> GetDocumentosAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAsuntoPenal, int? idRenglonSeccion);
        Task<List<ArchivosAsuntosPenales>> GetArchivosAsuntoPenalByIds(int[] ids);
        Task<ResultOperation<int>> UpdateArchivosAsuntosPenales(ArchivosAsuntosPenales entity, DataFile dataFile);
        Task<ResultOperation<bool>> DeleteArchivoAsuntoPenal(int[] ids, string usuarioModificacion);
        #endregion

        #region Reasignar
        Task<ResultOperation<ResponseReasignar>> ReasignarAsync(int[] idList, UserInformationView userInformationView, string rfcAbogado = null!, string rfcAdministrador = null!);
        #endregion

        #region Modificar
        Task<ResultOperation<int>> AddAsuntosPenalesModificacionAsync(AsuntosPenales entityAsuntoPenal, AsuntosPenalesModificacion entity);
        Task<AsuntosPenalesModificacion> GetAsuntosPenalesModificacionByIdAsuntoPenal(int id);
        
        #endregion 

        #region Descartar           
        Task<ResultOperation<int>> DescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales,AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseRequerimientoDescartarAsunto>> RequerimientoDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseRequisitosDescartarAsunto>> RequisitosDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseEtapaInicialDescartarAsunto>> EtapaInicialDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseEtapaComplementariaDescartarAsunto>> EtapaComplementariaDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseEtapaIntermediaDescartarAsunto>> EtapaIntermediaDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultOperation<ResponseCalcularFecha>> CalcularFecha(string baseDate, int addDays, bool nextDay);
        #endregion 
        
    }
}