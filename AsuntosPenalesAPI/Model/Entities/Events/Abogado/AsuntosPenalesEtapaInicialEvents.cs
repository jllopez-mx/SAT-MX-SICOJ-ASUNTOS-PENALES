
using System; 
using System.Collections.Generic; 
using System.Linq; 
using Sicoj.Utils; 
 
using System.Threading.Tasks; 
using AsuntosPenalesAPI.Model.ViewModels.Enums; 
 
 
namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado 
{ 
    public class AsuntosPenalesEtapaInicialEvents 
    { 
        public static AsuntosPenalesImputados Create( 
 
            ref AsuntosPenales entity,  
            string? rfc,  
            bool contribuyente,  
            string? nombre, 
            string Rfc     
        ) 
        { 
             
            Guard.ValidateStringRfc(ref rfc, "RFC contribuyente"); 
            Guard.ValidateStringAlphanumeric(ref nombre, "Contribuyente"); 
     
 
            AsuntosPenalesImputados entityImputados = new() 
            { 
            id_asunto_penal = entity.id,  
            rfc = rfc!,  
            contribuyente = contribuyente,  
            nombre = nombre!,  
            usuario_creacion = Rfc, 
            id_estado_procesal = EnumEstadoProcesal.INVESTIGACION_INICIAL.GetHashCode(), 
 
            };  
 
            return entityImputados;  
 
        } 
 
        // public static void UpdateImputados( 
        //             ref  AsuntosPenalesImputados entity, 
        //                 bool concluye_investigacion,  
        //                 int id_terminacion_investigacion,  
        //                 DateTime fecha_terminacion_investigacion, 
        //                 bool solucion_alterna, 
        //                 string condiciones,  
        //                 DateTime fecha_autorizacion_acuerdo_reparatorio, 
        //                 decimal reparacion_daño,  
        //                 DateTime fecha_celebracion_acuerdo_reparatorio, 
        //                 bool conclusion_asunto,  
        //                 DateTime fecha_conclusion, 
        //                 DateTime fecha_criterio_oportunidad, 
        //                 int id_tipo_solucion_alterna,  
        //                 DateTime fecha_solicitud_audiencia_inicial, 
        //                 int id_centro_justicia,  
        //                 string causa_penal, 
        //                 DateTime fecha_audiencia_inicial, 
        //                 bool auto_vinculacion_proceso,  
        //                 DateTime fecha_auto_vinculacion, 
        //                 bool orden_aprehension, 
        //                 DateTime fecha_orden_aprehension, 
        //                 string usuario_modificacion 
 
        // ) 
        // { 
            
        //                 entity.concluye_investigacion = concluye_investigacion; 
        //                 entity.id_terminacion_investigacion = id_terminacion_investigacion;  
        //                 entity.fecha_terminacion_investigacion = fecha_terminacion_investigacion; 
        //                 entity.solucion_alterna = solucion_alterna; 
        //                 entity.condiciones = condiciones;  
        //                 entity.fecha_autorizacion_acuerdo_reparatorio = fecha_autorizacion_acuerdo_reparatorio; 
        //                 entity.reparacion_daño = reparacion_daño;  
        //                 entity.fecha_celebracion_acuerdo_reparatorio = fecha_celebracion_acuerdo_reparatorio; 
        //                 entity.conclusion_asunto = conclusion_asunto;  
        //                 entity.fecha_conclusion = fecha_conclusion; 
        //                 entity.fecha_criterio_oportunidad = fecha_criterio_oportunidad; 
        //                 entity.id_tipo_solucion_alterna = id_tipo_solucion_alterna;  
        //                 entity.fecha_solicitud_audiencia_inicial = fecha_solicitud_audiencia_inicial; 
        //                 entity.id_centro_justicia = id_centro_justicia;  
        //                 entity.causa_penal = causa_penal; 
        //                 entity.fecha_audiencia_inicial = fecha_audiencia_inicial; 
        //                 entity.auto_vinculacion_proceso = auto_vinculacion_proceso;  
        //                 entity.fecha_auto_vinculacion = fecha_auto_vinculacion; 
        //
        //                 entity.fecha_orden_aprehension = fecha_orden_aprehension; 
        //                 entity. usuario_modificacion = usuario_modificacion; 
 
        // } 
    } 
 
 
}
