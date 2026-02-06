using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.Entities.Events.Administrador
{
    public class AsuntosPenalesAbogadoAdministradorEvents
    {
        public static AsuntosPenalesAbogado Create(
            string? rfc_abogado,
            string usuarioCreacion
        )
        {
            Guard.ValidateStringRfc(ref rfc_abogado!, "Abogado");

            AsuntosPenalesAbogado entity = new()
            {
                id_abogado = rfc_abogado,
            };
            return entity;
        }
    }
}
