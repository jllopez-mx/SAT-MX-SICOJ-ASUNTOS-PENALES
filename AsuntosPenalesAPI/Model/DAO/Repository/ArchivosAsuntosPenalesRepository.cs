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
    public class ArchivosAsuntosPenalesRepository : IArchivosAsuntosPenalesRepository
    {

        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public ArchivosAsuntosPenalesRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion


        // public async Task<List<ResponseArchivosAsuntosPenales>> GetAllArchivosAsync()
        //     {
        //         var response = await _database.ExecuteFunctionAsync(
        //             EnumFunctions.ARCHIVOS_ASUNTOS_PENALES_GET_ALL,
        //             null!
        //         );
        //         if (response.ExisteError)
        //         {
        //             throw new Exception(response.Mensaje);
        //         }

        //         if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
        //         {
        //             return null!;
        //         }

        //         List<ResponseArchivosAsuntosPenales> resultList = new();

        //         foreach (DataRow item in response.Data.Tables[0].Rows)
        //         {
        //             resultList.Add(
        //                 new()
        //             {      
        //                     // id = item.IsNull(0) ? null! : item.Field<int?>(0),  
        //                     // id_tipo_archivo_asuntospenales = item.IsNull(1) ? null! : item.Field<int?>(1),                                  
        //                     // file_name = item.IsNull(4) ? null! : item.Field<string?>(4),
        //                     // seccion = item.IsNull(3) ? null! : item.Field<int?>(3),

        //                 }
        //             );
        //         }

        //         return resultList;
        //     }


        public async Task<ResultTransaction> AddFileAsyncRepository(ArchivosAsuntosPenales entity, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asuntospenales", NpgsqlDbType.Integer, entity.id_asuntospenales!),
                new ParameterPGsql("p_id_tipo_documento", NpgsqlDbType.Integer, entity.id_tipo_documento!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer, entity.id_seccion!),
                new ParameterPGsql("p_id_renglon_seccion", NpgsqlDbType.Integer, entity.id_renglon_seccion!),
                new ParameterPGsql("p_file_name", NpgsqlDbType.Text, entity.file_name!),
                new ParameterPGsql("p_path_file", NpgsqlDbType.Text, entity.path_file!),
                new ParameterPGsql("p_content_type", NpgsqlDbType.Text, entity.content_type!),
                new ParameterPGsql("p_owner_name", NpgsqlDbType.Text, entity.owner_name!),
                new ParameterPGsql("p_usuario_creacion", NpgsqlDbType.Text, entity.usuario_creacion!),
                new ParameterPGsql("p_size", NpgsqlDbType.Varchar, entity.size!),
                new ParameterPGsql("p_permanente", NpgsqlDbType.Boolean, entity.permanente!),
                new ParameterPGsql("p_id_rol", NpgsqlDbType.Integer, entity.id_rol!),

            };

            var response = await _database.ExecuteFunctionFileAsync(EnumFunctions.File_Repository_INSERT, dataFile, parameters);
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

        public async Task<int?> GetDocumentosDisconnectedCount(int? idAsuntoPenal, List<int>? idSeccion, int? idRenglonSeccion, bool activo)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntoPenal),
                new ("p_id_seccion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (idSeccion is null || !idSeccion.Any()) ? DBNull.Value :  idSeccion.ToArray()),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_activo", NpgsqlDbType.Boolean, activo),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.DocumentosCount,
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

            return response.Data.Tables[0].Rows[0].IsNull(0)
                ? null!
                : response.Data.Tables[0].Rows[0].Field<int>(0);
        }

        public async Task<List<ResponseArchivosAsuntosPenales>> GetDocumentosHistoricoDisconnected(bool paginado, int? idAsuntoPenal, List<int>? idSeccion, int? idRenglonSeccion, bool activo, int? Fetch = null, int? Page = null!, string? OrderByColumn = null!, bool? OrderDesc = null!)
        {
            ParameterPGsql[] parameters =
            {
                new ("p_paginado", NpgsqlDbType.Boolean, paginado),
                new ("p_page_size", NpgsqlDbType.Integer, Fetch),
                new ("p_page", NpgsqlDbType.Integer, Page),
                new ("p_order_column", NpgsqlDbType.Varchar, OrderByColumn),
                new ("p_order_desc", NpgsqlDbType.Boolean, OrderDesc),
                new ("p_id_asunto_penal", NpgsqlDbType.Integer, idAsuntoPenal),
                new ("p_id_seccion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (idSeccion is null || !idSeccion.Any()) ? DBNull.Value :  idSeccion.ToArray()),
                new ("p_id_renglon_seccion", NpgsqlDbType.Integer, idRenglonSeccion),
                new ("p_activo", NpgsqlDbType.Boolean, activo),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.Documentos,
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

            List<ResponseArchivosAsuntosPenales> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        IdAsuntoPenal = item.IsNull(1) ? null! : item.Field<int>(1),
                        IdTipoDocumento = item.IsNull(2) ? 0 : item.Field<int>(2),
                        TipoDocumento = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        Nombre = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        TamanoDocumento = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        IdSeccion = item.IsNull(6) ? 0 : item.Field<int>(6),
                        Seccion = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        IdRenglonSeccion = item.IsNull(8) ? 0 : item.Field<int>(8),
                        Permanente = !item.IsNull(9) && item.Field<bool>(9),                    }
                );
            }

            return resultList;
        }



        public async Task<List<ResponseArchivosAsuntosPenales>> GetArchivosByIdRegistroAsync_Repository(int id_asuntospenales)
        {

            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, id_asuntospenales!),
            };
            var response = await _database.ExecuteFunctionAsync(
              EnumFunctions.FILE_GET_BY_ID_AP, parameters

          );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseArchivosAsuntosPenales> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        Id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        IdAsuntoPenal = item.IsNull(1) ? 0! : item.Field<int>(1),
                        IdSeccion = item.IsNull(2) ? null : item.Field<int>(2),
                        Seccion = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        IdTipoDocumento = item.IsNull(4) ? null : item.Field<int>(4),
                        TipoDocumento = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        Nombre = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        TamanoDocumento = item.IsNull(8) ? null! : item.Field<string>(8)!,
                        Permanente = !item.IsNull(9) && item.Field<bool>(9)
                    }
                );
            }

            return resultList;
        }


        public async Task<ArchivosAsuntosPenales> GetByIdArchivoAsyncRepository(int id)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id", NpgsqlDbType.Integer, id),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.File_Repository_GET_BY_ID,
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
                file_name = response.Data.Tables[0].Rows[0].IsNull(1) ? null! : response.Data.Tables[0].Rows[0].Field<string>(1)!,
                path_file = response.Data.Tables[0].Rows[0].IsNull(2) ? null! : response.Data.Tables[0].Rows[0].Field<string>(2)!,
                content_type = response.Data.Tables[0].Rows[0].IsNull(3) ? null! : response.Data.Tables[0].Rows[0].Field<string>(3)!,




            };
        }
        public async Task<List<ArchivosAsuntosPenales>> GetByIdsAsync(int[] id)
        {
            ParameterPGsql[] parameters = { new ParameterPGsql("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, id), };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.FILEs_GET_BY_ID_AP,
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

            List<ArchivosAsuntosPenales> resultList = new();
            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_asuntospenales = item.IsNull(1) ? 0! : item.Field<int>(1),
                        id_seccion = item.IsNull(2) ? null! : item.Field<int>(2),
                        id_renglon_seccion = item.IsNull(3) ? null! : item.Field<int>(3),
                        id_tipo_documento = item.IsNull(4) ? null! : item.Field<int>(4),
                        file_name = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        path_file = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        size = item.IsNull(7) ? null! : item.Field<string>(7),
                        fecha_creacion = item.IsNull(8) ? new() : item.Field<DateTime>(8),
                        fecha_modificacion = item.IsNull(9) ? new() : item.Field<DateTime>(9),
                        owner_name = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        activo = item.IsNull(11) ? false : item.Field<bool>(11),
                        permanente = item.IsNull(12) ? false : item.Field<bool>(12),
                        usuario_modificacion = item.IsNull(13) ? null! : item.Field<string>(13)!,
                    }
                );
            }
            return resultList;
        }
        
        public async Task<ResultTransaction> DeleteAsync(int[] ids, string usuarioModificacion)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Integer, ids!),
                new ParameterPGsql("p_usuario_modificacion", NpgsqlDbType.Text, usuarioModificacion!),
            };
            var response = await _database.ExecuteFunctionAsync(EnumFunctions.CONSULTA_DELETE_ARCHIVO, parameters);
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

        public async Task<List<ArchivosAsuntosPenales>> GetArchivosByIdRenglonSeccion_Async_Repository(int id_asuntospenales,int id_seccion,int id_renglonseccion, int id_tipo_documento )
        {

            ParameterPGsql[] parameters =
            {
                new ParameterPGsql("p_id_asunto_penal", NpgsqlDbType.Integer, id_asuntospenales!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer,id_seccion!),
                new ParameterPGsql("p_id_renglon_seccion", NpgsqlDbType.Integer, id_renglonseccion!),
                new ParameterPGsql("p_id_tipo_documento", NpgsqlDbType.Integer, id_tipo_documento!), 

            };
            var response = await _database.ExecuteFunctionAsync(
            EnumFunctions.FILE_GET_BY_ID_AP_SECCION, parameters

        );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ArchivosAsuntosPenales> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        id = item.IsNull(0) ? 0 : item.Field<int>(0),
                        id_asuntospenales = item.IsNull(1) ? 0! : item.Field<int>(1),
                        id_seccion = item.IsNull(2) ? null! : item.Field<int>(2),
                        id_renglon_seccion = item.IsNull(3) ? null! : item.Field<int>(3),
                        id_tipo_documento = item.IsNull(4) ? null! : item.Field<int>(4),
                        file_name = item.IsNull(5) ? null! : item.Field<string>(5)!,
                        path_file = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        size = item.IsNull(7) ? null! : item.Field<string>(7),
                        fecha_creacion = item.IsNull(8) ? new() : item.Field<DateTime>(8),
                        fecha_modificacion = item.IsNull(9) ? new() : item.Field<DateTime>(9),
                        owner_name = item.IsNull(10) ? null! : item.Field<string>(10)!,
                        activo = item.IsNull(11) ? false : item.Field<bool>(11),
                        permanente = item.IsNull(12) ? false : item.Field<bool>(12),

                    }
                );
            }

            return resultList;
        }

        public async Task<ResultTransaction> UpdateAsync(ArchivosAsuntosPenales entityDocumento, DataFile dataFile)
        {
            ParameterPGsql[] parameters =
            {
                new ParameterPGsql(
                    "p_id",
                    NpgsqlDbType.Integer,
                    entityDocumento.id!
                ),
                new ParameterPGsql(
                    "p_id_asunto",
                    NpgsqlDbType.Integer,
                    entityDocumento.id_asuntospenales!
                ),
                new ParameterPGsql(
                    "p_usuario_modificacion",
                    NpgsqlDbType.Text,
                    entityDocumento.usuario_modificacion!
                ),
                new ParameterPGsql(
                    "p_archivo",
                    NpgsqlDbType.Boolean,
                    dataFile is not null
                ),
                new ParameterPGsql("p_id_tipo_documento", NpgsqlDbType.Integer, entityDocumento.id_tipo_documento!),
                new ParameterPGsql("p_id_seccion", NpgsqlDbType.Integer,entityDocumento.id_seccion!),
                new ParameterPGsql("p_id_renglon_seccion", NpgsqlDbType.Integer, entityDocumento.id_renglon_seccion!),
                //new ParameterPGsql("p_id_unidad_administrativa", NpgsqlDbType.Integer, entityDocumento.id_unidad_administrativa!),
                new ParameterPGsql("p_file_name", NpgsqlDbType.Text, entityDocumento.file_name!),
                new ParameterPGsql("p_file_path", NpgsqlDbType.Text, entityDocumento.path_file !),
                new ParameterPGsql("p_content_type", NpgsqlDbType.Text,entityDocumento.content_type!),
                new ParameterPGsql("p_file_size", NpgsqlDbType.Text, entityDocumento.size!),
                new ParameterPGsql("p_owner_name", NpgsqlDbType.Text, entityDocumento.owner_name!),
                new ParameterPGsql("p_permanente", NpgsqlDbType.Boolean, entityDocumento.permanente!),
                new ParameterPGsql("p_id_rol", NpgsqlDbType.Integer, entityDocumento.id_rol!),
            };

            var response = await _database.ExecuteFunctionFileAsync(
                EnumFunctions.Archivo_asuntos_penales_update, //Crear función de update archivos
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

    }

}