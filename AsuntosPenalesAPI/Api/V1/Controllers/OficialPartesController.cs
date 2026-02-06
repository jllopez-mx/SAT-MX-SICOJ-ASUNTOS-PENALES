using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.DTO.ContractsValidations;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using AsuntosPenalesAPI.Model.Entities.Events.OficialPartes;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;
using Sicoj.Utils.ViewModels;
using System.Linq.Expressions;
using Microsoft.OpenApi.Expressions;
using Sicoj.Utils.Files;
using AsuntosPenalesAPI.Model.Entities.Events;
using AsuntosPenalesAPI.Model.DAO.ServicesDAO;
using Sicoj.Utils.Extentions;
using FluentValidation;




namespace AsuntosPenalesAPI.Api.V1.Controllers
{
    [ApiController]
    [Route("sicoj/asuntos-penales/api/v1/oficial-partes/asuntos-penales")]
    public class OficialPartesController : ControllerBase
    {
        #region Variables / Contructor

        private readonly IRedisClient _redisClient;
        private static object _lock = new object();
        private readonly ILogger<OficialPartesController> _logger;
        private readonly IAsuntosPenalesOficialPartesService _asuntosPenalesOficialPartesService;
        private readonly RequestCreateAsuntosPenalesValidator _requestCreateAsuntosPenalesValidator;
        private readonly RequestCreateControlDocumentalValidator _requestCreateControlDocumentalValidator;
        private readonly RequestUpdateTurnarAsuntosPenalesValidator _updateTurnarAsuntosPenalesValidator;
        private readonly RequestUpdateAsuntosPenalesValidator _updateAsuntosPenalesValidator;
        private readonly RequestDeleteAsuntosPenalesValidator _deleteAsuntosPenalesValidator;
        private readonly RequestDeleteArchivosAsuntosPenalesValidator _deleteArchivosAsuntosPenalesValidator;
        private readonly IValidator<RequestCreateFileAsuntosPenales> _createArchivoValidator;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;
        private readonly IValidator<RequestCreateSolicitudTransparencia> _validatorRequestCreateSolicitudTransparencia;
        private readonly IValidator<RequestUpdateSolicitudTransparencia> _validatorRequestUpdateSolicitudTransparencia;
        private readonly IFileSystemService _fileSystemService;
        private readonly IGenericService _genericService;


        public OficialPartesController(ILogger<OficialPartesController> logger,
        IAsuntosPenalesOficialPartesService asuntosPenalesOficialPartesService,
        RequestCreateAsuntosPenalesValidator requestCreateAsuntosPenalesValidator,
        RequestCreateControlDocumentalValidator requestCreateControlDocumentalValidator,
        IRedisClient redisClient,
        RequestUpdateTurnarAsuntosPenalesValidator updateTurnarAsuntosPenalesValidator,
        RequestUpdateAsuntosPenalesValidator updateAsuntosPenalesValidator,
        RequestDeleteAsuntosPenalesValidator deleteAsuntosPenalesValidator,
        RequestDeleteArchivosAsuntosPenalesValidator deleteArchivosAsuntosPenalesValidator,
        IValidator<RequestCreateSolicitudTransparencia> validatorRequestCreateSolicitudTransparencia,

        IValidator<RequestUpdateSolicitudTransparencia> validatorUpdateSolicitudTransparencia,
        IFileSystemService fileSystemService,
        IGenericService genericService,
        IValidator<RequestCreateFileAsuntosPenales> createArchivoValidator,
        IValidator<RequestDocumentoUpdate> requestDocumentoValidator)
        {
            _logger = logger;
            _asuntosPenalesOficialPartesService = asuntosPenalesOficialPartesService ?? throw new ArgumentNullException(nameof(asuntosPenalesOficialPartesService));
            _requestCreateAsuntosPenalesValidator = requestCreateAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(requestCreateAsuntosPenalesValidator));
            _requestCreateControlDocumentalValidator = requestCreateControlDocumentalValidator ?? throw new ArgumentNullException(nameof(requestCreateControlDocumentalValidator));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _updateTurnarAsuntosPenalesValidator = updateTurnarAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(updateTurnarAsuntosPenalesValidator));
            _updateAsuntosPenalesValidator = updateAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(updateAsuntosPenalesValidator));
            _deleteAsuntosPenalesValidator = deleteAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(deleteAsuntosPenalesValidator));
            _deleteArchivosAsuntosPenalesValidator = deleteArchivosAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(deleteArchivosAsuntosPenalesValidator));
            _validatorRequestCreateSolicitudTransparencia = validatorRequestCreateSolicitudTransparencia ?? throw new ArgumentNullException(nameof(validatorRequestCreateSolicitudTransparencia));
            _validatorRequestUpdateSolicitudTransparencia = validatorUpdateSolicitudTransparencia ?? throw new ArgumentNullException(nameof(validatorUpdateSolicitudTransparencia));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _createArchivoValidator = createArchivoValidator ?? throw new ArgumentNullException(nameof(createArchivoValidator));
            _requestDocumentoUpdateValidator = requestDocumentoValidator ?? throw new ArgumentNullException(nameof(requestDocumentoValidator));
        }

        #endregion
        /// <summary>
        /// Método para la bandeja de pendientes de abogado
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesOficialPartesByFilters>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetBandeja([FromQuery] PagerQuery request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                if (!Filters.MapSort<EnumOrderColumnAsuntosPenales>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesOficialPartesByFilters>>("La columna de ordenamiento no es válida.")
                        );
                }

                var result = await _asuntosPenalesOficialPartesService.GetBandejaAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    sessionInformation.UserInformation.IdAdministracionCentral
                );
                if (result is null)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse("No se encontraron resultados")
                    );
                }
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Método para obtener el historico de Asuntos Penale para el oficial de partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesOficialPartesByFilters>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("historico")]
        public async Task<IActionResult> GetHistoricoAsync([FromQuery] PagerQueryFilters request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                RequestFiltrosAsuntosPenales filters = new();
                Filters.MapFilters(request, filters);


                if (!Filters.MapSort<EnumOrderColumnAsuntosPenales>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<RequestFiltrosAsuntosPenales>>("La columna de ordenamiento no es válida.")
                        );
                }

                var result = await _asuntosPenalesOficialPartesService.GetHistoricoAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    Filters.GetStringValue(filters!.ByNoAsuntoPenal.FirstOrDefault())!,
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionHasta.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByOficioSolicitud.FirstOrDefault())!,
                    Filters.GetStringValue(filters.ByNoExpedienteCadido.FirstOrDefault())!,
                    Filters.GetIntValue(filters.ByIdUnidadRealizoSolicitud.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByAdminControl.FirstOrDefault()),
                    Filters.GetIntValue(filters.BySubadministracion.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByNombreAbogado.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByIdEstadoTarea.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByIdEstadoProcesal.FirstOrDefault()),
                    Filters.GetBoolValue(filters.ByTipoUnidad.FirstOrDefault()),
                    sessionInformation.UserInformation.IdAdministracionCentral
                    );

                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        ///Método para consultar Asuntos Penales por Id
        /// </summary>
        /// <param name="id">Id Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsuntosPenalesById(int id)
        {
            try
            {

                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var result = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesByIdDisconnected(id);
                if (result is null)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse("No se encontraron resultados")
                    );
                }
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        /// Método para agregar un asunto penal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost()]
        public async Task<IActionResult> Post(
            RequestCreateAsuntosPenales request
        )
        {
            try
            {

                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }


                var validationResult = await _requestCreateAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                AsuntosPenales entity = null!;
                try

                {
                    entity = AsuntosPenalesOficialPartesEvents.CreateModalidadFisico(
                        request.numero_expediente_cadido,
                        request.oficio_solicitud,
                        DateTime.Parse(request.fecha_recepcion),
                        DateTime.Parse(request.fecha_vencimiento),
                        request.interno,
                        request.id_unidad_realiza_solicitud,
                        request.id_admin_controla,
                        sessionInformation.UserInformation.IdAdministracionCentral
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesOficialPartesService.AddFisico(entity);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Método para agregar un asunto penal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("ControlDocumental")]
        public async Task<IActionResult> PostDocumental(
            RequestCreateControlDocumental request
        )
        {
            try
            {

                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                var validationResult = await _requestCreateControlDocumentalValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                AsuntosPenales entity = null!;
                try

                {
                    entity = AsuntosPenalesOficialPartesEvents.CreateControlDocumental(
                        request.oficio_solicitud,
                        DateTime.Parse(request.fecha_recepcion_solicitud),
                        request.id_unidad_realiza_solicitud,
                        sessionInformation.UserInformation.IdAdministracionCentral
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesOficialPartesService.AddControlDocumental(entity);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Método para turnar asuntos penales 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("turnar")]
        public async Task<IActionResult> PatchTurnar(
            RequestUpdateTurnarAsuntosPenales request
        )
        {

            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.id}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede turnar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var validationResult = await _updateTurnarAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La asunto penal a turnar no existe."
                        )
                    );

                }
                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La asunto penal ya se encuentra eliminado, no puede ser turnado."
                        )
                    );
                    }
                    if (entityExists.turnado)
                    {
                        return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La asunto penal ya se encuentra turnado."
                        )
                    );
                    }
                }

                try
                {
                    AsuntosPenalesOficialPartesEvents.Updateturnar(ref entityExists!,
                        request.numero_empleado,
                        request.id_admin_controla
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesOficialPartesService.UpdateTurnarAsuntosPenales(entityExists);
                return Ok(result);


            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }



        /// <summary>
        ///Método tomar el registro
        /// </summary>
        /// <returns></returns>

        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("tomar")]
        public async Task<IActionResult> PatchTomar([FromQuery] int id)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                EnumModulosRedis enumModulo = default!;
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<bool>(
                            "El asunto penal no existe."
                        )
                    );
                }

                if (!entityExists.activo)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede tomar."
                    )
                );
                }
                var response = await _redisClient.Take(enumModulo, id, sessionInformation.UserInformation.Rfc!, sessionInformation.UserInformation.Nombre!);
                return Ok(response);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }



        /// <summary>
        ///Método soltar el registro
        /// </summary>
        /// <returns></returns>     
        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("soltar")]
        public async Task<IActionResult> PatchSoltar([FromQuery] int id)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                EnumModulosRedis enumModulo = default!;
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<bool>(
                            "El asunto penal no existe."
                        )
                    );
                }

                var result = await _redisClient.Drop(enumModulo, id, sessionInformation.UserInformation.Rfc!);
                return Ok(result);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        /// Método para actualizar asuntos penales 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("asuntos-penales")]
        public async Task<IActionResult> PatchAsuntoPenal(
            RequestUpdateAsuntosPenales request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.id}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var validationResult = await _updateAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal para actualizar no existe."
                        )
                    );
                }

                if (entityExists is not null)
                {
                    if (entityExists.activo == false)
                    {
                        return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado."
                    )
                );
                    }
                }
                try
                {
                    AsuntosPenalesOficialPartesEvents.UpdateGuardarModalidadFisico(ref entityExists!,
                        request.numero_expediente_cadido,
                        request.oficio_solicitud,
                        DateTime.Parse(request.fecha_recepcion),
                        DateTime.Parse(request.fecha_vencimiento),
                        request.id_unidad_realiza_solicitud,
                        request.id_admin_controla,
                        request.interno
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesOficialPartesService.UpdateAsuntosPenales(entityExists);
                return Ok(result);


            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        /// Método para actualizar asuntos penales
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete]
        public async Task<IActionResult> Delete(
            RequestDeleteAsuntosPenales request
        )
        {
            try
            {
                var validationResult = await _deleteAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La asunto penal que desea eliminar no existe."
                        )
                    );
                }

                if (!entityExists.activo)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede eliminar."
                    )
                );
                }

                if (entityExists.turnado)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "El asunto penal ya se encuentra turnado, no se puede eliminar."
                    )
                );
                }

                try
                {
                    AsuntosPenalesOficialPartesEvents.DeleteAsuntosPenales(ref entityExists

                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesOficialPartesService.DeleteAsuntosPenales(entityExists);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        [ProducesResponseType(typeof(ResultOperation<ResponseArchivosAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("archivo/historico")]
        public async Task<IActionResult> GetDocumentosHistoricoAsync([FromQuery] PagerQuery request, int idAsuntoPenal, int? idRenglonSeccion)
        {
            try
            {

                if (!Filters.MapSort<EnumOrderColumnDocumentosByFiltros>(request, null!, false, out string orderByColumn, out bool orderDesc))
                {
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseArchivosAsuntosPenales>>("La columna de ordenamiento no es válida."));
                }

                var result = await _genericService.GetDocumentosAsync(
                        request.fetch,
                        request.page,
                        orderByColumn,
                        orderDesc,
                        idAsuntoPenal,
                        idRenglonSeccion);

                return Ok(result);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        ///Método para Agregar un archivo al registro de asuntos penales 
        /// </summary>
        /// <param name="request">Datos del archivo y del registro de asuntos penales</param>
        /// <returns>Id del registro de archivo agregado</returns>
        [ProducesResponseType(typeof(ResultOperation<bool>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("archivo")]
        public async Task<IActionResult> PostFile(
            [FromForm] RequestCreateFileAsuntosPenales
            request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _createArchivoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }


                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntosPenales}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha apartado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.idAsuntosPenales);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede agregar un archivo."
                    )
                );
                }

                EnumFileType[] requiredExtention = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                if (!_fileSystemService.FileTryOut(
                    request.FileAsuntosPenales,
                    Path.Combine("ASUNTOS PENALES", request.idAsuntosPenales.ToString()),
                    out var dataFile, out string message, requiredExtention))
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            message
                        )
                    );
                }

                ArchivosAsuntosPenales entity = null!;

                try
                {
                    entity = ArchivosAsuntosPenalesEvents.CreateArchivo(
                        request.idAsuntosPenales,
                        request.idTipoDocumento,
                        dataFile.File.FileName,
                        dataFile.FilePath,
                        dataFile.File.ContentType,
                        sessionInformation.UserInformation.Rfc!,
                        sessionInformation.UserInformation.Rfc!,
                        request.idSeccion,
                        request.idRenglonSeccion,
                        _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                        false,
                        EnumRolesSicoj.OFICIAL_DE_PARTES.GetHashCode()
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _genericService.AddArchivosAsyncService(entity, dataFile);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        ///Método para listar los archivos asociados a una consulta
        /// </summary>
        /// <param name="id">Id Autorización Comercio Exterior</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseArchivosAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-archivos/{id}")]
        public async Task<IActionResult> GetAllArchivosAsync(int id)
        {

            try
            {
                var result = await _asuntosPenalesOficialPartesService.GetAllArchivoService(id);
                if (result is null)
                {
                    return BadRequest("No existe el registro.");
                }

                return Ok(
                    ResultOperation.SuccessResponseNoMessage(
                        result
                    )
                );
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        ///Método para obtener el documento de la consulta
        /// </summary>
        /// <param name="id">Id del archivo registrado</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseArchivosAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("visualizar-archivos/{id}")]
        public async Task<IActionResult> Descargar(int id)
        {
            try
            {
                var entityExists = await _asuntosPenalesOficialPartesService.GetByIdArchivoService(id);
                if (entityExists is null)
                {
                    return BadRequest("No existe el registro.");
                }

                var response = await _fileSystemService.GetFileAsync(entityExists.path_file);
                if (response is null)
                {
                    return BadRequest("No existe el documento.");
                }

                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    Inline = true,
                    FileName = Path.GetFileName(entityExists.path_file)
                };

                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
                return File(response, entityExists.content_type);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }

        }



        /// <summary>
        /// Método para borrar archivo
        /// </summary>
        /// <param name="id">id del registro que se desea eliminar o inactivar</param>
        /// <returns>Result Operation con mensaje de operación exitosa</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("archivo")]
        public async Task<IActionResult> DeleteArchivos(
        RequestDeleteArchivosAsuntosPenales request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                var validationResult = await _deleteArchivosAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }
                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<int>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityListExists = await _genericService.GetArchivosAsuntoPenalByIds(request.id.ToArray());
                if (entityListExists is null || !entityListExists.Any())
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "Los documentos no existen."
                        )
                    );
                }
                if (!request.id.All(value => entityListExists.Select(x => x.id).Contains(value)))
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "Algunos documentos no existen."
                        )
                    );
                }
                try
                {
                    for (int i = 0; i < entityListExists.Count; i++)
                    {
                        var entity = entityListExists[i];
                        AsuntosPenalesOficialPartesEvents.DeleteModalidaArchivoAsuntosPenales(ref entity, sessionInformation.UserInformation.Rfc);
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _genericService.DeleteArchivoAsuntoPenal(request.id.ToArray(), sessionInformation.UserInformation.Rfc);
                return Ok(result);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("archivo")]
        public async Task<IActionResult> PatchDocumento(
            [FromForm] RequestDocumentoUpdate request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                Filters.ValidateContractValues(request);
                var validationResult = await _requestDocumentoUpdateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(validationResult.ToString(" - ")));
                }

                string key = null!;

                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";


                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<int>(
                            "La autorización ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };

                AsuntosPenales entityAsuntos = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.idAsuntoPenal);
                if (entityAsuntos is null || !entityAsuntos.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El asunto penal no existe o fue eliminado."
                        )
                    );
                }

                var entityDocumentoList = await _genericService.GetArchivosAsuntoPenalByIds(new int[] { request.id });
                if (entityDocumentoList is null || !entityDocumentoList.Any(c => c.activo))
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "No existe el documento o fue eliminado."
                        )
                    );
                }

                var entityDocumento = entityDocumentoList.FirstOrDefault(c => c.activo);
                if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine("ASUNTOS PENALES", request.idAsuntoPenal.ToString()),
                        out var dataFile, out string message, requiredExtentions, entityDocumento!.path_file))
                {
                    return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                message
                            )
                        );
                }

                try
                {
                    if (dataFile is not null)
                    {
                        ArchivosAsuntosPenalesEvents.UpdateWithFile(
                            ref entityDocumento,
                            request.idTipoArchivo,
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            EnumRolesSicoj.OFICIAL_DE_PARTES.GetHashCode()
                        );
                    }
                    else
                    {
                        ArchivosAsuntosPenalesEvents.Update(
                            ref entityDocumento,
                            request.idTipoArchivo,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            EnumRolesSicoj.OFICIAL_DE_PARTES.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation<int> result = await _genericService.UpdateArchivosAsuntosPenales(entityDocumento!, dataFile!);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        ///Método para consultar Asuntos Penales por numeroAsunto
        /// </summary>
        /// <param name="numeroAsunto"> Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseNumeroAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("numero-asunto/{numeroAsunto}")]
        public async Task<IActionResult> GetAsuntosPenalesByNumeroAsunto(string numeroAsunto)
        {
            try
            {

                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var result = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesByNumeroAsunto(numeroAsunto);
                if (result is null)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse("No se encontraron resultados")
                    );
                }
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        #region Solicitud-Transparencia
        /// <summary>
        /// Crea Solicitud Transparencia : Oficial de Partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("solicitud-transparencia")]
        public async Task<IActionResult> PostSolicitudTransparencia(
         [FromForm] RequestCreateSolicitudTransparencia request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var validationResult = await _validatorRequestCreateSolicitudTransparencia.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    _fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine("ASUNTOS PENALES", "solicitud-transparencia", request.idAsunto.ToString()),
                        out dataFile);
                }

                ArchivoAsuntoPenal entityDocumento = null!;
                SolicitudTransparencia entity = null!;
                try
                {

                    entity = AsuntosPenalesOficialPartesEvents.CreateModalidadSolicitudTransparencia(
                       request.idAsunto,
                       request.noSolicitud,
                       DateTime.Parse(request.fechaSolicitud!)
                   );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivoGlobal(
                            request.idAsunto!,
                            request.idTipoArchivo.GetValueOrDefault(),
                            request.idSeccion.GetValueOrDefault(),
                            sessionInformation.UserInformation.IdAdministracionCentral.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File.ContentType,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            request.noFolio
                        );
                    }

                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }


                var result = await _asuntosPenalesOficialPartesService.AddSolicitudTransparenciaService(entity, entityDocumento, dataFile!);
                if (result.Success == false)
                {
                    return Ok(result.Messages);
                }

                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Edita solicitud transparencia : Oficial Partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("solicitud-transparencia")]
        public async Task<IActionResult> PatchSolicitudTransparencia(
          [FromForm] RequestUpdateSolicitudTransparencia request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsunto}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var validationResult = await _validatorRequestUpdateSolicitudTransparencia.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetByIdSolicitudTransparenciaService(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto no existe."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesOficialPartesEvents.UpdateSolicitudTransparencia(ref entityExists,
                    request.id,
                    request.idAsunto,
                    request.noSolicitud,
                    DateTime.Parse(request.fechaSolicitud!)
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesOficialPartesService.UpdateSolicitudTransparenciaService(entityExists);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }


        /// <summary>
        /// Solicitud Transparencia  por Id  : Oficial Partes
        /// </summary>
        /// <param name="id">Id Requerimiento</param>
        /// <returns></returns>
        [ProducesResponseType(
            typeof(ResultOperation<ResponseSolicitudTransparenciaList>),
            200
        )]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("solicitud-transparencia/{id}")]
        public async Task<IActionResult> GetByIdSolicitudTransparencia(int id)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }

                var result = await _asuntosPenalesOficialPartesService.GetByIdSolicitudTransparenciaServices(id);
                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Tabla de Solicitud Transparencia : Oficial Partes
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(
            typeof(ResultOperation<ResponseSolicitudTransparenciaList>),
            200
        )]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-solicitud-transparencia/{idAsunto}")]
        public async Task<IActionResult> GetTablaSolicitudTransparencia(int idAsunto)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }


                var result = await _asuntosPenalesOficialPartesService.GetTablaSolicitudTransparenciaService(idAsunto);

                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Elimina solicitud Transparencia : Oficial Partes
        /// </summary>
        /// <param name="idAsunto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("solicitud-transparencia")]
        public async Task<IActionResult> DeleteSolicitudTransparencia(
           int idAsunto,
           int id
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }


                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{idAsunto}");
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>("El registro no se puede editar ya que el usuario no lo ha apartado.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse(
                            "El registro ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityExists = await _asuntosPenalesOficialPartesService.GetByIdSolicitudTransparenciaService(id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El registro no existe."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesOficialPartesEvents.DeleteSolicitudTransparencia(ref entityExists,
                    id
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesOficialPartesService.DeleteSolicitudTransparenciaService(entityExists);
                return Ok(result);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseFechaVencimientoSeccion>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("fecha-vencimiento-obtener")]
        public async Task<IActionResult> GetFechaVecnimientoAsync([FromBody] RequestFiltrosFechaVencimientoObtener request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }


                var result = await _asuntosPenalesOficialPartesService.GetFechasVencimientoAsync(
                    request.fecha_inicial,
                    request.fecha_final,
                    request.Secciones
                    );

                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }

        /// <summary>
        /// Método para actualizar fechas de vencimiento asuntos penales 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("asuntos-penales-actualizar")]
        public async Task<IActionResult> PatchFechaVencimiento(
            [FromBody] RequestActualizarMasivo request
        )
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                var result = await _asuntosPenalesOficialPartesService.UpdateAsuntosPenalesMasivo(request.fecha_vencimiento, request.idAsunto, request.idModulo, request.idSeccion, request.idSeccionRenglon);
                return Ok(result);
            }



            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }
        
        #region  Envio correo
        /// <summary>
        /// Envió de correos : Oficial de Partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("Email")]
        public async Task<IActionResult> Email(
         [FromForm] RequestEmailMessage request
        )
        {
            try
            {       
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                   return Ok(
                       ResultOperation.FailureErrorResponse(
                           "No se pudo obtener la información del usuario."
                       )
                   );
                } 

              
                Email entity = null!;
                try
                {
                    entity = AsuntosPenalesOficialPartesEvents.CreaCorreo(  
                        request.To,
                        request.Cc,
                        request.Subject,                      
                        request.IsHtml,
                        request.Body
                        
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesOficialPartesService.EnvioEmail(entity);
                if (result.Success == false)
                {
                    return Ok(result.Messages);
                }

                return Ok(result);
            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        _e.ManageException()
                    )
                );
            }
        }
        #endregion

    }

}
