using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities.Events.ReporteAdministradorGeneral;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
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
    [Route("sicoj/asuntos-penales/api/v1/reporte-administrador-general/asuntos-penales")]
    public class ReporteAdministradorGeneralController : ControllerBase
    {
         private readonly ILogger<OficialPartesController> _logger;
        private readonly IRedisClient _redisClient;
        private readonly IReporteAdministradorGeneralService _ReporteAdministradorGeneralService;

        public ReporteAdministradorGeneralController(ILogger<OficialPartesController> logger, IRedisClient redisClient, IReporteAdministradorGeneralService reporteAdministradorGeneralService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            _ReporteAdministradorGeneralService = reporteAdministradorGeneralService ?? throw new ArgumentNullException(nameof(reporteAdministradorGeneralService));
        }

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


                    var datos = await _ReporteAdministradorGeneralService.ExportarReporteGeneral(
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


                    using var workbook = ReporteAdministradorGeneralEvents.GenerarExcelClosedXmlGeneral(datos);
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
    }
}