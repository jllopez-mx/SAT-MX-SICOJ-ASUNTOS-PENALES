using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.ViewModels.Enums;


namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesDelitosEvents 
    {
        public static AsuntosPenalesDelitos Create(
    
        ref AsuntosPenales entity,        
        int id_delito, 
        string Rfc
        )
        {
            AsuntosPenalesDelitos entityDelitos = new()
            {
                id_asunto_penal = entity.id,
                id_delito = id_delito, 
                usuario_creacion = Rfc
            };
            return entityDelitos;
        }
    }
}