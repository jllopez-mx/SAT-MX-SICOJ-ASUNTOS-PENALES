using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;
using System.Data;


namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class AsuntosPenalesDescartarRepository : IAsuntosPenalesDescartarRepository
    {
           #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AsuntosPenalesDescartarRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> DescartarAsync(AsuntosPenales entityAsuntosPenales,AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartar,
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

        public async Task<ResultTransaction> DescartarRequerimientoAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                 new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartarRequerimiento,
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

        public async Task<ResultTransaction> DescartarRequisitosAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                 new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartarRequisitos,
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




        public async Task<ResultTransaction> DescartarEtapaInicialAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entity.id_imputado!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartarEtapaInicial,
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




        public async Task<ResultTransaction> DescartarEtapaComplementariaAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entity.id_imputado!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartarEtapaComplementaria,
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

        public async Task<ResultTransaction> DescartarEtapaIntermediaAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entityAsuntosPenales.id!),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entity.id_imputado),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion),
                new ParameterPGsql("p_id_seccion_archivo", NpgsqlDbType.Array | NpgsqlDbType.Integer, (listaSecciones is null || !listaSecciones.Any()) ? DBNull.Value :  listaSecciones.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntosPenales.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntosPenales.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, entityAsuntosPenales.usuario_modificacion!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.AsuntosPenalesDescartarEtapaIntermedia,
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