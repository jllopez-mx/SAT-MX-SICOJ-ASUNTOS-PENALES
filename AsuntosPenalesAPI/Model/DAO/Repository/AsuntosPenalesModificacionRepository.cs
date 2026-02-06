using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils.Models;
using Sicoj.Utils.Postgres;
using System.Data;
using NpgsqlTypes;
using AsuntosPenalesAPI.Model.IDAO.IRepository;

namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class AsuntosPenalesModificacionRepository: IAsuntosPenalesModificacionRepository
    {

        #region Variables
        private readonly ISqlTools _database;
        #endregion
        
        #region Constructor
        public AsuntosPenalesModificacionRepository(ISqlTools database)
        {
            _database =
                database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<ResultTransaction> AddAsync(AsuntosPenales entityAsuntoPenal, AsuntosPenalesModificacion entity)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, entity.id_asunto_penal!),
                new ParameterPGsql("p_id_imputado", NpgsqlDbType.Integer, entity.id_imputado.GetValueOrDefault()),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion!),
                new ParameterPGsql("p_id_renglon_seccion", NpgsqlDbType.Integer, entity.id_renglon_seccion!),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Integer, entity.id_estado_procesal!),
                new ParameterPGsql("p_id_estado_procesal_nuevo", NpgsqlDbType.Integer, entityAsuntoPenal.id_estado_procesal!),
                new ParameterPGsql("p_fecha_control_solicitudes", NpgsqlDbType.Date, entityAsuntoPenal.fecha_control_solicitudes!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
            };
            var response = await _database.ExecuteFunctionAsync(EnumFunctions.GenAsuntosPenalesModificacionCreate, parameters);
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


    public async Task<AsuntosPenalesModificacion> GetByIdAsuntoPenalAsync(int idAsuntoPenal)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntoPenal), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesModificacionByIdAsuntoPenal,
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
                id_seccion  = response.Data.Tables[0].Rows[0].IsNull(2)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(2),
                id_renglon_seccion  = response.Data.Tables[0].Rows[0].IsNull(3)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(3),
                fecha_creacion = response.Data.Tables[0].Rows[0].IsNull(4)
                    ? new()
                    : response.Data.Tables[0].Rows[0].Field<DateTime>(4),
                usuario_creacion = response.Data.Tables[0].Rows[0].IsNull(5)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(5),
                fecha_modificacion = response.Data.Tables[0].Rows[0].IsNull(6)
                    ? new()
                    : response.Data.Tables[0].Rows[0]. Field<DateTime>(6),
                usuario_modificacion = response.Data.Tables[0].Rows[0].IsNull(7)
                    ? null!
                    : response.Data.Tables[0].Rows[0].Field<string>(7),
                activo = response.Data.Tables[0].Rows[0].IsNull(8)
                    ? false
                    : response.Data.Tables[0].Rows[0].Field<bool>(8),
                id_estado_procesal  = response.Data.Tables[0].Rows[0].IsNull(9)
                    ? null
                    : response.Data.Tables[0].Rows[0].Field<int>(9),
            };
        }
    }
}