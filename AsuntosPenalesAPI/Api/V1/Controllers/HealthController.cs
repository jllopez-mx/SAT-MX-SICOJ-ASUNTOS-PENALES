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
using System.Reflection;




namespace AsuntosPenalesAPI.Api.V1.Controllers
{
[Route("sicoj/asuntos-penales/api/[controller]")][ApiController]
public class HealthController : ControllerBase{
    [ApiExplorerSettings(IgnoreApi = true)]    [HttpGet]
    public IActionResult Status()    {
        Assembly assembly = Assembly.GetExecutingAssembly();        AssemblyName name = assembly.GetName();
        Version version = name.Version!;        return Ok($"Assembly: {name.Name} - Versión: {version}");
    }}
}
