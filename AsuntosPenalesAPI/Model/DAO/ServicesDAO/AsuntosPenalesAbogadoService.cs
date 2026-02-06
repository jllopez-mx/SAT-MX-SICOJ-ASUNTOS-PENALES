using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class AsuntosPenalesAbogadoService : IAsuntosPenalesAbogadoService
    {
        private readonly IApiService _apiService;
        private readonly IAsuntosPenalesAbogadoRepository _repositoryAbogado;
        private readonly IArchivosAsuntosPenalesRepository _repositoryArchivos;
        private readonly IAsuntosPenalesRemisionRepository _repositoryRemision;
        private readonly IAsuntosPenalesRepository _repositoryAsuntosPenales;
        private readonly IAsuntosPenalesModificacionRepository _repositoryModificacion;

        private readonly IRedisClient _redisClient;
        private readonly CatalogosEnpoints _catalogosEnpoints;
        private readonly string _routeAdministracionCentral = null!;

        public AsuntosPenalesAbogadoService(
            IConfiguration configuration,
            IOptions<CatalogosEnpoints> catalogosEnpoints,
            IAsuntosPenalesAbogadoRepository repositoryAbogado,
            IArchivosAsuntosPenalesRepository repositoryArchivos,
            IAsuntosPenalesRemisionRepository repositoryRemision,
            IAsuntosPenalesModificacionRepository repositoryModificacion,
            IApiService apiService,
            IAsuntosPenalesRepository repositoryAsuntosPenales,
            IRedisClient redisClient
        )
        {
            _catalogosEnpoints = catalogosEnpoints.Value ?? throw new ArgumentNullException(nameof(_catalogosEnpoints));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _repositoryAbogado = repositoryAbogado ?? throw new ArgumentNullException(nameof(repositoryAbogado));
            _repositoryArchivos = repositoryArchivos;
            _repositoryRemision = repositoryRemision ?? throw new ArgumentNullException(nameof(repositoryRemision));
            _repositoryAsuntosPenales = repositoryAsuntosPenales ?? throw new ArgumentNullException(nameof(repositoryAsuntosPenales));
            _repositoryModificacion = repositoryModificacion ?? throw new ArgumentNullException(nameof(repositoryModificacion));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _routeAdministracionCentral = configuration.GetValue<string>(
                "CatalogsEndpoints:RouteAdministracionCentral"
            )!;
        }


        public async Task<ResultOperation> GetBandejaAsync(
            int Fetch,
            int Page,
            string? OrderByColumn,
            bool OrderDesc,
            int? idAdminCentral,
            string? rfc_abogado
        )
        {
            try
            {
                var countResult = await _repositoryAbogado.GetBandejaCountAsync(
                    idAdminCentral,
                    rfc_abogado
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                var result = await _repositoryAbogado.GetBandejaAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    idAdminCentral,
                    rfc_abogado
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                result.ForEach(c =>
                {
                    if (c.idUnidadRealizaSolicitud!.Value == 0)
                    {
                        c.unidadRealizaSolicitud = "Ministerio público";
                    }
                    ;
                });
                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseAsuntosPenalesAbogadoByFilters>(
                        new(Page, Fetch, countResult.GetValueOrDefault()),
                        result
                    )
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetHistoricoAsync(
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
        )
        {
            try
            {
                var countResult = await _repositoryAbogado.GetHistoricoCountAsync(
                    numeroasuntopenal!,
                    fecharecepcion_desde,
                    fecharecepcion_hasta,
                    fechavencimiento_desde,
                    fechavencimiento_hasta,
                    oficiosolicitud!,
                    numeroexpedientecadido!,
                    id_unidadrealizasolicitud,
                    admin_control,
                    id_subadministracion,
                    nombre_abogado,
                    id_estadotarea,
                    id_estadoprocesal,
                    id_administracion_adscrita,
                    tipoUnidad
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesOficialPartesByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }

                var result = await _repositoryAbogado.GetHistoricoAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    numeroasuntopenal!,
                    fecharecepcion_desde,
                    fecharecepcion_hasta,
                    fechavencimiento_desde,
                    fechavencimiento_hasta,
                    oficiosolicitud!,
                    numeroexpedientecadido!,
                    id_unidadrealizasolicitud,
                    admin_control,
                    id_subadministracion,
                    nombre_abogado,
                    id_estadotarea,
                    id_estadoprocesal,
                    id_administracion_adscrita,
                    tipoUnidad
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesAdministradorByFilters>>("No se encontraron resultados");
                }

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.unidadRealizaSolicitud!))
                        item.unidadRealizaSolicitud = "Ministerio Público";
                }

                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseAsuntosPenalesAbogadoByFilters>(
                        new(Page, Fetch, countResult.GetValueOrDefault()),
                        result
                    )
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAbogadoAsuntoPenalById(int id, string? rfc_abogado)
        {
            try
            {
                var result = await _repositoryAbogado.GetAbogadoAsuntoPenalById(id, rfc_abogado);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                if (result.idEstadoProcesal!.Value == EnumEstadoProcesal.EN_REPARACION.GetHashCode())
                {
                    var resultModificacion = await _repositoryModificacion.GetByIdAsuntoPenalAsync(id);
                    result.idSeccion = resultModificacion.id_seccion;
                }


                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.idEstadoTarea!.Value > 0)
                {
                    var responseEstadoTarea =
                        await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
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

                // if (result.unidadRealizaSolicitud.Value > 0)
                // {
                //     var responseAdministracionCentral = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                //             $"{_routeAdministracionCentral}/{result.unidadRealizaSolicitud.Value}"
                //         );
                //     if (responseAdministracionCentral is not null && responseAdministracionCentral.Success && responseAdministracionCentral.Result is not null)
                //     {
                //         resultOperation.Result.unidadRealizaSolicitud.Label = responseAdministracionCentral.Result.nombre;
                //     }
                //     else
                //         resultOperation.AddWarningMessage(
                //             "No se pudo recuperar el nombre del tipo asunto."
                //         );
                // }

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
                            $"{_catalogosEnpoints.RouteAdministracion}/{result.idAdminControla.Value}"
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

                // if (result.idSubAdministracion!.Value > 0)
                // {
                //     result.subAdministracion = "Sin catálogo";
                //     // var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos., result.tipoSentencia.ToString());
                //     // if(!string.IsNullOrEmpty(catalogValue))
                //     // {
                //     //     result.tipoSentencia.Label = catalogValue;
                //     // }
                //     // else
                //     //     resultOperation.AddWarningMessage(
                //     //             "No se pudo recuperar el nombre del tipo de sentencia."
                //     //         );
                // }

                if ( result.idSubAdministracion.HasValue && result.idSubAdministracion!.Value > 0)
                {
                    var responseSubadministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                            $"{_catalogosEnpoints.RouteSubadministracion}/{result.idSubAdministracion.Value}"
                        );
                    if (
                        responseSubadministracion is not null && responseSubadministracion.Success && responseSubadministracion.Result is not null
                    )
                    {
                        resultOperation.Result.subAdministracion = responseSubadministracion.Result.nombre;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre de la subadministracion."
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

        public async Task<
            ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(int id_registro)
        {
            var result = await _repositoryArchivos.GetArchivosByIdRegistroAsync_Repository(
                id_registro
            );
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<
                    List<ResponseArchivosAsuntosPenales>
                >(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<AsuntosPenales> GetAsuntoPenalById(int id) =>
            await _repositoryAsuntosPenales.GetByIdAsync(id);

        public async Task<ResultOperation<List<ResponseRemision>>> GetAllRemision(int idAsuntoPenal)
        {
            var result = await _repositoryRemision.GetRemisionesDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseRemision>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales entityDocumento,
            DataFile dataFile
        )
        {
            try
            {
                var result = await _repositoryAbogado.AnalisisAsync(
                    entity,
                    entityAnalisis,
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

        public async Task<ResultOperation> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        )
        {
            try
            {
                var result = await _repositoryAbogado.ProcedibilidadAsync(
                    entity,
                    entityProcedibilidad
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

        public async Task<ArchivosAsuntosPenales> GetByIdArchivoDeleteService(int id) =>
            await _repositoryArchivos.GetByIdArchivoAsyncRepository(id);


        public async Task<ArchivosAsuntosPenales> GetByIdArchivoService(int id) =>
            await _repositoryArchivos.GetByIdArchivoAsyncRepository(id);

        public async Task<ResultOperation<int>> AddArchivosAsyncService(
            ArchivosAsuntosPenales entity,
            DataFile dataFile
        )
        {
            try
            {
                var result = await _repositoryArchivos.AddFileAsyncRepository(entity, dataFile);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponse(
                    result.Result.GetValueOrDefault(),
                    "El registro de agregó correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        )
        {
            try
            {
                var result = await _repositoryAbogado.PersonasMoralesAsync(
                    entity,
                    entityPersonasMorales
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

        public async Task<ResultOperation> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        )
        {
            try
            {
                var result = await _repositoryAbogado.MedidasCautelaresAsync(
                    entityMedidasCautelares
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

        public async Task<ResultOperation> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos)
        {
            try
            {
                var result = await _repositoryAbogado.DelitosAsync(entityDelitos);
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

        public async Task<ResultOperation<List<ResponsePersonasMorales>>> GetAllPersonasMorales(
            int idAsuntoPenal
        )
        {
            var result = await _repositoryAbogado.GetPersonasMoralesDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponsePersonasMorales>>(
                    new()
                );
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation<List<ResponseDelitos>>> GetAllDelitos(int idAsuntoPenal)
        {
            var result = await _repositoryAbogado.GetDelitosDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseDelitos>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation<List<ResponseMedidasCautelares>>> GetAllMedidasCautelares(
            int idImputado
        )
        {
            var result = await _repositoryAbogado.GetMedidasCautelaresDisconnected(idImputado);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseMedidasCautelares>>(
                    new()
                );
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdProcedibilidadDisconnected(
            int id
        )
        {
            var response =
                await _repositoryAbogado.GetAsuntosPenalesByIdDisconnectedProcedibilidadAsync(id);

            if (response is null)
                return null!;
            var resultOperation = ResultOperation.SuccessResponseNoMessage(response);
            if (response.id_requisito_procedibilidad!.Value > 0)
            {
                var responseRequisitoProcedibilidad =
                    await _redisClient.GetCatalogValue(EnumCatalogos.RequisitosProcedibilidadPenales, response.id_requisito_procedibilidad.Value.ToString());
                if (
                    !string.IsNullOrEmpty(responseRequisitoProcedibilidad)
                )
                {
                    resultOperation.Result.requisito_procedibilidad =
                        responseRequisitoProcedibilidad;
                }
                else
                    resultOperation.AddWarningMessage(
                        "No se pudo recuperar el nombre del tipo asunto."
                    );
            }
            return response;
        }

        public async Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdAnalisisDisconnected(
            int id
        )
        {
            var response = await _repositoryAbogado.GetAsuntosPenalesByIdDisconnectedAnalisisAsync(
                id
            );

            if (response is null)
                return null!;
            var resultOperation = ResultOperation.SuccessResponseNoMessage(response);
            if (response.id_determinacion_asunto.HasValue && response.id_determinacion_asunto!.Value > 0)
            {
                var responseDeterminacionAsuntoPenal =
                    await _redisClient.GetCatalogValue(EnumCatalogos.DeterminacionAsuntoPenal, response.id_determinacion_asunto!.Value.ToString());
                if (
                    !string.IsNullOrEmpty(responseDeterminacionAsuntoPenal)
                )
                {
                    resultOperation.Result.determinacion_asunto_penal =
                        responseDeterminacionAsuntoPenal;
                }
                else
                    resultOperation.AddWarningMessage(
                        "No se pudo recuperar el nombre del tipo asunto."
                    );
            }
            return response;
        }

        public async Task<AsuntosPenalesProcedibilidad> GetAsuntosPenalesByIdProcedibilidad(
            int id
        ) => await _repositoryAbogado.GetAsuntosPenalesByIdProcedibilidad(id);

        public async Task<ResultOperation> UpdateProcedibilidad(
            AsuntosPenalesProcedibilidad entityProcedibilidad
        )
        {
            try
            {
                var result = await _repositoryAbogado.UpdateProcedibilidadAsync(
                    entityProcedibilidad
                );
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
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

        public async Task<List<AsuntosPenalesAnalisis>> GetAsuntosPenalesAnalisisByIdAsuntoPenal(int idAsuntoPenal) =>
           await _repositoryAbogado.GetAsuntosPenalesAnalisisByIdAsuntoPenal(idAsuntoPenal);

        public async Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id) =>
            await _repositoryAbogado.GetAsuntosPenalesByIdAnalisis(id);

        public async Task<ResultOperation> UpdateAnalisis(AsuntosPenalesAnalisis entityAnalisis)
        {
            try
            {
                var result = await _repositoryAbogado.UpdateAnalisisAsync(entityAnalisis);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
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

        public async Task<AsuntosPenalesImputados> GetImputadoById(int id) =>
            await _repositoryAbogado.GetImputadoById(id);

        public async Task<ResultOperation> GetImputadoByIdDisconnected(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetImputadoByIdDisconnected(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputados>(
                        "El imputado no existe"
                    );
                }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.solucionAlterna.GetValueOrDefault())
                {
                    if (
                        result.idTipoSolucionAlterna!.Value
                        == EnumTipoSolucionAlternaEtapaInicial.AcuerdoReparatorio.GetHashCode()
                    )
                    {
                        var listResult =
                            await _repositoryAbogado.GetAcuerdoReparatorioByImputadoDisconnected(
                                result.Id,
                                EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                            );
                        if (listResult is not null && listResult.Any())
                        {
                            resultOperation.Result.acuerdoReparatorio = listResult.FirstOrDefault();
                        }
                    }
                    else if (
                        result.idTipoSolucionAlterna.Value
                        == EnumTipoSolucionAlternaEtapaInicial.CriteroOportunidad.GetHashCode()
                    )
                    {
                        var listResult =
                            await _repositoryAbogado.GetCriterioOportunidadByImputadoDisconnected(
                                result.Id,
                                EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                            );
                        if (listResult is not null && listResult.Any())
                        {
                            resultOperation.Result.citerioOportunidad = listResult.FirstOrDefault();
                        }
                    }
                }
                if (result.idEstadoProcesal. HasValue && result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idEstadoTarea.HasValue && result.idEstadoTarea!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado tarea."
                        );
                }

                if (result.idTerminacionInvestigacion.HasValue && result.idTerminacionInvestigacion!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.FormaTerminacionInvestigacionPenales, result.idTerminacionInvestigacion.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.terminacionInvestigacion = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                if (result.idTipoSolucionAlterna.HasValue && result.idTipoSolucionAlterna!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.TipoSolucionAlternaPenales, result.idTipoSolucionAlterna.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.tipoSolucionAlterna = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                if (result.idCentroJusticia.HasValue && result.idCentroJusticia!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.CentroJusticiaPenales, result.idCentroJusticia.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.centroJusticia = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaInicial(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetImputadoByIdDisconnectedEtapaInicial(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputadosEtapaInicial>(
                        "No se encontraron resultados"
                    );
                }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.solucionAlterna.GetValueOrDefault())
                {
                    var listResultAcuerdo =
                        await _repositoryAbogado.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }

                    var listResultCriterio =
                        await _repositoryAbogado.GetCriterioOportunidadByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                        );
                    if (listResultCriterio is not null && listResultCriterio.Any())
                    {
                        resultOperation.Result.citerioOportunidad =
                            listResultCriterio.FirstOrDefault();
                    }
                }

                if (result.idEstadoProcesal.HasValue && result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idEstadoTarea.HasValue && result.idEstadoTarea!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado tarea."
                        );
                }

                if (result.idTerminacionInvestigacion.HasValue && result.idTerminacionInvestigacion!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.FormaTerminacionInvestigacionPenales, result.idTerminacionInvestigacion.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.terminacionInvestigacion = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                if (result.idSolucionAlterna.HasValue && result.idSolucionAlterna!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.TipoSolucionAlternaPenales, result.idSolucionAlterna.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.tipoSolucionAlterna = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                if (result.idCentroJusiticia.HasValue && result.idCentroJusiticia!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.CentroJusticiaPenales, result.idCentroJusiticia.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.centroJusticia = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaComplementaria(int id)
        {
            try
            {
                var result =
                    await _repositoryAbogado.GetImputadoByIdDisconnectedEtapaComplementaria(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputadosEtapaComplementaria>(
                        "No se encontraron resultados"
                    );
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.solucionAlternaCp.GetValueOrDefault())
                {
                    var listResultAcuerdo =
                        await _repositoryAbogado.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }
                    var listResultSobreseimiento =
                        await _repositoryAbogado.GetSobreseimientoByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (listResultSobreseimiento is not null && listResultSobreseimiento.Any())
                    {
                        resultOperation.Result.sobreseimiento =
                            listResultSobreseimiento.FirstOrDefault();
                    }

                    var listResultSuspensionCondicional =
                        await _repositoryAbogado.GetSuspencionCondicionalByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (
                        listResultSuspensionCondicional is not null
                        && listResultSuspensionCondicional.Any()
                    )
                    {
                        resultOperation.Result.suspencionCondicional =
                            listResultSuspensionCondicional.FirstOrDefault();
                    }
                }
                if (result.procedimientoAbreviadoCp.GetValueOrDefault())
                {
                    var listResultSentencia =
                        await _repositoryAbogado.GetSentenciaByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (listResultSentencia is not null && listResultSentencia.Any())
                    {
                        resultOperation.Result.sentencia = listResultSentencia.FirstOrDefault();
                    }
                }

                if (result.idEstadoProcesal.HasValue && result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                       !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idEstadoTarea.HasValue && result.idEstadoTarea!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado tarea."
                        );
                }

                if (result.idTipoSentencia.HasValue && result.idTipoSentencia!.Value > 0)
                {
                    result.tipoSentencia = "Sin catálogo";
                    // var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos., result.tipoSentencia.ToString());
                    // if(!string.IsNullOrEmpty(catalogValue))
                    // {
                    //     result.tipoSentencia.Label = catalogValue;
                    // }
                    // else
                    //     resultOperation.AddWarningMessage(
                    //             "No se pudo recuperar el nombre del tipo de sentencia."
                    //         );


                }

                if (result.idTipoSolucionAlternaCp.HasValue && result.idTipoSolucionAlternaCp!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.TipoSolucionAlternaPenales, result.idTipoSolucionAlternaCp.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.tipoSolucionAlternaCp = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }
                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaIntermedia(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetImputadoByIdDisconnectedEtapaIntermedia(
                    id
                );
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputadosEtapaComplementaria>(
                        "No se encontraron resultados"
                    );
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.solucionAlternaIn.GetValueOrDefault())
                {
                    var listResultAcuerdo =
                        await _repositoryAbogado.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }
                    var listResultSobreseimiento =
                        await _repositoryAbogado.GetSobreseimientoByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (listResultSobreseimiento is not null && listResultSobreseimiento.Any())
                    {
                        resultOperation.Result.sobreseimiento =
                            listResultSobreseimiento.FirstOrDefault();
                    }

                    var listResultSuspensionCondicional =
                        await _repositoryAbogado.GetSuspencionCondicionalByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (
                        listResultSuspensionCondicional is not null
                        && listResultSuspensionCondicional.Any()
                    )
                    {
                        resultOperation.Result.suspencionCondicional =
                            listResultSuspensionCondicional.FirstOrDefault();
                    }
                }
                if (result.procedimientoAbreviadoIn.GetValueOrDefault())
                {
                    var listResultSentencia =
                        await _repositoryAbogado.GetSentenciaByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (listResultSentencia is not null && listResultSentencia.Any())
                    {
                        resultOperation.Result.sentencia = listResultSentencia.FirstOrDefault();
                    }
                }

                if (result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idEstadoTarea!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado tarea."
                        );
                }

                if (result.idTipoSolucionAlternaIn.HasValue && result.idTipoSolucionAlternaIn.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.TipoSolucionAlternaPenales, result.idTipoSolucionAlternaIn.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.tipoSolucionAlternaIn = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage("No se pudo recuperar el nombre del .");
                }
                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaJuicio(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetImputadoByIdDisconnectedEtapaJuicio(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputadosEtapaJuicio>(
                        "No se encontraron resultados"
                    );
                }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);

                var listResultSentencia =
                    await _repositoryAbogado.GetSentenciaByImputadoDisconnected(
                        result.Id,
                        EnumTipoEtapaInvestigacion.JUCIO.GetHashCode()
                    );
                if (listResultSentencia is not null && listResultSentencia.Any())
                {
                    resultOperation.Result.sentencia = listResultSentencia.FirstOrDefault();
                }

                if (result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idEstadoTarea!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, result.idEstadoTarea.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoTarea = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado tarea."
                        );
                }

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AsuntosPenalesDelitos> GetDelitoById(int id) =>
            await _repositoryAbogado.GetByIdDelitosAsync(id);

        public async Task<AsuntosPenalesMedidasCautelares> GetMedidaCautelarById(int id) =>
            await _repositoryAbogado.GetByIdMedidasCautelaresAsync(id);

        public async Task<ResultOperation> DeleteDelitos(AsuntosPenalesDelitos entity)
        {
            try
            {
                var result = await _repositoryAbogado.DeleteDelitos(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );
                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se elimino  correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> DeleteMedidasCautelares(
            AsuntosPenalesMedidasCautelares entity
        )
        {
            try
            {
                var result = await _repositoryAbogado.DeleteMedidasCautelares(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );
                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se elimino  correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        )
        {
            try
            {
                var result = await _repositoryAbogado.ImputadosAsync(entity, entityImputados);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponseNoMessage(result.Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation<List<ResponseImputados>>> GetAllImputadosDisconnected(
            int idAsuntoPenal
        )
        {
            var result = await _repositoryAbogado.GetImputadosByIdAsuntoPenalDisconnected(
                idAsuntoPenal
            );
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseImputados>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal) =>
            await _repositoryAbogado.GetImputados(idAsuntoPenal);


        public async Task<ResultOperation> UpdateImputadosEtapaInicialAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                var result = await _repositoryAbogado.UpdateImputadosEtapaInicialAsync(
                    entity,
                    entityImputados,
                    entityAcuerdoReparatorio,
                    entityCriterioOportunidad,
                    entityDocumento,
                    dataFile
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

        public async Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repositoryAbogado.GetAcuerdoReparatorioByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<List<AsuntosPenalesSentencia>> GetSentenciaByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) => await _repositoryAbogado.GetSentenciaByImputado(idImputado, idTipoEtapaInvestigacion);

        public async Task<List<AsuntosPenalesSobreseimiento>> GetSobreseimientoByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repositoryAbogado.GetSobreseimientoByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<
            List<AsuntosPenalesSuspensionCondicional>
        > GetSuspencionCondicionalByImputado(int idImputado, int idTipoEtapaInvestigacion) =>
            await _repositoryAbogado.GetSuspencionCondicionalByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repositoryAbogado.GetCriterioOportunidadByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<
            List<ArchivosAsuntosPenales>
        > GetArchivosByIdRenglonSeccion_Async_Repository(
            int id_asuntospenales,
            int id_seccion,
            int id_renglonseccion,
            int id_tipo_documento
        ) =>
            await _repositoryArchivos.GetArchivosByIdRenglonSeccion_Async_Repository(
                id_asuntospenales,
                id_seccion,
                id_renglonseccion,
                id_tipo_documento
            );

        public async Task<ResultOperation> UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        )
        {
            try
            {
                var result =
                    await _repositoryAbogado.UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
                        entity,
                        entityImputados
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

        public async Task<ResultOperation> UpdateImputadosEtapaComplementariaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                var result = await _repositoryAbogado.UpdateImputadosEtapaComplementariaAsync(
                    entity,
                    entityImputados,
                    entityAcuerdoReparatorio,
                    entitySobreseimiento,
                    entitySuspencion,
                    entitySentencia,
                    entityDocumento,
                    dataFile
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

        public async Task<ResultOperation> UpdateImputadosEtapaIntermediaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                var result = await _repositoryAbogado.UpdateImputadosEtapaIntermediaAsync(
                    entity,
                    entityImputados,
                    entityAcuerdoReparatorio,
                    entitySobreseimiento,
                    entitySuspencion,
                    entitySentencia,
                    entityDocumento,
                    dataFile
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

        public async Task<ResultOperation> UpdateImputadosEtapaJuicioAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitySuspencion,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                if (entitySentencia.id_tipo_sentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode())
                {
                    if (entitySentencia.fecha_ejecucion < entity.fecha_recepcion)
                    {
                        return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de ejecución no puede ser menor a la fecha de recepción."
                        );
                    }
                    if (entitySentencia.fecha_emision_sentencia < entity.fecha_recepcion)
                    {
                        return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de emisión no puede ser menor a la fecha de recepción."
                        );
                    }
                    if (entitySentencia.conclusion_asunto)
                    {
                        if (entitySentencia.fecha_conclusion < entity.fecha_recepcion)
                        {
                            return ResultOperation<int?>.FailureWarningResponse<int?>(
                                "La fecha de conclusíon no puede ser menor a la fecha de recepción."
                            );
                        }
                    }
                }
                else
                {
                    if (entitySentencia.fecha_emision_sentencia < entity.fecha_recepcion)
                    {
                        return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de emisión no puede ser menor a la fecha de recepción."
                        );

                    }
                }

                var result = await _repositoryAbogado.UpdateImputadosEtapaJuicioAsync(
                    entity,
                    entityImputados,
                    entityAcuerdoReparatorio,
                    entitySobreseimiento,
                    entitySuspencion,
                    entitySentencia,
                    entityDocumento,
                    dataFile
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

        public async Task<ResultOperation> ApelacionImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityApelacion,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                if (entityApelacion.fecha_presentacion < entity.fecha_recepcion)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de presentación no puede ser menor a la fecha de recepción."
                        );
                }
                if (entityApelacion.fecha_resolucion < entity.fecha_recepcion)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de resolución no puede ser menor a la fecha de recepción."
                        );
                }
                var result = await _repositoryAbogado.ApelacionImputadosAsync(
                    entity,
                    entityImputados,
                    entityApelacion,
                    entityDocumento,
                    dataFile
                );

                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se agrego correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> ApelacionImputadosUpdateAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityApelacion
        )
        {
            try
            {
                if (entityApelacion.fecha_presentacion < entity.fecha_recepcion)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de presentación no puede ser menor a la fecha de recepción."
                        );
                }
                if (entityApelacion.fecha_resolucion < entity.fecha_recepcion)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>(
                            "La fecha de resolución no puede ser menor a la fecha de recepción."
                        );
                }
                var result = await _repositoryAbogado.ApelacionImputadosUpdateAsync(
                    entityApelacion
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

        public async Task<AsuntosPenalesApelacion> GetApelacionById(int id_asuntospenales) =>
            await _repositoryAbogado.GetApelacionByImputadoById(id_asuntospenales);

        public async Task<ResultOperation<List<ResponseApelacion>>> GetApelacionDisconnected(
            int idAsuntoPenal
        )
        {
            var result = await _repositoryAbogado.GetApelacionByImputadoDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseApelacion>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> GetApelacionDisconnectedById(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetApelacionByImputadoDisconnectedById(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseApelacion>(
                        "No se encontraron resultados"
                    );
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idTipoResolucion.HasValue && result.idTipoResolucion!.Value > 0)
                {
                    result.tipoResolucion = "Sin catálogo";
                }

                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> DeleteApelacion(AsuntosPenalesApelacion entity)
        {
            try
            {
                var result = await _repositoryAbogado.DeleteApelacion(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );
                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se elimino  correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> AmparoImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAmparo,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            try
            {
                var result = await _repositoryAbogado.AmparoImputadosAsync(
                    entity,
                    entityImputados,
                    entityAmparo,
                    entityDocumento,
                    dataFile
                );

                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );

                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se agrego correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetAmparoDisconnectedById(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetAmparoByImputadoDisconnectedById(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseApelacion>(
                        "No se encontraron resultados"
                    );
                }
                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);
                if (result.idEstadoProcesal!.Value > 0)
                {
                    var responseCatalogo = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, result.idEstadoProcesal.Value.ToString());
                    if (
                        !string.IsNullOrEmpty(responseCatalogo)
                    )
                    {
                        resultOperation.Result.estadoProcesal = responseCatalogo;
                    }
                    else
                        resultOperation.AddWarningMessage(
                            "No se pudo recuperar el nombre del estado procesal."
                        );
                }

                if (result.idTipoAmparo.HasValue && result.idTipoAmparo!.Value > 0)
                {
                    result.tipoAmparo = "Sin catálogo";
                    // var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos., result.tipoSentencia.ToString());
                    // if(!string.IsNullOrEmpty(catalogValue))
                    // {
                    //     result.tipoSentencia.Label = catalogValue;
                    // }
                    // else
                    //     resultOperation.AddWarningMessage(
                    //             "No se pudo recuperar el nombre del tipo de sentencia."
                    //         );      
                }

                // if (result.tipoAmparo.Value > 0)
                // {
                //     var responseCatalogo = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                //         $"{_routeTipoAmparo}/{result.tipoAmparo.Value}"
                //     );
                //     if (
                //         responseCatalogo is not null
                //         && responseCatalogo.Success
                //         && responseCatalogo.Result is not null
                //     )
                //     {
                //         resultOperation.Result.tipoAmparo.Label = responseCatalogo.Result.nombre;
                //     }
                //     else
                //         resultOperation.AddWarningMessage(
                //             "No se pudo recuperar el nombre del tipo Amparo."
                //         );
                // }


                if (result.idTipoResolucion.HasValue && result.idTipoResolucion!.Value > 0)
                {
                    result.tipoResolucion = "Sin catálogo";
                    // var catalogValue = await _redisClient.GetCatalogValue(EnumCatalogos., result.tipoSentencia.ToString());
                    // if(!string.IsNullOrEmpty(catalogValue))
                    // {
                    //     result.tipoSentencia.Label = catalogValue;
                    // }
                    // else
                    //     resultOperation.AddWarningMessage(
                    //             "No se pudo recuperar el nombre del tipo de sentencia."
                    //         );      
                }


                // if (result.tipoResolucion.Value > 0)
                // {
                //     var responseCatalogo = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                //         $"{_routeTipoResolucion}/{result.tipoResolucion.Value}"
                //     );
                //     if (
                //         responseCatalogo is not null
                //         && responseCatalogo.Success
                //         && responseCatalogo.Result is not null
                //     )
                //     {
                //         resultOperation.Result.tipoResolucion.Label = responseCatalogo
                //             .Result
                //             .nombre;
                //     }
                //     else
                //         resultOperation.AddWarningMessage(
                //             "No se pudo recuperar el nombre del tipo Amparo."
                //         );
                // }


                return ResultOperation.SuccessResponse(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation<List<ResponseAmparo>>> GetAmparoDisconnected(
            int idAsuntoPenal
        )
        {
            var result = await _repositoryAbogado.GetAmparoByImputadoDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseAmparo>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<AsuntosPenalesAmparo> GetAmparoById(int id_asuntospenales) =>
            await _repositoryAbogado.GetAmparoByImputadoById(id_asuntospenales);

        public async Task<ResultOperation> AmparoImputadosUpdateAsync(
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAmparo
        )
        {
            try
            {
                var result = await _repositoryAbogado.AmparoImputadosUpdateAsync(entityAmparo);

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

        public async Task<ResultOperation> DeleteAmparo(AsuntosPenalesAmparo entity)
        {
            try
            {
                var result = await _repositoryAbogado.DeleteAmparo(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>(
                        $"{result.MsgError!}:{result.DetailError}"
                    );
                return ResultOperation.SuccessResponse(
                    result.Result,
                    "El registro se elimino correctamente."
                );
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AsuntosPenalesModificacion> GetModificacionByIdAsunto(int idAsuntoPenal) =>
            await _repositoryAbogado.GetModificacionByIdAsunto(idAsuntoPenal);

        public async Task<List<ResponseAcuseConclusion>> Exporta_PDF(string noAsunto) =>
        await _repositoryAbogado.ExportarPDFAsyncRepository(noAsunto);
        
        #region Solicitud Transparencia

        public async Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile)
        {
            try
            {
                var consultaDetails = await _repositoryAsuntosPenales.GetByIdAsync(entity.id_asunto);
                if (consultaDetails == null)
                {
                    return ResultOperation<int?>.FailureErrorResponse("No se pudo encontrar el Asunto Penal con el ID especificado.");
                }

                DateTime dateTime = DateTime.Now;
                if (entity.fechaSolicitud.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de solicitud no debe ser mayor a la fecha actual.");
                }


                var result = await _repositoryAbogado.AddAsyncSolicitudTransparenciaService(entity, entityDocumento, dataFile);
                if (!result.Success)
                    return ResultOperation<int?>.FailureErrorResponse($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage(result.Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaService(int id) =>
        await _repositoryAbogado.GetByIdSolicitudTransparenciaRepository(id);

        public async Task<ResultOperation> UpdateSolicitudTransparenciaService(SolicitudTransparencia entity)
        {
            try
            {
                var consultaDetails = await _repositoryAsuntosPenales.GetByIdAsync(entity.id_asunto);
                if (consultaDetails == null)
                {
                    return ResultOperation<int?>.FailureErrorResponse("No se pudo encontrar la consulta con el ID especificado.");
                }

                DateTime dateTime = DateTime.Now;
                if (entity.fechaSolicitud.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de solicitud no debe ser mayor a la fecha actual.");
                }

                var result = await _repositoryAbogado.UpdateSolicitudTransparenciaRepository(entity);
                if (!result.Success)
                    return ResultOperation<int?>.FailureErrorResponse($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage(result.Result);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ResultOperation> GetByIdSolicitudTransparenciaServices(int id)
        {
            try
            {
                var result = await _repositoryAbogado.GetByIdSolicitudTransparenciaRepositorys(id);
                if (result is null)
                {
                    return ResultOperation.SuccessResponseNoMessage("No se encontraron resultados");
                }
                return ResultOperation.SuccessResponseNoMessage(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetTablaSolicitudTransparenciaService(int idAsunto)
        {
            try
            {
                int idAlerta = EnumAlertaSeccion.COMPLETA.GetHashCode();
                string alerta = EnumAlertaSeccion.COMPLETA.ToString();


                var countResult = await _repositoryAbogado.GetTablaSolicitudTransparenciaCountRepository(idAsunto);

                if (countResult is null || countResult <= 0)
                {
                    idAlerta = EnumAlertaSeccion.NO_REGISTRADO.GetHashCode();
                    alerta = EnumAlertaSeccion.NO_REGISTRADO.ToString();

                    return ResultOperation.SuccessResponseNoMessage(
                        new DataTableViewAlerta<ResponseSolicitudTransparenciaList>(new(idAlerta, alerta),
                        null!)
                    );
                }

                var result = await _repositoryAbogado.GetTablaSolicitudTransparenciaRepository(idAsunto);

                if (result is null)
                {
                    return ResultOperation.SuccessResponseNoMessage(
                    new DataTableViewAlerta<ResponseSolicitudTransparenciaList>(new(idAlerta, alerta),
                    null!)
                );
                }

                return ResultOperation.SuccessResponseNoMessage(
                       new DataTableViewAlerta<ResponseSolicitudTransparenciaList>(new(idAlerta, alerta),
                       result)
                   );

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> DeleteSolicitudTransparenciaService(SolicitudTransparencia entity)
        {
            try
            {

                var result = await _repositoryAbogado.DeleteSolicitudTransparenciaRepository(entity.id);
                if (!result.Success)
                    return ResultOperation<int?>.FailureErrorResponse($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage(result.Result);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

    }


}
