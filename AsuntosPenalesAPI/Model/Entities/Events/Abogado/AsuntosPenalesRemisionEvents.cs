using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesRemisionEvents
    {
        public static AsuntosPenalesRemision Create(

            int? id_asunto_penal,
            int? id_tipo_autoridad,
            int? id_unidad_administrativa_remite,
            int? id_unidad_administrativa_recibe,
            int? id_unidad_administrativa_externa,
            DateTime fecha_oficio,
            string numero_oficio,            
            string usuarioCreacion
        )
        {
            Guard.CatalogValue(ref id_asunto_penal, "Unidad administrativa Central");
            Guard.CatalogValue(ref id_tipo_autoridad, "Tipo autoridad");
            Guard.CatalogValue(ref id_unidad_administrativa_remite, "Unidad administrativa que remite");
            Guard.CatalogValue(ref id_unidad_administrativa_recibe, "Unidad administrativa que recibe", id_unidad_administrativa_recibe.GetValueOrDefault() == EnumTipoAutoridad.Interna.GetHashCode());
            Guard.CatalogValue(ref id_unidad_administrativa_externa, "Unidad administrativa externa", id_unidad_administrativa_externa.GetValueOrDefault() == EnumTipoAutoridad.Externa.GetHashCode());

            AsuntosPenalesRemision entityRemision = new()
            {
                id_asunto_penal = id_asunto_penal.GetValueOrDefault(),
                id_tipo_autoridad = id_tipo_autoridad.GetValueOrDefault(),
                id_unidad_administrativa_remite = id_unidad_administrativa_remite.GetValueOrDefault(),
                id_unidad_administrativa_recibe = id_unidad_administrativa_recibe,                
                id_unidad_administrativa_externa = id_unidad_administrativa_externa,
                fecha_oficio = fecha_oficio,
                numero_oficio = numero_oficio,                
                usuario = usuarioCreacion
            };

            return entityRemision;
        }
    }
}