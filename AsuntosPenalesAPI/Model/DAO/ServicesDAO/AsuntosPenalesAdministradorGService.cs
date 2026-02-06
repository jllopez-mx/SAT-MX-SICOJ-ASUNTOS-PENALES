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
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.ViewModels;
using Sicoj.Utils.Redis;



namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class AsuntosPenalesAdministradorGService : IAsuntosPenalesAdministradorGService
    {
        private readonly IApiService _apiService;
        private readonly IAsuntosPenalesAdministradorGRepository _repository;
        private readonly string _routeInfoUsuario;
        private readonly IRedisClient _redisClient;
        private readonly string _routeAdministracionCentral = null!;
        private readonly string _routeUnidadAdministrativa = null!;
        private readonly string _routeSubadministracion = null!;

        public AsuntosPenalesAdministradorGService(IConfiguration configuration, IAsuntosPenalesRepository repositoryAP, IAsuntosPenalesAdministradorGRepository repository, IApiService apiService, IRedisClient redisClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _routeInfoUsuario = configuration.GetValue<string>("CatalogsEndpoints:RouteInfoUsuario")!;
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _routeUnidadAdministrativa = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracion")!;
            _routeSubadministracion = configuration.GetValue<string>("CatalogsEndpoints:RouteSubadministracion")!;
            _routeAdministracionCentral = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracionCentral")!;

        }

        public async Task<AsuntosPenales> GetAsuntoPenalById(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task<ResultOperation<ResponseReactivar>> ReactivarAsync(AsuntosPenales entity, AsuntosPenalesAbogado entityAbogado)
        {
            var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_routeInfoUsuario}/{entityAbogado.id_abogado}");
            if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
            {
                return ResultOperation.FailureErrorResponse<ResponseReactivar>($"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración.");
            }

            // if (!UserSession.ValidateUser(responseAbogado!.Result!, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.ASUNTOS_PENALES, out string message, false, null!, true, entity.id_administracion_central, true, entity.id_unidad_realiza_solicitud, true, entity.id_subadministracion))
            // {
            //     return ResultOperation.FailureErrorResponse<ResponseReactivar>(message);
            // }

            var result = await _repository.ReactivarAsync(entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseReactivar>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseReactivar()
            {
                numeroasuntopenal = entity.numero_asunto_penal!,
            });
        }

        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
        string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad, UserInformationView userInformationView
        )
        {
            try
            {
                var countResult = await _repository.GetHistoricoCountAsync(
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
                    tipo_unidad,
                    userInformationView.IdAdministracionCentral
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesAdministradorUAByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }

                var result = await _repository.GetHistoricoAsync(
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
                    tipo_unidad,
                    userInformationView.IdAdministracionCentral
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

        public async Task<List<AsuntosPenalesHistoricoImputados>> GetHistoricoImputados(int id_imputado) =>
        await _repository.GetHistoricoImputados(id_imputado);


        public async Task<ResultOperation> GetAdminAsuntoPenalById(int id)
        {
            try
            {
                var result = await _repository.GetAdminAsuntoPenalById(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.idEstadoTarea.HasValue && result.idEstadoTarea!.Value > 0)
                {
                    var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString()!);

                    if (!string.IsNullOrEmpty(catalogValue))
                    {
                        resultOperation.Result.estadoTarea = catalogValue;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }
                if (result.idEstadoProcesal.HasValue && result.idEstadoProcesal!.Value > 0)
                {
                    var responseEstadoProcesal = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (!string.IsNullOrEmpty(responseEstadoProcesal))
                    {
                        resultOperation.Result.estadoProcesal = responseEstadoProcesal;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
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
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }
                if (result.idAdminControla.HasValue && result.idAdminControla!.Value > 0)
                {
                    var responseAdministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_routeUnidadAdministrativa}/{result.idAdminControla.Value}"
                        );
                    if (responseAdministracion is not null && responseAdministracion.Success && responseAdministracion.Result is not null)
                    {
                        resultOperation.Result.adminControla = responseAdministracion.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
                }
                if (result.idSubAdministracion.HasValue && result.idSubAdministracion!.Value > 0)
                {
                    var responseSubadministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_routeSubadministracion}/{result.idSubAdministracion.Value}"
                        );
                    if (responseSubadministracion is not null && responseSubadministracion.Success && responseSubadministracion.Result is not null)
                    {
                        resultOperation.Result.subAdministracion = responseSubadministracion.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
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



        public async Task<ResultOperation> UpdateFechaVencimientoAsync(
            AsuntosPenales entity
        )
        {
            try
            {
                var result =
                    await _repository.UpdateFechaVencimientoAsync(
                        entity
                    );

                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se actualizo correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetRegistroFechasAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
        string? numeroasuntopenal, int? admin_control, UserInformationView userInformationView
        )
        {
            try
            {
                var countResult = await _repository.GetRegistroFechasCountAsync(
                    numeroasuntopenal,
                    admin_control
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesAdministradorByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }

                var result = await _repository.GetRegistroFechasAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    numeroasuntopenal,
                    admin_control
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseFechaVencimientoExtraordinaria>>("No se encontraron resultados");
                }

                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseFechaVencimientoExtraordinaria>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
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
                     await _repository.ExportarReporteGeneralAsyncRepository(
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