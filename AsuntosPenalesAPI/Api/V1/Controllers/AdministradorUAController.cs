using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.DTO.ContractsValidations;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events;
using AsuntosPenalesAPI.Model.Entities.Events.AdministradorUA;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
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
    [Route("sicoj/asuntos-penales/api/v1/aua/asuntos-penales")]
    public class AUAController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<AUAController> _logger;
        private readonly IAsuntosPenalesAdministradorUAService _asuntosPenalesAdministradorUAService;
        private readonly IGenericService _genericService;
        private static object _lock = new object();
        private readonly IRedisClient _redisClient;
        private readonly IFileSystemService _fileSystemService;
        private readonly RequestRemisionAsuntosPenalesValidator _createRemisionAsuntosPenalesValidator;

        public AUAController(ILogger<AUAController> logger,
            IAsuntosPenalesAdministradorUAService asuntosPenalesAdministradorUAService,
            IGenericService genericService,
            IRedisClient redisClient,
            IFileSystemService fileSystemService,
            RequestRemisionAsuntosPenalesValidator createRemisionAsuntosPenalesValidator
)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _asuntosPenalesAdministradorUAService = asuntosPenalesAdministradorUAService ?? throw new ArgumentNullException(nameof(asuntosPenalesAdministradorUAService));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _createRemisionAsuntosPenalesValidator = createRemisionAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(createRemisionAsuntosPenalesValidator));

        }
        #endregion

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
                var entityExists = await _asuntosPenalesAdministradorUAService.GetAsuntoPenalById(id);
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
                var entityExists = await _asuntosPenalesAdministradorUAService.GetAsuntoPenalById(id);
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
                var result = await _asuntosPenalesAdministradorUAService.GetAdministradorUAsuntoPenalById(
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


        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAdministradorUAByFilters>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_AUA)]
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

                var result = await _asuntosPenalesAdministradorUAService.GetHistoricoAsync(
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
                    1,
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
        /// Método para crear reasignaciones de Asuntos Penales
        /// </summary>
        /// <param name="request">Datos de Asuntos Penales</param>
        /// <returns>Result Operation con Id de registro creado</returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("reasignar")]
        public async Task<IActionResult> PatchReasignar(
            RequestReasignarAdministrador request
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

                var result = await _genericService.ReasignarAsync(request.idList.ToArray(), sessionInformation.UserInformation, request.rfcAbogado!, request.rfcAdministrador!);
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


                var datos = await _asuntosPenalesAdministradorUAService.ExportarReporteGeneral(
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


                using var workbook = AsuntosPenalesAdministradorUAEvents.GenerarExcelClosedXmlGeneral(datos);
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

                var entityExists = await _asuntosPenalesAdministradorUAService.GetAsuntoPenalById(request.idAsuntoPenal);
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

                    AsuntosPenalesAdministradorUAEvents.UpdateRemitir(ref entityExists,
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
                var result = await _asuntosPenalesAdministradorUAService.RemitirAsync(entityExists, entityRemision, entityDocumento, dataFile!);
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