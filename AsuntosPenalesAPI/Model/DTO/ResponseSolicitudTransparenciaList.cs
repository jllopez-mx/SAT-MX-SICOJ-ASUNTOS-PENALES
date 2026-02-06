namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseSolicitudTransparenciaList
    {
    public int id { get; set; }
    public int id_rol { get; set; }
    public string? nombre_rol { get; set; } = null!;
    public int id_asunto { get; set; }
    public string? no_solicitud { get; set; } = null!;
    public string? fecha_solicitud { get; set; }
    public string? fecha_registro { get; set; }
    public string? fecha_modificacion { get; set; }
    }
}