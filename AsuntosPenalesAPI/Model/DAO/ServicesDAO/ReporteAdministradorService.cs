using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using Sicoj.Utils.Redis;

namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class ReporteAdministradorService : IReporteAdministradorService
    {
        private readonly IReporteAdministradorRepository _repositoryReporteAdministrador;
        private readonly IRedisClient _redisClient;

        public ReporteAdministradorService(IConfiguration configuration, IReporteAdministradorRepository repository, IRedisClient redisClient)
        {
            _repositoryReporteAdministrador = repository ?? throw new ArgumentNullException(nameof(repository));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));

        }

        public async Task<List<ResponseReporteGeneral>> ExportarReporteGeneral(
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

             ) =>
                     await _repositoryReporteAdministrador.ExportarReporteGeneralAsyncRepository(
               noAsunto,
               fechaRecepcionDesde,
               fechaRecepcionHasta,
               fechaVencimientoDesde,
               fechaVencimientoHasta,
               oficioSolicitud,
               NoExpedienteCadido,
               UnidadRealizaSolicitud,
               idAdministracion,
               idSubadministracion,
               idAbogadoAsigno,
               EstadoTarea,
               EstadoProcesal,
               TipoConclusion,
               fechaConclusionDesde,
               fechaConclusionHasta,
               DeterminacionAsunto,
               RequisitosProcedibilidad,
               Delito,
               TipoSolucionAlterna,
               FechaPresentacionRequisito,
               FechaDelAutoVinculacion,
               FechaEmisionSentencia

          );
    }
}