using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Enums;
using Sicoj.Utils.Extentions;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Api.V1.Controllers
{
    [ApiController]
    [Route("sicoj/asuntos-penales/api/v1/generic/asuntos-penales")]
    public class GenericController : ControllerBase
    {
        private readonly IGenericService _genericService;

        public GenericController(IGenericService genericService)
        {
            _genericService =
                genericService
                ?? throw new ArgumentNullException(
                nameof(genericService)
                );
        }

        [ProducesResponseType(typeof(ResultOperation<ResponseCalcularFecha>), 200)]
        [ProducesResponseType(typeof(ResultOperation), 400)]
        [Authorize(EnumRoles.RR_OP)]
        [HttpGet("calcular-fecha")]
        public async Task<IActionResult> CalcularFecha([FromQuery] string baseDate, int addDays, bool nextDay)
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
                var result = await _genericService.CalcularFecha(baseDate, addDays, nextDay);

                return Ok(result);
            }
            catch (Exception _e)
            {
                return BadRequest(
                    ResultOperation.FailureErrorResponse(
                        $"Ah ocurrido un error inesperado => {_e.Message}"
                    )
                );
            }
        }
        


        
    }
}