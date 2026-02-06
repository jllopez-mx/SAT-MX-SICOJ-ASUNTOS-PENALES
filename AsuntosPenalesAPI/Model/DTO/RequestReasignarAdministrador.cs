namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestReasignarAdministrador
    {
        public List<int> idList { get; set; } = null!;
        public string? rfcAdministrador { get; set; }
        public string? rfcAbogado { get; set; }
    }
}
