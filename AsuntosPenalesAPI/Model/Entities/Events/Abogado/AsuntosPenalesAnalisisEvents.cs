using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Microsoft.IdentityModel.Tokens;

namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesAnalisisEvents
    {
        public static AsuntosPenalesAnalisis Create(
            ref AsuntosPenales entity,
            bool requerimiento,
            string oficio_requerimiento,
            DateTime? fecha_requerimiento, 
            bool? requerimiento_atendido, 
            string oficio_atencion, 
            DateTime? fecha_atencion, 
            int? id_determinacion_asunto_penal, 
            DateTime fecha_determinacion,
            string usuario_creacion
        )

        {
            Guard.CatalogValue(ref id_determinacion_asunto_penal, "Determinacion de Asunto Penal");

            AsuntosPenalesAnalisis entityAnalisis = new()
            {
                id_asunto_penal = entity.id,
                requerimiento = requerimiento,
                oficio_requerimiento = oficio_requerimiento,
                fecha_requerimiento = fecha_requerimiento,
                requerimiento_atendido = requerimiento_atendido,
                oficio_atencion = oficio_atencion,
                fecha_atencion = fecha_atencion,
                id_determinacion_asunto_penal = id_determinacion_asunto_penal.GetValueOrDefault(), 
                fecha_determinacion = fecha_determinacion,
                usuario_creacion = usuario_creacion
            };


            if(id_determinacion_asunto_penal == EnumDeterminacionAsuntoPenal.PROCEDENTE.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.ASIGNADO.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.PROCEDENTE.GetHashCode();
            }
            else if(id_determinacion_asunto_penal == EnumDeterminacionAsuntoPenal.IMPROCEDENTE.GetHashCode())
            {
                entity.id_estado_tarea = EnumEstadoTarea.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode();
                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode();
            }

            return entityAnalisis;
        }
        public static void UpdateAnalisis(ref AsuntosPenalesAnalisis entityAnalisis,
        
                            ref AsuntosPenales entity, 
                            bool requerimiento, 
                            string oficio_requerimiento,
                            string? fecha_requerimiento,
                            bool? requerimiento_atendido, 
                            string oficio_atencion,
                            string? fecha_atencion, 
                            int? id_determinacion_asunto_penal,
                            DateTime fecha_determinacion,  
                            string usuarioModificacion
            )
            
        {        
                
                entityAnalisis.requerimiento = requerimiento;
                entityAnalisis.oficio_requerimiento = oficio_requerimiento;               
                entityAnalisis.fecha_requerimiento = string.IsNullOrWhiteSpace(fecha_requerimiento) 
                ? (DateTime?)null 
                : DateTime.Parse(fecha_requerimiento);

                entityAnalisis.requerimiento_atendido = requerimiento_atendido;
                entityAnalisis.oficio_atencion = oficio_atencion;
                entityAnalisis.fecha_atencion = string.IsNullOrWhiteSpace(fecha_atencion) 
                ? (DateTime?)null 
                : DateTime.Parse(fecha_atencion);

                entityAnalisis.id_determinacion_asunto_penal = id_determinacion_asunto_penal.GetValueOrDefault();  
                entityAnalisis.id_estado_procesal = entity.id_estado_procesal.GetValueOrDefault(); 
                entityAnalisis.fecha_determinacion = fecha_determinacion;
                entityAnalisis.usuario_modificacion = usuarioModificacion;
        }


    }
}