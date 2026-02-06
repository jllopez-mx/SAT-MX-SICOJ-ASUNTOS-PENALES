using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.Entities;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.OficialPartes
{
    public class AsuntosPenalesOficialPartesEvents
    {
        public static AsuntosPenales CreateModalidadFisico(
            string? noExpedienteCadido,
            string? oficioSolicitud,
            DateTime fechaRecepcion,
            DateTime fechaVencimiento,
            bool? interno,
            int? idUnidadRealizaSolicitud,
            //int? idUnidadAdministrativa
            int? idAmminControla,
            int? idAdminCentral
        )
        {
            Guard.ValidateStringEmpty(ref noExpedienteCadido!, "No. Expediente Candido");
            Guard.ValidateStringEmpty(ref oficioSolicitud!, "Oficio Solicitud");
            Guard.CatalogValue(ref idUnidadRealizaSolicitud!, "Unidad Realiza Solicitud", false);
            //Guard.CatalogValue(ref idUnidadAdministrativa, "Unidad administrativa");
            Guard.CatalogValue(ref idAmminControla!, "Administracion Controla");
            Guard.BooleanValue(interno!, "Tipo de Unidad");

            AsuntosPenales entity = new()
            {
                numero_expediente_cadido = noExpedienteCadido,
                oficio_solicitud = oficioSolicitud,
                id_unidad_realiza_solicitud = idUnidadRealizaSolicitud,
                interno = interno,
                fecha_recepcion = fechaRecepcion,
                fecha_vencimiento = fechaVencimiento,
                id_administracion_central = idAdminCentral,
                id_estado_tarea = EnumEstadoTarea.PENDIENTE_DE_TURNAR.GetHashCode(),
                id_estado_procesal = EnumEstadoProcesal.ACTIVO.GetHashCode(),
                id_admin_controla = idAmminControla.GetValueOrDefault(),

            };
            return entity;
        }

        public static AsuntosPenales CreateControlDocumental(
            string? oficioSolicitud,
            DateTime fechaRecepcion,
            int? idUnidadRealizaSolicitud,
            //int? idUnidadAdministrativa
            int? idAdminCentral
        )
        {
            bool interno;
            if (idUnidadRealizaSolicitud > 0)
            {
                interno = true;
            }
            else
            {
                interno = false;
            }
            Guard.ValidateStringEmpty(ref oficioSolicitud!, "Oficio Solicitud");
            Guard.CatalogValue(ref idUnidadRealizaSolicitud!, "Unidad Realiza Solicitud", false);
            Guard.BooleanValue(interno!, "Tipo de Unidad");

            AsuntosPenales entity = new()
            {
                oficio_solicitud = oficioSolicitud,
                id_unidad_realiza_solicitud = idUnidadRealizaSolicitud,
                interno = interno,
                fecha_recepcion = fechaRecepcion,
                id_administracion_central = idAdminCentral,
                id_estado_tarea = EnumEstadoTarea.PENDIENTE_DE_REGISTRO.GetHashCode(),
                id_estado_procesal = EnumEstadoProcesal.ACTIVO.GetHashCode(),

            };
            return entity;
        }

        public static void UpdateGuardarModalidadFisico(ref AsuntosPenales entity,
            string? noExpedienteCadido,
            string? oficioSolicitud,
            DateTime fechaRecepcion,
            DateTime fechaVencimiento,
            int? idUnidadRealizaSolicitud,
            int? id_admin_controla,
            bool interno
        )
        {
            Guard.ValidateStringEmpty(ref noExpedienteCadido!, "No. Expediente Candido");
            Guard.ValidateStringEmpty(ref oficioSolicitud!, "Oficio Solicitud");
            Guard.CatalogValue(ref id_admin_controla, "id admin controla");
            Guard.CatalogValue(ref idUnidadRealizaSolicitud!, "Unidad Realiza Solicitud", false);
            Guard.BooleanValue(interno!, "Tipo de Unidad");

            if (entity.id_estado_procesal == EnumEstadoProcesal.EN_REPARACION.GetHashCode())
            {
                entity.id_estado_procesal = EnumEstadoProcesal.EN_ESTUDIO.GetHashCode();
            }
            if (entity.id_estado_tarea == EnumEstadoTarea.PENDIENTE_DE_REGISTRO.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.PENDIENTE_DE_TURNAR.GetHashCode();
            }

            entity.numero_expediente_cadido = noExpedienteCadido;
            entity.oficio_solicitud = oficioSolicitud;
            entity.id_unidad_realiza_solicitud = idUnidadRealizaSolicitud;
            entity.interno = interno;
            entity.fecha_recepcion = fechaRecepcion;
            entity.fecha_vencimiento = fechaVencimiento;
            entity.id_admin_controla = id_admin_controla.GetValueOrDefault();
        }


        public static void Updateturnar(ref AsuntosPenales entity,

            string? numero_empleado,
            int? id_admin_controla
            )

        {

            Guard.ValidateStringEmpty(ref numero_empleado!, "numero empleado");
            Guard.CatalogValue(ref id_admin_controla!, "id admin controla");




            entity.numero_empleado = numero_empleado;
            entity.id_admin_controla = id_admin_controla.GetValueOrDefault();
            entity.id_estado_tarea = EnumEstadoTarea.PENDIENTE_DE_ASIGNAR.GetHashCode();
            entity.id_estado_procesal = EnumEstadoProcesal.ACTIVO.GetHashCode();

        }


        public static void DeleteAsuntosPenales(ref AsuntosPenales entity

      )
        {

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

        #region Email

        public static Email CreaCorreo(
        List<string> to,
         List<string> Cc,
         string Subject,
         bool IsHtml,
         string Body

     )
        {

            Email entity = new()
            {
                To = to,
                Cc = Cc,
                Subject = Subject,
                IsHtml = IsHtml,
                Body = Body,
                Priority = (System.Net.Mail.MailPriority?)1,
            };


            return entity;
        }

        #endregion

    }
    

}