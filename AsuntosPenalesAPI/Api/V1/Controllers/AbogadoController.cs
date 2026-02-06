using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.DTO.ContractsValidations;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events;
using AsuntosPenalesAPI.Model.Entities.Events.Abogado;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using FluentValidation;
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
    [Route("sicoj/asuntos-penales/api/v1/abogado/asuntos-penales")]
    public class AbogadoController : ControllerBase
    {
        private readonly ILogger<AbogadoController> _logger;
        private readonly IRedisClient _redisClient;
        private readonly IAsuntosPenalesAbogadoService _asuntosPenalesAbogadoService;
        private readonly RequestRemisionAsuntosPenalesValidator _createRemisionAsuntosPenalesValidator;
        private readonly RequestAnalisisAsuntosPenalesValidator _createAnalisisAsuntosPenalesValidator;
        private readonly RequestProcedibilidadAsuntosPenalesValidator _createProcedibilidadAsuntosPenalesValidator;
        private readonly RequestPersonasMoralesAsuntosPenalesValidator _createPersonasMoralesAsuntosPenalesValidator;
        private readonly RequestDeleteArchivosAsuntosPenalesValidator _deleteArchivosAsuntosPenalesValidator;
        private readonly RequestDelitosAsuntosPenalesValidator _createDelitosAsuntosPenalesValidator;
        private readonly RequestUpdateProcedibilidadValidator _updateProcedibilidadValidator;
        private readonly RequestUpdateAnalisisValidator _updateAnalisisValidator;
        private readonly RequestDeleteDelitoValidator _deleteDelitoValidator;
        private readonly RequestDeleteMedidaCautelarValidator _deleteMedidaCautelarValidator;
        private readonly RequestImputadosAsuntosPenalesValidator _createImputadosAsuntosPenalesValidator;
        private readonly RequestUpdateImputadosValidator _updateImputadosValidator;
        private readonly RequestUpdateEtapaInicialInvestigacionValidator _updateEtapaInicialInvestigacionValidator;
        private readonly RequestEtapaInicialInvestigacionAsuntosPenalesValidator _createEtapaInicialInvestigacionAsuntosPenalesValidator;
        private readonly RequestMedidasCautelaresAsuntosPenalesValidator _createMedidasCautelaresAsuntosPenalesValidator;
        private readonly RequestUpdateEtapaComplementariaInvestigacionValidator _updateEtapaComplementariaInvestigacionValidator;
        private readonly RequestUpdateFechaPlazoEtapaComplementariaInvestigacionValidator _updateFechaPlazoEtapaComplementariaInvestigacionValidator;
        private readonly RequestUpdateEtapaIntermediaInvestigacionValidator _updateEtapaIntermediaInvestigacionValidator;
        private readonly RequestUpdateEtapaJuicioInvestigacionValidator _updateEtapaJuicioInvestigacionValidator;
        private readonly RequestImputadosApelacionAsuntosPenalesValidator _createImputadosApelacionAsuntosPenalesValidator;
        private readonly RequestUpdateImputadosApelacionAsuntosPenalesValidator _updateImputadosApelacionAsuntosPenalesValidator;
        private readonly RequestDeleteApelacionValidator _deleteApelacionValidator;
        private readonly RequestImputadosAmparoAsuntosPenalesValidator _createImputadosAmparoAsuntosPenalesValidator;
        private readonly RequestUpdateImputadosAmparoAsuntosPenalesValidator _updateImputadosAmparoAsuntosPenalesValidator;
        private readonly RequestDeleteAmparoValidator _deleteAmparoValidator;
        private readonly IValidator<RequestCreateSolicitudTransparencia> _validatorRequestCreateSolicitudTransparencia;
        private readonly IValidator<RequestUpdateSolicitudTransparencia> _validatorRequestUpdateSolicitudTransparencia;
        private static object _lock = new object();
        private readonly IFileSystemService _fileSystemService;
        private readonly IGenericService _genericService;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;

        public AbogadoController(
            ILogger<AbogadoController> logger,
            IRedisClient redisClient,
            IAsuntosPenalesAbogadoService asuntosPenalesAbogadoService,
            IAsuntosPenalesAbogadoRepository asuntosPenalesRepositoryAbogado,
            RequestRemisionAsuntosPenalesValidator createRemisionAsuntosPenalesValidator,
            IFileSystemService fileSystemService,
            RequestAnalisisAsuntosPenalesValidator createAnalisisAsuntosPenalesValidator,
            RequestProcedibilidadAsuntosPenalesValidator createProcedibilidadAsuntosPenalesValidator,
            RequestDeleteArchivosAsuntosPenalesValidator deleteArchivosAsuntosPenalesValidator,
            RequestPersonasMoralesAsuntosPenalesValidator createPersonasMoralesAsuntosPenalesValidator,
            RequestDelitosAsuntosPenalesValidator createDelitosAsuntosPenalesValidator,
            RequestUpdateProcedibilidadValidator updateProcedibilidadValidator,
            RequestUpdateAnalisisValidator updateAnalisisValidator,
            RequestDeleteDelitoValidator deleteDelitoValidator,
            RequestImputadosAsuntosPenalesValidator createImputadosAsuntosPenalesValidator,
            RequestUpdateImputadosValidator updateImputadosValidator,
            RequestUpdateEtapaInicialInvestigacionValidator updateEtapaInicialInvestigacionValidator,
            RequestEtapaInicialInvestigacionAsuntosPenalesValidator createEtapaInicialInvestigacionAsuntosPenalesValidator,
            RequestMedidasCautelaresAsuntosPenalesValidator createMedidasCautelaresAsuntosPenalesValidator,
            RequestDeleteMedidaCautelarValidator deleteMedidaCautelarValidator,
            RequestUpdateEtapaComplementariaInvestigacionValidator updateEtapaComplementariaInvestigacionValidator,
            RequestUpdateEtapaIntermediaInvestigacionValidator updateEtapaIntermediaInvestigacionValidator,
            RequestUpdateEtapaJuicioInvestigacionValidator updateEtapaJuicioInvestigacionValidator,
            RequestUpdateFechaPlazoEtapaComplementariaInvestigacionValidator updateFechaPlazoEtapaComplementariaInvestigacionValidator,
            RequestImputadosApelacionAsuntosPenalesValidator createImputadosApelacionAsuntosPenalesValidator,
            RequestUpdateImputadosApelacionAsuntosPenalesValidator updateImputadosApelacionAsuntosPenalesValidator,
            RequestDeleteApelacionValidator deleteApelacionValidator,
            RequestImputadosAmparoAsuntosPenalesValidator createImputadosAmparoAsuntosPenalesValidator,
            RequestUpdateImputadosAmparoAsuntosPenalesValidator updateImputadosAmparoAsuntosPenalesValidator,
            RequestDeleteAmparoValidator deleteAmparoValidator,
            IValidator<RequestCreateSolicitudTransparencia> validatorRequestCreateSolicitudTransparencia,
            IValidator<RequestUpdateSolicitudTransparencia> validatorUpdateSolicitudTransparencia,
            IValidator<RequestDocumentoUpdate> requestDocumentoUpdateValidator,
            IGenericService genericService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _asuntosPenalesAbogadoService =
                asuntosPenalesAbogadoService
                ?? throw new ArgumentNullException(nameof(asuntosPenalesAbogadoService));
            _fileSystemService =
                fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _createRemisionAsuntosPenalesValidator =
                createRemisionAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createRemisionAsuntosPenalesValidator));
            _createAnalisisAsuntosPenalesValidator =
                createAnalisisAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createAnalisisAsuntosPenalesValidator));
            _createProcedibilidadAsuntosPenalesValidator =
                createProcedibilidadAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createProcedibilidadAsuntosPenalesValidator)
                );
            _deleteArchivosAsuntosPenalesValidator =
                deleteArchivosAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(deleteArchivosAsuntosPenalesValidator));
            _createPersonasMoralesAsuntosPenalesValidator =
                createPersonasMoralesAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createPersonasMoralesAsuntosPenalesValidator)
                );
            _createDelitosAsuntosPenalesValidator =
                createDelitosAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createDelitosAsuntosPenalesValidator));
            _updateProcedibilidadValidator =
                updateProcedibilidadValidator
                ?? throw new ArgumentNullException(nameof(updateProcedibilidadValidator));
            _updateAnalisisValidator =
                updateAnalisisValidator
                ?? throw new ArgumentNullException(nameof(updateAnalisisValidator));
            _deleteDelitoValidator =
                deleteDelitoValidator
                ?? throw new ArgumentNullException(nameof(deleteDelitoValidator));
            _createImputadosAsuntosPenalesValidator =
                createImputadosAsuntosPenalesValidator
                ?? throw new ArgumentNullException(nameof(createImputadosAsuntosPenalesValidator));
            _updateImputadosValidator =
                updateImputadosValidator
                ?? throw new ArgumentNullException(nameof(updateImputadosValidator));
            _updateEtapaInicialInvestigacionValidator =
                updateEtapaInicialInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(updateEtapaInicialInvestigacionValidator)
                );
            _createEtapaInicialInvestigacionAsuntosPenalesValidator =
                createEtapaInicialInvestigacionAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createEtapaInicialInvestigacionAsuntosPenalesValidator)
                );
            _createMedidasCautelaresAsuntosPenalesValidator =
                createMedidasCautelaresAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createMedidasCautelaresAsuntosPenalesValidator)
                );
            _deleteMedidaCautelarValidator =
                deleteMedidaCautelarValidator
                ?? throw new ArgumentNullException(nameof(_deleteMedidaCautelarValidator));
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
            _updateFechaPlazoEtapaComplementariaInvestigacionValidator =
                updateFechaPlazoEtapaComplementariaInvestigacionValidator
                ?? throw new ArgumentNullException(
                    nameof(updateFechaPlazoEtapaComplementariaInvestigacionValidator)
                );
            _createImputadosApelacionAsuntosPenalesValidator =
                createImputadosApelacionAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createImputadosApelacionAsuntosPenalesValidator)
                );
            _updateImputadosApelacionAsuntosPenalesValidator =
                updateImputadosApelacionAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(updateImputadosApelacionAsuntosPenalesValidator)
                );
            _deleteApelacionValidator =
                deleteApelacionValidator
                ?? throw new ArgumentNullException(
                    nameof(deleteApelacionValidator)
                );
            _createImputadosAmparoAsuntosPenalesValidator =
                createImputadosAmparoAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(createImputadosAmparoAsuntosPenalesValidator)
                );
            _updateImputadosAmparoAsuntosPenalesValidator =
                updateImputadosAmparoAsuntosPenalesValidator
                ?? throw new ArgumentNullException(
                    nameof(updateImputadosAmparoAsuntosPenalesValidator)
                );
            _deleteAmparoValidator =
                deleteAmparoValidator
                ?? throw new ArgumentNullException(
                    nameof(deleteAmparoValidator)
                );
            _validatorRequestCreateSolicitudTransparencia =
            validatorRequestCreateSolicitudTransparencia
            ?? throw new ArgumentNullException(nameof(validatorRequestCreateSolicitudTransparencia));
            _validatorRequestUpdateSolicitudTransparencia =
            validatorUpdateSolicitudTransparencia
            ?? throw new ArgumentNullException(nameof(validatorUpdateSolicitudTransparencia));
            _requestDocumentoUpdateValidator =
                requestDocumentoUpdateValidator
                ?? throw new ArgumentNullException(
                nameof(requestDocumentoUpdateValidator)
                );
            _genericService =
                genericService
                ?? throw new ArgumentNullException(
                nameof(genericService)
                );

        }


        /// <summary>
        /// Método tomar el registro
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(id);
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
        /// Método para soltar el registro
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(id);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<bool>("El asunto penal no existe.")
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
        /// Método para consultar Asunto Penal por Id
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

                var result = await _asuntosPenalesAbogadoService.GetAbogadoAsuntoPenalById(
                    id,
                    sessionInformation.UserInformation.Rfc
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
        /// Método para consultar bandeja de historico de Abogado
        /// </summary>
        /// <param name="id">filtros</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAbogadoByFilters>), 200)]
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

                var result = await _asuntosPenalesAbogadoService.GetHistoricoAsync(
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
                    sessionInformation.UserInformation.ListRoles[0].IdUnidadAdministrativa,
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
        /// Método para listar los asuntos penales remitidos de un Abogado 
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseRemision>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-remisiones/{id}")]
        public async Task<IActionResult> GetRemisiones(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetAllRemision(id);
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
        /// Método para la bandeja de pendientes del oficial de partes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAbogadoByFilters>), 200)]
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
                if (
                    !Filters.MapSort<EnumOrderColumnAsuntosPenales>(
                        request,
                        null!,
                        false,
                        out string orderByColumn,
                        out bool orderDesc
                    )
                )
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse<
                            List<ResponseAsuntosPenalesAbogadoByFilters>>("La columna de ordenamiento no es válida.")
                    );
                }
                var result = await _asuntosPenalesAbogadoService.GetBandejaAsync(
                    request.fetch,
                    request.page,
                    orderByColumn,
                    orderDesc,
                    sessionInformation.UserInformation.IdSubadministracion,
                    sessionInformation.UserInformation.Rfc
                );
                if (result is null)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse("No se encontraron resultados.")
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                var result = await _asuntosPenalesAbogadoService.AnalisisAsync(
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
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdAnalisisDisconnected(
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
        /// Metodo para consultar Imputados de un Asunto penalv
        /// </summary>
        /// <param name="id">Id asunto penal </param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-imputados/{id}")]
        public async Task<IActionResult> GetImputadosDisconnected(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetAllImputadosDisconnected(
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

        ///Método para consultar Asuntos Penales por Id
        /// </summary>
        /// <param name="id">Id Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputados(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(id);
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                var result = await _asuntosPenalesAbogadoService.ProcedibilidadAsync(
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdAnalisis(request.id);
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
                var result = await _asuntosPenalesAbogadoService.UpdateAnalisis(
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                var result = await _asuntosPenalesAbogadoService.PersonasMoralesAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetAllPersonasMorales(
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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

                var result = await _asuntosPenalesAbogadoService.DelitosAsync(entityDelitos);
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
                var result = await _asuntosPenalesAbogadoService.GetAllDelitos(id);
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                var entityDelitosExists = await _asuntosPenalesAbogadoService.GetDelitoById(
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
                try
                {
                    AsuntosPenalesAbogadoEvents.DeleteDelito(ref entityDelitosExists);
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.DeleteDelitos(entityDelitosExists);
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
        //Método para Agregar un archivo al registro de AP
        // </summary>
        // <param name="request">Datos del archivo y del registro de AP </param>
        // <returns>Id del registro de archivo agregado</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPost("archivo")]
        public async Task<IActionResult> PostFile(
            [FromForm] RequestCreateFileAsuntosPenales request
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
                    request.idAsuntosPenales
                );
                if (entityExists is null)
                {
                    return Ok(ResultOperation.FailureErrorResponse("El Asunto Penal no existe."));
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La asunto penal ya se encuentra eliminado no se puede agregar un archivo."
                        )
                    );
                }

                EnumFileType[] requiredExtention =
                {
                    EnumFileType.JPG,
                    EnumFileType.JPEG,
                    EnumFileType.PDF,
                };
                if (
                    !_fileSystemService.FileTryOut(
                        request.FileAsuntosPenales,
                        Path.Combine("ASUNTOS PENALES", request.idAsuntosPenales.ToString()),
                        out var dataFile,
                        out string message,
                        requiredExtention
                    )
                )
                {
                    return Ok(ResultOperation.FailureErrorResponse(message));
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
                        EnumRolesSicoj.ABOGADO.GetHashCode()
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.AddArchivosAsyncService(
                    entity,
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

        // <summary>
        // Método para borrar Archivos
        // </summary>
        // <param name="id">id del registro que se desea eliminar o inactivar</param>
        // <returns>Result Operation con mensaje de operación exitosa</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("archivo")]
        public async Task<IActionResult> DeleteArchivos(RequestDeleteArchivosAsuntosPenales request)
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
                        AsuntosPenalesAbogadoEvents.DeleteModalidaArchivoAsuntosPenales(ref entity, sessionInformation.UserInformation.Rfc);
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

        // <summary>
        //Método para listar los archivos asociados a una consulta
        // </summary>
        // <param name="id">Id Autorización Comercio Exterior</param>
        // <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseArchivosAsuntosPenales>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("list-archivos/{id}")]
        public async Task<IActionResult> GetAllArchivosAsync(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetAllArchivoService(
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
                var entityExists = await _asuntosPenalesAbogadoService.GetByIdArchivoService(id);
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
                    FileName = Path.GetFileName(entityExists.path_file),
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
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidadDisconnected(
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
        [HttpPatch("procedibilidad")]
        public async Task<IActionResult> PatchProcedibilidad(RequestUpdateProcedibilidad request)
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

                var validationResult = await _updateProcedibilidadValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
                    request.idAsuntoPenal
                );
                if (entityExists is null)
                {
                    return Ok(ResultOperation.FailureErrorResponse("La asunto penal no existe."));
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
                }

                var entityProcedibilidadExists =
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidad(
                        request.id
                    );
                if (entityProcedibilidadExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("El caso de procedibilidad no existe.")
                    );
                }

                if (!entityProcedibilidadExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El caso de procedibilidad ya encuentra eliminado, no puede ser editado."
                        )
                    );
                }
                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    entityProcedibilidadExists.id_estado_procesal = modificacion.id_estado_procesal;
                }
                try
                {
                    AsuntosPenalesProcedibilidadEvents.UpdateProcedibilidad(
                    ref entityProcedibilidadExists,
                    ref entityExists!,
                    request.id_requisito_procedibilidad,
                    request.numero_oficio_procedibilidad,
                    DateTime.Parse(request.fecha_presentacion),
                    request.cuantia,
                    request.numero_carpeta_investigacion,
                    request.agente_ministerio_publico,
                    sessionInformation.UserInformation.Rfc!
                );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.UpdateProcedibilidad(
                    entityProcedibilidadExists
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);


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
                var result = await _asuntosPenalesAbogadoService.ImputadosAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnectedEtapaInicial(id);
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                    await _asuntosPenalesAbogadoService.GetAcuerdoReparatorioByImputado(
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
                    await _asuntosPenalesAbogadoService.GetCriterioOportunidadByImputado(
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
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
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
                var result = await _asuntosPenalesAbogadoService.UpdateImputadosEtapaInicialAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnectedEtapaComplementaria(id);
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
        [HttpPatch("imputados/etapa-complementaria/fecha-plazo")]
        public async Task<IActionResult> PatchEtapaComplementariaFechaPlazoInvestigacion(
            [FromForm] RequestUpdateFechaPlazoEtapaInvestigacionComplementaria request
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
                    await _updateFechaPlazoEtapaComplementariaInvestigacionValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }

                try
                {
                    AsuntosPenalesImputadosEvents.UpdateImputadosEtapaComplementariaFechaPlazoInvestigacion(
                        ref entityExists!,
                        ref entityImputadosExists,

                        string.IsNullOrEmpty(request.fechaPlazoInvestigacionComplementaria)
                            ? null!
                            : DateTime.Parse(request.fechaPlazoInvestigacionComplementaria),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result =
                    await _asuntosPenalesAbogadoService.UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
                        entityExists!,
                        entityImputadosExists
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                    await _asuntosPenalesAbogadoService.GetAcuerdoReparatorioByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSentenciaByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSobreseimientoByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSuspencionCondicionalByImputado(
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
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
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
                    await _asuntosPenalesAbogadoService.UpdateImputadosEtapaComplementariaAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnectedEtapaIntermedia(id);
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                    await _asuntosPenalesAbogadoService.GetAcuerdoReparatorioByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSentenciaByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSobreseimientoByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSuspencionCondicionalByImputado(
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
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
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
                    await _asuntosPenalesAbogadoService.UpdateImputadosEtapaIntermediaAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnectedEtapaJuicio(id);
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                    await _asuntosPenalesAbogadoService.GetAcuerdoReparatorioByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSentenciaByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSobreseimientoByImputado(
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
                    await _asuntosPenalesAbogadoService.GetSuspencionCondicionalByImputado(
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
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
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
                    await _asuntosPenalesAbogadoService.UpdateImputadosEtapaJuicioAsync(
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

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);
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
                            AsuntosPenalesMedidasCautelaresEvents.Create(
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
                var result = await _asuntosPenalesAbogadoService.MedidasCautelaresAsync(
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
                var result = await _asuntosPenalesAbogadoService.GetAllMedidasCautelares(
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
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.id_asunto_penal);
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
                    await _asuntosPenalesAbogadoService.GetMedidaCautelarById(request.id);
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
                    AsuntosPenalesMedidasCautelaresEvents.DeleteMedidaCautelar(
                        ref entityMedidaCautelarExists
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.DeleteMedidasCautelares(
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseApelacion>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-apelacion/list/{id}")]
        public async Task<IActionResult> GetApelaciones(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetApelacionDisconnected(id);
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

        [ProducesResponseType(typeof(ResultOperation<ResponseApelacion>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-apelacion/{id}")]
        public async Task<IActionResult> GetApelacionById(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetApelacionDisconnectedById(id);
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
        [HttpPost("imputados/etapa-apelacion")]
        public async Task<IActionResult> PostEtapaApelacion(
            [FromForm] RequestApelacion request
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
                    await _createImputadosApelacionAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                ArchivosAsuntosPenales entityDocumento = null!;
                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_APELACION_INVESTIGACIÓN.GetHashCode(),
                        request.idImputado,
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
                            Path.Combine("ASUNTOS PENALES", request.idImputado.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }

                AsuntosPenalesApelacion entityApelacion = null!;
                try
                {
                    entityApelacion = AsuntosPenalesImputadosEvents.CreateApelacion(
                        ref entityImputadosExists,
                        string.IsNullOrEmpty(request.fechaPresentacion)
                            ? null!
                            : DateTime.Parse(request.fechaPresentacion),
                        request.numeroTocaPenal,
                        request.TipoOrganoJurisdiccional,
                        request.idTipoResolucion,
                        string.IsNullOrEmpty(request.fechaResolucion)
                            ? null!
                            : DateTime.Parse(request.fechaResolucion),

                        request.descripcionResolucion,
                        sessionInformation.UserInformation.Rfc!

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
                var result = await _asuntosPenalesAbogadoService.ApelacionImputadosAsync(
                    entityExists!,
                    entityImputadosExists,
                    entityApelacion,
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
        /// Método para turnar asuntos penales
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("imputados/etapa-apelacion")]
        public async Task<IActionResult> PatchEtapaApelacion(
            [FromBody] RequestUpdateApelacion request
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
                    await _updateImputadosApelacionAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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

                var entityApelacionExists =
                    await _asuntosPenalesAbogadoService.GetApelacionById(request.id);

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La Apelacion no existe o se encuentra eliminado."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesImputadosEvents.UpdateApelacion(
                        ref entityApelacionExists,
                        string.IsNullOrEmpty(request.fechaPresentacion)
                            ? null!
                            : DateTime.Parse(request.fechaPresentacion),
                        request.numeroTocaPenal,
                        request.TipoOrganoJurisdiccional,
                        request.idTipoResolucion,
                        string.IsNullOrEmpty(request.fechaResolucion)
                            ? null!
                            : DateTime.Parse(request.fechaResolucion),

                        request.descripcionResolucion,
                        sessionInformation.UserInformation.Rfc!

                    );

                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.ApelacionImputadosUpdateAsync(
                    entityExists!,
                    entityImputadosExists,
                    entityApelacionExists
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
        /// Método para eliminar medidas apelacion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("imputados/apelacion")]
        public async Task<IActionResult> DeleteApelacion(RequestDeleteApelacion request)
        {
            try
            {
                var validationResult = await _deleteApelacionValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.id_asunto_penal);
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

                var entityApelacionExists =
                    await _asuntosPenalesAbogadoService.GetApelacionById(request.id);

                if (entityImputadosExists is null || !entityImputadosExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La Apelacion no existe o se encuentra eliminado."
                        )
                    );
                }

                if (entityImputadosExists.id != entityApelacionExists.id_imputado)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La medida cautelar no pertenece al imputado."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesImputadosEvents.DeleteApelacion(
                        ref entityApelacionExists
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.DeleteApelacion(
                    entityApelacionExists
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

        [ProducesResponseType(typeof(ResultOperation<List<ResponseAmparo>>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-amparo/list/{id}")]
        public async Task<IActionResult> GetAmparo(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetAmparoDisconnected(id);
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

        [ProducesResponseType(typeof(ResultOperation<ResponseAmparo>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-amparo/{id}")]
        public async Task<IActionResult> GetAmparoById(int id)
        {
            try
            {
                var result = await _asuntosPenalesAbogadoService.GetAmparoDisconnectedById(id);
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
        [HttpPost("imputados/etapa-amparo")]
        public async Task<IActionResult> PostEtapaAmparo(
            [FromForm] RequestAmparo request
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
                    await _createImputadosAmparoAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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
                ArchivosAsuntosPenales entityDocumento = null!;
                var ArchivoAsuntosPenalesListExits =
                    await _asuntosPenalesAbogadoService.GetArchivosByIdRenglonSeccion_Async_Repository(
                        request.idAsuntoPenal,
                        EnumSecciones.ETAPA_AMPARO_INVESTIGACIÓN.GetHashCode(),
                        request.idImputado,
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
                            Path.Combine("ASUNTOS PENALES", request.idImputado.ToString()),
                            out dataFile,
                            out string message,
                            requiredExtention
                        )
                    )
                    {
                        return Ok(ResultOperation.FailureErrorResponse(message));
                    }
                }

                AsuntosPenalesAmparo entityAmparo = null!;
                try
                {
                    entityAmparo = AsuntosPenalesImputadosEvents.CreateAmparo(
                        ref entityImputadosExists,
                        request.idTipoAmparo,
                        request.idEstadoProcesal,
                        string.IsNullOrEmpty(request.fechaPresentacion)
                            ? null!
                            : DateTime.Parse(request.fechaPresentacion),
                        request.numeroJuicioAmparo,
                        request.TipoOrganoJurisdiccional,
                        request.juzgado,
                        request.idTipoResolucion,
                        string.IsNullOrEmpty(request.fechaResolucion)
                            ? null!
                            : DateTime.Parse(request.fechaResolucion),
                        request.descripcionResolucion,
                        string.IsNullOrEmpty(request.fechaNotificacion)
                            ? null!
                            : DateTime.Parse(request.fechaNotificacion),
                        request.alegatos,
                        request.numeroOficio,
                        string.IsNullOrEmpty(request.fechaOficio)
                            ? null!
                            : DateTime.Parse(request.fechaOficio),
                        sessionInformation.UserInformation.Rfc!

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
                var result = await _asuntosPenalesAbogadoService.AmparoImputadosAsync(
                    entityExists!,
                    entityImputadosExists,
                    entityAmparo,
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
        /// Método para turnar asuntos penales
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("imputados/etapa-amparo")]
        public async Task<IActionResult> PatchEtapaAmparo(
            [FromBody] RequestUpdateAmparo request
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
                    await _updateImputadosAmparoAsuntosPenalesValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }

                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.idAsuntoPenal);

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

                var entityAmparoExists =
                    await _asuntosPenalesAbogadoService.GetAmparoById(request.id);

                if (entityAmparoExists is null || !entityAmparoExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Amparo no existe o se encuentra eliminado."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesImputadosEvents.UpdateAmparo(
                        ref entityAmparoExists,
                        request.idTipoAmparo,
                        request.idEstadoProcesal,
                        string.IsNullOrEmpty(request.fechaPresentacion)
                            ? null!
                            : DateTime.Parse(request.fechaPresentacion),
                        request.numeroJuicioAmparo,
                        request.TipoOrganoJurisdiccional,
                        request.juzgado,
                        request.idTipoResolucion,
                        string.IsNullOrEmpty(request.fechaResolucion)
                            ? null!
                            : DateTime.Parse(request.fechaResolucion),
                        request.descripcionResolucion,
                        string.IsNullOrEmpty(request.fechaNotificacion)
                            ? null!
                            : DateTime.Parse(request.fechaNotificacion),
                        request.alegatos,
                        request.numeroOficio,
                        string.IsNullOrEmpty(request.fechaOficio)
                            ? null!
                            : DateTime.Parse(request.fechaOficio),
                        sessionInformation.UserInformation.Rfc!
                    );

                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.AmparoImputadosUpdateAsync(
                    entityImputadosExists,
                    entityAmparoExists
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
        /// Método para eliminar medidas apelacion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpDelete("imputados/amparo")]
        public async Task<IActionResult> DeleteAmparo(RequestDeleteAmparo request)
        {
            try
            {
                var validationResult = await _deleteAmparoValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(validationResult.ToString(" - "))
                    );
                }
                var entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(
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
                    await _asuntosPenalesAbogadoService.GetAllImputados(request.id_asunto_penal);
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

                var entityAmparoExists =
                    await _asuntosPenalesAbogadoService.GetAmparoById(request.id);

                if (entityAmparoExists is null || !entityAmparoExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El amparo no existe o se encuentra eliminado."
                        )
                    );
                }

                if (entityImputadosExists.id != entityAmparoExists.id_imputado)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "La medida cautelar no pertenece al imputado."
                        )
                    );
                }

                try
                {
                    AsuntosPenalesImputadosEvents.DeleteAmparo(
                        ref entityAmparoExists
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }
                var result = await _asuntosPenalesAbogadoService.DeleteAmparo(
                    entityAmparoExists
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
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                EnumFileType[] requiredExtentions = { EnumFileType.JPG, EnumFileType.JPEG, EnumFileType.PDF };


                AsuntosPenales entityExists = await _asuntosPenalesAbogadoService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null || !entityExists.activo)
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
                        Path.Combine("ASUNTOS PENALES",
                        request.idAsuntoPenal.ToString()),
                        out var dataFile,
                        out string message,
                        requiredExtentions,
                        entityDocumento!.path_file))
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
                            ref entityDocumento!,
                            request.idTipoArchivo,
                            dataFile.File.FileName,
                            dataFile.FilePath,
                            dataFile.File!.ContentType,
                            _fileSystemService.ConvertBytesToMegaBytesString(dataFile.File.Length),
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
                        );
                    }
                    else
                    {
                        ArchivosAsuntosPenalesEvents.Update(
                            ref entityDocumento!,
                            request.idTipoArchivo,
                            sessionInformation.UserInformation.Rfc!,
                            sessionInformation.UserInformation.Rfc!,
                            EnumRolesSicoj.ABOGADO.GetHashCode()
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

        #region Exporta PDF        

        /// <summary>
        /// Generar PDF : Administrador
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("Acuse-Conlusion")]

        public async Task<IActionResult> Generar_Acuse_Conlusion(string noAsunto)
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

                var datos = await _asuntosPenalesAbogadoService.Exporta_PDF(noAsunto);
                if (datos is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<int>(
                            "No existe el acuse de conclusión."
                        )
                    );
                }

                var pdfBytes = AsuntosPenalesImputadosEvents.GenerarPDF(datos);

                return File(pdfBytes, "application/pdf", "AcuseConclusion.pdf");
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

                    entity = AsuntosPenalesAbogadoEvents.CreateModalidadSolicitudTransparencia(
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


                var result = await _asuntosPenalesAbogadoService.AddSolicitudTransparenciaService(entity, entityDocumento, dataFile!);
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
                var entityExists = await _asuntosPenalesAbogadoService.GetByIdSolicitudTransparenciaService(request.id);
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
                    AsuntosPenalesAbogadoEvents.UpdateSolicitudTransparencia(ref entityExists,
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
                var result = await _asuntosPenalesAbogadoService.UpdateSolicitudTransparenciaService(entityExists);
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

                var result = await _asuntosPenalesAbogadoService.GetByIdSolicitudTransparenciaServices(id);
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


                var result = await _asuntosPenalesAbogadoService.GetTablaSolicitudTransparenciaService(idAsunto);

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

                var entityExists = await _asuntosPenalesAbogadoService.GetByIdSolicitudTransparenciaService(id);
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
                    AsuntosPenalesAbogadoEvents.DeleteSolicitudTransparencia(ref entityExists,
                    id
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse(_ex.Message));
                }

                var result = await _asuntosPenalesAbogadoService.DeleteSolicitudTransparenciaService(entityExists);
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
