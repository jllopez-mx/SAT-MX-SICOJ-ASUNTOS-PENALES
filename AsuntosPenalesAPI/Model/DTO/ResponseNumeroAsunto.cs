
namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseNumeroAsunto
    {
        public int id_asunto { get; set; }
        public string numero_asunto { get; set; } = null!;
        public int id_unidad_administrativa { get; set; }
        public string nombre_unidad { get; set; } = null!;
    }
}