using System;
using System.Collections.Generic;
using System.Linq;
using AsuntosPenalesAPI.Model.ViewModels.Enums;

using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities.Events.Genericos
{
    public class AsuntosPenalesModificacionEvents
    {
        public static AsuntosPenalesModificacion Create(
           ref AsuntosPenales entityAsuntosPenales,
           int? id_seccion,
           int? id_renglon_seccion,
           string usuarioCreacion
       )
        {
            if (entityAsuntosPenales.id_estado_procesal == EnumEstadoProcesal.ACTIVO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que su estado se encuentra en ACTIVO");
            }
            if (entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_REMITIDO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que se encuentra concluido");
            }
            AsuntosPenalesModificacion entity = new()
            {
                id_asunto_penal = entityAsuntosPenales.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_renglon_seccion = id_renglon_seccion,
                id_estado_procesal = entityAsuntosPenales.id_estado_procesal,
                usuario_creacion = usuarioCreacion,
            };

            entityAsuntosPenales.id_estado_procesal = EnumEstadoProcesal.EN_REPARACION.GetHashCode();
            entityAsuntosPenales.fecha_control_solicitudes = DateTime.Now;
            return entity;
        }
        
        public static AsuntosPenalesModificacion CreateImputados(
            ref AsuntosPenales entityAsuntosPenales,
            int id_imputado,
            int? id_seccion,
            int? id_renglon_seccion,
            string usuarioCreacion
        )
        {
            if(entityAsuntosPenales.id_estado_procesal == EnumEstadoProcesal.ACTIVO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que su estado se encuentra en ACTIVO");
            }
            if(entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_REMITIDO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que se encuentra concluido");
            }
            AsuntosPenalesModificacion entity = new()
            {
                id_asunto_penal = entityAsuntosPenales.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_imputado = id_imputado,
                id_renglon_seccion = id_renglon_seccion,
                id_estado_procesal = entityAsuntosPenales.id_estado_procesal,
                usuario_creacion = usuarioCreacion,
            };

            entityAsuntosPenales.id_estado_procesal = EnumEstadoProcesal.EN_REPARACION.GetHashCode();
            entityAsuntosPenales.fecha_control_solicitudes = DateTime.Now;
            return entity;
        }
    }
}