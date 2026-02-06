using System.ComponentModel.DataAnnotations;


namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestDeleteAmparo
    {
    [Required]
    public int id { get; set; }   
    [Required]
    public int id_imputado { get; set; }   
    public int id_asunto_penal { get; set; }   

    }
}