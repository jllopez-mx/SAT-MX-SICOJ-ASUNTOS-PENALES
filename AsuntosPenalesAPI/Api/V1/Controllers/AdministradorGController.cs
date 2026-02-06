using System.Security.Principal;
using AsuntosPenalesAPI.Model.DAO.Repository;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.DTO.ContractsValidations;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events.AdministradorGeneral;
using AsuntosPenalesAPI.Model.Entities.Events.Genericos;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.Redis;


namespace AsuntosPenalesAPI.Api.V1.Controllers
{
    [ApiController]
    [Route("sicoj/asuntos-penales/api/v1/ag/asuntos-penales")]
    public class AdministradorGController : ControllerBase
    {
        #region Variables / Contructor
        private readonly ILogger<AdministradorGController> _logger;
        private readonly IAsuntosPenalesAdministradorGService _asuntosPenalesAdministradorGService;
        private readonly IAsuntosPenalesOficialPartesService _asuntosPenalesOficialPartesService;
        private readonly RequestUpdateAsuntosPenalesValidator _updateAsuntosPenalesValidator;
        private readonly IAsuntosPenalesAbogadoService _asuntosPenalesAbogadoService;
        private readonly RequestUpdateAnalisisValidator _updateAnalisisValidator;
        private readonly IGenericService _genericService;
        private readonly IRedisClient _redisClient;
        private static object _lock = new object();

        public AdministradorGController(
            ILogger<AdministradorGController> logger,
            IAsuntosPenalesAdministradorGService asuntosPenalesAdministradorGService,
            IAsuntosPenalesOficialPartesService asuntosPenalesOficialPartesService,
            IGenericService genericService,
            IAsuntosPenalesAbogadoService asuntosPenalesAbogadoService,
            RequestUpdateAsuntosPenalesValidator updateAsuntosPenalesValidator,
            RequestUpdateAnalisisValidator updateAnalisisValidator,
            IRedisClient redisClient)

        {
            _asuntosPenalesAdministradorGService = asuntosPenalesAdministradorGService;
            _logger = logger;
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _genericService = genericService ?? throw new ArgumentNullException(nameof(genericService));
            _updateAsuntosPenalesValidator = updateAsuntosPenalesValidator ?? throw new ArgumentNullException(nameof(updateAsuntosPenalesValidator));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _asuntosPenalesOficialPartesService = asuntosPenalesOficialPartesService ?? throw new ArgumentNullException(nameof(asuntosPenalesOficialPartesService));
            _asuntosPenalesAbogadoService =
                asuntosPenalesAbogadoService
                ?? throw new ArgumentNullException(nameof(asuntosPenalesAbogadoService));
            _updateAnalisisValidator =
                updateAnalisisValidator
                ?? throw new ArgumentNullException(nameof(updateAnalisisValidator));
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(id);
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
                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(id);
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
        [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesByIdAdminG>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsuntoPenalById(int id)
        {
            try
            {
                var result = await _asuntosPenalesAdministradorGService.GetAdminAsuntoPenalById(id);
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
                    1,//sessionInformation.TokenInfomation.RolesCollection.Any(c => c == EnumRoles.JAI_RAUA.EnumRolesString()),
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


        #region Reasignar
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
        #endregion


        /// <summary>
        /// Método para actualizar asuntos penales 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("asuntos-penales-DatosGenerales")]
        public async Task<IActionResult> PatchAsuntoPenalDatosGenerales(
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
                AsuntosPenalesModificacion modificacion = new();
                if (entityExists!.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())

                {
                    modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.id);

                    entityExists.id_estado_procesal = modificacion.id_estado_procesal;
                }


                try
                {
                    AsuntosPenalesAdministradorGeneralEvents.UpdateDatosGeneralesAG(ref entityExists!,
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

        #region Reactivar
        [ProducesResponseType(typeof(ResultOperation<ResponseReactivar>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("reactivar")]
        public async Task<IActionResult> PatchReactivar(RequestUpdateReactivar request)
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
                        ResultOperation.FailureErrorResponse<ResponseReactivar>("El registro no se puede usar ya que el usuario no ha apartado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseReactivar>(
                            "El Asunto Penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                var entityExists = await _asuntosPenalesOficialPartesService.GetAsuntosPenalesById(request.id);
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseReactivar>(
                            "el asunto pena no existe o fue eliminada."
                        )
                    );
                }
                var listaImputados =
                await _genericService.GetAllImputados(request.id);
                AsuntosPenalesAbogado entityAbogado = await _genericService.GetAbogadoByIdAsuntosPenalesAsync(request.id);

                List<AsuntosPenalesHistoricoImputados> historico_imputado = null!;

                AsuntosPenalesImputados imputado = null!;

                if (listaImputados is not null && listaImputados.Any())

                {
                    imputado = listaImputados.LastOrDefault()!;
                    historico_imputado = await _asuntosPenalesAdministradorGService.GetHistoricoImputados(imputado.id);
                }

                if (entityAbogado is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseReactivar>(
                            "El asunto penal no tiene asignado un abogado."
                        )
                    );
                }
                try
                {
                    AsuntosPenalesAdministradorGeneralEvents.Reactivar(ref entityExists, imputado, historico_imputado, sessionInformation.UserInformation.Rfc);
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseReactivar>(_ex.Message));
                }
                var result = await _asuntosPenalesAdministradorGService.ReactivarAsync(entityExists, entityAbogado);
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


        #region Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("modificar")]
        public async Task<IActionResult> PatchModificarAsunto([FromBody] RequestModificarDatosGenerales request)
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
                        ResultOperation.FailureInformationResponse(
                            "El Asunto Penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                            "El Asunto Penal fue eliminado."
                        )
                    );
                }

                // var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                // if (modificacion is not null &&  modificacion.id_seccion == 7 )
                // {
                //     return Ok(
                //         ResultOperation.FailureErrorResponse<int>(
                //             "Ya existe una modificacion en progreso."
                //         )
                //     );
                // }

                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.Create(
                        ref entityExists,
                        EnumSecciones.DATOS_GENERALES.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
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
        [HttpPatch("descartar")]
        public async Task<IActionResult> PatchDescartarAsunto([FromBody] RequestDescartarDatos request)
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
                        ResultOperation.FailureInformationResponse(
                            "El Asunto Penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null || !entityExists.activo)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe o fue eliminado."
                        )
                    );
                }
                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartar(
                        entityExists,
                        EnumSecciones.DATOS_GENERALES.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                        );

                    AsuntosPenalesAdministradorGeneralEvents.Descartar(
                            ref entityExists,
                            sessionInformation.UserInformation.Rfc!
                        );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.DATOS_GENERALES.GetHashCode(),
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode(),
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                        EnumSecciones.ETAPA_INTERMEDIA_INVESTIGACIÓN.GetHashCode(),
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                    };

                ResultOperation result = await _genericService.DescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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


        #region Requerimiento Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Requerimiento-Modificar")]
        public async Task<IActionResult> PatchRequerimientoModificarAsunto([FromBody] RequestRequerimientoModificarAsunto request)
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }
                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"; ;
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
                        ResultOperation.FailureInformationResponse(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                //  var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                //     if (modificacion is not null && modificacion.id_seccion == 8 )
                //     {
                //         return Ok(
                //             ResultOperation.FailureErrorResponse<int>(
                //                 "Ya existe una modificacion en progreso."
                //             )
                //         );
                //     }

                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.Create(
                        ref entityExists,
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
                return Ok(result);

            }
            catch (Exception _e)
            {
                _logger.LogError(_e, "Ha ocurrrido un error.");
                return BadRequest(
                    ResultOperation.FailureErrorResponse<int>(
                        _e.ManageException()
                    )
                );
            }
        }


        [ProducesResponseType(typeof(ResultOperation<ResponseRequerimientoDescartarAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Requerimiento-Descartar")]
        public async Task<IActionResult> PatchRequerimientoDescartarAsunto([FromBody] RequestDescartarRequerimientosAsunto request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartarAsunto>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                string key = null!;
                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartarAsunto>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseRequerimientoDescartarAsunto>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe ."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }
                var requerimientoList = await _asuntosPenalesAbogadoService.GetAsuntosPenalesAnalisisByIdAsuntoPenal(request.idAsuntoPenal)!;
                if (requerimientoList is null || !requerimientoList.Any(c => c.activo))
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartarAsunto>(
                            "El asunto no contiene registros de requerimiento."
                        )
                    );
                }

                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartar(
                        entityExists,
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesAdministradorGeneralEvents.DescartarSeccion(
                        ref entityExists,
                        EnumSecciones.REQUERIMIENTO.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartarAsunto>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.REQUERIMIENTO.GetHashCode()
                    };

                var result = await _genericService.RequerimientoDescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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


        #region Requisitos Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Requisitos-Modificar")]
        public async Task<IActionResult> PatchRequisitosModificarAsunto([FromBody] RequestRequisitosModificarAsunto request)
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }
                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"; ;
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
                        ResultOperation.FailureInformationResponse(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                // var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                //     if (modificacion is not null && modificacion.id_seccion ==10 )
                //     {
                //         return Ok(
                //             ResultOperation.FailureErrorResponse<int>(
                //                 "Ya existe una modificacion en progreso."
                //             )
                //         );
                //     }
                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.Create(
                        ref entityExists,
                        EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
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


        [ProducesResponseType(typeof(ResultOperation<ResponseRequisitosDescartarAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Requisitos-Descartar")]
        public async Task<IActionResult> PatchRequisitosDescartarAsunto([FromBody] RequestRequisitosDescartarAsunto request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequisitosDescartarAsunto>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                string key = null!;
                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseRequisitosDescartarAsunto>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseRequisitosDescartarAsunto>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe ."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityProcedibilidadExists =
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidad(
                        request.idAsuntoPenal
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

                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartar(
                        entityExists,
                        EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesAdministradorGeneralEvents.DescartarSeccion(
                        ref entityExists,
                        EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseRequisitosDescartarAsunto>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode()
                    };

                var result = await _genericService.RequisitosDescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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


        #region Etapa Investigacion Inicial  Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Inicial-Modificar")]
        public async Task<IActionResult> PatchEtapaInicialModificarAsunto([FromBody] RequestModificarAsunto request)
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }
                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"; ;
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
                        ResultOperation.FailureInformationResponse(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                // var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                // if (modificacion.activo == true )
                // {
                //     return Ok(
                //         ResultOperation.FailureErrorResponse<int>(
                //             "Ya existe una modificacion en progreso."
                //         )
                //     );
                // }

                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.CreateImputados(
                        ref entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
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


        [ProducesResponseType(typeof(ResultOperation<ResponseEtapaInicialDescartarAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Inicial-Descartar")]
        public async Task<IActionResult> PatchEtapaInicialDescartarAsunto([FromBody] RequestDescartarEtapas request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaInicialDescartarAsunto>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                string key = null!;
                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaInicialDescartarAsunto>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseEtapaInicialDescartarAsunto>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe ."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }

                var entityProcedibilidadExists =
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidad(
                        request.idAsuntoPenal
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
                            "El caso de procedibilidad ya se encuentra eliminado, no puede ser editado."
                        )
                    );
                }
                if (entityProcedibilidadExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("El asunto penal no cuenta con imputados.")
                    );
                }
                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartarEtapa(
                        entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesAdministradorGeneralEvents.DescartarSeccion(
                        ref entityExists,
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseEtapaInicialDescartarAsunto>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode()
                    };
                var result = await _genericService.EtapaInicialDescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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


        #region Etapa Investigacion Complementaria  Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Complementaria-Modificar")]
        public async Task<IActionResult> PatchEtapaComplementariaModificarAsunto([FromBody] RequestModificarAsunto request)
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }

                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"; ;
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
                        ResultOperation.FailureInformationResponse(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                //   var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                //     if (modificacion.activo == true )
                //     {
                //         return Ok(
                //             ResultOperation.FailureErrorResponse<int>(
                //                 "Ya existe una modificacion en progreso."
                //             )
                //         );
                //     }
                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.CreateImputados(
                        ref entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
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



        [ProducesResponseType(typeof(ResultOperation<ResponseEtapaInicialDescartarAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Complementaria-Descartar")]
        public async Task<IActionResult> PatchEtapaComplementariaDescartarAsunto([FromBody] RequestDescartarEtapas request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaInicialDescartarAsunto>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                string key = null!;
                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaInicialDescartarAsunto>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseEtapaInicialDescartarAsunto>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe ."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }

                var entityProcedibilidadExists =
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidad(
                        request.idAsuntoPenal
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
                            "El caso de procedibilidad ya se encuentra eliminado, no puede ser editado."
                        )
                    );
                }

                if (entityProcedibilidadExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("El asunto penal no cuenta con imputados.")
                    );
                }

                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartarEtapa(
                        entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesAdministradorGeneralEvents.DescartarSeccion(
                        ref entityExists,
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseEtapaComplementariaDescartarAsunto>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode()
                    };

                var result = await _genericService.EtapaComplementariaDescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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



        #region Etapa Investigacion Intermedia  Modificar / Descartar
        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Intermerdia-Modificar")]
        public async Task<IActionResult> PatchEtapaIntermediaModificarAsunto([FromBody] RequestModificarAsunto request)
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

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }

                string key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}"; ;
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
                        ResultOperation.FailureInformationResponse(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                //   var modificacion = await _genericService.GetAsuntosPenalesModificacionByIdAsuntoPenal(request.idAsuntoPenal);
                //     if (modificacion.activo == true )
                //     {
                //         return Ok(
                //             ResultOperation.FailureErrorResponse<int>(
                //                 "Ya existe una modificacion en progreso."
                //             )
                //         );
                //     }
                AsuntosPenalesModificacion entity = null!;
                try
                {
                    entity = AsuntosPenalesModificacionEvents.CreateImputados(
                        ref entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_INTERMEDIA_INVESTIGACIÓN.GetHashCode(),
                        null!,
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists, entity);
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


        [ProducesResponseType(typeof(ResultOperation<ResponseEtapaIntermediaDescartarAsunto>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Etapa Investigacion Intermedia-Descartar")]
        public async Task<IActionResult> PatchEtapaIntermediaDescartarAsunto([FromBody] RequestDescartarEtapas request)
        {
            try
            {
                var sessionInformation = UserSession.GetValue(HttpContext);
                if (sessionInformation is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaIntermediaDescartarAsunto>(
                            "No se pudo obtener la información del usuario."
                        )
                    );
                }
                string key = null!;
                key = $"{EnumModulosRedis.ASUNTOS_PENALES.ToStringValue()}{request.idAsuntoPenal}";
                var keyExists = await _redisClient.GetValueAsync<UserBlockingRegistration>(key);
                if (keyExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse<ResponseEtapaIntermediaDescartarAsunto>("El registro no se puede usar ya que el usuario no ha tomado el asunto penal.")
                    );
                }

                if (!keyExists.rfc.Equals(sessionInformation.UserInformation.Rfc))
                {
                    return Ok(
                        ResultOperation.FailureInformationResponse<ResponseEtapaInicialDescartarAsunto>(
                            "El asunto penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }
                AsuntosPenales entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
                if (entityExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse(
                            "El Asunto Penal no existe ."
                        )
                    );
                }
                if (entityExists.activo == false)
                {
                    return Ok(
                    ResultOperation.FailureErrorResponse(
                        "La asunto penal ya se encuentra eliminado no se puede modificar."
                    )
                );
                }

                var entityProcedibilidadExists =
                    await _asuntosPenalesAbogadoService.GetAsuntosPenalesByIdProcedibilidad(
                        request.idAsuntoPenal
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
                            "El caso de procedibilidad ya se encuentra eliminado, no puede ser editado."
                        )
                    );
                }

                if (entityProcedibilidadExists is null)
                {
                    return Ok(
                        ResultOperation.FailureErrorResponse("El asunto penal no cuenta con imputados.")
                    );
                }

                var entityImputado = await _asuntosPenalesAbogadoService.GetImputadoByIdDisconnected(request.idImputado);
                if (!entityImputado.Success)
                {
                    return Ok(
                        ResultOperation.FailureWarningResponse(
                            entityImputado.Messages[0].detailMessage
                        )
                    );
                }

                AsuntosPenalesDescartar entity = null!;
                try
                {
                    entity = AsuntosPenalesAdministradorGeneralEvents.CreateDescartarEtapa(
                        entityExists,
                        request.idImputado,
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );

                    AsuntosPenalesAdministradorGeneralEvents.DescartarSeccion(
                        ref entityExists,
                            EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<ResponseEtapaIntermediaDescartarAsunto>(_ex.Message));
                }

                List<int> listaSecciones = new()
                    {
                        EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode()
                    };

                var result = await _genericService.EtapaIntermediaDescartarAsuntoPenalAsync(entityExists, entity, listaSecciones);
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


        [ProducesResponseType(typeof(ResultOperation<int>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpPatch("Fecha-Vencimiento")]
        public async Task<IActionResult> PatchFechaVencimiento([FromBody] RequestUpdateFechaVencimiento request)
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
                        ResultOperation.FailureInformationResponse(
                            "El Asunto Penal ya se encuentra en uso por otro usuario."
                        )
                    );
                }

                var entityExists = await _asuntosPenalesAdministradorGService.GetAsuntoPenalById(request.idAsuntoPenal);
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
                            "El Asunto Penal fue eliminado."
                        )
                    );
                }
                try
                {
                    AsuntosPenalesAdministradorGeneralEvents.UpdateFechaVencimiento(
                        ref entityExists!,
                        string.IsNullOrEmpty(request.fechaVencimiento)
                        ? null!
                        : DateTime.Parse(request.fechaVencimiento),
                        sessionInformation.UserInformation.Rfc!
                    );
                }
                catch (Exception _ex)
                {
                    return Ok(ResultOperation.FailureWarningResponse<int>(_ex.Message));
                }

                var result =
                await _asuntosPenalesAdministradorGService.UpdateFechaVencimientoAsync(
                    entityExists!);
                return Ok(result);
                // ResultOperation result = await _genericService.AddAsuntosPenalesModificacionAsync(entityExists);
                // return Ok(result);

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


        // [ProducesResponseType(typeof(ResultOperation<ResponseAsuntosPenalesAdministradorUAByFilters>), 200)]
        // [ProducesResponseType(typeof(ResultOperation), 400)]
        // [Authorize(EnumRoles.RR_AUA)]
        // [HttpGet("fecha-vencimiento")]
        // public async Task<IActionResult> fechaVencimiento([FromQuery] PagerQueryFilters request)
        // {
        //     try
        //     {
        //         var sessionInformation = UserSession.GetValue(HttpContext);
        //         if (sessionInformation is null)
        //         {
        //             return Ok(
        //                 ResultOperation.FailureErrorResponse(
        //                     "No se pudo obtener la información del usuario."
        //                 )
        //             );
        //         }

        //         RequestFiltrosAsuntosPenales filters = new();
        //         Filters.MapFilters(request, filters);

        //         if (!Filters.MapSort<EnumOrderColumnAsuntosPenales>(request, null!, false, out string orderByColumn, out bool orderDesc))
        //         {
        //             return Ok(ResultOperation.FailureWarningResponse<List<RequestFiltrosAsuntosPenales>>("La columna de ordenamiento no es válida.")
        //                 );
        //         }
        //         var result = await _asuntosPenalesAdministradorGService.GetRegistroFechasAsync(
        //             request.fetch,
        //             request.page,
        //             orderByColumn,
        //             orderDesc,
        //             Filters.GetStringValue(filters!.ByNoAsuntoPenal.FirstOrDefault()),
        //             Filters.GetIntValue(filters.ByAdminControl.FirstOrDefault()),
        //             sessionInformation.UserInformation
        //             );


        //         return Ok(result);
        //     }
        //     catch (Exception _e)
        //     {
        //         _logger.LogError(_e, "Ha ocurrrido un error.");
        //         return BadRequest(
        //             ResultOperation.FailureErrorResponse(
        //                 _e.ManageException()
        //             )
        //         );
        //     }
        // }
    
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


                    var datos = await _asuntosPenalesAdministradorGService.ExportarReporteGeneral(
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


                    using var workbook = AsuntosPenalesAdministradorGeneralEvents.GenerarExcelClosedXmlGeneral(datos);
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
    }
}