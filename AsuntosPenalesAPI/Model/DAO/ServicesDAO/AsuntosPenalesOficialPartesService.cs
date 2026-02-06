using System.Text.Json;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Models;
using Sicoj.Utils.Models.Responses;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class AsuntosPenalesOficialPartesService : IAsuntosPenalesOficialPartesService
    {
        private readonly IAsuntosPenalesOficialPartesRepository _repository;
        private readonly IArchivosAsuntosPenalesRepository _repositoryArchivos;
        private readonly IAsuntosPenalesRepository _repositoryAsuntosPenales;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IApiService _apiService;
        private readonly IRedisClient _redisClient;
        //private readonly ControlDoc
        // private readonly string _routeEstadoTarea = null!;
        private readonly string _routeUpdNumero = null!;
        private readonly string _routeAdministracionCentral = null!;
        private readonly string _routeUnidadAdministrativa = null!;
        public AsuntosPenalesOficialPartesService(IConfiguration configuration, IAsuntosPenalesOficialPartesRepository repository, IArchivosAsuntosPenalesRepository repositoryArchivos, IApiService apiService, IAsuntosPenalesRepository repositoryAsuntosPenales, IRedisClient redisClient, IOptions<ProxyEnpoints> proxyEnpoints)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _repositoryArchivos = repositoryArchivos ?? throw new ArgumentNullException(nameof(repositoryArchivos));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            // _routeEstadoTarea = configuration.GetValue<string>("CatalogsEndpoints:RouteEstadoTarea")!;
            // _routeEstadoProcesal = configuration.GetValue<string>("CatalogsEndpoints:RouteEstadoProcesal")!;
            _routeUnidadAdministrativa = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracion")!;
            _routeAdministracionCentral = configuration.GetValue<string>("CatalogsEndpoints:RouteAdministracionCentral")!;
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _repositoryAsuntosPenales = repositoryAsuntosPenales ?? throw new ArgumentNullException(nameof(repositoryAsuntosPenales));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _routeUpdNumero = configuration.GetValue<string>("ControlDocumental:RouteUpdNumeroAsunto")!;
        }


        public async Task<ResultOperation> GetBandejaAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAdminCentral)
        {
            try
            {
                var countResult = await _repository.GetBandejaCountAsync(idAdminCentral);

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesOficialPartesByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
                }
                Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);
                var result = await _repository.GetBandejaAsync(
                    Fetch,
                    Page,
                    OrderByColumn,
                    OrderDesc,
                    idAdminCentral
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse("No se encontraron resultados");
                }

                result.ForEach(c =>
                {
                    if (string.IsNullOrEmpty(c.unidadRealizaSolicitud))
                        c.unidadRealizaSolicitud = "Ministerio Público";
                });

                _redisClient.ValidateTakeList(ref result, EnumModulosRedis.ASUNTOS_PENALES);

                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAsuntosPenalesOficialPartesByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> GetHistoricoAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc,
        string numeroasuntopenal, DateTime? fecharecepcion_desde, DateTime? fecharecepcion_hasta, DateTime? fechavencimiento_desde, DateTime? fechavencimiento_hasta, string oficiosolicitud, string numeroexpedientecadido, int? id_unidadrealizasolicitud, int? admin_control, int? id_subadministracion, string? nombre_abogado, int? id_estadotarea, int? id_estadoprocesal, bool? tipo_unidad, int? id_administracion_central
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
                    tipo_unidad,
                    id_administracion_central
                );

                if (countResult is null || countResult <= 0)
                {
                    return ResultOperation.SuccessResponseNoMessage(new DataTableView<ResponseAsuntosPenalesOficialPartesByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
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
                    tipo_unidad,
                    id_administracion_central
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesOficialPartesByFilters>>("No se encontraron resultados");
                }

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.unidadRealizaSolicitud))
                        item.unidadRealizaSolicitud = "Ministerio Público";
                }
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseAsuntosPenalesOficialPartesByFilters>(new(Page, Fetch, countResult.GetValueOrDefault()),
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseAsuntosPenalesById> GetAsuntosPenalesByIdDisconnected(int id)
        {
            var response = await _repository.GetAsuntosPenalesByIdDisconnectedAsync(id);

            if (response is null)
                return null!;

            _redisClient.ValidateTake(response, EnumModulosRedis.ASUNTOS_PENALES);
            var resultOperation = ResultOperation.SuccessResponseNoMessage(response);
            if (response.idEstadoTarea!.Value > 0)
            {
                var responseEstadoTarea = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoTarea, response.idEstadoTarea.Value.ToString());
                if (!string.IsNullOrEmpty(responseEstadoTarea))
                {
                    resultOperation.Result.estadoTarea = responseEstadoTarea;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            if (response.idEstadoProcesal!.Value > 0)
            {
                var responseEstadoProcesal = await _redisClient.GetCatalogValue(EnumCatalogos.EstadoProcesalPenales, response.idEstadoProcesal.Value.ToString());
                if (!string.IsNullOrEmpty(responseEstadoProcesal))
                {
                    resultOperation.Result.estadoProcesal = responseEstadoProcesal;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            if (response.idUnidadRealizaSolicitud.HasValue && response.idUnidadRealizaSolicitud!.Value > 0)
            {
                var responseAdministracionCentral = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                        $"{_routeAdministracionCentral}/{response.idUnidadRealizaSolicitud.Value}"
                    );
                if (responseAdministracionCentral is not null && responseAdministracionCentral.Success && responseAdministracionCentral.Result is not null)
                {
                    resultOperation.Result.unidadRealizaSolicitud = responseAdministracionCentral.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            if (response.idAdminControla!.Value > 0)
            {
                var responseAdministracion = await _apiService.GetResultOperationAsync<CatalogSicoj>(
                        $"{_routeUnidadAdministrativa}/{response.idAdminControla.Value}"
                    );
                if (responseAdministracion is not null && responseAdministracion.Success && responseAdministracion.Result is not null)
                {
                    resultOperation.Result.adminControla = responseAdministracion.Result.nombre;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar el nombre del tipo asunto.");
            }
            if (string.IsNullOrEmpty(response.unidadRealizaSolicitud))
                response.unidadRealizaSolicitud = "Ministerio Público";
            return response;

        }



        public async Task<AsuntosPenales> GetAsuntosPenalesById(int id) =>
        await _repositoryAsuntosPenales.GetByIdAsync(id);


        public async Task<ResultOperation> AddFisico(AsuntosPenales entity)
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                if (entity.fecha_recepcion.Date > dateTime.Date)
                {
                    return ResultOperation.FailureWarningResponse<int?>("La fecha de recepcion  no puede ser mayor a la fecha actual.");
                }
                var result = await _repository.AddAsuntosPenalesAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result, "El registro de agregó correctamente.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> AddControlDocumental(AsuntosPenales entity)
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                if (entity.fecha_recepcion.Date > dateTime.Date)
                {
                    return ResultOperation.FailureWarningResponse<int?>("La fecha de recepcion  no puede ser mayor a la fecha actual.");
                }
                var result = await _repository.AddControlDocumentalAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result, "El registro de agregó correctamente.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> UpdateAsuntosPenales(AsuntosPenales entity)
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                if (entity.fecha_recepcion.Date > dateTime.Date)
                {
                    return ResultOperation.FailureWarningResponse<int?>("La fecha de recepcion  no puede ser mayor a la fecha actual.");
                }
                var result = await _repository.UpdateAsuntosPenalesAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result, "El registro se actualizo correctamente.");
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ResultOperation<ResponseTurnado>> UpdateTurnarAsuntosPenales(AsuntosPenales entity)
        {
            try
            {

                var result = await _repository.TurnarAsuntosPenalesAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<ResponseTurnado>($"{result.MsgError!}:{result.DetailError}");

                var resultOperation = ResultOperation.SuccessResponse(new ResponseTurnado(), "El registro se turno  correctamente.");

                var entityExists = await _repositoryAsuntosPenales.GetByIdAsync(result.Result.GetValueOrDefault());
                if (entityExists is not null)
                {
                    resultOperation.Result.noAsuntoPenal = entityExists.numero_asunto_penal!;
                }
                else
                    resultOperation.AddWarningMessage("No se pudo recuperar la información del asunto penal despues de turnarla.");

                var request = new RequestActualizarNumeroAsunto()
                {
                    id = entity.id,
                    numeroAsunto = resultOperation.Result.noAsuntoPenal,
                    idModulo = 4,
                    idTipoAsunto = 0
                };

                var response = await _apiService.PostAsync<RequestActualizarNumeroAsunto>(_routeUpdNumero, request);
                return resultOperation;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ResultOperation> DeleteAsuntosPenales(AsuntosPenales entity)
        {
            try
            {

                var result = await _repository.DeleteAsuntosPenalesAsync(entity);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>($"{result.MsgError!}:{result.DetailError}");
                return ResultOperation.SuccessResponse(result.Result, "El registro se elimino  correctamente.");
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

        public async Task<ResultOperation<List<ResponseArchivosAsuntosPenales>>> GetAllArchivoService(int id_registro)
        {
            var result = await _repositoryArchivos.GetArchivosByIdRegistroAsync_Repository(id_registro);
            if (result is null || !result.Any())
            {
                return ResultOperation.SuccessResponseNoMessage<List<ResponseArchivosAsuntosPenales>>(new());
            }
            return ResultOperation.SuccessResponseNoMessage(result);
        }


        public async Task<ArchivosAsuntosPenales> GetByIdArchivoService(int id) =>
        await _repositoryArchivos.GetByIdArchivoAsyncRepository(id);

        public async Task<ArchivosAsuntosPenales> GetByIdArchivoDeleteService(int id) =>
        await _repositoryArchivos.GetByIdArchivoAsyncRepository(id);

        public async Task<ResponseNumeroAsunto> GetAsuntosPenalesByNumeroAsunto(string numero_asunto)
        {
            var response = await _repository.GetAsuntosPenalesByNumeroAsuntoAsync(numero_asunto);

            if (response is null)
                return null!;

            return response;

        }


        #region Solicitud Transparencia

        public async Task<ResultOperation> AddSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile)
        {
            try
            {
                var consultaDetails = await _repository.GetAsuntosPenalesByIdDisconnectedAsync(entity.id_asunto);
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
                var consultaDetails = await _repository.GetAsuntosPenalesByIdDisconnectedAsync(entity.id_asunto);
                if (consultaDetails == null)
                {
                    return ResultOperation<int?>.FailureErrorResponse("No se pudo encontrar el Asunto con el ID especificado.");
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
        public async Task<ResultOperation> GetFechasVencimientoAsync(
        string? fecha_inicial,
        string? fecha_final,
        List<int>? secciones
        )
        {
            try
            {
                DateTime parsedDate = DateTime.Parse(fecha_inicial!);
                string formattedDate = parsedDate.ToString("yyyy-MM-dd");
                DateTime parsedDatefinal = DateTime.Parse(fecha_final!);
                string formattedDate2 = parsedDatefinal.ToString("yyyy-MM-dd");
                DateTime? fechaInicialDate = null;
                DateTime? fechaFinalDate = null;

                if (DateTime.TryParse(formattedDate, out DateTime parsedFechaInicial))
                {
                    fechaInicialDate = parsedFechaInicial;
                }

                if (DateTime.TryParse(formattedDate2, out DateTime parsedFechaFinal))
                {
                    fechaFinalDate = parsedFechaFinal;
                }

                var result = await _repository.FechasVencimientoAsync(
                    fechaInicialDate,
                    fechaFinalDate,
                    secciones
                );

                if (result is null)
                {
                    return ResultOperation.FailureWarningResponse<List<ResponseFechaVencimientoSeccion>>("No se encontraron resultados");
                }

                foreach (var item in result)
                {
                    item.idSeccion = 7;
                    item.idModulo = 4;
                    item.idSeccionRenglon = null;
                    item.mesesCalendario = 4;
                }

                return ResultOperation.SuccessResponseNoMessage(
                    new List<ResponseFechaVencimientoSeccion>(
                    result)
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResultOperation> UpdateAsuntosPenalesMasivo(
            string? fecha_vencimiento,
            int? idAsunto,
            int? idModulo,
            int? idSeccion,
            int? idSeccionRenglon
        )
        {
            try
            {
                if (idModulo != 4)
                {
                    return ResultOperation<int>.FailureErrorResponse("No se pueden actualizar asusntos que sean de otro modulo.");
                }
                DateTime fechaVencimiento = DateTime.Parse(fecha_vencimiento!);
                var result = await _repository.UpdateAsuntosPenalesMasivoAsync(fechaVencimiento, idAsunto, idModulo, idSeccion, idSeccionRenglon);
                if (!result.Success)
                    return ResultOperation.FailureErrorResponse<int?>($"{result.MsgError!}:{result.DetailError}");

                return ResultOperation.SuccessResponse(result.Result, "La actualización ha sido exitosa");
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #region Envio email
        public async Task<ResultOperation> EnvioEmail(Email entity)

        {
            try
            {
                using var httpClient = new HttpClient();

                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(string.Join(",", entity.To)), "To");
                formData.Add(new StringContent(string.Join(",", entity.Cc)), "Cc");
                formData.Add(new StringContent(entity.Subject), "Subject");
                formData.Add(new StringContent(entity.Body), "Body");
                formData.Add(new StringContent(entity.IsHtml.ToString()), "IsHtml");
                formData.Add(new StringContent(entity.Priority?.ToString() ?? string.Empty), "Priority");


                var response = await httpClient.PostAsync($"{_proxyEnpoints.RouteEmail}", formData);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error al enviar correo: {response.StatusCode}");
                }


                return ResultOperation.SuccessResponseNoMessage(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}