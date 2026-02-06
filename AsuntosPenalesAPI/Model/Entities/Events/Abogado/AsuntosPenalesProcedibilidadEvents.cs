
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.ViewModels.Enums;

namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesProcedibilidadEvents
    {
        public static AsuntosPenalesProcedibilidad Create(
        
                            ref AsuntosPenales entity, 
                            int id_requisito_procedibilidad, 
                            string numero_oficio_procedibilidad, 
                            DateTime fecha_presentacion, 
                            decimal cuantia, 
                            string numero_carpeta_investigacion, 
                            string agente_ministerio_publico,  
                            string Rfc
        )
        {
            AsuntosPenalesProcedibilidad entityProcedibilidad = new()
            {
                id_asunto_penal = entity.id,
                id_requisito_procedibilidad = id_requisito_procedibilidad,
                numero_oficio_procedibilidad = numero_oficio_procedibilidad,
                fecha_presentacion = fecha_presentacion,
                cuantia = cuantia,
                numero_carpeta_investigacion = numero_carpeta_investigacion,
                agente_ministerio_publico = agente_ministerio_publico,
                usuario_creacion = Rfc
            };
                return entityProcedibilidad;
        }

        public static void UpdateProcedibilidad(ref AsuntosPenalesProcedibilidad entityProcedibilidad,
        
                            ref AsuntosPenales entity, 
                            int id_requisito_procedibilidad, 
                            string numero_oficio_procedibilidad, 
                            DateTime fecha_presentacion, 
                            decimal cuantia, 
                            string numero_carpeta_investigacion, 
                            string agente_ministerio_publico,  
                            string usuarioModificacion

            )

        {        
            
                entityProcedibilidad.id_requisito_procedibilidad = id_requisito_procedibilidad;
                entityProcedibilidad.numero_oficio_procedibilidad = numero_oficio_procedibilidad;
                entityProcedibilidad.fecha_presentacion = fecha_presentacion;
                entityProcedibilidad.cuantia = cuantia;
                entityProcedibilidad.numero_carpeta_investigacion = numero_carpeta_investigacion;
                entityProcedibilidad.agente_ministerio_publico = agente_ministerio_publico;
                entityProcedibilidad.usuario_modificacion = usuarioModificacion;
        }

    }
}


