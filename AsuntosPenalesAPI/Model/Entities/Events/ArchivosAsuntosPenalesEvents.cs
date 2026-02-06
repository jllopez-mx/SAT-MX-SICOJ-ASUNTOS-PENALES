using AsuntosPenalesAPI.Model.ViewModels.Enums;

namespace AsuntosPenalesAPI.Model.Entities.Events
{
    public class ArchivosAsuntosPenalesEvents
    {
        public static ArchivosAsuntosPenales CreateArchivo(
            int id_asuntospenales,
            int? id_tipo_documento,
            string file_name,
            string path_file,
            string content_type,
            string owner_name,
            string usuario_creacion,
            int? id_seccion,
            int? id_renglon_seccion,
            string size,
            bool permanente,
            int id_rol
        )
        {
            ArchivosAsuntosPenales entity =
                new()
                {
                    id_asuntospenales = id_asuntospenales,
                    id_tipo_documento = id_tipo_documento,
                    file_name = file_name,
                    path_file = path_file,
                    content_type = content_type,
                    owner_name = owner_name,
                    usuario_creacion = usuario_creacion,
                    id_seccion = id_seccion,
                    id_renglon_seccion = id_renglon_seccion,
                    size = size,
                    permanente = permanente,
                    id_rol = id_rol

                };
            return entity;
        }

        public static void UpdateWithFile(
            ref ArchivosAsuntosPenales entity,
            int id_tipo_archivo,
            string file_name,
            string file_path,
            string content_type,
            string file_size,
            string owner_name,
            string usuarioCreacion,
            int id_rol
        )
        {
            entity.id_tipo_documento = id_tipo_archivo;
            entity.file_name = file_name;
            entity.path_file = file_path;
            entity.content_type = content_type;
            entity.size = file_size;
            entity.owner_name = owner_name;
            entity.usuario_modificacion = usuarioCreacion;
            entity.id_rol = id_rol;
        }

        public static void Update(
            ref ArchivosAsuntosPenales entity,
            int id_tipo_archivo,
            string owner_name,
            string usuarioCreacion,
            int id_rol
        )
        {
            entity.id_tipo_documento = id_tipo_archivo;
            entity.owner_name = owner_name;
            entity.usuario_modificacion = usuarioCreacion;
            entity.id_rol = id_rol;
        }
        
        public static ArchivoAsuntoPenal CreateArchivoGlobal(
            int id_consulta,
            int? id_tipo_documento,
            int id_seccion,
            int id_administracion,
            string file_name,
            string path_file,
            string content_type,
            string owner_name,
            string usuario_creacion,
            string size,
            string noFolio

        )
        {

            ArchivoAsuntoPenal entity = new()
            {
                id_rol = EnumRol.Oficial_De_Partes.GetHashCode(),
                id_consulta = id_consulta,
                id_tipo_documento = id_tipo_documento,
                file_name = file_name,
                path_file = path_file,
                content_type = content_type,
                owner_name = owner_name,
                usuario_creacion = usuario_creacion,
                id_seccion=id_seccion,
                size=size,
                estatus=true,
                id_administracion=id_administracion,
                no_folio=noFolio
            
            };
            return entity;
        }
    }
}