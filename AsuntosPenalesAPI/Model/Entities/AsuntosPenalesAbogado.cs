
namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesAbogado
    {
        public int id { get; set; }
        public int id_asunto_penal { get; set; }
        public string id_abogado { get; set; } = null!;
        public DateTime fecha_asignacion { get; set; }
        public bool remitido { get; set; }
        public bool reasingado { get; set; }

    }
}