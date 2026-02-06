namespace AsuntosPenalesAPI.Model.Entities.Events.Administrador
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