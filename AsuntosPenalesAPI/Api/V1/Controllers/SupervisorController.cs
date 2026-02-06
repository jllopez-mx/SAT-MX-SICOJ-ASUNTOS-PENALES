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
    [Route("sicoj/asuntos-penales/api/v1/supervisor/asuntos-penales")]
    public class SupervisorController : ControllerBase
    {
        private readonly ILogger<SupervisorController> _logger;
        private readonly IRedisClient _redisClient;
        private readonly IAsuntosPenalesAbogadoService _asuntosPenalesAbogadoService;
         private readonly IAsuntosPenalesAdministradorGService _asuntosPenalesAdministradorGService;
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
        private static object _lock = new object();
        private readonly IFileSystemService _fileSystemService;
        private readonly IGenericService _genericService;
        private readonly IValidator<RequestDocumentoUpdate> _requestDocumentoUpdateValidator;

        public SupervisorController(
            ILogger<SupervisorController> logger,
            IRedisClient redisClient,
            IAsuntosPenalesAdministradorGService asuntosPenalesAdministradorGService,
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
            IValidator<RequestDocumentoUpdate> requestDocumentoUpdateValidator,
            IGenericService genericService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _asuntosPenalesAdministradorGService = asuntosPenalesAdministradorGService;
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

                var result = await _asuntosPenalesAdministradorGService.GetHistoricoAsync(
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
                    Filters.GetBoolValue(filters.ByTipoUnidad.FirstOrDefault()),
                    sessionInformation.UserInformation
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
                    1,
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

        // /// <summary>
        // ///Método para obtener el documento de la consulta
        // /// </summary>
        // /// <param name="id">Id del archivo registrado</param>
        // /// <returns></returns>
        // [ProducesResponseType(typeof(ResultOperation<ResponseArchivosAsuntosPenales>), 200)]
        // [ProducesResponseType(typeof(ResultOperation), 400)]
        // [Authorize(EnumRoles.RR_OP)]
        // [HttpGet("visualizar-archivos/{id}")]
        // public async Task<IActionResult> Descargar(int id)
        // {
        //     try
        //     {
        //         var entityExists = await _asuntosPenalesAbogadoService.GetByIdArchivoService(id);
        //         if (entityExists is null)
        //         {
        //             return BadRequest("No existe el registro.");
        //         }

        //         var response = await _fileSystemService.GetFileAsync(entityExists.path_file);
        //         if (response is null)
        //         {
        //             return BadRequest("No existe el documento.");
        //         }

        //         var contentDisposition = new System.Net.Mime.ContentDisposition
        //         {
        //             Inline = true,
        //             FileName = Path.GetFileName(entityExists.path_file),
        //         };

        //         Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
        //         return File(response, entityExists.content_type);
        //     }
        //      catch (Exception _e)
        //     {
        //         _logger.LogError(_e, "Ha ocurrrido un error.");
        //         return BadRequest(
        //             ResultOperation.FailureErrorResponse(
        //                 _e.ManageException()
        //             )
        //         );
        //     }
        // }

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


        ///Método para consultar Asuntos Penales por Id
        /// </summary>
        /// <param name="id">Id Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-inicial/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaInicial( int id)
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


        ///Método para consultar Asuntos Penales por Id
        /// </summary>
        /// <param name="id">Id Asuntos Penales</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<ResponseImputados>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("imputados/etapa-complementaria/{id}")]
        public async Task<IActionResult> GetAsuntosPenalesByIdImputadosEtapaComplementaria( int id)
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
        /// Método para eliminar medidas apelacion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        
    }
}
