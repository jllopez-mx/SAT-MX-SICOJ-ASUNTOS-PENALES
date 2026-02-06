using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.Administrador
{
    public class AsuntosPenalesMedidasCauteralesEvents
    {
        public static AsuntosPenalesMedidasCautelares Create(
            AsuntosPenalesImputados entityImputados,
            int? idMedida,
            string usuario_creacion
        )
        {
            Guard.CatalogValue(ref idMedida, "Medida Cautelar");

            AsuntosPenalesMedidasCautelares entity =
                new()
                {
                    id_medida = idMedida.GetValueOrDefault(),
                    id_imputado = entityImputados.id,
                    usuario_creacion = usuario_creacion,
                };
            return entity;
        }


        public static void DeleteMedidaCautelar(ref AsuntosPenalesMedidasCautelares entity
        
        )
        {   

        }
    }
}