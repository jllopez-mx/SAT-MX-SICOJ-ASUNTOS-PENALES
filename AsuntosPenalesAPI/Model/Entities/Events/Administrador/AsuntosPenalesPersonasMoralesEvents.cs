using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.Administrador
{
    public class AsuntosPenalesPersonasMoralesEvents
    {
        public static AsuntosPenalesPersonasMorales Create(

            ref AsuntosPenales entity, 
            string? rfc, 
            bool contribuyente, 
            string? nombre,
            string Rfc    
        )
        {
            Guard.ValidateStringRfc(ref rfc, "RFC contribuyente");
            Guard.ValidateStringAlphanumeric(ref nombre, "Contribuyente");
            AsuntosPenalesPersonasMorales entityPersonasMorales = new()
            {
            id_asunto_penal = entity.id, 
            rfc = rfc!, 
            contribuyente = contribuyente, 
            nombre = nombre!, 
            usuario_creacion = Rfc
            }; 

            return entityPersonasMorales; 

        }
    }
}