
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateTurnarAsuntosPenales
    {
    [Required]
        public int id { get; set; }      
        public string numero_empleado { get; set; } = null!; 
        public int id_admin_controla { get; set; }

    }
}