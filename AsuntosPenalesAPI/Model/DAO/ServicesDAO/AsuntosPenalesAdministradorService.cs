using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events.Administrador;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;


namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class AsuntosPenalesAdministradorService : IAsuntosPenalesAdministradorService
    {
        private readonly IApiService _apiService;
        private readonly IAsuntosPenalesAdministradorRepository _repository;
        private readonly IArchivosAsuntosPenalesRepository _repositoryArchivos;
        private readonly string _routeUsuarioInfoDetalle = null!;
        // private readonly string _routeEstadoTarea = null!;
        // private readonly string _routeEstadoProcesal = null!;
        private readonly string _routeAdministracionCentral = null!;
        private readonly string _routeUnidadAdministrativa = null!;
        private readonly string _routeSubadministracion = null!;
        private readonly IAsuntosPenalesRemisionRepository _repositoryRemision;
        private readonly IAsuntosPenalesRepository _repositoryAsuntosPenales;
        private readonly IRedisClient _redisClient;

        public AsuntosPenalesAdministradorService(IConfiguration configuration, IAsuntosPenalesAdministradorRepository repository, IApiService apiService, IArchivosAsuntosPenalesRepository repositoryArchivos, IAsuntosPenalesRemisionRepository repositoryRemision, IAsuntosPenalesRepository repositoryAsuntosPenales, IRedisClient redisClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _routeUsuarioInfoDetalle = configuration.GetValue<string>("CatalogsEndpoints:RouteInfoUsuario")!;
            // _routeEstadoTarea = configuration.GetValue<string>("CatalogsEndpoints:RouteEstadoTarea")!;
            // _routeEstadoProcesal = configuration.GetValue<string>("CatalogsEndpoints:RouteEstadoProcesal")!;
            _routeUnidadAdministrativa = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracion")!;
            _routeSubadministracion = configuration.GetValue<string>("CatalogsEndpoints:RouteSubadministracion")!;
            _routeAdministracionCentral = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracionCentral")!;
            _repositoryArchivos = repositoryArchivos;
            _repositoryRemision = repositoryRemision ?? throw new ArgumentNullException(nameof(repositoryRemision));
            _repositoryAsuntosPenales = repositoryAsuntosPenales ?? throw new ArgumentNullException(nameof(repositoryAsuntosPenales));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));


        }

        public async Task<ResultOperation> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral, int? idAdministracion)
        {
            try
            {
                var countResult = await _repository.GetBandejaCountAsync(idAdminCentral, idAdministracion);

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesAdministradorByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }

                var result = await _repository.GetBandejaAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    idAdminCentral,
                    idAdministracion
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.unidadRealizaSolicitud))
                        item.unidadRealizaSolicitud = "Ministerio Público";
                }

                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseAsuntosPenalesAdministradorByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

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

        public async Task<AsuntosPenales> GetAsuntoPenalById(int id) =>
            await _repositoryAsuntosPenales.GetByIdAsync(id);

        public async Task<ResultOperation<int>> UpdateAsuntoPenalAdministrador(AsuntosPenales entity)
        {
            try
            {
                // DateTime dateTime = DateTime.Now;
                // if (entity.fecha_presentacion.Date > dateTime.Date)
                // {
                //     return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
                // }

                // if (entity.fecha_recepcion.Date > dateTime.Date)
                // {
                //     return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser mayor a la fecha actual.");
                // }

                // if (entity.fecha_recepcion.Date < entity.fecha_presentacion.Date)
                // {
                //     return ResultOperation.FailureWarningResponse<int>("La fecha de presentación no puede ser menor a la fecha de recepción.");
                // }

                //entity.fecha_vencimiento = entity.fecha_recepcion.AddDays(90).Date;

                var result = await _repository.UpdateAsuntosPenalesAdminAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result.GetValueOrDefault(), "El registro se actualizó correctamente.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
        string? numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string? oficiosolicitud, string? numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, int? id_administracion_adscrita, bool? tipo_unidad
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
                    tipo_unidad
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesAdministradorByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
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
                    tipo_unidad
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesAdministradorByFilters>>("No se encontraron resultados");
                }

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.unidadRealizaSolicitud))
                        item.unidadRealizaSolicitud = "Ministerio Público";
                }


                return ResultOperation.SuccessResponse(
                    new DataTableView<ResponseAsuntosPenalesAdministradorByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation<ResponseAsignar>> UpdateAsignarAsuntosPenales(AsuntosPenales entity, AsuntosPenalesAbogado entityAbogado)
        {
            try
            {
                var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>($"{_routeUsuarioInfoDetalle}/{entityAbogado.id_abogado}");
                if (responseAbogado is null || !responseAbogado.Success || responseAbogado.Result is null)
                {
                    return ResultOperation.FailureErrorResponse<ResponseAsignar>($"No se pudo realizar la validación del abogado.");
                }

                // if (!UserSession.ValidateUser(responseAbogado.Result, EnumRolesSicoj.ABOGADO, EnumModulosSicoj.ASUNTOS_PENALES, out string message, false, null, true, entity.id_administracion_central, true, entity.id_admin_controla, true, entity.id_subadministracion))
                // {
                //     return ResultOperation.FailureErrorResponse<ResponseAsignar>($"{message}");
                // }

                var result = await _repository.AsignarAsuntosPenalesAsync(entity, entityAbogado);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<ResponseAsignar>($"{result.MsgError!}:{result.DetailError}");


                return ResultOperation.SuccessResponseNoMessage(new ResponseAsignar()
                {
                    noAsunto = entity.numero_asunto_penal!,
                    abogado = responseAbogado.Result.Nombre!
                });

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> RemitirAsync(AsuntosPenales entity, AsuntosPenalesRemision entityRemision, ArchivosAsuntosPenales entityDocumento, DataFile dataFile)
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                if (entityRemision.fecha_oficio.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de oficio no puede ser mayor a la fecha actual.");
                }

                var result = await _repository.RemitirAsync(entity, entityRemision, entityDocumento, dataFile);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponseNoMessage();
            }
            catch (Exception)
            {
                throw;
            }
        }




        public async Task<ResultOperation<int>> AddArchivosAsyncService(ArchivosAsuntosPenales entity, DataFile dataFile)
        {

            try
            {

                var result = await _repositoryArchivos.AddFileAsyncRepository(entity, dataFile);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result.GetValueOrDefault(), "El registro de agregó correctamente.");
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


        public async Task<ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(int id_registro)
        {
            var result = await _repositoryArchivos.GetArchivosByIdRegistroAsync_Repository(id_registro);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseArchivosAsuntosPenales>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }


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
                var result = await _repository.AnalisisAsync(
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

        public async Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdAnalisisDisconnected(
           int id
       )
        {
            var response = await _repository.GetAsuntosPenalesByIdDisconnectedAnalisisAsync(
                id
            );

            if (response is null)
                return null!;
            var resultOperation = ResultOperation.SuccessResponseNoMessage(response);
            if (response.id_determinacion_asunto!.Value > 0)
            {
                var responseDeterminacionAsuntoPenal =
                    await _redisClient.GetCatalogValue(EnumCatalogos.DeterminacionAsuntoPenal, response.id_determinacion_asunto.Value.ToString());
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

        public async Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id) =>
            await _repository.GetAsuntosPenalesByIdAnalisis(id);

        public async Task<ResultOperation> UpdateAnalisis(AsuntosPenalesAnalisis entityAnalisis)
        {
            try
            {
                var result = await _repository.UpdateAnalisisAsync(entityAnalisis);
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

        public async Task<ResultOperation> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        )
        {
            try
            {
                var result = await _repository.ProcedibilidadAsync(
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

        public async Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdProcedibilidadDisconnected(
            int id
        )
        {
            var response =
                await _repository.GetAsuntosPenalesByIdDisconnectedProcedibilidadAsync(id);

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

        public async Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal) =>
            await _repository.GetImputados(idAsuntoPenal);

        public async Task<ResultOperation> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        )
        {
            try
            {
                var result = await _repository.ImputadosAsync(entity, entityImputados);
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

        public async Task<ResultOperation> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        )
        {
            try
            {
                var result = await _repository.PersonasMoralesAsync(
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

        public async Task<ResultOperation<List<ResponsePersonasMorales>>> GetAllPersonasMorales(
            int idAsuntoPenal
        )
        {
            var result = await _repository.GetPersonasMoralesDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponsePersonasMorales>>(
                    new()
                );
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos)
        {
            try
            {
                var result = await _repository.DelitosAsync(entityDelitos);
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

        public async Task<AsuntosPenalesDelitos> GetDelitoById(int id) =>
            await _repository.GetByIdDelitosAsync(id);

        public async Task<ResultOperation> DeleteDelitos(AsuntosPenalesDelitos entity)
        {
            try
            {
                var result = await _repository.DeleteDelitos(entity);
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

        public async Task<ResultOperation<List<ResponseDelitos>>> GetAllDelitos(int idAsuntoPenal)
        {
            var result = await _repository.GetDelitosDisconnected(idAsuntoPenal);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseDelitos>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaInicial(int id)
        {
            try
            {
                var result = await _repository.GetImputadoByIdDisconnectedEtapaInicial(id);
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
                        await _repository.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }

                    var listResultCriterio =
                        await _repository.GetCriterioOportunidadByImputadoDisconnected(
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

        public async Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repository.GetAcuerdoReparatorioByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repository.GetCriterioOportunidadByImputado(
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
                var result = await _repository.UpdateImputadosEtapaInicialAsync(
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

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaComplementaria(int id)
        {
            try
            {
                var result =
                    await _repository.GetImputadoByIdDisconnectedEtapaComplementaria(id);
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
                        await _repository.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }
                    var listResultSobreseimiento =
                        await _repository.GetSobreseimientoByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                        );
                    if (listResultSobreseimiento is not null && listResultSobreseimiento.Any())
                    {
                        resultOperation.Result.sobreseimiento =
                            listResultSobreseimiento.FirstOrDefault();
                    }

                    var listResultSuspensionCondicional =
                        await _repository.GetSuspencionCondicionalByImputadoDisconnected(
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
                        await _repository.GetSentenciaByImputadoDisconnected(
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
                var result = await _repository.UpdateImputadosEtapaComplementariaAsync(
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

        public async Task<List<AsuntosPenalesSentencia>> GetSentenciaByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) => await _repository.GetSentenciaByImputado(idImputado, idTipoEtapaInvestigacion);

        public async Task<List<AsuntosPenalesSobreseimiento>> GetSobreseimientoByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        ) =>
            await _repository.GetSobreseimientoByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<
            List<AsuntosPenalesSuspensionCondicional>
        > GetSuspencionCondicionalByImputado(int idImputado, int idTipoEtapaInvestigacion) =>
            await _repository.GetSuspencionCondicionalByImputado(
                idImputado,
                idTipoEtapaInvestigacion
            );

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaIntermedia(int id)
        {
            try
            {
                var result = await _repository.GetImputadoByIdDisconnectedEtapaIntermedia(
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
                        await _repository.GetAcuerdoReparatorioByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (listResultAcuerdo is not null && listResultAcuerdo.Any())
                    {
                        resultOperation.Result.acuerdoReparatorio =
                            listResultAcuerdo.FirstOrDefault();
                    }
                    var listResultSobreseimiento =
                        await _repository.GetSobreseimientoByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                        );
                    if (listResultSobreseimiento is not null && listResultSobreseimiento.Any())
                    {
                        resultOperation.Result.sobreseimiento =
                            listResultSobreseimiento.FirstOrDefault();
                    }

                    var listResultSuspensionCondicional =
                        await _repository.GetSuspencionCondicionalByImputadoDisconnected(
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
                        await _repository.GetSentenciaByImputadoDisconnected(
                            result.Id,
                            EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
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
                var result = await _repository.UpdateImputadosEtapaIntermediaAsync(
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

        public async Task<ResultOperation> GetImputadoByIdDisconnectedEtapaJuicio(int id)
        {
            try
            {
                var result = await _repository.GetImputadoByIdDisconnectedEtapaJuicio(id);
                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<ResponseImputadosEtapaJuicio>(
                        "No se encontraron resultados"
                    );
                }

                var resultOperation = ResultOperation.SuccessResponseNoMessage(result);

                var listResultSentencia =
                    await _repository.GetSentenciaByImputadoDisconnected(
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

                var result = await _repository.UpdateImputadosEtapaJuicioAsync(
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

        public async Task<ResultOperation> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        )
        {
            try
            {
                var result = await _repository.MedidasCautelaresAsync(
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

        public async Task<ResultOperation<List<ResponseMedidasCautelares>>> GetAllMedidasCautelares(
            int idImputado
        )
        {
            var result = await _repository.GetMedidasCautelaresDisconnected(idImputado);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseMedidasCautelares>>(
                    new()
                );
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }

        public async Task<AsuntosPenalesMedidasCautelares> GetMedidaCautelarById(int id) =>
            await _repository.GetByIdMedidasCautelaresAsync(id);

        public async Task<ResultOperation> DeleteMedidasCautelares(
            AsuntosPenalesMedidasCautelares entity
        )
        {
            try
            {
                var result = await _repository.DeleteMedidasCautelares(entity);
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
          
        #region Solicitud Transparencia

        public async Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile)
        {
            try
            {
                var consultaDetails = await _repository.GetAdminAsuntoPenalById(entity.id_asunto);
                if (consultaDetails == null)
                {
                    return ResultOperation<int?>.FailureErrorResponse("No se pudo encontrar el Asunto Penal con el ID especificado.");
                }

                DateTime dateTime = DateTime.Now;
                if (entity.fechaSolicitud.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de solicitud no debe ser mayor a la fecha actual.");
                }


                var result = await _repository.AddAsyncSolicitudTransparenciaService(entity, entityDocumento, dataFile);
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
        await _repository.GetByIdSolicitudTransparenciaRepository(id);

        public async Task<ResultOperation> UpdateSolicitudTransparenciaService(SolicitudTransparencia entity)
        {
            try
            {
                var consultaDetails = await _repository.GetAdminAsuntoPenalById(entity.id_asunto);
                if (consultaDetails == null)
                {
                    return ResultOperation<int?>.FailureErrorResponse("No se pudo encontrar la consulta con el ID especificado.");
                }

                DateTime dateTime = DateTime.Now;
                if (entity.fechaSolicitud.Date > dateTime.Date)
                {
                    return ResultOperation<int?>.FailureWarningResponse<int?>("La fecha de solicitud no debe ser mayor a la fecha actual.");
                }

                var result = await _repository.UpdateSolicitudTransparenciaRepository(entity);
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
                var result = await _repository.GetByIdSolicitudTransparenciaRepositorys(id);
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


                var countResult = await _repository.GetTablaSolicitudTransparenciaCountRepository(idAsunto);

                if (countResult is null || countResult <= 0)
                {
                    idAlerta = EnumAlertaSeccion.NO_REGISTRADO.GetHashCode();
                    alerta = EnumAlertaSeccion.NO_REGISTRADO.ToString();

                    return ResultOperation.SuccessResponseNoMessage(
                        new DataTableViewAlerta<ResponseSolicitudTransparenciaList>(new(idAlerta, alerta),
                        null!)
                    );
                }

                var result = await _repository.GetTablaSolicitudTransparenciaRepository(idAsunto);

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

                var result = await _repository.DeleteSolicitudTransparenciaRepository(entity.id);
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