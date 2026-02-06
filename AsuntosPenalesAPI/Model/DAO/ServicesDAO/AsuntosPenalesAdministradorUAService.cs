using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DAO.Repository;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events.AdministradorUA;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.ViewModels;
using Sicoj.Utils.Redis;



namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class AsuntosPenalesAdministradorUAService : IAsuntosPenalesAdministradorUAService
    {
        private readonly IApiService _apiService;
        private readonly IAsuntosPenalesAdministradorUARepository _repositoryAdministradorUA;
        private readonly string _routeUsuarioInfoDetalle = null!;
        private readonly IRedisClient _redisClient;
        private readonly string _routeAdministracionCentral = null!;
        private readonly string _routeUnidadAdministrativa = null!;
        private readonly string _routeSubadministracion = null!;


        public AsuntosPenalesAdministradorUAService(IConfiguration configuration, IAsuntosPenalesAdministradorUARepository repository, IApiService apiService, IRedisClient redisClient)
        {
            _repositoryAdministradorUA = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _routeUsuarioInfoDetalle = configuration.GetValue<string>("CatalogsEndpoints:RouteInfoUsuario")!;
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _routeAdministracionCentral = configuration.GetValue<string>(
                "CatalogsEndpoints:RouteAdministracionCentral"
            )!;
            _routeUnidadAdministrativa = configuration.GetValue<string>(
                "CatalogsEndpoints:RouteAdministracion"
            )!;
            _routeSubadministracion = configuration.GetValue<string>(
                "CatalogsEndpoints:RouteSubadministracion"
            )!;
        }

        public async Task<AsuntosPenales> GetAsuntoPenalById(int id) =>
            await _repositoryAdministradorUA.GetByIdAsync(id);

        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
        string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad
        )
        {
            try
            {
                var countResult = await _repositoryAdministradorUA.GetHistoricoCountAsync(
                    numeroasuntopenal,
                    fecharecepcion_desde,
                    fecharecepcion_hasta,
                    fechavencimiento_desde,
                    fechavencimiento_hasta,
                    oficiosolicitud,
                    numeroexpedientecadido,
                    id_unidadrealizasolicitud,
                    admin_control,
                    id_subadministracion,
                    nombre_abogado,
                    id_estadotarea,
                    id_estadoprocesal,
                    id_administracion_adscrita,
                    tipo_unidad
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesAdministradorUAByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }

                var result = await _repositoryAdministradorUA.GetHistoricoAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    numeroasuntopenal,
                    fecharecepcion_desde,
                    fecharecepcion_hasta,
                    fechavencimiento_desde,
                    fechavencimiento_hasta,
                    oficiosolicitud,
                    numeroexpedientecadido,
                    id_unidadrealizasolicitud,
                    admin_control,
                    id_subadministracion,
                    nombre_abogado,
                    id_estadotarea,
                    id_estadoprocesal,
                    id_administracion_adscrita,
                    tipo_unidad
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesAdministradorUAByFilters>>("No se encontraron resultados");
                }

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.unidadRealizaSolicitud))
                        item.unidadRealizaSolicitud = "Ministerio Público";
                }


                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseAsuntosPenalesAdministradorUAByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAdministradorUAsuntoPenalById(int id)
        {
            try
            {
                var result = await _repositoryAdministradorUA.GetAdministradorUAsuntoPenalById(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.idEstadoTarea!.Value > 0)
                {
                    var responseEstadoTarea =
                        await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea!.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseEstadoTarea)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseEstadoTarea;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado de tarea del  asunto."
                        );
                }
                if (result.idEstadoProcesal!.Value > 0)
                {
                    var responseEstadoProcesal =
                        await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseEstadoProcesal)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseEstadoProcesal;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del tipo asunto."
                        );
                }
                if (result.idUnidadRealizaSolicitud.HasValue && result.idUnidadRealizaSolicitud!.Value > 0)
                {
                    var responseAdministracionCentral = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_routeAdministracionCentral}/{result.idUnidadRealizaSolicitud.Value}"
                        );
                    if (responseAdministracionCentral is not null && responseAdministracionCentral.Success && responseAdministracionCentral.Result is not null)
                    {
                        resultOperation.Result.unidadRealizaSolicitud = responseAdministracionCentral.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del tipo asunto."
                        );
                }
                if (result.idAdminControla.HasValue && result.idAdminControla!.Value > 0)
                {
                    var responseAdministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_routeUnidadAdministrativa}/{result.idAdminControla.Value}"
                        );
                    if (
                        responseAdministracion is not null && responseAdministracion.Success && responseAdministracion.Result is not null
                    )
                    {
                        resultOperation.Result.adminControla = responseAdministracion.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del tipo asunto."
                        );
                }
                if (result.idSubAdministracion.HasValue && result.idSubAdministracion!.Value > 0)
                {
                    var responseSubadministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_routeSubadministracion}/{result.idSubAdministracion.Value}"
                        );
                    if (
                        responseSubadministracion is not null && responseSubadministracion.Success && responseSubadministracion.Result is not null
                    )
                    {
                        resultOperation.Result.subAdministracion = responseSubadministracion.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del tipo asunto."
                        );
                }

                if (string.IsNullOrEmpty(result.unidadRealizaSolicitud))
                    result.unidadRealizaSolicitud = "Ministerio Público";

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
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
                     await _repositoryAdministradorUA.ExportarReporteGeneralAsyncRepository(
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
        public async Task<ResultOperation> RemitirAsync(
            AsuntosPenales entity,
            AsuntosPenalesRemision entityRemision,
            ArchivosAsuntosPenales entityDocumento,
            DataFile dataFile
        )
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                if (entityRemision.fecha_oficio.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>(
                        "La fecha de oficio no puede ser mayor a la fecha actual."
                    );
                }

                var result = await _repositoryAdministradorUA.RemitirAsync(
                    entity,
                    entityRemision,
                    entityDocumento,
                    dataFile
                );
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponseNoMessage();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}