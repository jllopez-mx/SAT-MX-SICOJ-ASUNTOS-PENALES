using System.ComponentModel.DataAnnotations;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateReactivar
    {
        [Required]
        public int id { get; set; }
    
    }
}