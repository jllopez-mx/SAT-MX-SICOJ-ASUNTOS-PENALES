using System.ComponentModel.DataAnnotations;


namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestReasignarAsuntosPenales
    {
        public List<int> idList { get; set; } = null!;
        public string idAbogado { get; set; } = null!;
        

    }
}