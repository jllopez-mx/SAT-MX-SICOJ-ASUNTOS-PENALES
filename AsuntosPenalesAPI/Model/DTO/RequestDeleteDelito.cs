using System.ComponentModel.DataAnnotations;

namespace AsuntosPenalesAPI.Model.DTO
{
     public class RequestDeleteDelito
    {
    [Required]
    public int id { get; set; }   
    [Required]
    public int id_asunto_penal { get; set; }   

       
    }

}
