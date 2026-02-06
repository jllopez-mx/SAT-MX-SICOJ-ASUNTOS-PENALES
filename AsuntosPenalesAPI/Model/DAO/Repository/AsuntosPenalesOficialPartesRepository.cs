using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;

namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class AsuntosPenalesOficialPartesRepository : IAsuntosPenalesOficialPartesRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AsuntosPenalesOficialPartesRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region AsuntosPenales

        public async Task<List<ResponseAsuntosPenalesOficialPartesByFilters>> GetBandejaAsync(
        int pageSize,
        int page,
        string? orderByColumn,
        bool orderDesc,
        int? idAdminCentral

        )
        {
            ParameterPGsql[] parameters =
                {
                new ParameterPGsql("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ParameterPGsql("p_page", NpgsqlDbType.Integer, page),
                new ParameterPGsql("order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ParameterPGsql("order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ParameterPGsql("p_id_admin", NpgsqlDbType.Integer, idAdminCentral)
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ALL,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseAsuntosPenalesOficialPartesByFilters> resultList = new();


            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        numero_asunto_penal = item.IsNull(1) ? null! : item.Field<string?>(1),
                        fecha_recepcion = item.IsNull(2) ? new() : item.Field<DateTime>(2),
                        fecha_vencimiento = item.IsNull(3) ? new() : item.Field<DateTime>(3),
                        oficio_solicitud = item.IsNull(4) ? null! : item.Field<string?>(4),
                        numero_expediente_cadido = item.IsNull(5) ? null! : item.Field<string?>(5),
                        unidadRealizaSolicitud = item.IsNull(7) ? null! : item.Field<string>(7),
                        idUnidadRealizaSolicitud = item.IsNull(6) ? 0 : item.Field<int>(6),
                        estadoTarea = item.IsNull(9) ? null! : item.Field<string>(9),
                        idEstadoTarea = item.IsNull(8) ? 0 : item.Field<int>(8),
                        estadoProcesal = item.IsNull(11) ? null! : item.Field<string>(11),
                        idEstadoProcesal = item.IsNull(10) ? 0 : item.Field<int>(10),
                        interno = item.IsNull(12) ? false : item.Field<bool>(12),
                    }
                );
            }

            return resultList;
        }

        public async Task<int?> GetBandejaCountAsync(int? idAdminCentral)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_admin", NpgsqlDbType.Integer, idAdminCentral),
            };

            var response = await _database.ExecuteFunctionAsync(EnumFunctions.ASUNTOSPENALES_GET_ALL_COUNT, parameters!);
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);

        }

        public async Task<int?> GetHistoricoCountAsync(
            string numeroasuntopenal,
            DateTime? fecharecepcion_desde,
            DateTime? fecharecepcion_hasta,
            DateTime? fechavencimiento_desde,
            DateTime? fechavencimiento_hasta,
            string oficiosolicitud,
            string numeroexpedientecadido,
            int? id_unidadrealizasolicitud,
            int? id_admin_controla,
            int? id_subadministracion,
            string? nombre_abogado,
            int? id_estadotarea,
            int? id_estadoprocesal,
            bool? tipo_unidad,
            int? id_administracion_central)
        {

            ParameterPGsql[] parameters =
                {
                new ParameterPGsql("p_numeroasuntopenal", NpgsqlDbType.Varchar, numeroasuntopenal),
                new ParameterPGsql("p_fecha_recepcion_desde", NpgsqlDbType.Date, fecharecepcion_desde),
                new ParameterPGsql("p_fecha_recepcion_hasta", NpgsqlDbType.Date, fecharecepcion_hasta),
                new ParameterPGsql("p_fecha_vencimiento_desde", NpgsqlDbType.Date, fechavencimiento_desde),
                new ParameterPGsql("p_fecha_vencimiento_hasta", NpgsqlDbType.Date, fechavencimiento_hasta),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, oficiosolicitud),
                new ParameterPGsql("p_numero_expediente_cadido", NpgsqlDbType.Varchar, numeroexpedientecadido),
                new ParameterPGsql("p_unidad_realiza_solicitud", NpgsqlDbType.Integer, id_unidadrealizasolicitud),
                new ParameterPGsql("p_admincontrola", NpgsqlDbType.Integer, id_admin_controla),
                new ParameterPGsql("p_subadministracion", NpgsqlDbType.Integer, id_subadministracion),
                new ParameterPGsql("p_nombre_abogado", NpgsqlDbType.Varchar, nombre_abogado),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, id_estadotarea),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, id_estadoprocesal),
                new ParameterPGsql("p_tipo_unidad", NpgsqlDbType.Boolean, tipo_unidad),
                new ParameterPGsql("p_id_administracion_central", NpgsqlDbType.Integer, id_administracion_central)
            };

            var response = await _database.ExecuteFunctionAsync(EnumFunctions.ASUNTOSPENALES_BY_FILTERS_COUNT, parameters!);
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);

        }

        public async Task<ResultTransaction> AddAsuntosPenalesAsync(AsuntosPenales entity)
        {
            ParameterPGsql[] parameters =
            {

                new ParameterPGsql(
                    "p_fecha_recepcion",
                    NpgsqlDbType.Date,
                    entity.fecha_recepcion!),
                new ParameterPGsql(
                    "p_fecha_vencimiento",
                    NpgsqlDbType.Date,
                    entity.fecha_vencimiento!),
                new ParameterPGsql(
                    "p_oficio_solicitud",
                    NpgsqlDbType.Varchar,
                    entity.oficio_solicitud!
                ),
                new ParameterPGsql(
                    "p_numero_expediente_cadido",
                    NpgsqlDbType.Varchar,
                    entity.numero_expediente_cadido!
                ),
                new ParameterPGsql(
                    "p_unidad_realiza_solicitud",
                    NpgsqlDbType.Integer,
                    entity.id_unidad_realiza_solicitud!),
                new ParameterPGsql(
                    "p_unidad_tipo",
                    NpgsqlDbType.Boolean,
                    entity.interno!),
                new ParameterPGsql(
                    "p_id_admin_controla",
                    NpgsqlDbType.Integer,
                    entity.id_admin_controla!),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!),
                new ParameterPGsql(
                    "p_id_administracion_central",
                    NpgsqlDbType.Integer,
                    entity.id_administracion_central!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_CREATE,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> AddControlDocumentalAsync(AsuntosPenales entity)
        {
            ParameterPGsql[] parameters =
            {

                new ParameterPGsql(
                    "p_fecha_recepcion",
                    NpgsqlDbType.Date,
                    entity.fecha_recepcion!),
                new ParameterPGsql(
                    "p_oficio_solicitud",
                    NpgsqlDbType.Varchar,
                    entity.oficio_solicitud!
                ),
                new ParameterPGsql(
                    "p_unidad_realiza_solicitud",
                    NpgsqlDbType.Integer,
                    entity.id_unidad_realiza_solicitud!),
                new ParameterPGsql(
                    "p_unidad_tipo",
                    NpgsqlDbType.Boolean,
                    entity.interno!),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!),
                new ParameterPGsql(
                    "p_id_administracion_central",
                    NpgsqlDbType.Integer,
                    entity.id_administracion_central!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_CREATE_CONTROL,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResponseAsuntosPenalesById> GetAsuntosPenalesByIdDisconnectedAsync(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_BY_ID,
                parameters
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0),
                numero_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1) ? null! : response.Data.Tables[0].Rows[0].Field<string>(1),
                fecha_recepcion = response.Data.Tables[0].Rows[0].IsNull(2) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(2)!,
                fecha_vencimiento = response.Data.Tables[0].Rows[0].IsNull(3) ? new()! : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!,
                oficio_solicitud = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string?>(4),
                numero_expediente_cadido = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<string?>(5),
                idUnidadRealizaSolicitud =
                response.Data.Tables[0].Rows[0].IsNull(6) ? null : response.Data.Tables[0].Rows[0].Field<int>(6),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(7) ? null : response.Data.Tables[0].Rows[0].Field<int>(7),
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(8) ? null : response.Data.Tables[0].Rows[0].Field<int>(8),
                turnado = response.Data.Tables[0].Rows[0].IsNull(9) ? false : response.Data.Tables[0].Rows[0].Field<bool>(9),
                idAdminControla =
                response.Data.Tables[0].Rows[0].IsNull(10) ? null : response.Data.Tables[0].Rows[0].Field<int>(10),
                activo = response.Data.Tables[0].Rows[0].IsNull(11) ? false : response.Data.Tables[0].Rows[0].Field<bool>(11),
                interno = response.Data.Tables[0].Rows[0].IsNull(12) ? false : response.Data.Tables[0].Rows[0].Field<bool>(12)

            };
        }
        public async Task<List<ResponseAsuntosPenalesOficialPartesByFilters>> GetHistoricoAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            string numeroasuntopenal,
            DateTime? fecharecepcion_desde,
            DateTime? fecharecepcion_hasta,
            DateTime? fechavencimiento_desde,
            DateTime? fechavencimiento_hasta,
            string oficiosolicitud,
            string numeroexpedientecadido,
            int? id_unidadrealizasolicitud,
            int? id_admin_controla,
            int? id_subadministracion,
            string? nombre_abogado,
            int? id_estadotarea,
            int? id_estadoprocesal,
            bool? tipo_unidad,
            int? id_administracion_central
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ParameterPGsql("p_page", NpgsqlDbType.Integer, page),
                new ParameterPGsql("order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ParameterPGsql("order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ParameterPGsql("p_numeroasuntopenal", NpgsqlDbType.Varchar, numeroasuntopenal),
                new ParameterPGsql("p_fecha_recepcion_desde", NpgsqlDbType.Date, fecharecepcion_desde),
                new ParameterPGsql("p_fecha_recepcion_hasta", NpgsqlDbType.Date, fecharecepcion_hasta),
                new ParameterPGsql("p_fecha_vencimiento_desde", NpgsqlDbType.Date, fechavencimiento_desde),
                new ParameterPGsql("p_fecha_vencimiento_hasta", NpgsqlDbType.Date, fechavencimiento_hasta),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, oficiosolicitud),
                new ParameterPGsql("p_numero_expediente_cadido", NpgsqlDbType.Varchar, numeroexpedientecadido),
                new ParameterPGsql("p_unidad_realiza_solicitud", NpgsqlDbType.Integer, id_unidadrealizasolicitud),
                new ParameterPGsql("p_admincontrola", NpgsqlDbType.Integer, id_admin_controla),
                new ParameterPGsql("p_subadministracion", NpgsqlDbType.Integer, id_subadministracion),
                new ParameterPGsql("p_nombre_abogado", NpgsqlDbType.Varchar, nombre_abogado),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, id_estadotarea),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, id_estadoprocesal),
                new ParameterPGsql("p_tipo_unidad", NpgsqlDbType.Boolean, tipo_unidad),
                new ParameterPGsql("p_id_administracion_central", NpgsqlDbType.Integer, id_administracion_central)
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ALL_BY_FILTERS,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseAsuntosPenalesOficialPartesByFilters> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        numero_asunto_penal = item.IsNull(1) ? null! : item.Field<string>(1),
                        fecha_recepcion = item.IsNull(2) ? new() : item.Field<DateTime>(2),
                        fecha_vencimiento = item.IsNull(3) ? null! : item.Field<DateTime>(3)!,
                        oficio_solicitud = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        numero_expediente_cadido = item.IsNull(5) ? null! : item.Field<string>(5),
                        unidadRealizaSolicitud = item.IsNull(16) ? null! : item.Field<string>(16),
                        idUnidadRealizaSolicitud = item.IsNull(6) ? null : item.Field<int>(6),
                        abogado = item.IsNull(18) ? null! : item.Field<string>(18),
                        idAbogado = item.IsNull(9) ? null! : item.Field<string>(9),
                        subAdministracion = item.IsNull(15) ? null! : item.Field<string>(15),
                        idSubAdministracion = item.IsNull(8) ? null : item.Field<int>(8),
                        adminControla = item.IsNull(12) ? null! : item.Field<string>(12),
                        idAdminControla = item.IsNull(7) ? null : item.Field<int>(7),
                        estadoTarea = item.IsNull(13) ? null! : item.Field<string>(13),
                        idEstadoTarea = item.IsNull(10) ? null : item.Field<int>(10),
                        estadoProcesal = item.IsNull(14) ? null! : item.Field<string>(14),
                        idEstadoProcesal = item.IsNull(11) ? null : item.Field<int>(11),
                        interno = item.IsNull(17) ? false : item.Field<bool>(17),
                        conteoImputados = item.IsNull(19) ? null : item.Field<int>(19)
                    }
                );
            }

            return resultList;
        }

        public async Task<ResultTransaction> TurnarAsuntosPenalesAsync(AsuntosPenales entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id",NpgsqlDbType.Integer,entity.id),
                new ParameterPGsql("p_numero_de_empleado_op", NpgsqlDbType.Varchar, entity.numero_empleado!),
                new ParameterPGsql("p_id_admin_controla",NpgsqlDbType.Integer,entity.id_admin_controla),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, entity.id_estado_tarea!),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!)
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_TURNAR_ASUNTOS_PENALES,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> UpdateAsuntosPenalesAsync(AsuntosPenales entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id",NpgsqlDbType.Integer,entity.id),
                new ParameterPGsql("p_numeroasuntopenal", NpgsqlDbType.Varchar, entity.numero_asunto_penal!),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, entity.oficio_solicitud!),
                new ParameterPGsql("p_fecha_recepcion", NpgsqlDbType.Date, entity.fecha_recepcion!),
                new ParameterPGsql("p_numero_expediente_cadido", NpgsqlDbType.Varchar, entity.numero_expediente_cadido!),
                new ParameterPGsql("p_fecha_vencimiento", NpgsqlDbType.Date, entity.fecha_vencimiento!),
                new ParameterPGsql("p_unidad_realiza_solicitud", NpgsqlDbType.Integer, entity.id_unidad_realiza_solicitud!),
                new ParameterPGsql("p_id_admin_controla",NpgsqlDbType.Integer,entity.id_admin_controla),
                new ParameterPGsql("p_unidad_tipo",NpgsqlDbType.Boolean,entity.interno),
                new ParameterPGsql("p_id_estado_procesal",NpgsqlDbType.Integer,entity.id_estado_procesal),
                new ParameterPGsql("p_id_estado_tarea",NpgsqlDbType.Integer,entity.id_estado_tarea),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_UPDATE_ASUNTOS_PENALES,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResultTransaction> DeleteAsuntosPenalesAsync(AsuntosPenales entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id",NpgsqlDbType.Integer,entity.id),

            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_DELETE_ASUNTOS_PENALES,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }
        #endregion

        public async Task<ResponseNumeroAsunto> GetAsuntosPenalesByNumeroAsuntoAsync(string numero_asunto)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_numero_asunto", NpgsqlDbType.Varchar, numero_asunto), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_BY_NUMERO_ASUNTO,
                parameters
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return new()
            {
                id_asunto = response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0),
                numero_asunto = response.Data.Tables[0].Rows[0].IsNull(1) ? null! : response.Data.Tables[0].Rows[0].Field<string>(1)!,
                id_unidad_administrativa = response.Data.Tables[0].Rows[0].IsNull(2) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(2),
                nombre_unidad = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3)!

            };
        }

        #region Solicitud Transparencia

        public async Task<ResultTransaction> AddAsyncSolicitudTransparenciaService(SolicitudTransparencia entity, ArchivoAsuntoPenal entityDocumento, DataFile dataFile)
        {


            ParameterPGsql[] parameters =
            {

                new ParameterPGsql("p_id_rol", NpgsqlDbType.Integer, entity.id_rol!),
                new ParameterPGsql("p_id_asunto", NpgsqlDbType.Integer, entity.id_asunto!),
                new ParameterPGsql("p_no_solicitud",NpgsqlDbType.Varchar,entity.noSolicitud!),
                new ParameterPGsql("p_fecha_solicitud", NpgsqlDbType.Date, entity.fechaSolicitud!),
                new ParameterPGsql(
                    "p_archivo",
                    NpgsqlDbType.Boolean,
                    dataFile is not null
                ),
                new ParameterPGsql("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!),
                new ParameterPGsql("p_file_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.file_name!),
                new ParameterPGsql("p_file_path", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.path_file !),
                new ParameterPGsql("p_content_type", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.content_type!),
                new ParameterPGsql("p_file_size", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.size!),
                new ParameterPGsql("p_owner_name", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!),
                new ParameterPGsql("p_no_folio", NpgsqlDbType.Text, entityDocumento is null ? DBNull.Value : entityDocumento.no_folio!),
                //new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, EnumEstadoTarea.ASIGNADO.GetHashCode()),
                //new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, EnumEstadoProcesal.REQUERIDO.GetHashCode())
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.SOLICITUD_TRANSPARENCIA_INSERT,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<SolicitudTransparencia> GetByIdSolicitudTransparenciaRepository(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesReadIdSolicitudTransparencia,
                parameters
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_rol = response.Data.Tables[0].Rows[0].IsNull(1) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(1),
                nombre_rol = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2),
                id_asunto = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                noSolicitud = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string>(4),
                fechaSolicitud = response.Data.Tables[0].Rows[0].IsNull(5) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(5),
                fecha_registro = response.Data.Tables[0].Rows[0].IsNull(6) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(6),
                fecha_modificacion = response.Data.Tables[0].Rows[0].IsNull(7) ? new() : response.Data.Tables[0].Rows[0].Field<DateTime>(7),
            };
        }

        public async Task<ResultTransaction> UpdateSolicitudTransparenciaRepository(SolicitudTransparencia entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entity.id!),
                new ParameterPGsql("p_id_asunto", NpgsqlDbType.Integer, entity.id_asunto!),
                new ParameterPGsql("p_no_solicitud",NpgsqlDbType.Varchar,entity.noSolicitud!),
                new ParameterPGsql("p_fecha_solicitud", NpgsqlDbType.Date, entity.fechaSolicitud!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesUpdateSolicitudTransparencia,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        public async Task<ResponseSolicitudTransparenciaList> GetByIdSolicitudTransparenciaRepositorys(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesReadIdSolicitudTransparencia,
                parameters
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_rol = response.Data.Tables[0].Rows[0].IsNull(1) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(1),
                nombre_rol = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2),
                id_asunto = response.Data.Tables[0].Rows[0].IsNull(3) ? 0 : response.Data.Tables[0].Rows[0].Field<int>(3),
                no_solicitud = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string>(4),
                fecha_solicitud = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(5).ToString("yyyy-MM-dd"),
                fecha_registro = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(6).ToString("yyyy-MM-dd"),
                fecha_modificacion = response.Data.Tables[0].Rows[0].IsNull(7) ? null! : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("yyyy-MM-dd"),
            };
        }

        public async Task<int?> GetTablaSolicitudTransparenciaCountRepository(int idAsunto)
        {
            ParameterPGsql[] parameters =
            {
            new ParameterPGsql("p_id_asunto", NpgsqlDbType.Integer, idAsunto),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesReadCountSolicitudTransparencia,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);
        }

        public async Task<List<ResponseSolicitudTransparenciaList>> GetTablaSolicitudTransparenciaRepository(int idAsunto)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("order_column", NpgsqlDbType.Varchar, ""),
                new ParameterPGsql("order_desc", NpgsqlDbType.Boolean, false),
                new ParameterPGsql("p_id_asunto", NpgsqlDbType.Integer, idAsunto),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesReadTablaSolicitudTransparencia,
                parameters!
             );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseSolicitudTransparenciaList> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_rol = item.IsNull(1) ? 0 : item.Field<int>(1),
                        nombre_rol = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        id_asunto = item.IsNull(3) ? 0 : item.Field<int>(3),
                        no_solicitud = item.IsNull(4) ? null! : item.Field<string>(4),
                        fecha_solicitud = item.IsNull(5) ? null! : item.Field<DateTime>(5).ToString("yyyy-MM-dd"),
                        fecha_registro = item.IsNull(6) ? null! : item.Field<DateTime>(6).ToString("yyyy-MM-dd"),
                        fecha_modificacion = item.IsNull(7) ? null! : item.Field<DateTime>(7).ToString("yyyy-MM-dd"),
                    }
                );
            }

            return resultList;

        }

        public async Task<ResultTransaction> DeleteSolicitudTransparenciaRepository(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.OficialPartesDeleteSolicitudTransparencia,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }

        #endregion

        public async Task<List<ResponseFechaVencimientoSeccion>> FechasVencimientoAsync(

            DateTime? fecha_inicial,
            DateTime? fecha_final,
            List<int>? secciones

        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_fecha_inicial", NpgsqlDbType.Date, fecha_inicial),
                new ParameterPGsql("p_fecha_final", NpgsqlDbType.Date, fecha_final),
                new ParameterPGsql("p_secciones", NpgsqlDbType.Array | NpgsqlDbType.Integer, (secciones is null || !secciones.Any()) ? DBNull.Value :  secciones.ToArray()),

            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_FECHA_VENCIMIENTOREGISTRO,
                parameters!
            );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseFechaVencimientoSeccion> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        idAsunto = item.IsNull(0) ? 0 : item.Field<int>(0),
                        fecha_vencimiento = item.IsNull(1) ? null! : item.Field<DateTime>(1).ToString("yyyy-MM-dd")!,
                    }
                );
            }

            return resultList;
        }
        
         public async Task<ResultTransaction> UpdateAsuntosPenalesMasivoAsync(DateTime? fecha_vencimiento, int? idAsunto, int? idModulo, int? idSeccion, int? idSeccionRenglon)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_fecha_vencimiento",NpgsqlDbType.Date,fecha_vencimiento),
                new ParameterPGsql("p_id_asunto", NpgsqlDbType.Integer, idAsunto),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_UPDATE_ASUNTOS_PENALES_MASIVO,
                parameters
            );
            if (response.ExisteError)
            {
                return new()
                {
                    Success = false,
                    MsgError = response.Mensaje,
                    NoError = response.CodeSqlError,
                };
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
                };
            }

            return new()
            {
                Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
                Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
                MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
                DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
                NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
            };
        }
    }

}