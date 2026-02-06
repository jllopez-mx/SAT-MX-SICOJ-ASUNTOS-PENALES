using System.Data;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;

namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class AsuntosPenalesAbogadoRepository : IAsuntosPenalesAbogadoRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AsuntosPenalesAbogadoRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        #region AsuntosPenales


        public async Task<List<ResponseAsuntosPenalesAbogadoByFilters>> GetBandejaAsync(
            int pageSize,
            int page,
            string? orderByColumn,
            bool orderDesc,
            int? idSubAdmin,
            string? rfc_abogado
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ParameterPGsql("p_page", NpgsqlDbType.Integer, page),
                new ParameterPGsql("order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ParameterPGsql("order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ParameterPGsql("p_subadministracion", NpgsqlDbType.Integer, idSubAdmin),
                new ParameterPGsql("p_rfc_abogado", NpgsqlDbType.Varchar, rfc_abogado),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOS_PENALES_GET_BANDEJA_ABOGADO,
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

            List<ResponseAsuntosPenalesAbogadoByFilters> resultList = new();

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
                        unidadRealizaSolicitud = item.IsNull(7) ? null! : item.Field<string>(7),
                        idUnidadRealizaSolicitud = item.IsNull(6) ? 0 : item.Field<int>(6),
                        nombre_abogado = item.IsNull(10) ? null! : item.Field<string>(10),
                        idAbogado = item.IsNull(17) ? null! : item.Field<string>(17),

                        subAdministracion = item.IsNull(16) ? null! : item.Field<string>(16),
                        idSubAdministracion = item.IsNull(9) ? 0 : item.Field<int>(9),
                        adminControla = item.IsNull(13) ? null! : item.Field<string>(13),
                        idAdminControla = item.IsNull(8) ? 0 : item.Field<int>(8),
                        estadoTarea = item.IsNull(14) ? null! : item.Field<string>(14),
                        idEstadoTarea = item.IsNull(11) ? 0 : item.Field<int>(11),

                        estadoProcesal = item.IsNull(15) ? null! : item.Field<string>(15),
                        idEstadoProcesal = item.IsNull(12) ? 0 : item.Field<int>(12),
                        interno = item.IsNull(18) ? false : item.Field<bool>(18),
                        conteoImputados = item.IsNull(19) ? 0 : item.Field<int>(19),
                    }
                );
            }

            return resultList;
        }

        public async Task<int?> GetBandejaCountAsync(int? idSubAdmin, string? rfc_abogado)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_subadministracion", NpgsqlDbType.Integer, idSubAdmin),
                new ParameterPGsql("p_rfc_abogado", NpgsqlDbType.Varchar, rfc_abogado),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOS_PENALES_GET_BANDEJA_ABOGADO_COUNT,
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

        public async Task<List<ResponseAsuntosPenalesAbogadoByFilters>> GetHistoricoAsync(
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
            int? id_administracion_adsc,
            bool? tipoUnidad
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_page_size", NpgsqlDbType.Integer, pageSize),
                new ParameterPGsql("p_page", NpgsqlDbType.Integer, page),
                new ParameterPGsql("order_column", NpgsqlDbType.Varchar, orderByColumn),
                new ParameterPGsql("order_desc", NpgsqlDbType.Boolean, orderDesc),
                new ParameterPGsql("p_numeroasuntopenal", NpgsqlDbType.Varchar, numeroasuntopenal),
                new ParameterPGsql(
                    "p_fecha_recepcion_desde",
                    NpgsqlDbType.Date,
                    fecharecepcion_desde
                ),
                new ParameterPGsql(
                    "p_fecha_recepcion_hasta",
                    NpgsqlDbType.Date,
                    fecharecepcion_hasta
                ),
                new ParameterPGsql(
                    "p_fecha_vencimiento_desde",
                    NpgsqlDbType.Date,
                    fechavencimiento_desde
                ),
                new ParameterPGsql(
                    "p_fecha_vencimiento_hasta",
                    NpgsqlDbType.Date,
                    fechavencimiento_hasta
                ),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, oficiosolicitud),
                new ParameterPGsql(
                    "p_numero_expediente_cadido",
                    NpgsqlDbType.Varchar,
                    numeroexpedientecadido
                ),
                new ParameterPGsql(
                    "p_unidad_realiza_solicitud",
                    NpgsqlDbType.Integer,
                    id_unidadrealizasolicitud
                ),
                new ParameterPGsql("p_admincontrola", NpgsqlDbType.Integer, id_admin_controla),
                new ParameterPGsql(
                    "p_subadministracion",
                    NpgsqlDbType.Integer,
                    id_subadministracion
                ),
                new ParameterPGsql("p_nombre_abogado", NpgsqlDbType.Varchar, nombre_abogado),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, id_estadotarea),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, id_estadoprocesal),
                new ParameterPGsql(
                    "p_administracion_adscrita",
                    NpgsqlDbType.Integer,
                    id_administracion_adsc
                ),
                new ParameterPGsql(
                    "p_tipo_unidad",
                    NpgsqlDbType.Boolean,
                    tipoUnidad
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ALL_BY_FILTERS_ABOGADO,
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

            List<ResponseAsuntosPenalesAbogadoByFilters> resultList = new();

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
                        unidadRealizaSolicitud = item.IsNull(7) ? null! : item.Field<string>(7),
                        idUnidadRealizaSolicitud = item.IsNull(6) ? 0 : item.Field<int>(6),

                        adminControla = item.IsNull(9) ? null! : item.Field<string>(9),
                        idAdminControla = item.IsNull(8) ? 0 : item.Field<int>(8),
                        subAdministracion = item.IsNull(11) ? null! : item.Field<string>(11),
                        idSubAdministracion = item.IsNull(10) ? 0 : item.Field<int>(10),

                        nombre_abogado = item.IsNull(12) ? null! : item.Field<string>(12),
                        idAbogado = item.IsNull(17) ? null! : item.Field<string>(17),
                        estadoTarea = item.IsNull(14) ? null! : item.Field<string>(14),
                        idEstadoTarea = item.IsNull(13) ? 0 : item.Field<int>(13),

                        estadoProcesal = item.IsNull(16) ? null! : item.Field<string>(16),
                        idEstadoProcesal = item.IsNull(15) ? 0 : item.Field<int>(15),

                        interno = item.IsNull(18) ? false : item.Field<bool>(18),
                        conteoImputados = item.IsNull(19) ? 0 : item.Field<int>(19)
                    }
                );
            }

            return resultList;
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
            int? id_administracion_adsc,
            bool? tipoUnidad
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_numeroasuntopenal", NpgsqlDbType.Varchar, numeroasuntopenal),
                new ParameterPGsql(
                    "p_fecha_recepcion_desde",
                    NpgsqlDbType.Date,
                    fecharecepcion_desde
                ),
                new ParameterPGsql(
                    "p_fecha_recepcion_hasta",
                    NpgsqlDbType.Date,
                    fecharecepcion_hasta
                ),
                new ParameterPGsql(
                    "p_fecha_vencimiento_desde",
                    NpgsqlDbType.Date,
                    fechavencimiento_desde
                ),
                new ParameterPGsql(
                    "p_fecha_vencimiento_hasta",
                    NpgsqlDbType.Date,
                    fechavencimiento_hasta
                ),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, oficiosolicitud),
                new ParameterPGsql(
                    "p_numero_expediente_cadido",
                    NpgsqlDbType.Varchar,
                    numeroexpedientecadido
                ),
                new ParameterPGsql(
                    "p_unidad_realiza_solicitud",
                    NpgsqlDbType.Integer,
                    id_unidadrealizasolicitud
                ),
                new ParameterPGsql("p_admincontrola", NpgsqlDbType.Integer, id_admin_controla),
                new ParameterPGsql(
                    "p_subadministracion",
                    NpgsqlDbType.Integer,
                    id_subadministracion
                ),
                new ParameterPGsql("p_nombre_abogado", NpgsqlDbType.Varchar, nombre_abogado),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Integer, id_estadotarea),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, id_estadoprocesal),
                new ParameterPGsql(
                    "p_administracion_adscrita",
                    NpgsqlDbType.Integer,
                    id_administracion_adsc
                ),
                new ParameterPGsql(
                    "p_tipo_unidad",
                    NpgsqlDbType.Boolean,
                    tipoUnidad
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ALL_BY_FILTERS_ABOGADO_COUNT,
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


        public async Task<ResponseAsuntosPenalesByIdAdmin> GetAbogadoAsuntoPenalById(
            int id_asuntopenal,
            string? rfc_abogado
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, id_asuntopenal),
                new ParameterPGsql("p_rfc_abogado", NpgsqlDbType.Varchar, rfc_abogado),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ASUNTO_PENAL_ABOGADO_BY_ID,
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
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                numero_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(1),
                fecha_recepcion = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(2)!,
                fecha_vencimiento = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? new()!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!,
                oficio_solicitud = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string?>(4),
                numero_expediente_cadido = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string?>(5),
                idUnidadRealizaSolicitud =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(6),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(7)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(7),
                idAdminControla =
                response.Data.Tables[0].Rows[0].IsNull(8)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(8),
                idSubAdministracion =
                response.Data.Tables[0].Rows[0].IsNull(9)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(9),
                nombre_abogado = response.Data.Tables[0].Rows[0].IsNull(10)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<string?>(10),
                idAbogado = response.Data.Tables[0].Rows[0].IsNull(11)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<string?>(11),

                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(12)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(12),
                interno = response.Data.Tables[0].Rows[0].IsNull(13)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(13),
            };
        }

        public async Task<ResultTransaction> AnalisisAsync(
            AsuntosPenales entity,
            AsuntosPenalesAnalisis entityAnalisis,
            ArchivosAsuntosPenales? entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_requerimiento",
                    NpgsqlDbType.Boolean,
                    entityAnalisis.requerimiento!
                ),
                new ParameterPGsql(
                    "p_oficio_requerimiento",
                    NpgsqlDbType.Text,
                    entityAnalisis.oficio_requerimiento!
                ),
                new ParameterPGsql(
                    "p_fecha_requerimiento",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_requerimiento!
                ),
                new ParameterPGsql(
                    "p_requerimiento_atendido",
                    NpgsqlDbType.Boolean,
                    entityAnalisis.requerimiento_atendido!
                ),
                new ParameterPGsql(
                    "p_oficio_atencion",
                    NpgsqlDbType.Text,
                    entityAnalisis.oficio_atencion!
                ),
                new ParameterPGsql(
                    "p_fecha_atencion",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_atencion!
                ),
                new ParameterPGsql(
                    "p_id_determinacion_asunto_penal",
                    NpgsqlDbType.Integer,
                    entityAnalisis.id_determinacion_asunto_penal!
                ),
                new ParameterPGsql(
                    "p_fecha_determinacion",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_determinacion!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal
                ),
                //archivo
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityAnalisis.usuario_creacion!
                ),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.INSERT_ANALISIS_ASUNTO,
                dataFile!,
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

        public async Task<ResultTransaction> ProcedibilidadAsync(
            AsuntosPenales entity,
            AsuntosPenalesProcedibilidad entityProcedibilidad
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_id_requisito_procedibilidad",
                    NpgsqlDbType.Integer,
                    entityProcedibilidad.id_requisito_procedibilidad!
                ),
                new ParameterPGsql(
                    "p_numero_oficio_procedibilidad",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.numero_oficio_procedibilidad!
                ),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityProcedibilidad.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_cuantia",
                    NpgsqlDbType.Numeric,
                    entityProcedibilidad.cuantia!
                ),
                new ParameterPGsql(
                    "p_numero_carpeta_investigacion",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.numero_carpeta_investigacion!
                ),
                new ParameterPGsql(
                    "p_agente_ministerio_publico",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.agente_ministerio_publico!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.usuario_creacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_REQUISITOS_PROCEDIBILIDAD_ASUNTO,
                parameters!
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

        public async Task<ResultTransaction> UpdateProcedibilidadAsync(
            AsuntosPenalesProcedibilidad entityProcedibilidad
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityProcedibilidad.id),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entityProcedibilidad.id_estado_procesal),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityProcedibilidad.id_asunto_penal),
                new ParameterPGsql(
                    "p_id_requisito_procedibilidad",
                    NpgsqlDbType.Integer,
                    entityProcedibilidad.id_requisito_procedibilidad!
                ),
                new ParameterPGsql(
                    "p_numero_oficio_procedibilidad",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.numero_oficio_procedibilidad!
                ),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityProcedibilidad.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_cuantia",
                    NpgsqlDbType.Numeric,
                    entityProcedibilidad.cuantia!
                ),
                new ParameterPGsql(
                    "p_numero_carpeta_investigacion",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.numero_carpeta_investigacion!
                ),
                new ParameterPGsql(
                    "p_agente_ministerio_publico",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.agente_ministerio_publico!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityProcedibilidad.usuario_modificacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_REQUISITOS_PROCEDIBILIDAD_ASUNTO,
                parameters!
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

        public async Task<ResultTransaction> PersonasMoralesAsync(
            AsuntosPenales entity,
            AsuntosPenalesPersonasMorales entityPersonasMorales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql("p_rfc", NpgsqlDbType.Text, entityPersonasMorales.rfc!),
                new ParameterPGsql(
                    "p_contribuyente",
                    NpgsqlDbType.Boolean,
                    entityPersonasMorales.contribuyente!
                ),
                new ParameterPGsql("p_nombre", NpgsqlDbType.Text, entityPersonasMorales.nombre!),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityPersonasMorales.usuario_creacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_PERSONAS_MORALES,
                parameters!
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

        public async Task<ResultTransaction> MedidasCautelaresAsync(
            List<AsuntosPenalesMedidasCautelares> entityMedidasCautelares
        )
        {
            var arrayObject = entityMedidasCautelares
                .Select(c => $"{c.id_imputado},{c.id_medida},{c.usuario_creacion}")
                .ToArray();
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_array", NpgsqlDbType.Array | NpgsqlDbType.Text, arrayObject),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_MEDIDAS_CAUTELARES,
                parameters!
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

        public async Task<ResultTransaction> DelitosAsync(List<AsuntosPenalesDelitos> entityDelitos)
        {
            var arrayObject = entityDelitos
                .Select(c => $"{c.id_asunto_penal},{c.id_delito},{c.usuario_creacion}")
                .ToArray();
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_array", NpgsqlDbType.Array | NpgsqlDbType.Text, arrayObject),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_DELITOS,
                parameters!
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

        public async Task<List<ResponsePersonasMorales>> GetPersonasMoralesDisconnected(
            int idAsuntosPenales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesPersonasMoralesDisconnected,
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

            List<ResponsePersonasMorales> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsuntoPenal = item.IsNull(1) ? 0 : item.Field<int>(1),
                        rfc = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        nombre = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        contribuyente = item.IsNull(4) ? false : item.Field<bool>(4),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<ResponseDelitos>> GetDelitosDisconnected(int idAsuntosPenales)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesDelitosDisconnected,
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

            List<ResponseDelitos> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsuntoPenal = item.IsNull(1) ? 0 : item.Field<int>(1),
                        id_delito = item.IsNull(2) ? 0 : item.Field<int>(2),
                        codigo = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        delito = item.IsNull(4) ? null! : item.Field<string>(4)!,
                    }
                );
            }

            return resultList;
        }

        public async Task<List<ResponseMedidasCautelares>> GetMedidasCautelaresDisconnected(
            int idImputado
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesMedidasCautelaresDisconnected,
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

            List<ResponseMedidasCautelares> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idMedida = item.IsNull(1) ? 0 : item.Field<int>(1),
                        nombre = item.IsNull(2) ? null! : item.Field<string>(2)!,
                    }
                );
            }

            return resultList;
        }

        public async Task<ResponseAsuntosPenalesByIdProcedibilidad> GetAsuntosPenalesByIdDisconnectedProcedibilidadAsync(
            int idAsuntosPenales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesProcedibilidadByIdDisconnected,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_requisito_procedibilidad =
                response.Data.Tables[0].Rows[0].IsNull(1)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int?>(1),

                numero_oficio_procedibilidad = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fecha_presentacion = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!.ToString("dd/MM/yyyy"),
                cuantia = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<decimal>(4)!,
                numero_carpeta_investigacion = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                agente_ministerio_publico = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(7),
            };
        }

        public async Task<ResponseAsuntosPenalesByIdAnalisis> GetAsuntosPenalesByIdDisconnectedAnalisisAsync(
            int idAsuntosPenales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesAnalisisByIdDisconnected,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                requerimiento = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(2),
                oficio_requerimiento = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                fecha_requerimiento = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4)!.ToString("dd/MM/yyyy"),
                requerimiento_atendido = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(5),
                oficio_atencion = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                fecha_atencion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7)!.ToString("dd/MM/yyyy"),
                id_determinacion_asunto =
                response.Data.Tables[0].Rows[0].IsNull(8)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int?>(8),
                fecha_determinacion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9)!.ToString("dd/MM/yyyy"),
            };
        }

        public async Task<AsuntosPenalesProcedibilidad> GetAsuntosPenalesByIdProcedibilidad(
            int idAsuntosPenales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesProcedibilidadById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_requisito_procedibilidad = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                numero_oficio_procedibilidad = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fecha_presentacion = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!,
                cuantia = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<decimal>(4)!,
                numero_carpeta_investigacion = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                agente_ministerio_publico = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(7),
                activo = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }

        public async Task<AsuntosPenalesAnalisis> GetAsuntosPenalesByIdAnalisis(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesAnalisisById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                requerimiento = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(2),
                oficio_requerimiento = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                fecha_requerimiento = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4)!,
                requerimiento_atendido = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(5),
                oficio_atencion = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                fecha_atencion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7)!,
                id_determinacion_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(8),
                fecha_determinacion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9)!,
                activo = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(10),
            };
        }

        public async Task<List<AsuntosPenalesAnalisis>> GetAsuntosPenalesAnalisisByIdAsuntoPenal(int idAsuntoPenal)
        {
            ParameterPGsql[] parameters =
             {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntoPenal),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesAnalisisByIdAsuntoPenal,
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
            List<AsuntosPenalesAnalisis> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                        id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                        requerimiento = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(2),
                        oficio_requerimiento = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                        fecha_requerimiento = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4)!,
                        requerimiento_atendido = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(5),
                        oficio_atencion = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,
                        fecha_atencion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7)!,
                        id_determinacion_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(8),
                        fecha_determinacion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9)!,
                        activo = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(10),
                    }
                );
            }

            return resultList;
        }



        public async Task<ResultTransaction> UpdateAnalisisAsync(
            AsuntosPenalesAnalisis entityAnalisis
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityAnalisis.id),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAnalisis.id_asunto_penal),
                new ParameterPGsql(
                    "p_requerimiento",
                    NpgsqlDbType.Boolean,
                    entityAnalisis.requerimiento!
                ),
                new ParameterPGsql(
                    "p_oficio_requerimiento",
                    NpgsqlDbType.Text,
                    entityAnalisis.oficio_requerimiento!
                ),
                new ParameterPGsql(
                    "p_fecha_requerimiento",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_requerimiento!
                ),
                new ParameterPGsql(
                    "p_requerimiento_atendido",
                    NpgsqlDbType.Boolean,
                    entityAnalisis.requerimiento_atendido!
                ),
                new ParameterPGsql(
                    "p_oficio_atencion",
                    NpgsqlDbType.Text,
                    entityAnalisis.oficio_atencion!
                ),
                new ParameterPGsql(
                    "p_fecha_atencion",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_atencion!
                ),
                new ParameterPGsql(
                    "p_id_determinacion_asunto_penal",
                    NpgsqlDbType.Integer,
                    entityAnalisis.id_determinacion_asunto_penal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityAnalisis.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_fecha_determinacion",
                    NpgsqlDbType.Date,
                    entityAnalisis.fecha_determinacion!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityAnalisis.usuario_modificacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_ANALISIS_ASUNTO,
                parameters!
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

        public async Task<AsuntosPenalesDelitos> GetByIdDelitosAsync(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesDelitos,
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
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1)!,
                id_delito = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(2),
            };
        }

        public async Task<AsuntosPenalesMedidasCautelares> GetByIdMedidasCautelaresAsync(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesMedidasCautelares,
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
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_imputado = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1)!,
                id_medida = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(2),
                activo = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(3),
            };
        }

        public async Task<ResultTransaction> DeleteDelitos(AsuntosPenalesDelitos entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entity.id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_DELETE_DELITOS,
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

        public async Task<ResultTransaction> DeleteMedidasCautelares(
            AsuntosPenalesMedidasCautelares entity
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entity.id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_DELETE_MEDIDAS_CAUTELARES,
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

        public async Task<ResultTransaction> ImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql("p_rfc", NpgsqlDbType.Text, entityImputados.rfc!),
                new ParameterPGsql(
                    "p_contribuyente",
                    NpgsqlDbType.Boolean,
                    entityImputados.contribuyente!
                ),
                new ParameterPGsql("p_nombre", NpgsqlDbType.Text, entityImputados.nombre!),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal_ap",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_tarea!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_IMPUTADOS,
                parameters!
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

        public async Task<AsuntosPenalesImputados> GetImputadoById(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputados,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                nombre = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(4),
                id_estado_procesal = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(5),
                id_estado_tarea = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<int>(6),
                activo = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(7),
            };
        }

        public async Task<ResponseImputados> GetImputadoByIdDisconnected(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputadosDisconnected,
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

            return new()
            {
                Id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(3),
                nombre = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(5)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int?>(5),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int?>(6),
                concluyeInvestigacion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(7),
                idTerminacionInvestigacion =
                response.Data.Tables[0].Rows[0].IsNull(8)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(8),
                fechaTerminacionInvestigacion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9)!,
                solucionAlterna = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(10),
                idTipoSolucionAlterna =
                response.Data.Tables[0].Rows[0].IsNull(11)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<int?>(11),
                fechaSolicitudAudienciaInicial = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(12)!,
                idCentroJusticia =
                response.Data.Tables[0].Rows[0].IsNull(13)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<int?>(13),
                causaPenal = response.Data.Tables[0].Rows[0].IsNull(14)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(14)!,
                fechaAudienciaInicial = response.Data.Tables[0].Rows[0].IsNull(15)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(15)!,
                autoVinculacionProceso = response.Data.Tables[0].Rows[0].IsNull(16)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(16),
                fechaAutoVinculacion = response.Data.Tables[0].Rows[0].IsNull(17)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(17)!,
                ordenAprehension = response.Data.Tables[0].Rows[0].IsNull(18)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(18),
                fechaOrdenAprehension = response.Data.Tables[0].Rows[0].IsNull(19)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(19)!,
            };
        }

        public async Task<ResponseImputadosEtapaInicial> GetImputadoByIdDisconnectedEtapaInicial(
            int id
        )
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputadosEtapaInicialDisconnected,
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
            return new()
            {
                Id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(3),
                nombre = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(5)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(5),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(6),
                concluyeInvestigacion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(7),
                idTerminacionInvestigacion =
                response.Data.Tables[0].Rows[0].IsNull(8)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(8),
                fechaTerminacionInvestigacion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9)!,
                solucionAlterna = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(10),
                idSolucionAlterna =
                response.Data.Tables[0].Rows[0].IsNull(11)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<int?>(11),
                fechaSolicitudAudienciaInicial = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(12)!,
                idCentroJusiticia =
                response.Data.Tables[0].Rows[0].IsNull(13)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<int?>(13),
                causaPenal = response.Data.Tables[0].Rows[0].IsNull(14)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(14)!,
                fechaAudienciaInicial = response.Data.Tables[0].Rows[0].IsNull(15)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(15)!,
                autoVinculacionProceso = response.Data.Tables[0].Rows[0].IsNull(16)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(16),
                fechaAutoVinculacion = response.Data.Tables[0].Rows[0].IsNull(17)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(17)!,
                ordenAprehension = response.Data.Tables[0].Rows[0].IsNull(18)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(18),
                fechaOrdenAprehension = response.Data.Tables[0].Rows[0].IsNull(19)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(19)!,
            };
        }

        public async Task<ResponseImputadosEtapaComplementaria> GetImputadoByIdDisconnectedEtapaComplementaria(
            int id
        )
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputadosEtapaComplementariaDisconnected,
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
            return new()
            {
                Id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(3),
                nombre = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(5)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(5),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(6),
                procedimientoAbreviadoCp = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(7),
                solucionAlternaCp = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(8),
                idTipoSolucionAlternaCp = response.Data.Tables[0].Rows[0].IsNull(9)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(9),
                escritoAcusacionCp = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(10),
                fechaEscritoAcusacionCp = response.Data.Tables[0].Rows[0].IsNull(11)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(11)!.ToString("dd/MM/yyyy"),
                fechaPlazoInvestigacionCp = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(12)!.ToString("dd/MM/yyyy"),
            };
        }

        public async Task<ResponseImputadosEtapaIntermedia> GetImputadoByIdDisconnectedEtapaIntermedia(
            int id
        )
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputadosEtapaIntermerdiaDisconnected,
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
            return new()
            {
                Id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(3),
                nombre = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(5)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(5),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(6),
                fechaAudienciaIntermedia = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("dd/MM/yyyy"),
                autoAperturaJuicioOral = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(8)!,
                fechaAperturaJuicioOral = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(9).ToString("dd/MM/yyyy"),
                procedimientoAbreviadoIn = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(10)!,
                solucionAlternaIn = response.Data.Tables[0].Rows[0].IsNull(11)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(11),
                idTipoSolucionAlternaIn =
                response.Data.Tables[0].Rows[0].IsNull(12)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<int?>(12),
            };
        }

        public async Task<ResponseImputadosEtapaJuicio> GetImputadoByIdDisconnectedEtapaJuicio(
            int id
        )
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesByIdImputadosEtapaJuicioDisconnected,
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
            return new()
            {

                Id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idAsuntoPenal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                rfc = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                contribuyente = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<bool>(3)!,
                nombre = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                idEstadoProcesal =
                response.Data.Tables[0].Rows[0].IsNull(5)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(5),
                idEstadoTarea =
                response.Data.Tables[0].Rows[0].IsNull(6)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int?>(6),
                fechaIncialAudienciaJuicio = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("dd/MM/yyyy"),
                fechaFinalAudienciaJuicio = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(8)!.ToString("dd/MM/yyyy"),
            };
        }

        public async Task<List<ResponseImputados>> GetImputadosByIdAsuntoPenalDisconnected(
            int idAsuntosPenales
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesImputadosDisconnected,
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

            List<ResponseImputados> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsuntoPenal = item.IsNull(1) ? 0 : item.Field<int>(1),
                        rfc = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        nombre = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        contribuyente = item.IsNull(4) ? false : item.Field<bool>(4),
                        estadoProcesal = item.IsNull(6) ? null! : item.Field<string>(6),
                        idEstadoProcesal = item.IsNull(5) ? 0 : item.Field<int>(5),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<AsuntosPenalesImputados>> GetImputados(int idAsuntosPenales)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesImputados,
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
            List<AsuntosPenalesImputados> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_asunto_penal = item.IsNull(1) ? 0 : item.Field<int>(1),
                        rfc = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        contribuyente = item.IsNull(3) ? false : item.Field<bool>(3),
                        nombre = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        id_estado_procesal = item.IsNull(5) ? 0 : item.Field<int>(5),
                        id_estado_tarea = item.IsNull(6) ? 0 : item.Field<int>(6),
                        concluye_investigacion = item.IsNull(7) ? null! : item.Field<bool>(7),
                        id_terminacion_investigacion = item.IsNull(8) ? null : item.Field<int>(8),
                        fecha_terminacion_investigacion = item.IsNull(9)
                            ? null!
                            : item.Field<DateTime>(9),
                        solucion_alterna = item.IsNull(10) ? null! : item.Field<bool>(10),
                        id_tipo_solucion_alterna = item.IsNull(11) ? null! : item.Field<int>(11),
                        fecha_solicitud_audiencia_inicial = item.IsNull(12)
                            ? null!
                            : item.Field<DateTime>(12),
                        id_centro_justicia = item.IsNull(13) ? null! : item.Field<int>(13),
                        causa_penal = item.IsNull(14) ? null! : item.Field<string>(14),
                        fecha_audiencia_inicial = item.IsNull(15)
                            ? null!
                            : item.Field<DateTime>(15),
                        auto_vinculacion_proceso = item.IsNull(16) ? null! : item.Field<bool>(16),
                        fecha_auto_vinculacion = item.IsNull(17) ? null! : item.Field<DateTime>(17),
                        orden_aprehension = item.IsNull(18) ? null! : item.Field<bool>(18),
                        fecha_orden_aprehension = item.IsNull(19)
                            ? null!
                            : item.Field<DateTime>(19),
                        fecha_plazo_investigacion_complementaria = item.IsNull(20)
                            ? null!
                            : item.Field<DateTime>(20),
                        procedimiento_abreviado_cp = item.IsNull(21) ? null! : item.Field<bool>(21),
                        solucion_alterna_cp = item.IsNull(22) ? null! : item.Field<bool>(22),
                        id_tipo_solucion_alterna_cp = item.IsNull(23) ? null! : item.Field<int>(23),
                        escrito_acusacion_cp = item.IsNull(24) ? null! : item.Field<bool>(24),
                        fecha_escrito_acusacion_cp = item.IsNull(25)
                            ? null!
                            : item.Field<DateTime>(25),
                        fecha_audiencia_intermedia = item.IsNull(26)
                            ? null!
                            : item.Field<DateTime>(26),
                        auto_apertura_juicio_oral = item.IsNull(27) ? null! : item.Field<bool>(27),
                        fecha_apertura_juicio_oral = item.IsNull(28)
                            ? null!
                            : item.Field<DateTime>(28),
                        procedimiento_abreviado_in = item.IsNull(29) ? null! : item.Field<bool>(29),
                        solucion_alterna_in = item.IsNull(30) ? null! : item.Field<bool>(30),
                        id_tipo_solucion_alterna_in = item.IsNull(31) ? null! : item.Field<int>(31),
                        fecha_incial_audiencia_juicio = item.IsNull(32)
                            ? null!
                            : item.Field<DateTime>(32),
                        fecha_final_audiencia_juicio = item.IsNull(33)
                            ? null!
                            : item.Field<DateTime>(33),
                        fecha_creacion = item.IsNull(34) ? new() : item.Field<DateTime>(34),
                        usuario_creacion = item.IsNull(35) ? null! : item.Field<string>(35)!,
                        activo = item.IsNull(36) ? false : item.Field<bool>(36),
                        fecha_modificacion = item.IsNull(37) ? null! : item.Field<DateTime>(37),
                        usuario_modificacion = item.IsNull(38) ? null! : item.Field<string>(38),
                    }
                );
            }

            return resultList;
        }

        // public async Task<ResultTransaction> UpdateImputadosAsync(
        //         AsuntosPenalesImputados entityImputados, AsuntosPenales entity
        //     )
        //     {
        //         ParameterPGsql[] parameters =
        //         {
        //             new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
        //             new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
        //             new ParameterPGsql("p_contribuyente",NpgsqlDbType.Boolean,entityImputados.contribuyente!),
        //             new ParameterPGsql("p_rfc", NpgsqlDbType.Text, entityImputados.rfc!),
        //             new ParameterPGsql("p_nombre", NpgsqlDbType.Text, entityImputados.nombre!),
        //             new ParameterPGsql("p_id_estado_procesal",NpgsqlDbType.Integer, entityImputados.id_estado_procesal!),
        //             new ParameterPGsql("p_id_estado_tarea",NpgsqlDbType.Integer, entityImputados.id_estado_tarea!),
        //             new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityImputados.usuario_modificacion! ),

        //         };
        //         var response = await _database.ExecuteFunctionAsync(
        //             EnumFunctions.UPDATE_IMPUTADOS,
        //             parameters!
        //         );
        //         if (response.ExisteError)
        //         {
        //             return new()
        //             {
        //                 Success = false,
        //                 MsgError = response.Mensaje,
        //                 NoError = response.CodeSqlError,
        //             };
        //         }

        //         if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
        //         {
        //             return new()
        //             {
        //                 Success = false,
        //                 MsgError = "No se pudo obtener la respuesta de la operación en base de datos.",
        //             };
        //         }

        //         return new()
        //         {
        //             Result = response.Data.Tables[0].Rows[0].Field<int?>(0),
        //             Success = response.Data.Tables[0].Rows[0].Field<bool>(1),
        //             MsgError = response.Data.Tables[0].Rows[0].Field<string?>(2)!,
        //             DetailError = response.Data.Tables[0].Rows[0].Field<string?>(3)!,
        //             NoError = response.Data.Tables[0].Rows[0].Field<string?>(4)!,
        //         };
        //     }


        public async Task<ResultTransaction> UpdateImputadosEtapaInicialAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_concluye_investigacion",
                    NpgsqlDbType.Boolean,
                    entityImputados.concluye_investigacion!
                ),
                new ParameterPGsql(
                    "p_id_terminacion_investigacion",
                    NpgsqlDbType.Integer,
                    entityImputados.id_terminacion_investigacion!
                ),
                new ParameterPGsql(
                    "p_fecha_terminacion_investigacion",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_terminacion_investigacion!
                ),
                new ParameterPGsql(
                    "p_solucion_alterna",
                    NpgsqlDbType.Boolean,
                    entityImputados.solucion_alterna!
                ),
                new ParameterPGsql(
                    "p_id_tipo_solucion_alterna",
                    NpgsqlDbType.Integer,
                    entityImputados.id_tipo_solucion_alterna!
                ),
                new ParameterPGsql(
                    "p_acuerdo_reparatorio",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is not null
                ),
                new ParameterPGsql(
                    "p_id_acuerdo_reparatorio",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null ? DBNull.Value : entityAcuerdoReparatorio.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_acuerdo",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_condiciones_acuerdo",
                    NpgsqlDbType.Text,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.condiciones!
                ),
                new ParameterPGsql(
                    "p_fecha_autorizacion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_autorizacion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_reparacion_daño_acuerdo",
                    NpgsqlDbType.Numeric,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.reparacion_daño!
                ),
                new ParameterPGsql(
                    "p_fecha_celebracion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_celebracion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_acuerdo",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.conclusion_asunto!
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_conclusion!
                ),
                new ParameterPGsql(
                    "p_criterio_oportunidad",
                    NpgsqlDbType.Boolean,
                    entityCriterioOportunidad is not null
                ),
                new ParameterPGsql(
                    "p_id_criterio_oportunidad",
                    NpgsqlDbType.Integer,
                    entityCriterioOportunidad is null ? DBNull.Value : entityCriterioOportunidad.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_criterio",
                    NpgsqlDbType.Integer,
                    entityCriterioOportunidad is null
                        ? DBNull.Value
                        : entityCriterioOportunidad.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_condiciones_criterio",
                    NpgsqlDbType.Text,
                    entityCriterioOportunidad is null
                        ? DBNull.Value
                        : entityCriterioOportunidad.condiciones!
                ),
                new ParameterPGsql(
                    "p_fecha_criterio",
                    NpgsqlDbType.Date,
                    entityCriterioOportunidad is null
                        ? DBNull.Value
                        : entityCriterioOportunidad.fecha_criterio_oportunidad!
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_criterio",
                    NpgsqlDbType.Boolean,
                    entityCriterioOportunidad is null
                        ? DBNull.Value
                        : entityCriterioOportunidad.conclusion_asunto!
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_criterio",
                    NpgsqlDbType.Date,
                    entityCriterioOportunidad is null
                        ? DBNull.Value
                        : entityCriterioOportunidad.fecha_conclusion!
                ),
                new ParameterPGsql(
                    "p_fecha_solicitud_audiencia_inicial",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_solicitud_audiencia_inicial!
                ),
                new ParameterPGsql(
                    "p_id_centro_justicia",
                    NpgsqlDbType.Integer,
                    entityImputados.id_centro_justicia!
                ),
                new ParameterPGsql(
                    "p_causa_penal",
                    NpgsqlDbType.Text,
                    entityImputados.causa_penal!
                ),
                new ParameterPGsql(
                    "p_fecha_audiencia_inicial",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_audiencia_inicial!
                ),
                new ParameterPGsql(
                    "p_auto_vinculacion_proceso",
                    NpgsqlDbType.Boolean,
                    entityImputados.auto_vinculacion_proceso!
                ),
                new ParameterPGsql(
                    "p_fecha_auto_vinculacion",
                    NpgsqlDbType.Date,
                    entityImputados!.fecha_auto_vinculacion
                ),
                new ParameterPGsql(
                    "p_orden_aprehension",
                    NpgsqlDbType.Boolean,
                    entityImputados.orden_aprehension!
                ),
                new ParameterPGsql(
                    "p_fecha_orden_aprehension",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_orden_aprehension!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal_ap",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_modificacion!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_ETAPA_INICIAL,
                parameters!
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

        public async Task<List<AsuntosPenalesAcuerdoReparatorio>> GetAcuerdoReparatorioByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoRepatarorio,
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
            List<AsuntosPenalesAcuerdoReparatorio> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_imputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        id_tipo_etapa_investigacion = item.IsNull(2) ? null : item.Field<int>(2),
                        condiciones = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        fecha_autorizacion_acuerdo_reparatorio = item.IsNull(4)
                            ? new()
                            : item.Field<DateTime>(4)!,
                        reparacion_daño = item.IsNull(5) ? null : item.Field<decimal>(5),
                        fecha_celebracion_acuerdo_reparatorio = item.IsNull(6)
                            ? new()
                            : item.Field<DateTime>(6),
                        conclusion_asunto = item.IsNull(7) ? false : item.Field<bool>(7),
                        fecha_conclusion = item.IsNull(8) ? null! : item.Field<DateTime>(8),
                        usuario_creacion = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        fecha_creacion = item.IsNull(10) ? new() : item.Field<DateTime>(10),
                        usuario_modificacion = item.IsNull(11) ? null! : item.Field<string>(11),
                        fecha_modificacion = item.IsNull(12) ? null! : item.Field<DateTime>(12),
                        activo = item.IsNull(13) ? false : item.Field<bool>(13),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<AsuntosPenalesCriterioOportunidad>> GetCriterioOportunidadByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenCriterioOportunidad,
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
            List<AsuntosPenalesCriterioOportunidad> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_imputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        condiciones = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        fecha_criterio_oportunidad = item.IsNull(3)
                            ? new()
                            : item.Field<DateTime>(3)!,
                        conclusion_asunto = item.IsNull(4) ? false : item.Field<bool>(4),
                        fecha_conclusion = item.IsNull(5) ? new()! : item.Field<DateTime>(5),
                        usuario_creacion = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        fecha_creacion = item.IsNull(7) ? new() : item.Field<DateTime>(7),
                        usuario_modificacion = item.IsNull(8) ? null! : item.Field<string>(8),
                        fecha_modificacion = item.IsNull(9) ? null! : item.Field<DateTime>(9),
                        activo = item.IsNull(10) ? false : item.Field<bool>(10),
                    }
                );
            }

            return resultList;
        }

        public async Task<
            List<ResponseAcuerdoReparatorio>
        > GetAcuerdoReparatorioByImputadoDisconnected(int idImputado, int idTipoEtapaInvestigacion)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoRepatarorioDisconnected,
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
            List<ResponseAcuerdoReparatorio> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        condicionesAcuerdoReparatorio = item.IsNull(2)
                            ? null!
                            : item.Field<string>(2)!,
                        fechaAutorizacionAcuerdoReparatorio = item.IsNull(3)
                            ? null!
                            : item.Field<DateTime>(3).ToString("dd/MM/yyyy")!,
                        reparacionDañoAcuerdoReparatorio = item.IsNull(4)
                            ? null!
                            : item.Field<decimal>(4),
                        fechaCelebracionAcuerdoReparatorio = item.IsNull(5)
                            ? null!
                            : item.Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                        conclusionAsuntoAcuerdoReparatorio = item.IsNull(6)
                            ? false
                            : item.Field<bool>(6),
                        fechaConclusionAcuerdoReparatorio = item.IsNull(7)
                            ? null!
                            : item.Field<DateTime>(7).ToString("dd/MM/yyyy")!,
                    }
                );
            }

            return resultList;
        }

        public async Task<
            List<ResponseCriterioOportunidad>
        > GetCriterioOportunidadByImputadoDisconnected(int idImputado, int idTipoEtapaInvestigacion)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenCriterioOportunidadDisconnected,
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
            List<ResponseCriterioOportunidad> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        condicionesCriterioOportunidad = item.IsNull(2)
                            ? null!
                            : item.Field<string>(2)!,
                        fechaCriterioOportunidad = item.IsNull(3)
                            ? null!
                            : item.Field<DateTime>(3).ToString("dd/MM/yyyy")!,
                        conclusionAsuntoCriterioOportunidad = item.IsNull(4)
                            ? false
                            : item.Field<bool>(4),
                        fechaConclusionCriterioOportunidad = item.IsNull(5)
                            ? null!
                            : item.Field<DateTime>(5).ToString("dd/MM/yyyy")!,
                    }
                );
            }

            return resultList;
        }

        public async Task<List<AsuntosPenalesSentencia>> GetSentenciaByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSentencia,
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
            List<AsuntosPenalesSentencia> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_imputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        id_tipo_sentencia = item.IsNull(2) ? null : item.Field<int>(2),
                        fecha_emision_sentencia = item.IsNull(3) ? new() : item.Field<DateTime>(3)!,
                        reparacion_daño = item.IsNull(4) ? null : item.Field<decimal>(4),
                        cumplimiento_privada_libertad = item.IsNull(5)
                            ? false
                            : item.Field<bool>(5),
                        id_anio = item.IsNull(6) ? null : item.Field<int>(6),
                        id_mes = item.IsNull(7) ? null : item.Field<int>(7),
                        id_dia = item.IsNull(8) ? null : item.Field<int>(8),
                        otorgamiento_beneficios = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        acciones_ejecucion = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        fecha_ejecucion = item.IsNull(11) ? new() : item.Field<DateTime>(11),
                        conclusion_asunto = item.IsNull(12) ? false : item.Field<bool>(12),
                        fecha_conclusion = item.IsNull(13) ? null! : item.Field<DateTime>(13),
                        fecha_creacion = item.IsNull(14) ? new() : item.Field<DateTime>(14),
                        usuario_creacion = item.IsNull(15) ? null! : item.Field<string>(15)!,
                        activo = item.IsNull(16) ? false : item.Field<bool>(16),
                        usuario_modificacion = item.IsNull(17) ? null! : item.Field<string>(17),
                        fecha_modificacion = item.IsNull(18) ? null! : item.Field<DateTime>(18),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<ResponseSentencia>> GetSentenciaByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSentenciaDisconnected,
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
            List<ResponseSentencia> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        idTipoSentencia = response.Data.Tables[0].Rows[0].IsNull(2) ? null
                                : response.Data.Tables[0].Rows[0].Field<int?>(2),
                        fechaEmisionSentencia = item.IsNull(3)
                            ? null!
                            : item.Field<DateTime>(3)!.ToString("dd/MM/yyyy"),
                        reparacionDanioSentencia = item.IsNull(4) ? 0 : item.Field<decimal>(4),
                        cumplimientoPrivadaLibertad = item.IsNull(5) ? false : item.Field<bool>(5),
                        anios = item.IsNull(6) ? null : item.Field<int>(6),
                        meses = item.IsNull(7) ? null : item.Field<int>(7),
                        dias = item.IsNull(8) ? null : item.Field<int>(8),
                        otorgamientoBeneficios = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        accionesEjecucion = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        fechaEjecucion = item.IsNull(11)
                            ? null!
                            : item.Field<DateTime>(11).ToString("dd/MM/yyyy"),
                        conclusionAsuntoSentencia = item.IsNull(12) ? false : item.Field<bool>(12),
                        fechaConclusionSentencia = item.IsNull(13)
                            ? null!
                            : item.Field<DateTime>(13).ToString("dd/MM/yyyy"),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<AsuntosPenalesSobreseimiento>> GetSobreseimientoByImputado(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSobreseimiento,
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
            List<AsuntosPenalesSobreseimiento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_imputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        solicitud_sobreseimiento = item.IsNull(2) ? false : item.Field<bool>(2),
                        fecha_determinacion_sobreseimiento = item.IsNull(3)
                            ? new()
                            : item.Field<DateTime>(3)!,
                        conclusion_asunto = item.IsNull(4) ? false : item.Field<bool>(4),
                        fecha_conclusion = item.IsNull(5) ? null! : item.Field<DateTime>(5),
                        fecha_creacion = item.IsNull(6) ? new() : item.Field<DateTime>(6),
                        usuario_creacion = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        activo = item.IsNull(8) ? false : item.Field<bool>(8),
                        usuario_modificacion = item.IsNull(9) ? null! : item.Field<string>(9),
                        fecha_modificacion = item.IsNull(10) ? null! : item.Field<DateTime>(10),
                    }
                );
            }

            return resultList;
        }

        public async Task<List<ResponseSobreseimiento>> GetSobreseimientoByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSobreseimientoDisconnected,
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
            List<ResponseSobreseimiento> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        solicitudSobreseimiento = item.IsNull(2) ? false : item.Field<bool>(2),
                        fechaDeterminacionSobreseimiento = item.IsNull(3)
                            ? null!
                            : item.Field<DateTime>(3)!.ToString("dd/MM/yyyy"),
                        conclusionAsuntoSobreseimiento = item.IsNull(4)
                            ? false
                            : item.Field<bool>(4),
                        fechaConclusionSobreseimiento = item.IsNull(5)
                            ? null!
                            : item.Field<DateTime>(5).ToString("dd/MM/yyyy"),
                    }
                );
            }

            return resultList;
        }

        public async Task<
            List<AsuntosPenalesSuspensionCondicional>
        > GetSuspencionCondicionalByImputado(int idImputado, int idTipoEtapaInvestigacion)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSuspensionCondicional,
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
            List<AsuntosPenalesSuspensionCondicional> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_imputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        condiciones = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        fecha_celebracion = item.IsNull(3) ? new() : item.Field<DateTime>(3)!,
                        reparacion_daño = item.IsNull(4) ? null : item.Field<decimal>(4),
                        fecha_plazo = item.IsNull(5) ? new() : item.Field<DateTime>(5),
                        fecha_cumplimiento = item.IsNull(6) ? new() : item.Field<DateTime>(6),
                        conclusion_asunto = item.IsNull(7) ? false : item.Field<bool>(7),
                        fecha_conclusion = item.IsNull(8) ? null! : item.Field<DateTime>(8),
                        fecha_creacion = item.IsNull(9) ? new() : item.Field<DateTime>(9),
                        usuario_creacion = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        activo = item.IsNull(11) ? false : item.Field<bool>(11),
                        usuario_modificacion = item.IsNull(12) ? null! : item.Field<string>(12),
                        fecha_modificacion = item.IsNull(13) ? null! : item.Field<DateTime>(13),
                    }
                );
            }
            return resultList;
        }

        public async Task<
            List<ResponseSuspencionCondicional>
        > GetSuspencionCondicionalByImputadoDisconnected(
            int idImputado,
            int idTipoEtapaInvestigacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion",
                    NpgsqlDbType.Integer,
                    idTipoEtapaInvestigacion
                ),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAcuerdoSuspensionCondicionalDisconnected,
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
            List<ResponseSuspencionCondicional> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        condicionesSuspencion = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        fechaCelebracionSuspencion = item.IsNull(3)
                            ? null!
                            : item.Field<DateTime>(3)!.ToString("dd/MM/yyyy"),
                        reparacionDañoSuspencion = item.IsNull(4) ? null : item.Field<decimal>(4),
                        fechaPlazoSuspencion = item.IsNull(5)
                            ? null!
                            : item.Field<DateTime>(5).ToString("dd/MM/yyyy"),
                        fechaCumplimientoSuspencion = item.IsNull(6)
                            ? null!
                            : item.Field<DateTime>(6).ToString("dd/MM/yyyy"),
                        conclusionAsuntoSuspencion = item.IsNull(7) ? false : item.Field<bool>(7),
                        fechaConclusionSuspencion = item.IsNull(8)
                            ? null!
                            : item.Field<DateTime>(8).ToString("dd/MM/yyyy"),
                    }
                );
            }
            return resultList;
        }

        #endregion

        public async Task<ResultTransaction> UpdateImputadosEtapaComplementariaFechaPlazoInvestigacionAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql(
                    "p_fecha_plazo_investigacion_complementaria",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_plazo_investigacion_complementaria!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_modificacion
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.Update_fecha_plazo_investigacion_complementaria,
                parameters!
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

        public async Task<ResultTransaction> UpdateImputadosEtapaComplementariaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitysuspension,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_fecha_plazo_investigacion_complementaria",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_plazo_investigacion_complementaria!
                ),
                new ParameterPGsql(
                    "p_procedimiento_abreviado",
                    NpgsqlDbType.Boolean,
                    entityImputados.procedimiento_abreviado_cp!
                ),
                new ParameterPGsql(
                    "p_solucion_alterna_cp",
                    NpgsqlDbType.Boolean,
                    entityImputados.solucion_alterna_cp!
                ),
                new ParameterPGsql(
                    "p_id_tipo_solucion_alterna_cp",
                    NpgsqlDbType.Integer,
                    entityImputados.id_tipo_solucion_alterna_cp!
                ),
                new ParameterPGsql(
                    "p_escrito_acusacion_cp",
                    NpgsqlDbType.Boolean,
                    entityImputados.escrito_acusacion_cp!
                ),
                new ParameterPGsql(
                    "p_fecha_escrito_acusacion_cp",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_escrito_acusacion_cp!
                ),
                new ParameterPGsql(
                    "p_sentencia_value",
                    NpgsqlDbType.Boolean,
                    entitySentencia is not null
                ),
                new ParameterPGsql(
                    "p_id_tabla_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_tipo_sentencia!
                ),
                new ParameterPGsql(
                    "p_fecha_emision_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_emision_sentencia
                ),
                new ParameterPGsql(
                    "p_reparacion_danio",
                    NpgsqlDbType.Numeric,
                    entitySentencia is null ? DBNull.Value : entitySentencia.reparacion_daño
                ),
                new ParameterPGsql(
                    "p_cumplimiento_privada_libertad",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.cumplimiento_privada_libertad
                ),
                new ParameterPGsql(
                    "p_id_anios",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_anio
                ),
                new ParameterPGsql(
                    "p_id_meses",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_mes
                ),
                new ParameterPGsql(
                    "p_id_dias",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_dia
                ),
                new ParameterPGsql(
                    "p_otorgamiento_beneficios",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.otorgamiento_beneficios
                ),
                new ParameterPGsql(
                    "p_acciones_ejecucion",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.acciones_ejecucion
                ),
                new ParameterPGsql(
                    "p_fecha_ejecucion",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_ejecucion
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_sentencia",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null ? DBNull.Value : entitySentencia.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_acuerdo_reparatorio",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is not null
                ),
                new ParameterPGsql(
                    "p_id_acuerdo_reparatorio",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null ? DBNull.Value : entityAcuerdoReparatorio.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_reparatorio",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_condiciones_acuerdo",
                    NpgsqlDbType.Text,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.condiciones!
                ),
                new ParameterPGsql(
                    "p_fecha_autorizacion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_autorizacion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_reparacion_danio_acuerdo",
                    NpgsqlDbType.Numeric,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.reparacion_daño!
                ),
                new ParameterPGsql(
                    "p_fecha_celebracion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_celebracion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_acuerdo",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.conclusion_asunto!
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_conclusion!
                ),
                new ParameterPGsql(
                    "p_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is not null
                ),
                new ParameterPGsql(
                    "p_id_sobreseimiento",
                    NpgsqlDbType.Integer,
                    entitySobreseimiento is null ? DBNull.Value : entitySobreseimiento.id
                ),
                new ParameterPGsql(
                    "p_solicitud_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.solicitud_sobreseimiento
                ),
                new ParameterPGsql(
                    "p_fecha_determinacion_sobreseimiento",
                    NpgsqlDbType.Date,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.fecha_determinacion_sobreseimiento
                ),
                new ParameterPGsql(
                    "p_conlusion_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_sobreseimiento",
                    NpgsqlDbType.Date,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_sobreseimiento",
                    NpgsqlDbType.Integer,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_suspension",
                    NpgsqlDbType.Boolean,
                    entitysuspension is not null
                ),
                new ParameterPGsql(
                    "p_id_suspension",
                    NpgsqlDbType.Integer,
                    entitysuspension is null ? DBNull.Value : entitysuspension.id
                ),
                new ParameterPGsql(
                    "p_condiciones_suspension",
                    NpgsqlDbType.Text,
                    entitysuspension is null ? DBNull.Value : entitysuspension.condiciones
                ),
                new ParameterPGsql(
                    "p_fecha_celebracion_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_celebracion
                ),
                new ParameterPGsql(
                    "p_reparacion_danio_suspension",
                    NpgsqlDbType.Numeric,
                    entitysuspension is null ? DBNull.Value : entitysuspension.reparacion_daño
                ),
                new ParameterPGsql(
                    "p_plazo",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_plazo
                ),
                new ParameterPGsql(
                    "p_fecha_cumplimiento_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_cumplimiento
                ),
                new ParameterPGsql(
                    "p_conclusion_suspension",
                    NpgsqlDbType.Boolean,
                    entitysuspension is null ? DBNull.Value : entitysuspension.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_suspencion",
                    NpgsqlDbType.Integer,
                    entitysuspension is null
                        ? DBNull.Value
                        : entitysuspension.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal_ap",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_modificacion!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_ETAPA_COMPLEMENTARIA,
                parameters!
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

        public async Task<ResultTransaction> UpdateImputadosEtapaIntermediaAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitysuspension,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_fecha_audiencia_intermedia",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_audiencia_intermedia!
                ),
                new ParameterPGsql(
                    "p_auto_apertura_juicio_oral",
                    NpgsqlDbType.Boolean,
                    entityImputados.auto_apertura_juicio_oral!
                ),
                new ParameterPGsql(
                    "p_solucion_alterna_in",
                    NpgsqlDbType.Boolean,
                    entityImputados.solucion_alterna_in!
                ),
                new ParameterPGsql(
                    "p_id_tipo_solucion_alterna_in",
                    NpgsqlDbType.Integer,
                    entityImputados.id_tipo_solucion_alterna_in!
                ),
                new ParameterPGsql(
                    "p_procedimiento_abreviado_in",
                    NpgsqlDbType.Boolean,
                    entityImputados.procedimiento_abreviado_in!
                ),
                new ParameterPGsql(
                    "p_fecha_apertura_juicio_oral",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_apertura_juicio_oral!
                ),
                new ParameterPGsql(
                    "p_sentencia_value",
                    NpgsqlDbType.Boolean,
                    entitySentencia is not null
                ),
                new ParameterPGsql(
                    "p_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_tipo_sentencia!
                ),
                new ParameterPGsql(
                    "p_id_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id!
                ),
                new ParameterPGsql(
                    "p_fecha_emision_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_emision_sentencia
                ),
                new ParameterPGsql(
                    "p_reparacion_danio",
                    NpgsqlDbType.Numeric,
                    entitySentencia is null ? DBNull.Value : entitySentencia.reparacion_daño
                ),
                new ParameterPGsql(
                    "p_cumplimiento_privada_libertad",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.cumplimiento_privada_libertad
                ),
                new ParameterPGsql(
                    "p_id_anios",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_anio
                ),
                new ParameterPGsql(
                    "p_id_meses",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_mes
                ),
                new ParameterPGsql(
                    "p_id_dias",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_dia
                ),
                new ParameterPGsql(
                    "p_otorgamiento_beneficios",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.otorgamiento_beneficios
                ),
                new ParameterPGsql(
                    "p_acciones_ejecucion",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.acciones_ejecucion
                ),
                new ParameterPGsql(
                    "p_fecha_ejecucion",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_ejecucion
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_sentencia",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null ? DBNull.Value : entitySentencia.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_acuerdo_reparatorio",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is not null
                ),
                new ParameterPGsql(
                    "p_id_acuerdo_reparatorio",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null ? DBNull.Value : entityAcuerdoReparatorio.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_reparatorio",
                    NpgsqlDbType.Integer,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_condiciones_acuerdo",
                    NpgsqlDbType.Text,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.condiciones!
                ),
                new ParameterPGsql(
                    "p_fecha_autorizacion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_autorizacion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_reparacion_danio_acuerdo",
                    NpgsqlDbType.Numeric,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.reparacion_daño!
                ),
                new ParameterPGsql(
                    "p_fecha_celebracion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_celebracion_acuerdo_reparatorio!
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_acuerdo",
                    NpgsqlDbType.Boolean,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.conclusion_asunto!
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_acuerdo",
                    NpgsqlDbType.Date,
                    entityAcuerdoReparatorio is null
                        ? DBNull.Value
                        : entityAcuerdoReparatorio.fecha_conclusion!
                ),
                new ParameterPGsql(
                    "p_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is not null
                ),
                new ParameterPGsql(
                    "p_id_sobreseimiento",
                    NpgsqlDbType.Integer,
                    entitySobreseimiento is null ? DBNull.Value : entitySobreseimiento.id
                ),
                new ParameterPGsql(
                    "p_solicitud_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.solicitud_sobreseimiento
                ),
                new ParameterPGsql(
                    "p_fecha_determinacion_sobreseimiento",
                    NpgsqlDbType.Date,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.fecha_determinacion_sobreseimiento
                ),
                new ParameterPGsql(
                    "p_conlusion_sobreseimiento",
                    NpgsqlDbType.Boolean,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_sobreseimiento",
                    NpgsqlDbType.Date,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_sobreseimiento",
                    NpgsqlDbType.Integer,
                    entitySobreseimiento is null
                        ? DBNull.Value
                        : entitySobreseimiento.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_suspension",
                    NpgsqlDbType.Boolean,
                    entitysuspension is not null
                ),
                new ParameterPGsql(
                    "p_id_suspension",
                    NpgsqlDbType.Integer,
                    entitysuspension is null ? DBNull.Value : entitysuspension.id
                ),
                new ParameterPGsql(
                    "p_condiciones_suspension",
                    NpgsqlDbType.Text,
                    entitysuspension is null ? DBNull.Value : entitysuspension.condiciones
                ),
                new ParameterPGsql(
                    "p_fecha_celebracion_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_celebracion
                ),
                new ParameterPGsql(
                    "p_reparacion_danio_suspension",
                    NpgsqlDbType.Numeric,
                    entitysuspension is null ? DBNull.Value : entitysuspension.reparacion_daño
                ),
                new ParameterPGsql(
                    "p_plazo",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_plazo
                ),
                new ParameterPGsql(
                    "p_fecha_cumplimiento_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_cumplimiento
                ),
                new ParameterPGsql(
                    "p_conclusion_suspension",
                    NpgsqlDbType.Boolean,
                    entitysuspension is null ? DBNull.Value : entitysuspension.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_suspension",
                    NpgsqlDbType.Date,
                    entitysuspension is null ? DBNull.Value : entitysuspension.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_suspencion",
                    NpgsqlDbType.Integer,
                    entitysuspension is null
                        ? DBNull.Value
                        : entitysuspension.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal_ap",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_modificacion!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_ETAPA_INTERMEDIA,
                parameters!
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

        public async Task<ResultTransaction> ApelacionImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesApelacion entityApelacion,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asuntos_penales", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityApelacion.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_numero_toca_penal",
                    NpgsqlDbType.Text,
                    entityApelacion.numero_toca_penal!
                ),
                new ParameterPGsql(
                    "p_tipo_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityApelacion.tipo_organo_jurisdiccional!
                ),
                new ParameterPGsql(
                    "p_id_tipo_resolucion",
                    NpgsqlDbType.Integer,
                    entityApelacion.id_tipo_resolucion!
                ),
                new ParameterPGsql(
                    "p_fecha_resolucion",
                    NpgsqlDbType.Date,
                    entityApelacion.fecha_resolucion!
                ),
                new ParameterPGsql(
                    "p_descripcion_resolucion",
                    NpgsqlDbType.Text,
                    entityApelacion.descripcion_resolucion!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityApelacion.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_IMPUTADOS_APELACION,
                parameters!
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

        public async Task<ResultTransaction> ApelacionImputadosUpdateAsync(
            AsuntosPenalesApelacion entityApelacion
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityApelacion.id),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityApelacion.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_numero_toca_penal",
                    NpgsqlDbType.Text,
                    entityApelacion.numero_toca_penal!
                ),
                new ParameterPGsql(
                    "p_tipo_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityApelacion.tipo_organo_jurisdiccional!
                ),
                new ParameterPGsql(
                    "p_id_tipo_resolucion",
                    NpgsqlDbType.Integer,
                    entityApelacion.id_tipo_resolucion!
                ),
                new ParameterPGsql(
                    "p_fecha_resolucion",
                    NpgsqlDbType.Date,
                    entityApelacion.fecha_resolucion!
                ),
                new ParameterPGsql(
                    "p_descripcion_resolucion",
                    NpgsqlDbType.Text,
                    entityApelacion.descripcion_resolucion!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityApelacion.usuario_modificacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_APELACION,
                parameters!
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

        public async Task<ResultTransaction> UpdateImputadosEtapaJuicioAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            AsuntosPenalesSobreseimiento entitySobreseimiento,
            AsuntosPenalesSuspensionCondicional entitysuspension,
            AsuntosPenalesSentencia entitySentencia,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql(
                    "p_fecha_incial_audiencia_juicio",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_inicial_audiencia_juicio!
                ),
                new ParameterPGsql(
                    "p_fecha_final_audiencia_juicio",
                    NpgsqlDbType.Date,
                    entityImputados.fecha_final_audiencia_juicio!
                ),
                new ParameterPGsql(
                    "p_sentencia_value",
                    NpgsqlDbType.Boolean,
                    entitySentencia is not null
                ),
                new ParameterPGsql(
                    "p_id_tabla_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id!
                ),
                new ParameterPGsql(
                    "p_id_tipo_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_tipo_sentencia!
                ),
                new ParameterPGsql(
                    "p_fecha_emision_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_emision_sentencia
                ),
                new ParameterPGsql(
                    "p_reparacion_danio",
                    NpgsqlDbType.Numeric,
                    entitySentencia is null ? DBNull.Value : entitySentencia.reparacion_daño
                ),
                new ParameterPGsql(
                    "p_cumplimiento_privada_libertad",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.cumplimiento_privada_libertad
                ),
                new ParameterPGsql(
                    "p_id_anios",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_anio
                ),
                new ParameterPGsql(
                    "p_id_meses",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_mes
                ),
                new ParameterPGsql(
                    "p_id_dias",
                    NpgsqlDbType.Integer,
                    entitySentencia is null ? DBNull.Value : entitySentencia.id_dia
                ),
                new ParameterPGsql(
                    "p_otorgamiento_beneficios",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.otorgamiento_beneficios
                ),
                new ParameterPGsql(
                    "p_acciones_ejecucion",
                    NpgsqlDbType.Text,
                    entitySentencia is null ? DBNull.Value : entitySentencia.acciones_ejecucion
                ),
                new ParameterPGsql(
                    "p_fecha_ejecucion",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_ejecucion
                ),
                new ParameterPGsql(
                    "p_conclusion_asunto_sentencia",
                    NpgsqlDbType.Boolean,
                    entitySentencia is null ? DBNull.Value : entitySentencia.conclusion_asunto
                ),
                new ParameterPGsql(
                    "p_fecha_conclusion_sentencia",
                    NpgsqlDbType.Date,
                    entitySentencia is null ? DBNull.Value : entitySentencia.fecha_conclusion
                ),
                new ParameterPGsql(
                    "p_id_tipo_etapa_investigacion_sentencia",
                    NpgsqlDbType.Integer,
                    entitySentencia is null
                        ? DBNull.Value
                        : entitySentencia.id_tipo_etapa_investigacion!
                ),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    entity.id_estado_tarea!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal",
                    NpgsqlDbType.Integer,
                    entityImputados.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_id_estado_procesal_ap",
                    NpgsqlDbType.Integer,
                    entity.id_estado_procesal!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityImputados.usuario_modificacion!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_ETAPA_JUICIO,
                parameters!
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

        public async Task<List<ResponseApelacion>> GetApelacionByImputadoDisconnected(
            int idImputado
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenApelacionlDisconnected,
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
            List<ResponseApelacion> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        fechaPresentacion = item.IsNull(2)
                            ? null!
                            : item.Field<DateTime>(2).ToString("dd/MM/yyyy")!,
                        numeroTocaPenal = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        tipoOrganoJurisdiccional = item.IsNull(4) ? null! : item.Field<string>(4),
                        idTipoResolucion = item.IsNull(5) ? null : item.Field<int>(5),
                        fechaResolucion = item.IsNull(6)
                            ? null!
                            : item.Field<DateTime>(6).ToString("dd/MM/yyyy"),
                        descripcion_resolucion = item.IsNull(7) ? null! : item.Field<string>(7),
                        estadoProcesal = item.IsNull(9) ? null! : item.Field<string>(9),
                        idEstadoProcesal = item.IsNull(8) ? null : item.Field<int>(8),

                    }
                );
            }
            return resultList;
        }

        public async Task<ResponseApelacion> GetApelacionByImputadoDisconnectedById(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenApelacionlDisconnectedById,
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
            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(1)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int>(1),
                idImputado = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(2),
                fechaPresentacion = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(3).ToString("dd/MM/yyyy")!,
                numeroTocaPenal = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                tipoOrganoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                idTipoResolucion = response.Data.Tables[0].Rows[0].IsNull(6)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int>(6),
                fechaResolucion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(7).ToString("dd/MM/yyyy"),
                descripcion_resolucion = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(8),
            };
        }

        public async Task<AsuntosPenalesApelacion> GetApelacionByImputadoById(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenApelacionlById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_imputado = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                fecha_presentacion = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? new()!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(2),
                numero_toca_penal = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(3)!,
                tipo_organo_jurisdiccional = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(4),
                id_tipo_resolucion = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(5),
                fecha_resolucion = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(6),
                descripcion_resolucion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(7),
                activo = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(8),
            };
        }


        public async Task<ResultTransaction> DeleteApelacion(
            AsuntosPenalesApelacion entity
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entity.id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_DELETE_APELACION,
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

        public async Task<ResultTransaction> AmparoImputadosAsync(
            AsuntosPenales entity,
            AsuntosPenalesImputados entityImputados,
            AsuntosPenalesAmparo entityAmparo,
            ArchivosAsuntosPenales entityDocumento,
            DataFile? dataFile
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asuntos_penales", NpgsqlDbType.Integer, entity.id),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entityImputados.id),
                new ParameterPGsql("p_id_tipo_amparo", NpgsqlDbType.Integer, entityAmparo.id_tipo_amparo),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entityAmparo.id_estado_procesal),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_numero_juicio",
                    NpgsqlDbType.Varchar,
                    entityAmparo.numeroJuicioAmparo!
                ),
                new ParameterPGsql(
                    "p_tipo_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityAmparo.tipo_organo_jurisdiccional!
                ),
                new ParameterPGsql(
                    "p_juzgado",
                    NpgsqlDbType.Text,
                    entityAmparo.juzgado!
                ),
                new ParameterPGsql("p_id_tipo_resolucion", NpgsqlDbType.Integer, entityAmparo.id_tipo_resolucion),
                new ParameterPGsql(
                    "p_fecha_resolucion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_resolucion!
                ),
                new ParameterPGsql(
                    "p_descripcion_resolucion",
                    NpgsqlDbType.Text,
                    entityAmparo.descripcion_resolucion!
                ),
                new ParameterPGsql(
                    "p_fecha_notificacion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_resolucion!
                ),
                new ParameterPGsql(
                    "p_alegatos",
                    NpgsqlDbType.Boolean,
                    entityAmparo.alegatos!
                ),
                new ParameterPGsql(
                    "p_numero_oficio",
                    NpgsqlDbType.Text,
                    entityAmparo.numero_oficio!
                ),
                 new ParameterPGsql(
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_oficio!
                ),
                new ParameterPGsql("p_archivo", NpgsqlDbType.Boolean, dataFile is not null),
                new ParameterPGsql(
                    "p_id_tipo_documento",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_tipo_documento!
                ),
                new ParameterPGsql(
                    "p_id_seccion",
                    NpgsqlDbType.Integer,
                    entityDocumento is null ? DBNull.Value : entityDocumento.id_seccion!
                ),
                new ParameterPGsql(
                    "p_file_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.file_name!
                ),
                new ParameterPGsql(
                    "p_path_file",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.path_file!
                ),
                new ParameterPGsql(
                    "p_content_type",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.content_type!
                ),
                new ParameterPGsql(
                    "p_file_size",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.size!
                ),
                new ParameterPGsql(
                    "p_owner_name",
                    NpgsqlDbType.Text,
                    entityDocumento is null ? DBNull.Value : entityDocumento.owner_name!
                ),
                new ParameterPGsql(
                    "p_usuario_creacion",
                    NpgsqlDbType.Text,
                    entityAmparo.usuario_creacion!
                ),
                new ParameterPGsql(
                    "p_permanente",
                    NpgsqlDbType.Boolean,
                    entityDocumento is null ? DBNull.Value : entityDocumento.permanente!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.INSERT_IMPUTADOS_AMPARO,
                parameters!
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

        public async Task<ResponseAmparo> GetAmparoByImputadoDisconnectedById(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAmparoDisconnectedById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                idImputado = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                idEstadoProcesal = response.Data.Tables[0].Rows[0].IsNull(2)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int>(2),

                idTipoAmparo = response.Data.Tables[0].Rows[0].IsNull(3)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int>(3),

                fechaPresentacion = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4).ToString("dd/MM/yyyy")!,
                tipoOrganoJurisdiccional = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5),
                juzgado = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6)!,

                idTipoResolucion = response.Data.Tables[0].Rows[0].IsNull(7)
                        ? null
                        : response.Data.Tables[0].Rows[0].Field<int>(7),
                fechaResolucion = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(8).ToString("dd/MM/yyyy"),
                descripcionResolucion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(9),
                fechaNotificacion = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(10).ToString("dd/MM/yyyy"),
                alegatos = response.Data.Tables[0].Rows[0].IsNull(11)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(11),
                numeroOficio = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(12),
                fechaOficio = response.Data.Tables[0].Rows[0].IsNull(13)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(13).ToString("dd/MM/yyyy"),
                numeroJuicio = response.Data.Tables[0].Rows[0].IsNull(14)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(14),
            };
        }

        public async Task<List<ResponseAmparo>> GetAmparoByImputadoDisconnected(
            int idImputado
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, idImputado),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAmparoDisconnected,
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
            List<ResponseAmparo> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {

                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idImputado = item.IsNull(1) ? 0 : item.Field<int>(1),
                        idEstadoProcesal = item.IsNull(2) ? 0 : item.Field<int>(2),
                        estadoProcesal = item.IsNull(15) ? null! : item.Field<string>(15),
                        idTipoAmparo = item.IsNull(3) ? 0 : item.Field<int>(3),
                        fechaPresentacion = item.IsNull(4)
                    ? null!
                    : item.Field<DateTime>(4).ToString("dd/MM/yyyy"),
                        tipoOrganoJurisdiccional = item.IsNull(5) ? null! : item.Field<string>(5),
                        juzgado = item.IsNull(6) ? null! : item.Field<string>(6),
                        idTipoResolucion = item.IsNull(7) ? 0 : item.Field<int>(7),
                        fechaResolucion = item.IsNull(8)
                            ? null!
                            : item.Field<DateTime>(8).ToString("dd/MM/yyyy"),
                        descripcionResolucion = item.IsNull(9) ? null! : item.Field<string>(9),

                        fechaNotificacion = item.IsNull(10)
                            ? null!
                            : item.Field<DateTime>(10).ToString("dd/MM/yyyy"),
                        alegatos = item.IsNull(11) ? false : item.Field<bool>(11),
                        numeroOficio = item.IsNull(12) ? null! : item.Field<string>(12),
                        fechaOficio = item.IsNull(13)
                            ? null!
                            : item.Field<DateTime>(13).ToString("dd/MM/yyyy"),
                        numeroJuicio = item.IsNull(14) ? null! : item.Field<string>(14),

                    }

                );
            }
            return resultList;
        }

        public async Task<AsuntosPenalesAmparo> GetAmparoByImputadoById(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAmparoById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_imputado = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                id_estado_procesal = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(2),
                id_tipo_amparo = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(3),
                fecha_presentacion = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? new()!
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4),
                tipo_organo_jurisdiccional = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5),
                juzgado = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(6),
                id_tipo_resolucion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(7),
                fecha_resolucion = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(8),
                descripcion_resolucion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(9),
                fecha_notificacion = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(10),
                alegatos = response.Data.Tables[0].Rows[0].IsNull(11)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(11),
                numero_oficio = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(12),
                fecha_oficio = response.Data.Tables[0].Rows[0].IsNull(13)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(13),
                activo = response.Data.Tables[0].Rows[0].IsNull(14)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(14),
            };
        }


        public async Task<ResultTransaction> AmparoImputadosUpdateAsync(
            AsuntosPenalesAmparo entityAmparo
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entityAmparo.id),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entityAmparo.id_imputado),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entityAmparo.id_estado_procesal),
                new ParameterPGsql("p_id_tipo_amparo", NpgsqlDbType.Integer, entityAmparo.id_tipo_amparo),
                new ParameterPGsql(
                    "p_fecha_presentacion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_presentacion!
                ),
                new ParameterPGsql(
                    "p_numero_juicio",
                    NpgsqlDbType.Varchar,
                    entityAmparo.numeroJuicioAmparo!
                ),
                new ParameterPGsql(
                    "p_tipo_organo_jurisdiccional",
                    NpgsqlDbType.Text,
                    entityAmparo.tipo_organo_jurisdiccional!
                ),
                new ParameterPGsql(
                    "p_juzgado",
                    NpgsqlDbType.Text,
                    entityAmparo.juzgado!
                ),
                new ParameterPGsql(
                    "p_id_tipo_resolucion",
                    NpgsqlDbType.Integer,
                    entityAmparo.id_tipo_resolucion!
                ),
                new ParameterPGsql(
                    "p_fecha_resolucion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_resolucion!
                ),
                new ParameterPGsql(
                    "p_descripcion_resolucion",
                    NpgsqlDbType.Text,
                    entityAmparo.descripcion_resolucion!
                ),
                new ParameterPGsql(
                    "p_fecha_notificacion",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_notificacion!
                ),
                new ParameterPGsql(
                    "p_alegatos",
                    NpgsqlDbType.Boolean,
                    entityAmparo.alegatos!
                ),
                new ParameterPGsql(
                    "p_numero_oficio",
                    NpgsqlDbType.Text,
                    entityAmparo.numero_oficio!
                ),
                new ParameterPGsql(
                    "p_fecha_oficio",
                    NpgsqlDbType.Date,
                    entityAmparo.fecha_oficio!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityAmparo.usuario_modificacion!
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.UPDATE_IMPUTADOS_AMPARO,
                parameters!
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

        public async Task<ResultTransaction> DeleteAmparo(
            AsuntosPenalesAmparo entity
        )
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, entity.id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_DELETE_AMPARO,
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


        public async Task<AsuntosPenalesModificacion> GetModificacionByIdAsunto(int idAsuntoPenal)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, idAsuntoPenal) };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAmparoById,
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

            return new()
            {
                id = response.Data.Tables[0].Rows[0].IsNull(0)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(0),
                id_asunto_penal = response.Data.Tables[0].Rows[0].IsNull(1)
                    ? 0
                    : response.Data.Tables[0].Rows[0].Field<int>(1),
                id_estado_procesal = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(2),

            };
        }


        public async Task<List<ResponseAcuseConclusion>> ExportarPDFAsyncRepository(string noAsunto)
        {


            ParameterPGsql[] parameters =
            {

                new ParameterPGsql("p_no_asunto", NpgsqlDbType.Text, noAsunto)

            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GetReportePDF,
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

            List<ResponseAcuseConclusion> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        no_asunto = response.Data.Tables[0].Rows[0].IsNull(0) ? null! : response.Data.Tables[0].Rows[0].Field<string>(0)!,
                        numero_expediente_cadido = response.Data.Tables[0].Rows[0].IsNull(1) ? null! : response.Data.Tables[0].Rows[0].Field<string>(1),
                        estadoProcesal = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                        unidadRealizaSolicitud = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3),
                        subAdministracion = response.Data.Tables[0].Rows[0].IsNull(4) ? null! : response.Data.Tables[0].Rows[0].Field<string>(4)!,
                        adminControla = response.Data.Tables[0].Rows[0].IsNull(5) ? null! : response.Data.Tables[0].Rows[0].Field<string>(5)!,
                        abogado = response.Data.Tables[0].Rows[0].IsNull(6) ? null! : response.Data.Tables[0].Rows[0].Field<string>(6)!,

                    }
                );
            }

            return resultList;

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

    }
}
