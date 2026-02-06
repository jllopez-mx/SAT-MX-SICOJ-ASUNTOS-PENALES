using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesAbogadoEvents
    {
        public static void UpdateRemitir(
            ref AsuntosPenales entity,
            AsuntosPenalesRemision remision,
            string noEmpleado,
            string usuarioModificacion
        )
        {
            if (entity.id_estado_procesal == EnumEstadoProcesal.CONCLUIDO_REMITIDO.GetHashCode())
                throw new Exception(
                    "El Asunto Penal no se puede remitir debido a que tiene en el estado procesal: Concluido."
                );

            if (remision.id_tipo_autoridad == EnumTipoAutoridad.Interna.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.PENDIENTE_DE_TURNAR.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.REMITIDO.GetHashCode();
            }
            else if (remision.id_tipo_autoridad == EnumTipoAutoridad.Externa.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.CONCLUIDO_REMITIDO.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_REMITIDO.GetHashCode();
            }
            entity.usuario_modificacion = usuarioModificacion;
        }

        public static void DeleteModalidaArchivoAsuntosPenales(ref ArchivosAsuntosPenales entity,
        string usuarioModificacion           
        )
        {
            if (!entity.activo)
                throw new Exception("El documento ya se encuentra eliminado.");

            if (entity.permanente)
                throw new Exception("El documento no se puede eliminar.");

            entity.activo = false;
            entity.usuario_modificacion = usuarioModificacion;
            entity.id = entity.id;

            
        }

        public static void DeleteDelito(ref AsuntosPenalesDelitos entity) { }

        // public static ArchivosAsuntosPenales CreateArchivo(
        //     int id,
        //     int id_asuntospenales,
        //     int? id_tipo_documento,
        //     string file_name,
        //     string path_file,
        //     string content_type,
        //     string owner_name,
        //     string usuario_creacion,
        //     int? id_seccion,
        //     string size,
        //     bool permanente
        // )
        // {
        //     ArchivosAsuntosPenales entity =
        //         new()
        //         {
        //             id_asuntospenales = id_asuntospenales,
        //             id_tipo_documento = id_tipo_documento,
        //             file_name = file_name,
        //             path_file = path_file,
        //             content_type = content_type,
        //             owner_name = owner_name,
        //             usuario_creacion = usuario_creacion,
        //             id_seccion = id_seccion,
        //             size = size,
        //             permanente = permanente,
        //             id_renglon_seccion = id,
        //         };
        //     return entity;
        // }

        public static SolicitudTransparencia CreateModalidadSolicitudTransparencia(
            int idAsunto,
            string noSolicitud,
            DateTime fechaSolicitud

        )
        {

            SolicitudTransparencia entity = new()
            {
                id_rol = EnumRol.Oficial_De_Partes.GetHashCode(),
                id_asunto = idAsunto,
                noSolicitud = noSolicitud,
                fechaSolicitud = fechaSolicitud

            };
            return entity;
        }
        public static void UpdateSolicitudTransparencia(ref SolicitudTransparencia entity,
            int id,
            int idAsunto,
            string noSolicitud,
            DateTime fechaSolicitud
        )
        {
            entity.id = id;
            entity.id_asunto = idAsunto;
            entity.noSolicitud = noSolicitud;
            entity.fechaSolicitud = fechaSolicitud;
        }
        public static void DeleteSolicitudTransparencia(ref SolicitudTransparencia entity, int id

        )
        {
            entity.id = id;

        }
    }
}
