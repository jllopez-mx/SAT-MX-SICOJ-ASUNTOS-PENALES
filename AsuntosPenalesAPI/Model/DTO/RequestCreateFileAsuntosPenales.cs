
namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestCreateFileAsuntosPenales
    {     
        public IFormFile FileAsuntosPenales { get; set; } = null!;   
        public int idAsuntosPenales { get; set; }
        public int idTipoDocumento { get; set; }=new();
        public int idSeccion { get; set; }
        public int? idRenglonSeccion { get; set; }
    }
}