using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;

namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class AsuntosPenalesRemisionRepository : IAsuntosPenalesRemisionRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public AsuntosPenalesRemisionRepository(ISqlTools database)
        {
            _database =
                database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<List<ResponseRemision>> GetRemisionesDisconnected(int idAsuntosPenales)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asuntos_penales", NpgsqlDbType.Integer, idAsuntosPenales),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GenAsuntosPenalesRemisionDisconnected,
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

            List<ResponseRemision> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        idAsuntoPenal = item.IsNull(1) ? 0 : item.Field<int>(1),
                        TipoAutoridadRemision = item.IsNull(3) ? null! : item.Field<string>(3),
                        idTipoAutoridadRemision = item.IsNull(2) ? null : item.Field<int>(2),
                        UnidadAdministrativaRemite = item.IsNull(5) ? null! : item.Field<string>(5),
                        idUnidadAdministrativaRemite = item.IsNull(4) ? null : item.Field<int>(4),
                        UnidadAdministrativaRecibe = item.IsNull(7) ? null! : item.Field<string>(7),
                        idUnidadAdministrativaRecibe= item.IsNull(6) ? null : item.Field<int>(6),
                        UnidadAdministrativaExterna = item.IsNull(9) ? null! : item.Field<string>(9),
                        idUnidadAdministrativaExterna = item.IsNull(8) ? null : item.Field<int>(8),
                        FechaOficio = item.IsNull(10) ? null! : item.Field<DateTime>(10)!.ToString("dd/MM/yyyy"),
                        NumeroOficio = item.IsNull(11) ? null! : item.Field<string>(11)!,
                    }
                );
            }

            return resultList;
        }
    }
}