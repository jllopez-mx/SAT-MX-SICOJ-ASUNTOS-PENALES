using System.ComponentModel.DataAnnotations;


namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestImputados
    {
            public int idAsuntoPenal { get; set; }
            public string rfc {get; set; } = null!;
            public bool contribuyente  {get; set;}
            public string nombre {get; set;} = null!;

            


    }
}