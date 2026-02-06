using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.Entities.Events.Administrador;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Enums;
using Microsoft.Extensions.Options;
using Sicoj.Utils.Middleware;
using Sicoj.Utils.Models;
using Sicoj.Utils.ViewModels;
using Sicoj.Utils.Redis; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils.Extentions;
using System.Text.Json;
using Sicoj.Utils.Models.Responses;

namespace AsuntosPenalesAPI.Model.DAO.ServicesDAO
{
    public class GenericService : IGenericService
    {
        private readonly IApiService _apiService;
        private readonly IAsuntosPenalesRepository _repository;
        private readonly IArchivosAsuntosPenalesRepository _archivosRepository;
        private readonly ProxyEnpoints _proxyEnpoints;
        private readonly IAsuntosPenalesModificacionRepository _asuntosPenalesModificacionRepository;
        private readonly IAsuntosPenalesDescartarRepository _asuntosPenalesDescartarRepository;
        private readonly string _routeInfoUsuario = null!;
        private readonly string _routeFechaVencimiento = null!;
        public GenericService(IConfiguration configuration,
        IApiService apiService,
        IOptions<ProxyEnpoints> proxyEnpoints,
        IAsuntosPenalesRepository repository,
        IArchivosAsuntosPenalesRepository archivosRepository,
        IAsuntosPenalesModificacionRepository asuntosPenalesModificacionRepository,
        IAsuntosPenalesDescartarRepository asuntosPenalesDescartarRepository
)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _routeInfoUsuario = configuration.GetValue<string>(
                "CatalogsEndpoints:RouteInfoUsuario"
            )!;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _archivosRepository = archivosRepository ?? throw new ArgumentNullException(nameof(archivosRepository));
            _asuntosPenalesModificacionRepository = asuntosPenalesModificacionRepository ?? throw new ArgumentNullException(nameof(asuntosPenalesModificacionRepository));
            _asuntosPenalesDescartarRepository = asuntosPenalesDescartarRepository ?? throw new ArgumentNullException(nameof(asuntosPenalesDescartarRepository));
            _proxyEnpoints = proxyEnpoints.Value ?? throw new ArgumentNullException(nameof(proxyEnpoints));
            _routeFechaVencimiento = configuration.GetValue<string>("CatalogsEndPoints:RouteFechaVencimiento")!;
        }

        #region Reasigar
        public async Task<ResultOperation<ResponseReasignar>> ReasignarAsync(
            int[] idList,
            UserInformationView userInformationView,
            string rfcAbogado,
            string rfcAdministrador
        )
        {
            List<AsuntosPenales> entityList = await _repository.GetListByIdsAsync(idList);
            if (entityList is null || !entityList.Any())
            {
                return ResultOperation.FailureErrorResponse<ResponseReasignar>(
                    $"No existen los asuntos penales."
                );
            }

            var resultOperation = ResultOperation.SuccessResponseNoMessage(new ResponseReasignar());
            List<int> listInvalidos = new();
            List<AsuntosPenalesReasignar> listReasignacion = new();
            if (string.IsNullOrEmpty(rfcAbogado))
            {
                var responseAdministrador =
                    await _apiService.GetResultOperationAsync<UserInformationView>(
                        $"{_routeInfoUsuario}/{rfcAdministrador}"
                    );
                if (
                    responseAdministrador is null
                    || !responseAdministrador.Success
                    || responseAdministrador.Result is null
                )
                {
                    return ResultOperation.FailureErrorResponse<ResponseReasignar>(
                        $"No se pudo realizar la validación para verificar que el administrador seleccionado pertenezca a la administracion/subadministración."
                    );
                }

                foreach (var item in idList)
                {
                    var entity = entityList.FirstOrDefault(c => c.id == item && c.activo);
                    if (entity is null)
                    {
                        resultOperation.AddWarningMessage(
                            $"El asunto penal con el identificador {item} no existe o se eliminó."
                        );
                        listInvalidos.Add(item);
                        continue;
                    }
                    if (entity.id_admin_controla == responseAdministrador.Result.IdAdministracion)
                    {
                        resultOperation.AddWarningMessage(
                                $"El Asunto Penal con el número de asunto {entity.numero_asunto_penal} ya esta asignada a esta administración"
                            );
                        listInvalidos.Add(item);
                        continue;
                    }

                    try
                    {
                        AsuntosPenalesReasignar entityReasignar =
                            AsuntosPenalesAdministradorEvents.UpdateReasignarAdministrador(
                                ref entity,
                                userInformationView.Rfc,
                                entity.abogado.id_abogado,
                                userInformationView.IdAdministracion,
                                userInformationView.IdSubadministracion,
                                responseAdministrador.Result.IdAdministracion,
                                responseAdministrador.Result.IdSubadministracion
                            );

                        // if (
                        //     !UserSession.ValidateUser(
                        //         responseAdministrador!.Result!,
                        //         EnumRolesSicoj.ADMINISTRADOR,
                        //         EnumModulosSicoj.ASUNTOS_PENALES,
                        //         out string message,
                        //         false,
                        //         null!,
                        //         true,
                        //         entity.id_administracion_central
                        //     )
                        // )
                        // {
                        //     resultOperation.AddWarningMessage(
                        //         $"El Asunto Penal con el número de asunto {entity.numero_asunto_penal}: {message}"
                        //     );
                        // }

                        listReasignacion.Add(entityReasignar);
                    }
                    catch (Exception _e)
                    {
                        resultOperation.AddWarningMessage(
                            $"El Asunto Penal con el número de asunto {entity.numero_asunto_penal}: {_e.Message}"
                        );
                        listInvalidos.Add(item);
                        continue;
                    }
                }
            }
            else
            {
                var responseAbogado = await _apiService.GetResultOperationAsync<UserInformationView>(
                    $"{_routeInfoUsuario}/{rfcAbogado}"
                );
                if (
                    responseAbogado is null
                    || !responseAbogado.Success
                    || responseAbogado.Result is null
                )
                {
                    return ResultOperation.FailureErrorResponse<ResponseReasignar>(
                        $"No se pudo realizar la validación para verificar que el abogado seleccionado pertenezca a la administracion/subadministración."
                    );
                }

                foreach (var item in idList)
                {
                    var entity = entityList.FirstOrDefault(c => c.id == item && c.activo);
                    if (entity is null)
                    {
                        resultOperation.AddWarningMessage(
                            $"La autorización con el identificador {item} no existe o se eliminó."
                        );
                        listInvalidos.Add(item);
                        continue;
                    }

                    try
                    {
                        AsuntosPenalesReasignar entityReasignar =
                            AsuntosPenalesAdministradorEvents.UpdateReasignarAbogado(
                                ref entity,
                                userInformationView.Rfc,
                                responseAbogado.Result.Rfc,
                                entity.abogado.id_abogado,
                                userInformationView.IdAdministracion,
                                userInformationView.IdSubadministracion,
                                responseAbogado.Result.IdAdministracion,
                                responseAbogado.Result.IdSubadministracion
                            );

                        // if (
                        //     !UserSession.ValidateUser(
                        //         responseAbogado!.Result!,
                        //         EnumRolesSicoj.ABOGADO,
                        //         EnumModulosSicoj.ASUNTOS_PENALES,
                        //         out string message,
                        //         false,
                        //         null!,
                        //         true,
                        //         entity.id_administracion_central
                        //     )
                        // )
                        // {
                        //     resultOperation.AddWarningMessage(
                        //         $"El Asunto Penal con el número de asunto {entity.numero_asunto_penal}: {message}"
                        //     );
                        // }

                        listReasignacion.Add(entityReasignar);
                    }
                    catch (Exception _e)
                    {
                        resultOperation.AddWarningMessage(
                            $"LEl Asunto Pena  con el número de asunto {entity.numero_asunto_penal}: {_e.Message}"
                        );
                        listInvalidos.Add(item);
                        continue;
                    }
                }

                resultOperation.Result.abogado = responseAbogado.Result.Nombre!;
            }

            resultOperation.Result.ReasignacionesExitosas = listReasignacion.Count;
            resultOperation.Result.ReasignacionesIncorrectas = listInvalidos.Count;

            if (!listReasignacion.Any())
            {
                return resultOperation;
            }

            var result = await _repository.ReasignarAsync(listReasignacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseReasignar>(
                    $"{result.MsgError!}:{result.DetailError}"
                );

            return resultOperation;
        }
        #endregion

        public async Task<AsuntosPenalesAbogado> GetAbogadoByIdAsuntosPenalesAsync(int id) =>
        await _repository.GetAbogadoAsignadoAsync(id);

        public async Task<List<AsuntosPenalesImputados>> GetAllImputados(int idAsuntoPenal) =>
            await _repository.GetImputados(idAsuntoPenal);

        public async Task<ResultOperation<int>> AddArchivosAsyncService(ArchivosAsuntosPenales entity, DataFile dataFile)
        {
            var result = await _archivosRepository.AddFileAsyncRepository(entity, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<List<ArchivosAsuntosPenales>> GetArchivosAsuntoPenalByIds(int[] ids) =>
        await _archivosRepository.GetByIdsAsync(ids);

        public async Task<AsuntosPenalesModificacion> GetAsuntosPenalesModificacionByIdAsuntoPenal(int id) =>
        await _asuntosPenalesModificacionRepository.GetByIdAsuntoPenalAsync(id);

        public async Task<ResultOperation<int>> UpdateArchivosAsuntosPenales(ArchivosAsuntosPenales entity, DataFile dataFile)
        {
            var result = await _archivosRepository.UpdateAsync(entity, dataFile);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> AddAsuntosPenalesModificacionAsync(AsuntosPenales entityAsuntoPenal, AsuntosPenalesModificacion entity)
        {
            var result = await _asuntosPenalesModificacionRepository.AddAsync(entityAsuntoPenal, entity);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }

        public async Task<ResultOperation<int>> DescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            var result = await _asuntosPenalesDescartarRepository.DescartarAsync(entityAsuntosPenales, entity, listaSecciones!);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<int>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(result.Result.GetValueOrDefault());
        }
        public async Task<ResultOperation<bool>> DeleteArchivoAsuntoPenal(int[] ids, string usuarioModificacion)
        {
            var result = await _archivosRepository.DeleteAsync(ids, usuarioModificacion);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<bool>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(true);
        }

        public async Task<ResultOperation<ResponseRequerimientoDescartarAsunto>> RequerimientoDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {

            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={entityAsuntosPenales.fecha_recepcion.ToString("yyyy-MM-dd")}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartarAsunto>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequerimientoDescartarAsunto>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;

            var result = await _asuntosPenalesDescartarRepository.DescartarRequerimientoAsync(entityAsuntosPenales, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseRequerimientoDescartarAsunto>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseRequerimientoDescartarAsunto()
            {
                noAsunto = entityAsuntosPenales.numero_asunto_penal!
            });
        }

        public async Task<ResultOperation<ResponseRequisitosDescartarAsunto>> RequisitosDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={entityAsuntosPenales.fecha_recepcion.ToString("yyyy-MM-dd")}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequisitosDescartarAsunto>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseRequisitosDescartarAsunto>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;

            var result = await _asuntosPenalesDescartarRepository.DescartarRequisitosAsync(entityAsuntosPenales, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseRequisitosDescartarAsunto>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseRequisitosDescartarAsunto()
            {
                noAsunto = entityAsuntosPenales.numero_asunto_penal!
            });
        }


        public async Task<ResultOperation<ResponseEtapaInicialDescartarAsunto>> EtapaInicialDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={entityAsuntosPenales.fecha_recepcion.ToString("yyyy-MM-dd")}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaInicialDescartarAsunto>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaInicialDescartarAsunto>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;

            var result = await _asuntosPenalesDescartarRepository.DescartarEtapaInicialAsync(entityAsuntosPenales, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseEtapaInicialDescartarAsunto>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseEtapaInicialDescartarAsunto()
            {
                noAsunto = entityAsuntosPenales.numero_asunto_penal!
            });
        }

        public async Task<ResultOperation<ResponseEtapaComplementariaDescartarAsunto>> EtapaComplementariaDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={entityAsuntosPenales.fecha_recepcion.ToString("yyyy-MM-dd")}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaComplementariaDescartarAsunto>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaComplementariaDescartarAsunto>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;

            var result = await _asuntosPenalesDescartarRepository.DescartarEtapaComplementariaAsync(entityAsuntosPenales, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseEtapaComplementariaDescartarAsunto>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseEtapaComplementariaDescartarAsunto()
            {
                noAsunto = entityAsuntosPenales.numero_asunto_penal!
            });
        }

        public async Task<ResultOperation<ResponseEtapaIntermediaDescartarAsunto>> EtapaIntermediaDescartarAsuntoPenalAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={entityAsuntosPenales.fecha_recepcion.ToString("yyyy-MM-dd")}&addDays={90}&nextDay={true}");
            if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaIntermediaDescartarAsunto>("No se pudo calcular la fecha de vencimiento");
            }

            if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            {
                return ResultOperation.FailureWarningResponse<ResponseEtapaIntermediaDescartarAsunto>("La fecha de vencimiento no tiene el formato correcto.");
            }

            entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;

            var result = await _asuntosPenalesDescartarRepository.DescartarEtapaIntermediaAsync(entityAsuntosPenales, entity, listaSecciones);
            if (!result.Success)
                return ResultOperation.FailureErrorResponse<ResponseEtapaIntermediaDescartarAsunto>($"{result.MsgError!}:{result.DetailError}");

            return ResultOperation.SuccessResponseNoMessage(new ResponseEtapaIntermediaDescartarAsunto()
            {
                noAsunto = entityAsuntosPenales.numero_asunto_penal!
            });
        }

        public async Task<ResultOperation<ResponseCalcularFecha>> CalcularFecha(string baseDate, int addDays, bool nextDay)
        {
            DateTime parsedDate = DateTime.Parse(baseDate);
            string formattedDate = parsedDate.ToString("yyyy-MM-dd");

            // var responseFechaVencimiento = await _apiService.GetResultOperationAsync<string>($"{_proxyEnpoints.RouteFechasAutoCalculadas}?idModule={EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode()}&baseDate={formattedDate}&addDays={addDays}&nextDay={nextDay}");
            // if (responseFechaVencimiento is null || !responseFechaVencimiento.Success || string.IsNullOrEmpty(responseFechaVencimiento.Result))
            // {
            //     return ResultOperation.FailureWarningResponse<ResponseCalcularFecha>("No se pudo calcular la fecha de vencimiento");
            // }
            // if (!DateTime.TryParse(responseFechaVencimiento.Result, out DateTime fechaVencimiento))
            // {
            //     return ResultOperation.FailureWarningResponse<ResponseCalcularFecha>("La fecha de vencimiento no tiene el formato correcto.");
            // }
            var request = new
                {
                    fechaPivote = formattedDate,
                    idModulo = EnumModulosSicoj.ASUNTOS_PENALES.GetHashCode(),
                    idSeccion = 1,
                    numeroMesesCalendario = 4,
                    numDias = 0
                };
                
                var responseFechaVencimientoCal = await _apiService.PostAsync<ResultOperationResponse<ResponseFechaVencimiento>>(_routeFechaVencimiento, request);
            if (!DateTime.TryParse(responseFechaVencimientoCal.Result!.fecha_vencimiento, out DateTime fechaVencimientoCal))
            {
                return ResultOperation.FailureWarningResponse<ResponseCalcularFecha>("La fecha de vencimiento no tiene el formato correcto.");
            }

            return ResultOperation.SuccessResponseNoMessage(new ResponseCalcularFecha()
            {
                fechaVencimiento = fechaVencimientoCal.ToString("yyyy-MM-dd")
            });
        }


        public async Task<ResultOperation> GetDocumentosAsync(int Fetch, int Page, string? OrderByColumn, bool OrderDesc, int? idAsuntoPenal, int? idRenglonSeccion)
        {
            List<int> listSecciones = new()
            {
                EnumSecciones.DATOS_GENERALES.GetHashCode(),
                EnumSecciones.ETAPA_AMPARO_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.ETAPA_APELACION_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.ETAPA_COMPLEMENTARIA_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.ETAPA_INICIAL_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.ETAPA_INTERMEDIA_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.ETAPA_JUICIO_INVESTIGACIÓN.GetHashCode(),
                EnumSecciones.REQUERIMIENTO.GetHashCode(),
                EnumSecciones.REQUISITOS_PROCEDIBILIDAD.GetHashCode(),
            };

            var countResult = await _archivosRepository.GetDocumentosDisconnectedCount(idAsuntoPenal, listSecciones, null!, true);

            if (countResult is null || countResult <= 0)
            {
                return ResultOperation.SuccessResponseNoMessage(
                    new DataTableView<ResponseArchivosAsuntosPenales>(new(Page, Fetch, countResult.GetValueOrDefault()), new()));
            }

            Filters.GetDefaultPage(countResult.GetValueOrDefault(), ref Page, Fetch);

            var result = await _archivosRepository.GetDocumentosHistoricoDisconnected(
                true,
                idAsuntoPenal,
                listSecciones,
                idRenglonSeccion!,
                true,
                Fetch,
                Page,
                OrderByColumn,
                OrderDesc
                );

            if (result is null)
            {
                return ResultOperation.FailureWarningResponse<List<ResponseArchivosAsuntosPenales>>("No se encontraron resultados");
            }

            return ResultOperation.SuccessResponseNoMessage(
                new DataTableView<ResponseArchivosAsuntosPenales>(new(Page, Fetch, countResult.GetValueOrDefault()),
                result)
            );
        }


    }    
}
