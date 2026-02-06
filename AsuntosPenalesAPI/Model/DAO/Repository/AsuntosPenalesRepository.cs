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
    public class AsuntosPenalesRepository : IAsuntosPenalesRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AsuntosPenalesRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<AsuntosPenales> GetByIdAsync(int id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id", NpgsqlDbType.Integer, id) };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.ASUNTOSPENALES_GET_ASUNTO_PENAL_ADMIN_ENTITY_BY_ID,
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
                    : response.Data.Tables[0].Rows[0].Field<string?>(1)!,
                fecha_recepcion = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(2),
                fecha_vencimiento = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(3),
                oficio_solicitud = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string?>(4)!,
                numero_expediente_cadido = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string?>(5)!,
                id_unidad_realiza_solicitud = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(6),
                id_estado_procesal = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(7),
                id_admin_controla = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(8),
                id_subadministracion = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(9),
                nombre_abogado = response.Data.Tables[0].Rows[0].IsNull(10)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string?>(10)!,
                activo = response.Data.Tables[0].Rows[0].IsNull(11)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(11),
                id_estado_tarea = response.Data.Tables[0].Rows[0].IsNull(12)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(12),
                id_administracion_central = response.Data.Tables[0].Rows[0].IsNull(13)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(13),
                turnado = response.Data.Tables[0].Rows[0].IsNull(14)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(14),
            
            };
        }

        public async Task<List<AsuntosPenales>> GetListByIdsAsync(int[] ids)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, ids), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesByIds,
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

            List<AsuntosPenales> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(new()
                {
                    id = item.IsNull(0)
                     ? 0
                     : item.Field<int>(0),
                    numero_asunto_penal = item.IsNull(1)
                     ? null!
                     : item.Field<string>(1),
                    fecha_recepcion = item.IsNull(2)
                     ? new()
                     : item.Field<DateTime>(2),
                    fecha_vencimiento = item.IsNull(3)
                     ? new()
                     : item.Field<DateTime>(3),
                    oficio_solicitud = item.IsNull(4)
                     ? null!
                     : item.Field<string?>(4)!,
                    numero_expediente_cadido = item.IsNull(5)
                     ? null!
                     : item.Field<string?>(5)!,
                    id_unidad_realiza_solicitud = item.IsNull(6)
                     ? null!
                     : item.Field<int?>(6)!,
                    id_estado_tarea = item.IsNull(7)
                     ? null
                     : item.Field<int?>(7),
                    id_estado_procesal = item.IsNull(8)
                     ? null
                     : item.Field<int?>(8),
                    id_admin_controla = item.IsNull(9)
                     ? null
                     : item.Field<int?>(9),
                    id_administracion_central = item.IsNull(10)
                     ? null
                     : item.Field<int>(10),
                    fecha_turnado = item.IsNull(11)
                     ? new()
                     : item.Field<DateTime>(11),
                    activo = item.IsNull(12)
                     ? false
                     : item.Field<bool>(12)!,
                    numero_empleado = item.IsNull(13)
                     ? null!
                     : item.Field<string?>(13)!,
                    numero_empleado_adm = item.IsNull(14)
                     ? null!
                     : item.Field<string>(14),
                    fecha_asignacion = item.IsNull(15)
                     ? new()
                     : item.Field<DateTime>(15),
                    turnado = item.IsNull(16)
                     ? false
                     : item.Field<bool>(16),
                    usuario_modificacion = item.IsNull(17)
                     ? null!
                     : item.Field<string?>(17)!,              
                    interno = item.IsNull(18)
                     ? false
                     : item.Field<bool>(18)!,
                    id_subadministracion = item.IsNull(19)
                     ? null
                     : item.Field<int>(19),
                    abogado = new()
                    {
                        id = response.Data.Tables[0].Rows[0].IsNull(20)
                        ? 0
                        : response.Data.Tables[0].Rows[0].Field<int>(20),
                        id_abogado = response.Data.Tables[0].Rows[0].IsNull(21)
                        ? null!
                        : response.Data.Tables[0].Rows[0].Field<string>(21)!,
                    }
                });
            }

            return resultList;
        }

        public async Task<ResultTransaction> ReasignarAsync(
            List<AsuntosPenalesReasignar> listReasignacion
        )
        {
            var arrayObject = listReasignacion
                .Select(c =>
                    $"{c.id_asunto_penal_reasignado},{c.rfc_funcionario_reasignador},{c.rfc_funcionario_reasignado},{c.rfc_funcionario_retirado},{c.id_unidad_administrativa_reasignador},{c.id_subadministracion_reasignador},{c.id_unidad_administrativa_reasignado},{c.id_subadministracion_reasignado},{c.id_estado_procesal_previo},{c.id_estado_tarea_nuevo},{c.id_estado_procesal_nuevo}"
                )
                .ToArray();
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_array", NpgsqlDbType.Array | NpgsqlDbType.Text, arrayObject),
                new ParameterPGsql(
                    "p_id_estado_tarea",
                    NpgsqlDbType.Integer,
                    EnumEstadoTarea.REASIGNADO.GetHashCode()
                ),
            };
            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FUNC_REASIGNAR_ASUNTOS_PENALES,
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

         public async Task<AsuntosPenalesAbogado> GetAbogadoAsignadoAsync(int idAsuntoPenal)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntoPenal), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntoPenalUltimoAbogado,
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
                     : response.Data.Tables[0].Rows[0].Field<int>(1),
                id_abogado = response.Data.Tables[0].Rows[0].IsNull(2)
                     ? null!
                     : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                fecha_asignacion = response.Data.Tables[0].Rows[0].IsNull(3)
                     ? new()
                     : response.Data.Tables[0].Rows[0].Field<DateTime>(3)!,
                remitido = response.Data.Tables[0].Rows[0].IsNull(4)
                     ? false
                     : response.Data.Tables[0].Rows[0].Field<bool>(4)!,
                reasingado = response.Data.Tables[0].Rows[0].IsNull(5)
                     ? false
                     : response.Data.Tables[0].Rows[0].Field<bool>(5)!,
            };
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
}
    
}
