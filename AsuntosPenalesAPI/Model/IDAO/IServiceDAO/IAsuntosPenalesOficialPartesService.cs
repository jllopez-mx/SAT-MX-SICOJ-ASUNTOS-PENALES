using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;
using AsuntosPenalesAPI.Model.DTO;

namespace AsuntosPenalesAPI.Model.IDAO.IServiceDAO
{
    public interface IAsuntosPenalesOficialPartesService
    {
        Task<ResultOperation> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral);
        Task<ResponseAsuntosPenalesById> GetAsuntosPenalesByIdDisconnected(int id);
        Task<ResponseNumeroAsunto> GetAsuntosPenalesByNumeroAsunto(string numero_asunto);
        Task<AsuntosPenales> GetAsuntosPenalesById(int id);
        Task<ResultOperation> GetHistoricoAsync(int fetch, int page, string? orderByColumn, bool orderDesc, string numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string oficiosolicitud, string numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? sub_administracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, bool? tipo_unidad, int? id_administracion_central);
        Task<ResultOperation> AddFisico(AsuntosPenales entity);
        Task<ResultOperation> AddControlDocumental(AsuntosPenales entity);
        Task<ResultOperation<ResponseTurnado>> UpdateTurnarAsuntosPenales(AsuntosPenales entity);
        Task<ResultOperation> UpdateAsuntosPenales(AsuntosPenales entity);
        Task<ResultOperation> DeleteAsuntosPenales(AsuntosPenales entity);
        Task<ResultOperation<int>> AddArchivosAsyncService(ArchivosAsuntosPenales entity, DataFile dataFile);
        Task<ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(int id_asuntospenales);
        Task<ArchivosAsuntosPenales> GetByIdArchivoService(int id);
        Task<ArchivosAsuntosPenales> GetByIdArchivoDeleteService(int id);
        Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile);
        Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaService(int id);
        Task<ResultOperation> UpdateSolicitudTransparenciaService(SolicitudTransparencia entity);
        Task<ResultOperation> GetByIdSolicitudTransparenciaServices(int id);
        Task<ResultOperation> GetTablaSolicitudTransparenciaService(int idAsunto);
        Task<ResultOperation> DeleteSolicitudTransparenciaService(SolicitudTransparencia entity);
        Task<ResultOperation> GetFechasVencimientoAsync(string? fecha_inicial, string? fecha_final, List<int>? Secciones);
        Task<ResultOperation> UpdateAsuntosPenalesMasivo(string? fecha_vencimiento, int? idAsunto, int? idModulo, int? idSeccion, int? idSeccionRenglon);
        Task<ResultOperation> EnvioEmail(Email entity);
    }
}