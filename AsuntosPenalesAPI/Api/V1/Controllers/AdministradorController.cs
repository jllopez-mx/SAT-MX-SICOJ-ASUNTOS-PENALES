using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.DTO.ContractsValidations;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events;
using AsuntosPenalesAPI.Model.Entities.Events.Administrador;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Files;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;

namespace AsuntosPenalesAPI.Api.V1.Controllers
{
    [ApiController]
    [Route("sicoj/asuntos-penales/api/v1/administrador/asuntos-penales")]
    public class AdministradorController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<AdministradorController> _logger;
        private readonly IAsuntosPenalesAdministradorService _asuntosPenalesAdministradorService;
        private readonly RequestUpdateAsuntosPenalesAdministradorValidator _updateAsuntosPenalesAdminValidator;
        private readonly RequestUpdateAsignarAsuntosPenalesValidator _updateAsignarAsuntosPenalesValidator;
        private readonly RequestCreateReasignarAsuntosPenalesValidator _createReasignarAsuntosPenalesValidator;
        private readonly RequestRemisionAsuntosPenalesValidator _createRemisionAsuntosPenalesValidator;
        private readonly IValidator<RequestCreateFileAsuntosPenales> _createArchivoValidator;
        private readonly RequestDeleteArchivosAsuntosPenalesValidator _deleteArchivosAsuntosPenalesValidator;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;
        private readonly RequestUpdateAnalisisValidator _updateAnalisisValidator;
        private readonly RequestAnalisisAsuntosPenalesValidator _createAnalisisAsuntosPenalesValidator;
        private readonly RequestProcedibilidadAsuntosPenalesValidator _createProcedibilidadAsuntosPenalesValidator;
        private readonly RequestImputadosAsuntosPenalesValidator _createImputadosAsuntosPenalesValidator;
        private readonly RequestPersonasMoralesAsuntosPenalesValidator _createPersonasMoralesAsuntosPenalesValidator;
        private readonly RequestDelitosAsuntosPenalesValidator _createDelitosAsuntosPenalesValidator;
        private readonly RequestDeleteDelitoValidator _deleteDelitoValidator;
        private readonly RequestUpdateEtapaInicialInvestigacionValidator _updateEtapaInicialInvestigacionValidator;
        private readonly RequestUpdateEtapaComplementariaInvestigacionValidator _updateEtapaComplementariaInvestigacionValidator;
        private readonly RequestUpdateEtapaIntermediaInvestigacionValidator _updateEtapaIntermediaInvestigacionValidator;
        private readonly RequestUpdateEtapaJuicioInvestigacionValidator _updateEtapaJuicioInvestigacionValidator;
        private readonly RequestMedidasCautelaresAsuntosPenalesValidator _createMedidasCautelaresAsuntosPenalesValidator;
        private readonly RequestDeleteMedidaCautelarValidator _deleteMedidaCautelarValidator;
        private readonly IValidator<RequestCreateSolicitudTransparencia> _validatorRequestCreateSolicitudTransparencia;
        private readonly IValidator<RequestUpdateSolicitudTransparencia> _validatorRequestUpdateSolicitudTransparencia;
        private readonly IFileSystemService _fileSystemService;

        private static object _lock = new object();
        private readonly IRedisClient _redisClient;
        private readonly IGenericService _genericService;


        public AdministradorController(ILogger<AdministradorController> logger,
        IAsuntosPenalesAdministradorService asuntosPenalesAdministradorService,
        RequestUpdateAsuntosPenalesAdministradorValidator updateAsuntoPenalAdminValidator,
        RequestUpdateAsignarAsuntosPenalesValidator updateAsignarAsuntoPenalAdminValidator,
        RequestCreateReasignarAsuntosPenalesValidator createReasignarAsuntosPenalesValidator,
        RequestRemisionAsuntosPenalesValidator createRemisionAsuntosPenalesValidator,
        IRedisClient redisClient,
        IValidator<RequestCreateFileAsuntosPenales> createArchivoValidator,
        RequestDeleteArchivosAsuntosPenalesValidator deleteArchivosAsuntosPenalesValidator,
        IValidator<RequestDocumentoUpdate> requestDocumentoValidator,
        RequestUpdateAnalisisValidator updateAnalisisValidator,
        RequestAnalisisAsuntosPenalesValidator createAnalisisAsuntosPenalesValidator,
        RequestProcedibilidadAsuntosPenalesValidator createProcedibilidadAsuntosPenalesValidator,
        RequestImputadosAsuntosPenalesValidator createImputadosAsuntosPenalesValidator,
        RequestPersonasMoralesAsuntosPenalesValidator createPersonasMoralesAsuntosPenalesValidator,
        RequestDelitosAsuntosPenalesValidator createDelitosAsuntosPenalesValidator,
        RequestDeleteDelitoValidator deleteDelitoValidator,
        RequestUpdateEtapaInicialInvestigacionValidator updateEtapaInicialInvestigacionValidator,
        RequestUpdateEtapaComplementariaInvestigacionValidator updateEtapaComplementariaInvestigacionValidator,
        RequestUpdateEtapaIntermediaInvestigacionValidator updateEtapaIntermediaInvestigacionValidator,
        RequestUpdateEtapaJuicioInvestigacionValidator updateEtapaJuicioInvestigacionValidator,
        RequestMedidasCautelaresAsuntosPenalesValidator createMedidasCautelaresAsuntosPenalesValidator,
        RequestDeleteMedidaCautelarValidator deleteMedidaCautelarValidator,
        IValidator<RequestCreateSolicitudTransparencia> validatorRequestCreateSolicitudTransparencia,

        IValidator<RequestUpdateSolicitudTransparencia> validatorUpdateSolicitudTransparencia,
        IFileSystemService fileSystemService,
        IGenericService genericService)
        {
            _logger = logger;

            _asuntosPenalesAdministradorService = asuntosPenalesAdministradorService;
            _updateAsuntosPenalesAdminValidator = updateAsuntoPenalAdminValidator ?? throw new ArgumentNullException(nameof(updateAsuntoPenalAdminValidator));
            _updateAsignarAsuntosPenalesValidator = updateAsignarAsuntoPenalAdminValidator ?? throw new ArgumentNullException(nameof(updateAsignarAsuntoPenalAdminValidator));
            _createReasignarAsuntosPenalesValidator = createReasignarAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(createReasignarAsuntosPenalesValidator));
            _createRemisionAsuntosPenalesValidator = createRemisionAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(createRemisionAsuntosPenalesValidator));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _createArchivoValidator = createArchivoValidator ?? throw new ArgumentNullException(nameof(createArchivoValidator));
            _deleteArchivosAsuntosPenalesValidator = deleteArchivosAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(deleteArchivosAsuntosPenalesValidator));
            _updateAnalisisValidator =
                updateAnalisisValidator
                ?? throw new ArgumentNullException(nameof(updateAnalisisValidator));
            _createAnalisisAsuntosPenalesValidator =
                createAnalisisAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createAnalisisAsuntosPenalesValidator));
            _createProcedibilidadAsuntosPenalesValidator =
                createProcedibilidadAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createProcedibilidadAsuntosPenalesValidator)
                );
            _createImputadosAsuntosPenalesValidator =
                createImputadosAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createImputadosAsuntosPenalesValidator));
            _createPersonasMoralesAsuntosPenalesValidator =
                createPersonasMoralesAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createPersonasMoralesAsuntosPenalesValidator)
                );
            _createDelitosAsuntosPenalesValidator =
                createDelitosAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createDelitosAsuntosPenalesValidator)
                );
            _deleteDelitoValidator =
                deleteDelitoValidator
                ?? throw new ArgumentNullException(
                    nameof(deleteDelitoValidator)
                );
            _updateEtapaInicialInvestigacionValidator =
                updateEtapaInicialInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(updateEtapaInicialInvestigacionValidator)
                );
            _updateEtapaComplementariaInvestigacionValidator =
                updateEtapaComplementariaInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(updateEtapaComplementariaInvestigacionValidator)
                );
            _updateEtapaIntermediaInvestigacionValidator =
                updateEtapaIntermediaInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(_updateEtapaIntermediaInvestigacionValidator)
                );
            _updateEtapaJuicioInvestigacionValidator =
                updateEtapaJuicioInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(_updateEtapaJuicioInvestigacionValidator)
                );
            _createMedidasCautelaresAsuntosPenalesValidator =
               createMedidasCautelaresAsuntosPenalesValidator
               ?? throw new ArgumentNullException(
                   nameof(createMedidasCautelaresAsuntosPenalesValidator)
               );
            _deleteMedidaCautelarValidator =
                deleteMedidaCautelarValidator
                ?? throw new ArgumentNullException(nameof(_deleteMedidaCautelarValidator));
            _validatorRequestCreateSolicitudTransparencia = validatorRequestCreateSolicitudTransparencia ?? throw new ArgumentNullException(nameof(validatorRequestCreateSolicitudTransparencia));
            _validatorRequestUpdateSolicitudTransparencia = validatorUpdateSolicitudTransparencia ?? throw new ArgumentNullException(nameof(validatorUpdateSolicitudTransparencia));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _requestDocumentoUpdateValidator = requestDocumentoValidator ?? throw new ArgumentNullException(nameof(requestDocumentoValidator));
        }
        #endregion

        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAdministradorByFilters>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetBandejaAsync([FromQuery] PagerQuery request)
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
                    return Ok(ResultOperation.FailureWarningResponse<List<ResponseAsuntosPenalesAdministradorByFilters>>("La columna de ordenamiento no es válida.")
                        );
                }
                var result = await _asuntosPenalesAdministradorService.GetBandejaAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    sessionInformation.UserInformation.IdAdministracionCentral,
                    sessionInformation.UserInformation.IdAdministracion

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
        ///Método para consultar Asunto Penal por Id
        /// </summary>
        /// <param name="id">Id Asunto penal</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesByIdAdmin>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsuntoPenalById(int id)
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

                var result = await _asuntosPenalesAdministradorService.GetAdminAsuntoPenalById(id);
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
        /// Método para actualizar Asunto penal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("")]
        public async Task<IActionResult> PatchAsuntosPenales(
            RequestUpdateAsuntosPenalesAdministrador request
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
                var validationResult = await _updateAsuntosPenalesAdminValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se encontro el asunto penal a actualizar."
                        )
                    );
                }
                if (request.id_tipo_edicion == 1)
                {
                    try
                    {
                        AsuntosPenalesAdministradorEvents.UpdateGuardar(
                            ref entityExists,
                            request.numero_expediente_cadido,
                            request.oficio_solicitud,
                            DateTime.Parse(request.fecha_recepcion),
                            DateTime.Parse(request.fecha_vencimiento),
                            request.id_unidad_realiza_solicitud,
                            request!.id_unidad_administrativa,
                            request!.id_subadministracion);
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _asuntosPenalesAdministradorService.UpdateAsuntoPenalAdministrador(entityExists);
                    return Ok(result);
                }
                else if (request.id_tipo_edicion == 2)
                {
                    try
                    {
                        AsuntosPenalesAdministradorEvents.UpdateGuardarHistorico(
                            ref entityExists,
                            request.numero_expediente_cadido,
                            request.oficio_solicitud,
                            DateTime.Parse(request.fecha_recepcion),
                            DateTime.Parse(request.fecha_vencimiento),
                            request.id_unidad_realiza_solicitud,
                            request!.id_unidad_administrativa,
                            request!.id_subadministracion,
                            request!.nombre_abogado
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                    var result = await _asuntosPenalesAdministradorService.UpdateAsuntoPenalAdministrador(entityExists);
                    return Ok(result);
                }
                return Ok(ResultOperation.FailureWarningResponse<int>("El tipo de edición no es valido."));
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAdministradorByFilters>), 200)]
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

                var result = await _asuntosPenalesAdministradorService.GetHistoricoAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    Filters.GetStringValue(filters!.ByNoAsuntoPenal.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionHasta.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByOficioSolicitud.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByNoExpedienteCadido.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByIdUnidadRealizoSolicitud.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByAdminControl.FirstOrDefault()),
                    Filters.GetIntValue(filters.BySubadministracion.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByNombreAbogado.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByIdEstadoTarea.FirstOrDefault()),
                    Filters.GetIntValue(filters.ByIdEstadoProcesal.FirstOrDefault()),
                    sessionInformation.UserInformation.ListRoles[1].IdUnidadAdministrativa,
                    Filters.GetBoolValue(filters.ByTipoUnidad.FirstOrDefault())
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
        /// Método para actualizar autorizaciones de comercio exterior de modalida físicia y en linea
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("asignar")]
        public async Task<IActionResult> PostAsignar(
            RequestAsignarAsuntosPenales request
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

                var validationResult = await _updateAsignarAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" - ")));
                }
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(request.id);
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
                            "La asunto penal no existe o ya se encuentra eliminado."
                        )
                    );
                    }
                }
                AsuntosPenalesAbogado entityAbogado = null!;
                try
                {
                    AsuntosPenalesAdministradorEvents.CreateAsignar(ref entityExists!,
                        sessionInformation.TokenInfomation.workforceID,
                        sessionInformation.UserInformation.Rfc
                    );
                    entityAbogado = AsuntosPenalesAbogadoAdministradorEvents.Create(request.rfc_abogado, sessionInformation.UserInformation.Rfc);
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.UpdateAsignarAsuntosPenales(entityExists, entityAbogado);
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
        /// Método para crear reasignaciones de Asuntos Penales
        /// </summary>
        /// <param name="request">Datos de Asuntos Penales</param>
        /// <returns>Result Operation con Id de registro creado</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("reasignar")]
        public async Task<IActionResult> PatchReasignar(
            RequestReasignarAsuntosPenales request
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

                var result = await _genericService.ReasignarAsync(request.idList.ToArray(), sessionInformation.UserInformation, request.idAbogado);
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
        /// Método para remitir
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("remitir")]
        public async Task<IActionResult> PostRemision(
        [FromForm] RequestRemitir request
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
                var validationResult = await _createRemisionAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(ResultOperation.FailureWarningResponse(validationResult.ToString(" -- ")));
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>($"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}");
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }
                List<int> listaEstadosProcesales = new()
                {
                    EnumEstadoProcesal.PROCEDENTE.GetHashCode(),
                    EnumEstadoProcesal.INVESTIGACION_INICIAL.GetHashCode(),
                    EnumEstadoProcesal.INVESTIGACION_COMPLEMENTARIA.GetHashCode(),
                    EnumEstadoProcesal.ETAPA_INTERMEDIA.GetHashCode(),
                    EnumEstadoProcesal.ETAPA_JUICIO.GetHashCode()
                };
                if (listaEstadosProcesales.Contains((int)entityExists.id_estado_procesal!))
                {
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "El asunto no se puede remitir ya que se encuentra en atención"
                            )
                        );
                    }
                }

                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };
                    if (!_fileSystemService.FileTryOut(
                        request.documento,
                        Path.Combine("ASUNTOS PENALES", request.idAsuntoPenal.ToString()),
                        out dataFile, out string message, requiredExtention))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                message
                            )
                        );
                    }
                }

                ArchivosAsuntosPenales entityDocumento = null!;
                AsuntosPenalesRemision entityRemision = null!;
                try
                {
                    entityRemision = AsuntosPenalesRemisionEvents.Create(entityExists.id, request.idTipoAutoridad, sessionInformation.UserInformation.IdAdministracionCentral, request.idUnidadAdministrativaRecibe, request.idUnidadAdministrativaExterna, DateTime.Parse(request.fechaOficio!), request.numeroOficio!, sessionInformation.UserInformation.Rfc);

                    AsuntosPenalesAdministradorEvents.UpdateRemitir(ref entityExists,
                        entityRemision,
                        sessionInformation.UserInformation.Rfc,
                        sessionInformation.UserInformation.Rfc
                    );
                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc,
                            sessionInformation.UserInformation.Rfc,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.ADMINISTRADOR.GetHashCode()
                    );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.RemitirAsync(entityExists, entityRemision, entityDocumento, dataFile!);
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
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(id);
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
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(id);
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
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(request.idAsuntosPenales);
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
                        EnumRolesSicoj.ADMINISTRADOR.GetHashCode()
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
                        AsuntosPenalesAdministradorEvents.DeleteModalidaArchivoAsuntosPenales(ref entity, sessionInformation.UserInformation.Rfc);
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
                var entityExists = await _asuntosPenalesAdministradorService.GetByIdArchivoService(id);
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

                AsuntosPenales entityAsuntos = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                            EnumRolesSicoj.ADMINISTRADOR.GetHashCode()
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
        ///Método para listar los archivos asociados a una consulta
        /// </summary>
        /// <param name="id">Id Autorización Comercio Exterior</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseRemision>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-remisiones/{id}")]
        public async Task<IActionResult> GetRemisiones(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetAllRemision(id);
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
        /// Método para generar Analisisn del Asunto Penales (Requerimiento y Determinación)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("analisis-requerimiento-determinacion")]
        public async Task<IActionResult> PostAnalisisRyD([FromForm] RequestAnalisis request)
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
                var validationResult = await _createAnalisisAsuntosPenalesValidator.ValidateAsync(
                    request
                );
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention =
                    {
                        EnumFileType.JPG,
                        EnumFileType.JPEG,
                        EnumFileType.PDF,
                    };
                    if (
                        !_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("ASUNTOS PENALES", request.idAsuntoPenal.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }

                ArchivosAsuntosPenales entityDocumento = null!;
                AsuntosPenalesAnalisis entityAnalisis = null!;
                try
                {
                    entityAnalisis = AsuntosPenalesAnalisisEvents.Create(
                        ref entityExists,
                        request.requerimiento,
                        request.oficioRequerimiento!,
                        string.IsNullOrEmpty(request.fechaRequerimiento)
                            ? null!
                            : DateTime.Parse(request.fechaRequerimiento!),
                        request.requerimientoAtendido,
                        request.oficioAtencion!,
                        string.IsNullOrEmpty(request.fechaAtencion)
                            ? null!
                            : DateTime.Parse(request.fechaAtencion!),
                        request.idDeterminacionAsuntoPenal,
                        DateTime.Parse(request.fechaDeterminacion),
                        sessionInformation.UserInformation.Rfc
                    );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc,
                            sessionInformation.UserInformation.Rfc,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.AnalisisAsync(
                    entityExists,
                    entityAnalisis,
                    entityDocumento,
                    dataFile!
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

        /// Método para consultar Analsis por id asunto penal 
        /// </summary>
        /// <param name="id">Id Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("analisis/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdAnalisis(int id
        )
        {
            try
            {
                var result =
                    await _asuntosPenalesAdministradorService.GetAsuntosPenalesByIdAnalisisDisconnected(
                        id
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
        /// Método para turnar asuntos penales
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("analisis")]
        public async Task<IActionResult> PatchAnalisis(RequestUpdateAnalisis request)
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

                var validationResult = await _updateAnalisisValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La asunto penal a editar no existe.")
                    );
                }
                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La asunto penal ya se encuentra eliminado, no puede ser editado."
                            )
                        );
                    }
                }

                var entityAnalisisExists =
                    await _asuntosPenalesAdministradorService.GetAsuntosPenalesByIdAnalisis(request.id);
                if (entityAnalisisExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("El caso de Analisis no existe.")
                    );
                }

                if (!entityAnalisisExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El caso de Analisis ya encuentra eliminado, no puede ser editado."
                        )
                    );
                }

                //AsuntosPenalesModificacion modificacion= new();

                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    //modificacion = await _asuntosPenalesAbogadoService.GetModificacionByIdAsunto(request.idAsuntoPenal);

                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }

                try
                {
                    AsuntosPenalesAnalisisEvents.UpdateAnalisis(
                        ref entityAnalisisExists,
                        ref entityExists!,
                        request.requerimiento,
                        request.oficioRequerimiento!,
                        request.fechaRequerimiento!,
                        request.requerimientoAtendido,
                        request.oficioAtencion!,
                        request.fechaAtencion!,
                        request.idDeterminacionAsuntoPenal,
                        DateTime.Parse(request.fechaDeterminacion),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.UpdateAnalisis(
                    entityAnalisisExists
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
        /// Método de Requisitos y Procedibilidad perfil abogado
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("procedibilidad")]
        public async Task<IActionResult> PostProcedibilidad(RequestRequisitosProcedibilidad request)
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

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                var validationResult =
                    await _createProcedibilidadAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }
                AsuntosPenalesProcedibilidad entityProcedibilidad = null!;
                try
                {
                    entityProcedibilidad = AsuntosPenalesProcedibilidadEvents.Create(
                        ref entityExists,
                        request.id_requisito_procedibilidad,
                        request.numero_oficio_procedibilidad,
                        DateTime.Parse(request.fecha_presentacion),
                        request.cuantia,
                        request.numero_carpeta_investigacion,
                        request.agente_ministerio_publico,
                        sessionInformation.UserInformation.Rfc
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.ProcedibilidadAsync(
                    entityExists,
                    entityProcedibilidad
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
        [HttpGet("procedibilidad/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdProcedibilidadDisconnected(int id)
        {
            try
            {
                var result =
                    await _asuntosPenalesAdministradorService.GetAsuntosPenalesByIdProcedibilidadDisconnected(
                        id
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
        /// Método de imputados  perfil abogado
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("imputados")]
        public async Task<IActionResult> PostImputados(RequestImputados request)
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

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);


                var validationResult = await _createImputadosAsuntosPenalesValidator.ValidateAsync(
                    request
                );
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }
                AsuntosPenalesImputados entityImputados = null!;
                try
                {
                    entityImputados = AsuntosPenalesImputadosEvents.Create(
                        ref entityExists,
                        request.rfc,
                        request.contribuyente,
                        request.nombre,
                        sessionInformation.UserInformation.Rfc
                    );

                    AsuntosPenalesImputadosEvents.VerificarEstadosCreate(
                    ref entityExists!,
                    entityEtapaInicialListExists,
                    entityImputados
                );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.ImputadosAsync(
                    entityExists,
                    entityImputados
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
        /// Método de personas morales perfil abogado
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("persona-moral")]
        public async Task<IActionResult> PostPersonaMoral(RequestPersonaMorales request)
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

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                var validationResult =
                    await _createPersonasMoralesAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }
                AsuntosPenalesPersonasMorales entityPersonasMorales = null!;
                try
                {
                    entityPersonasMorales = AsuntosPenalesPersonasMoralesEvents.Create(
                        ref entityExists,
                        request.rfc,
                        request.contribuyente,
                        request.nombre,
                        sessionInformation.UserInformation.Rfc
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.PersonasMoralesAsync(
                    entityExists,
                    entityPersonasMorales
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
        ///Método para listar personas morales
        /// </summary>
        /// <param name="id">Id asunto penal </param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponsePersonasMorales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-personas-morales/{id}")]
        public async Task<IActionResult> GetPersonasMorales(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetAllPersonasMorales(
                    id
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

        // <summary>
        // Método de Requisitos y Procedibilidad perfil abogado
        // </summary>
        // <param name="request"></param>
        // <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("delitos")]
        public async Task<IActionResult> PostDelito(RequestDelitos request)
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

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                var validationResult = await _createDelitosAsuntosPenalesValidator.ValidateAsync(
                    request
                );
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }
                List<AsuntosPenalesDelitos> entityDelitos = new();
                foreach (var item in request.id_delito)
                {
                    try
                    {
                        entityDelitos.Add(
                            AsuntosPenalesDelitosEvents.Create(
                                ref entityExists,
                                item,
                                sessionInformation.UserInformation.Rfc
                            )
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                }

                var result = await _asuntosPenalesAdministradorService.DelitosAsync(entityDelitos);
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
        ///Método para listar delitos
        /// </summary>
        /// <param name="id">Id asunto penal </param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponsePersonasMorales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-delito/{id}")]
        public async Task<IActionResult> GetDelitos(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetAllDelitos(id);
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
        [HttpDelete("delito")]
        public async Task<IActionResult> Delito(RequestDeleteDelito request)
        {
            try
            {
                var validationResult = await _deleteDelitoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.id_asunto_penal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
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
                var entityDelitosExists = await _asuntosPenalesAdministradorService.GetDelitoById(
                    request.id
                );
                if (entityDelitosExists is null)
                {
                    return Ok(ResultOperation.FailureErrorResponse("El delito no existe."));
                }
                if (entityExists.id != entityDelitosExists.id_asunto_penal)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El delito no pertenece al asunto penal."
                        )
                    );
                }

                var result = await _asuntosPenalesAdministradorService.DeleteDelitos(entityDelitosExists);
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
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-inicial/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaInicial(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetImputadoByIdDisconnectedEtapaInicial(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        "Ah ocurrido un error inesperado." + e.Message
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
        [HttpPatch("imputados/etapa-inicial")]
        public async Task<IActionResult> PatchEtapaInvestigacion(
            [FromForm] RequestUpdateEtapaInvestigacionInicial request
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
                var validationResult =
                    await _updateEtapaInicialInvestigacionValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );

                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La asunto penal a editar no existe.")
                    );
                }

                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La asunto penal ya se encuentra eliminado, no puede ser editado."
                            )
                        );
                    }
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);

                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }

                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.id
                );

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }

                AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio = null!;
                AsuntosPenalesCriterioOportunidad entityCriterioOportunidad = null!; // Consulta Criterio Oportunidad

                var acuerdoReparatorioListExits =
                    await _asuntosPenalesAdministradorService.GetAcuerdoReparatorioByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                    );
                if (
                    acuerdoReparatorioListExits is not null
                    && acuerdoReparatorioListExits.Any(c => c.activo)
                )
                {
                    entityAcuerdoReparatorio = acuerdoReparatorioListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var criterioOportunidadListExits =
                    await _asuntosPenalesAdministradorService.GetCriterioOportunidadByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INICIAL.GetHashCode()
                    );
                if (
                    criterioOportunidadListExits is not null
                    && criterioOportunidadListExits.Any(c => c.activo)
                )
                {
                    entityCriterioOportunidad = criterioOportunidadListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                ArchivosAsuntosPenales entityDocumento = null!;

                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAdministradorService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                        request.id,
                        EnumTipoDocumento.DOCUMENTACIÓN_RELACIONADA.GetHashCode()
                    );
                if (
                    ArchivoAsuntosPenalesListExits is not null
                    && ArchivoAsuntosPenalesListExits.Any(c => c.activo)
                )
                {
                    entityDocumento = ArchivoAsuntosPenalesListExits.FirstOrDefault(c => c.activo)!;
                }

                if (
                    request.documento is not null
                    || request.idTipoArchivo.GetValueOrDefault() > 0
                    || request.idSeccion.GetValueOrDefault() > 0
                )
                {
                    if (entityDocumento is not null)
                    {
                        return Ok(
                            ResultOperation.FailureWarningResponse<int>(
                                "Los parametrós de documento no son requeridos por que ya existe un documento de DOCUMENTACION RELACIONADA."
                            )
                        );
                    }

                    if (request.documento is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."
                            )
                        );
                    }
                }
                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention =
                    {
                        EnumFileType.JPG,
                        EnumFileType.JPEG,
                        EnumFileType.PDF,
                    };
                    if (
                        !_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("ASUNTOS PENALES", request.id.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }
                var estadoProcesalCopy = entityExists!.id_estado_procesal.GetValueOrDefault();
                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }

                try
                {
                    AsuntosPenalesImputadosEvents.UpdateImputadosEtapaInicial(
                        estadoProcesalCopy,
                        ref entityImputadosExists,
                        ref entityAcuerdoReparatorio,
                        ref entityCriterioOportunidad,
                        request.concluyeInvestigacion,
                        request.idTerminacionInvestigacion,
                        string.IsNullOrEmpty(request.fechaTerminacionInvestigacion)
                            ? null!
                            : DateTime.Parse(request.fechaTerminacionInvestigacion),
                        request.solucionAlterna,
                        request.idTipoSolucionAlterna,
                        string.IsNullOrEmpty(request.fechaSolicitudAudienciaInicial)
                            ? null!
                            : DateTime.Parse(request.fechaSolicitudAudienciaInicial),
                        request.idCentroJusticia,
                        request.causaPenal,
                        string.IsNullOrEmpty(request.fechaAudienciaInicial)
                            ? null!
                            : DateTime.Parse(request.fechaAudienciaInicial),
                        request.autoVinculacionProceso,
                        string.IsNullOrEmpty(request.fechaAutoVinculacion)
                            ? null!
                            : DateTime.Parse(request.fechaAutoVinculacion),
                        request.ordenAprehension,
                        string.IsNullOrEmpty(request.fechaOrdenAprehension)
                            ? null!
                            : DateTime.Parse(request.fechaOrdenAprehension),
                        sessionInformation.UserInformation.Rfc!,
                        request.condicionesAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaAutorizacionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaAutorizacionAcuerdoReparatorio),
                        request.reparacionDañoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaCelebracionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaCelebracionAcuerdoReparatorio),
                        request.conclusionAsuntoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaConclusionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionAcuerdoReparatorio),
                        request.condicionesCriterioOportunidad,
                        string.IsNullOrEmpty(request.fechaCriterioOportunidad)
                            ? null!
                            : DateTime.Parse(request.fechaCriterioOportunidad),
                        request.conclusionAsuntoCriterioOportunidad,
                        string.IsNullOrEmpty(request.fechaConclusionCriterioOportunidad)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionCriterioOportunidad)
                    );

                    AsuntosPenalesImputadosEvents.VerificarEstados(
                        ref entityExists!,
                        entityEtapaInicialListExists,
                        entityImputadosExists
                    );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.UpdateImputadosEtapaInicialAsync(
                    entityExists!,
                    entityImputadosExists,
                    entityAcuerdoReparatorio,
                    entityCriterioOportunidad,
                    entityDocumento!,
                    dataFile
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
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-complementaria/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaComplementaria(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetImputadoByIdDisconnectedEtapaComplementaria(id);
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("imputados/etapa-complementaria")]
        public async Task<IActionResult> PatchEtapaComplementariaInvestigacion(
            [FromForm] RequestUpdateEtapaInvestigacionComplementaria request
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
                var validationResult =
                    await _updateEtapaComplementariaInvestigacionValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );

                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La asunto penal a editar no existe.")
                    );
                }

                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La asunto penal ya se encuentra eliminado, no puede ser editado."
                            )
                        );
                    }
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);

                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }

                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.id
                );

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }

                AsuntosPenalesSentencia entitySentencia = null!;
                AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio = null!;
                AsuntosPenalesSobreseimiento entitySobreseimiento = null!;
                AsuntosPenalesSuspensionCondicional entitySuspension = null!;

                var acuerdoReparatorioListExits =
                    await _asuntosPenalesAdministradorService.GetAcuerdoReparatorioByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                    );
                if (
                    acuerdoReparatorioListExits is not null
                    && acuerdoReparatorioListExits.Any(c => c.activo)
                )
                {
                    entityAcuerdoReparatorio = acuerdoReparatorioListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSentenciaListExits =
                    await _asuntosPenalesAdministradorService.GetSentenciaByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                    );
                if (
                    acuerdoSentenciaListExits is not null
                    && acuerdoSentenciaListExits.Any(c => c.activo)
                )
                {
                    entitySentencia = acuerdoSentenciaListExits.FirstOrDefault(c => c.activo)!;
                }

                var acuerdoSobreseimientoListExits =
                    await _asuntosPenalesAdministradorService.GetSobreseimientoByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                    );
                if (
                    acuerdoSobreseimientoListExits is not null
                    && acuerdoSobreseimientoListExits.Any(c => c.activo)
                )
                {
                    entitySobreseimiento = acuerdoSobreseimientoListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSuspencionListExits =
                    await _asuntosPenalesAdministradorService.GetSuspencionCondicionalByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.COMPLEMENTARIA.GetHashCode()
                    );
                if (
                    acuerdoSuspencionListExits is not null
                    && acuerdoSuspencionListExits.Any(c => c.activo)
                )
                {
                    entitySuspension = acuerdoSuspencionListExits.FirstOrDefault(c => c.activo)!;
                }

                ArchivosAsuntosPenales entityDocumento = null!;
                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAdministradorService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        request.id,
                        EnumTipoDocumento.DOCUMENTACIÓN_RELACIONADA.GetHashCode()
                    );
                if (
                    ArchivoAsuntosPenalesListExits is not null
                    && ArchivoAsuntosPenalesListExits.Any(c => c.activo)
                )
                {
                    entityDocumento = ArchivoAsuntosPenalesListExits.FirstOrDefault(c => c.activo)!;
                }

                if (
                    request.documento is not null
                    || request.idTipoArchivo.GetValueOrDefault() > 0
                    || request.idSeccion.GetValueOrDefault() > 0
                )
                {
                    if (entityDocumento is not null)
                    {
                        return Ok(
                            ResultOperation.FailureWarningResponse<int>(
                                "Los parametrós de documento no son requeridos por que ya existe un documento de DOCUMENTACION RELACIONADA."
                            )
                        );
                    }

                    if (request.documento is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."
                            )
                        );
                    }
                }
                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention =
                    {
                        EnumFileType.JPG,
                        EnumFileType.JPEG,
                        EnumFileType.PDF,
                    };
                    if (
                        !_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("ASUNTOS PENALES", request.id.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }


                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }


                try
                {
                    AsuntosPenalesImputadosEvents.UpdateImputadosEtapaComplementaria(
                        ref entityExists!,
                        ref entityImputadosExists,
                        ref entitySentencia,
                        ref entityAcuerdoReparatorio,
                        ref entitySobreseimiento,
                        ref entitySuspension,
                        string.IsNullOrEmpty(request.fechaPlazoInvestigacionComplementaria)
                            ? null!
                            : DateTime.Parse(request.fechaPlazoInvestigacionComplementaria),
                        request.procedimientoAbreviado,
                        request.escritoAcusacion,
                        string.IsNullOrEmpty(request.fechaEscritoAcusacion)
                            ? null!
                            : DateTime.Parse(request.fechaEscritoAcusacion),
                        request.idTipoSentencia,
                        string.IsNullOrEmpty(request.fechaEmisionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEmisionSentencia),
                        request.reparacionDañoSentencia,
                        request.cumplimientoPrivadaLibertadSentencia,
                        request.idAnioSentencia,
                        request.idMesSentencia,
                        request.idDiaSentencia,
                        request.otorgamientoBeneficiosSentencia,
                        request.accionesEjecucionSentencia,
                        string.IsNullOrEmpty(request.fechaEjecucionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEjecucionSentencia),
                        request.conclusionAsuntoSentencia,
                        string.IsNullOrEmpty(request.fechaConclusionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSentencia),
                        request.solucionAlterna,
                        request.idTipoSolucionAlterna,
                        request.condicionesAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaAutorizacionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaAutorizacionAcuerdoReparatorio),
                        request.reparacionDañoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaCelebracionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaCelebracionAcuerdoReparatorio),
                        request.conclusionAsuntoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaConclusionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionAcuerdoReparatorio),
                        request.solicitudSobreseimiento,
                        string.IsNullOrEmpty(request.fechaDeterminacionSobreseimiento)
                            ? null!
                            : DateTime.Parse(request.fechaDeterminacionSobreseimiento),
                        request.conclusionAsuntoSobreseimiento,
                        string.IsNullOrEmpty(request.fechaConclusionSobreseimiento)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSobreseimiento),
                        request.condicionesSuspension!,
                        string.IsNullOrEmpty(request.fechaCelebracionSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaCelebracionSuspension),
                        request.reparacionDañoSuspension,
                        string.IsNullOrEmpty(request.fechaPlazoSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaPlazoSuspension),
                        string.IsNullOrEmpty(request.fechaCumplimientoSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaCumplimientoSuspension),
                        request.conclusionAsuntoSuspension,
                        string.IsNullOrEmpty(request.fechaConclusionSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSuspension),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesImputadosEvents.VerificarEstados(
                        ref entityExists!,
                        entityEtapaInicialListExists,
                        entityImputadosExists
                    );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result =
                    await _asuntosPenalesAdministradorService.UpdateImputadosEtapaComplementariaAsync(
                        entityExists!,
                        entityImputadosExists,
                        entityAcuerdoReparatorio,
                        entitySobreseimiento,
                        entitySuspension,
                        entitySentencia,
                        entityDocumento!,
                        dataFile
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
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-intermedia/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaIntermedia(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetImputadoByIdDisconnectedEtapaIntermedia(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        "Ah ocurrido un error inesperado." + e.Message
                    )
                );
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("imputados/etapa-intermedia")]
        public async Task<IActionResult> PatchEtapaIntermediaInvestigacion(
            [FromForm] RequestUpdateEtapaInvestigacionIntermedia request
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
                var validationResult =
                    await _updateEtapaIntermediaInvestigacionValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );

                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La asunto penal a editar no existe.")
                    );
                }

                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La asunto penal ya se encuentra eliminado, no puede ser editado."
                            )
                        );
                    }
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);

                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }

                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.id
                );

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }

                AsuntosPenalesSentencia entitySentencia = null!;
                AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio = null!;
                AsuntosPenalesSobreseimiento entitySobreseimiento = null!;
                AsuntosPenalesSuspensionCondicional entitySuspension = null!;

                var acuerdoReparatorioListExits =
                    await _asuntosPenalesAdministradorService.GetAcuerdoReparatorioByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                    );
                if (
                    acuerdoReparatorioListExits is not null
                    && acuerdoReparatorioListExits.Any(c => c.activo)
                )
                {
                    entityAcuerdoReparatorio = acuerdoReparatorioListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSentenciaListExits =
                    await _asuntosPenalesAdministradorService.GetSentenciaByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                    );
                if (
                    acuerdoSentenciaListExits is not null
                    && acuerdoSentenciaListExits.Any(c => c.activo)
                )
                {
                    entitySentencia = acuerdoSentenciaListExits.FirstOrDefault(c => c.activo)!;
                }

                var acuerdoSobreseimientoListExits =
                    await _asuntosPenalesAdministradorService.GetSobreseimientoByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                    );
                if (
                    acuerdoSobreseimientoListExits is not null
                    && acuerdoSobreseimientoListExits.Any(c => c.activo)
                )
                {
                    entitySobreseimiento = acuerdoSobreseimientoListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSuspencionListExits =
                    await _asuntosPenalesAdministradorService.GetSuspencionCondicionalByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.INTERMEDIA.GetHashCode()
                    );
                if (
                    acuerdoSuspencionListExits is not null
                    && acuerdoSuspencionListExits.Any(c => c.activo)
                )
                {
                    entitySuspension = acuerdoSuspencionListExits.FirstOrDefault(c => c.activo)!;
                }

                ArchivosAsuntosPenales entityDocumento = null!;
                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAdministradorService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_INTERMEDIA_INVESTIGACIÓN.GetHashCode(),
                        request.id,
                        EnumTipoDocumento.DOCUMENTACIÓN_RELACIONADA.GetHashCode()
                    );
                if (
                    ArchivoAsuntosPenalesListExits is not null
                    && ArchivoAsuntosPenalesListExits.Any(c => c.activo)
                )
                {
                    entityDocumento = ArchivoAsuntosPenalesListExits.FirstOrDefault(c => c.activo)!;
                }

                if (
                    request.documento is not null
                    || request.idTipoArchivo.GetValueOrDefault() > 0
                    || request.idSeccion.GetValueOrDefault() > 0
                )
                {
                    if (entityDocumento is not null)
                    {
                        return Ok(
                            ResultOperation.FailureWarningResponse<int>(
                                "Los parametrós de documento no son requeridos por que ya existe un documento de DOCUMENTACION RELACIONADA."
                            )
                        );
                    }

                    if (request.documento is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."
                            )
                        );
                    }
                }
                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention =
                    {
                        EnumFileType.JPG,
                        EnumFileType.JPEG,
                        EnumFileType.PDF,
                    };
                    if (
                        !_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("ASUNTOS PENALES", request.id.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }

                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }


                try
                {
                    AsuntosPenalesImputadosEvents.UpdateImputadosEtapaIntermedia(
                        ref entityImputadosExists,
                        ref entitySentencia,
                        ref entityAcuerdoReparatorio,
                        ref entitySobreseimiento,
                        ref entitySuspension,
                        string.IsNullOrEmpty(request.fechaAudienciaIntermedia)
                            ? null!
                            : DateTime.Parse(request.fechaAudienciaIntermedia),
                        request.autoAperturaJuicioOral,
                        request.procedimientoAbreviadoIn,
                        string.IsNullOrEmpty(request.fechaAperturaJuicioOral)
                            ? null!
                            : DateTime.Parse(request.fechaAperturaJuicioOral),
                        request.idSentencia,
                        string.IsNullOrEmpty(request.fechaEmisionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEmisionSentencia),
                        request.reparacionDañoSentencia,
                        request.cumplimientoPrivadaLibertadSentencia,
                        request.idAnioSentencia,
                        request.idMesSentencia,
                        request.idDiaSentencia,
                        request.otorgamientoBeneficiosSentencia,
                        request.accionesEjecucionSentencia,
                        string.IsNullOrEmpty(request.fechaEjecucionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEjecucionSentencia),
                        request.conclusionAsuntoSentencia,
                        string.IsNullOrEmpty(request.fechaConclusionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSentencia),
                        request.solucionAlterna,
                        request.idTipoSolucionAlterna,
                        request.condicionesAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaAutorizacionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaAutorizacionAcuerdoReparatorio),
                        request.reparacionDañoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaCelebracionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaCelebracionAcuerdoReparatorio),
                        request.conclusionAsuntoAcuerdoReparatorio,
                        string.IsNullOrEmpty(request.fechaConclusionAcuerdoReparatorio)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionAcuerdoReparatorio),
                        request.solicitudSobreseimiento,
                        string.IsNullOrEmpty(request.fechaDeterminacionSobreseimiento)
                            ? null!
                            : DateTime.Parse(request.fechaDeterminacionSobreseimiento),
                        request.conclusionAsuntoSobreseimiento,
                        string.IsNullOrEmpty(request.fechaConclusionSobreseimiento)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSobreseimiento),
                        request.condicionesSuspension!,
                        string.IsNullOrEmpty(request.fechaCelebracionSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaCelebracionSuspension),
                        request.reparacionDañoSuspension,
                        string.IsNullOrEmpty(request.fechaPlazoSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaPlazoSuspension),
                        string.IsNullOrEmpty(request.fechaCumplimientoSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaCumplimientoSuspension),
                        request.conclusionAsuntoSuspension,
                        string.IsNullOrEmpty(request.fechaConclusionSuspension)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSuspension),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesImputadosEvents.VerificarEstados(
                        ref entityExists!,
                        entityEtapaInicialListExists,
                        entityImputadosExists
                    );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result =
                    await _asuntosPenalesAdministradorService.UpdateImputadosEtapaIntermediaAsync(
                        entityExists!,
                        entityImputadosExists,
                        entityAcuerdoReparatorio,
                        entitySobreseimiento,
                        entitySuspension,
                        entitySentencia,
                        entityDocumento!,
                        dataFile
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
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-juicio/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaJuicio(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetImputadoByIdDisconnectedEtapaJuicio(id);
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("imputados/etapa-juicio")]
        public async Task<IActionResult> PatchEtapaJuicioInvestigacion(
            [FromForm] RequestUpdateEtapaInvestigacionJuicio request
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
                var validationResult = await _updateEtapaJuicioInvestigacionValidator.ValidateAsync(
                    request
                );
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );

                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La asunto penal a editar no existe.")
                    );
                }

                if (entityExists is not null)
                {
                    if (!(entityExists.activo))
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse(
                                "La asunto penal ya se encuentra eliminado, no puede ser editado."
                            )
                        );
                    }
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);

                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }

                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.id
                );

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }

                AsuntosPenalesSentencia entitySentencia = null!;
                AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio = null!;
                AsuntosPenalesSobreseimiento entitySobreseimiento = null!;
                AsuntosPenalesSuspensionCondicional entitySuspension = null!;

                var acuerdoReparatorioListExits =
                    await _asuntosPenalesAdministradorService.GetAcuerdoReparatorioByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.JUCIO.GetHashCode()
                    );
                if (
                    acuerdoReparatorioListExits is not null
                    && acuerdoReparatorioListExits.Any(c => c.activo)
                )
                {
                    entityAcuerdoReparatorio = acuerdoReparatorioListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSentenciaListExits =
                    await _asuntosPenalesAdministradorService.GetSentenciaByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.JUCIO.GetHashCode()
                    );
                if (
                    acuerdoSentenciaListExits is not null
                    && acuerdoSentenciaListExits.Any(c => c.activo)
                )
                {
                    entitySentencia = acuerdoSentenciaListExits.FirstOrDefault(c => c.activo)!;
                }

                var acuerdoSobreseimientoListExits =
                    await _asuntosPenalesAdministradorService.GetSobreseimientoByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.JUCIO.GetHashCode()
                    );
                if (
                    acuerdoSobreseimientoListExits is not null
                    && acuerdoSobreseimientoListExits.Any(c => c.activo)
                )
                {
                    entitySobreseimiento = acuerdoSobreseimientoListExits.FirstOrDefault(c =>
                        c.activo
                    )!;
                }

                var acuerdoSuspencionListExits =
                    await _asuntosPenalesAdministradorService.GetSuspencionCondicionalByImputado(
                        request.id,
                        EnumTipoEtapaInvestigacion.JUCIO.GetHashCode()
                    );
                if (
                    acuerdoSuspencionListExits is not null
                    && acuerdoSuspencionListExits.Any(c => c.activo)
                )
                {
                    entitySuspension = acuerdoSuspencionListExits.FirstOrDefault(c => c.activo)!;
                }

                ArchivosAsuntosPenales entityDocumento = null!;
                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAdministradorService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_JUICIO_INVESTIGACIÓN.GetHashCode(),
                        request.id,
                        EnumTipoDocumento.DOCUMENTACIÓN_RELACIONADA.GetHashCode()
                    );
                if (
                    ArchivoAsuntosPenalesListExits is not null
                    && ArchivoAsuntosPenalesListExits.Any(c => c.activo)
                )
                {
                    entityDocumento = ArchivoAsuntosPenalesListExits.FirstOrDefault(c => c.activo)!;
                }

                if (
                    request.documento is not null
                    || request.idTipoArchivo.GetValueOrDefault() > 0
                    || request.idSeccion.GetValueOrDefault() > 0
                )
                {
                    if (entityDocumento is not null)
                    {
                        return Ok(
                            ResultOperation.FailureWarningResponse<int>(
                                "Los parametrós de documento no son requeridos por que ya existe un documento de DOCUMENTACION RELACIONADA."
                            )
                        );
                    }

                    if (request.documento is null)
                    {
                        return Ok(
                            ResultOperation.FailureErrorResponse<int>(
                                "El documento es obligatorio."
                            )
                        );
                    }
                }
                DataFile dataFile = null!;
                if (request.documento is not null)
                {
                    EnumFileType[] requiredExtention =
                    {
                        EnumFileType.JPG,
                        EnumFileType.JPEG,
                        EnumFileType.PDF,
                    };
                    if (
                        !_fileSystemService.FileTryOut(
                            request.documento,
                            Path.Combine("ASUNTOS PENALES", request.id.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }

                try
                {
                    AsuntosPenalesImputadosEvents.UpdateImputadosEtapaJuicio(
                        ref entityImputadosExists,
                        ref entitySentencia,
                        ref entityAcuerdoReparatorio,
                        ref entitySobreseimiento,
                        ref entitySuspension,
                        string.IsNullOrEmpty(request.fechaInicialAudienciaJuicio)
                            ? null!
                            : DateTime.Parse(request.fechaInicialAudienciaJuicio),
                        string.IsNullOrEmpty(request.fechaFinalAudienciaJuicio)
                            ? null!
                            : DateTime.Parse(request.fechaFinalAudienciaJuicio),
                        request.idSentencia,
                        string.IsNullOrEmpty(request.fechaEmisionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEmisionSentencia),
                        request.reparacionDañoSentencia,
                        request.cumplimientoPrivadaLibertadSentencia,
                        request.idAnioSentencia,
                        request.idMesSentencia,
                        request.idDiaSentencia,
                        request.otorgamientoBeneficiosSentencia,
                        request.accionesEjecucionSentencia,
                        string.IsNullOrEmpty(request.fechaEjecucionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaEjecucionSentencia),
                        request.conclusionAsuntoSentencia,
                        string.IsNullOrEmpty(request.fechaConclusionSentencia)
                            ? null!
                            : DateTime.Parse(request.fechaConclusionSentencia),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesImputadosEvents.VerificarEstados(
                        ref entityExists!,
                        entityEtapaInicialListExists,
                        entityImputadosExists
                    );

                    if (dataFile is not null)
                    {
                        entityDocumento = ArchivosAsuntosPenalesEvents.CreateArchivo(
                            request.idAsuntoPenal,
                            request.idTipoArchivo.GetValueOrDefault(),
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            request.documento!.ContentType,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            request.idSeccion,
                            null!,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            true,
                            EnumRolesSicoj.OFICIAL_DE_PARTES.GetHashCode()
                        );
                    }
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result =
                    await _asuntosPenalesAdministradorService.UpdateImputadosEtapaJuicioAsync(
                        entityExists!,
                        entityImputadosExists,
                        entityAcuerdoReparatorio,
                        entitySobreseimiento,
                        entitySuspension,
                        entitySentencia,
                        entityDocumento!,
                        dataFile
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
        /// Método para agreagr medidas cautelares abogado
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("medidas-cautelares")]
        public async Task<IActionResult> PostMedidasCautelares(RequestMedidasCautelares request)
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

                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(
                    $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"
                );
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "El registro no se puede editar ya que el usuario no lo ha apartado."
                        )
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

                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminada."
                        )
                    );
                }

                var validationResult =
                    await _createMedidasCautelaresAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" -- "))
                    );
                }

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.idAsuntoPenal);
                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }
                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.idImputado
                );
                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }

                List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares = new();
                foreach (var item in request.idMedida)
                {
                    try
                    {
                        entityMedidasCautelares.Add(
                            AsuntosPenalesMedidasCauteralesEvents.Create(
                                entityImputadosExists,
                                item,
                                sessionInformation.UserInformation.Rfc
                            )
                        );
                    }
                    catch (Exception _ex)
                    {
                        return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                    }
                }
                var result = await _asuntosPenalesAdministradorService.MedidasCautelaresAsync(
                    entityMedidasCautelares
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

        // <summary>
        // Método para listar medidas cautelares
        // </summary>
        // <param name="id">Id de imputado </param>
        // <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseMedidasCautelares>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-medidas-cautelares/{id}")]
        public async Task<IActionResult> GetMedidasCautelares(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorService.GetAllMedidasCautelares(
                    id
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
        /// Método para eliminar medidas cautelares
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("medidas-cautelares")]
        public async Task<IActionResult> MedidaCautelar(RequestDeleteMedidaCautelar request)
        {
            try
            {
                var validationResult = await _deleteMedidaCautelarValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAdministradorService.GetAsuntoPenalById(
                    request.id_asunto_penal
                );
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<string>(
                            "El asunto penal no existe o fue eliminado."
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

                var entityEtapaInicialListExists =
                    await _asuntosPenalesAdministradorService.GetAllImputados(request.id_asunto_penal);
                if (
                    entityEtapaInicialListExists is null
                    || !entityEtapaInicialListExists.Any(c => c.activo)
                )
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El asunto penal no contiene imputados"
                        )
                    );
                }
                var entityImputadosExists = entityEtapaInicialListExists.FirstOrDefault(c =>
                    c.id == request.id_imputado
                );
                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no existe o se encuentra eliminado."
                        )
                    );
                }
                if (entityExists.id != entityImputadosExists.id_asunto_penal)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El imputado no pertenece al asunto penal."
                        )
                    );
                }

                var entityMedidaCautelarExists =
                    await _asuntosPenalesAdministradorService.GetMedidaCautelarById(request.id);
                if (entityMedidaCautelarExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("La medida cautelar no existe.")
                    );
                }
                if (entityImputadosExists.id != entityMedidaCautelarExists.id_imputado)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La medida cautelar no pertenece al imputado."
                        )
                    );
                }
                try
                {
                    AsuntosPenalesMedidasCauteralesEvents.DeleteMedidaCautelar(
                        ref entityMedidaCautelarExists
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorService.DeleteMedidasCautelares(
                    entityMedidaCautelarExists
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

        #region 
        /// <summary>
        /// Exportar : Reporte Administrador
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(
            typeof(ResultOperation<>),
            200
        )]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("exportar")]
        public async Task<IActionResult> Exportar_Excel([FromQuery] QueryFilters request)
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
                Filters.MapFiltersQuery(request, filters);


                var datos = await _asuntosPenalesAdministradorService.ExportarReporteGeneral(
                    Filters.GetStringValue(filters!.ByNoAsuntoPenal.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaRecepcionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaVencimientoHasta.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByOficioSolicitud.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByNoExpedienteCadido.FirstOrDefault()),
                    filters.ByIdUnidadRealizoSolicitud.Adapt<List<int>>(),
                    Filters.GetIntValue(filters.ByAdminControl.FirstOrDefault()),
                    Filters.GetIntValue(filters.BySubadministracion.FirstOrDefault()),
                    Filters.GetStringValue(filters.ByNombreAbogado.FirstOrDefault()),
                    filters.ByIdEstadoTarea.Adapt<List<int>>(),
                    filters.ByIdEstadoProcesal.Adapt<List<int>>(),
                    filters.ByIdTipoConclusión.Adapt<List<int>>(),
                    Filters.GetDateTimeValue(filters.ByFechaConclusionDesde.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaConclusionHasta.FirstOrDefault()),
                    filters.ByDeterminacionAsunto.Adapt<List<int>>(),
                    filters.ByRequisitosProcedibilidad.Adapt<List<int>>(),
                    filters.ByDelito.Adapt<List<int>>(),
                    Filters.GetIntValue(filters.ByTipoSolucionAlterna.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaPresentaciónRequisito.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaDelAutoVinculacion.FirstOrDefault()),
                    Filters.GetDateTimeValue(filters.ByFechaEmisionSentencia.FirstOrDefault())
                    );

                if (datos is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "No se encontraron resultados"
                        )
                    );
                }


                using var workbook = AsuntosPenalesAdministradorEvents.GenerarExcelClosedXmlGeneral(datos);
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteGlobal.xlsx");



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

                    entity = AsuntosPenalesAdministradorEvents.CreateModalidadSolicitudTransparencia(
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


                var result = await _asuntosPenalesAdministradorService.AddSolicitudTransparenciaService(entity, entityDocumento, dataFile!);
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
        /// Edita solicitud transparencia : Administrador
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
                var entityExists = await _asuntosPenalesAdministradorService.GetByIdSolicitudTransparenciaService(request.id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La consulta no existe."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesAdministradorEvents.UpdateSolicitudTransparencia(ref entityExists,
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
                var result = await _asuntosPenalesAdministradorService.UpdateSolicitudTransparenciaService(entityExists);
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
        /// Solicitud Transparencia  por Id  : Administrador
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

                var result = await _asuntosPenalesAdministradorService.GetByIdSolicitudTransparenciaServices(id);
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
        /// Tabla de Solicitud Transparencia : Administrador
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


                var result = await _asuntosPenalesAdministradorService.GetTablaSolicitudTransparenciaService(idAsunto);

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
        /// Elimina solicitud Transparencia : Administrador
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

                var entityExists = await _asuntosPenalesAdministradorService.GetByIdSolicitudTransparenciaService(id);
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
                    AsuntosPenalesAdministradorEvents.DeleteSolicitudTransparencia(ref entityExists,
                    id
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesAdministradorService.DeleteSolicitudTransparenciaService(entityExists);
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