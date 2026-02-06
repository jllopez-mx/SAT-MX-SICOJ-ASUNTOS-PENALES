namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestDocumentoUpdate
    {
        public IFormFile documento { get; set; } = null!;
        public int idAsuntoPenal { get; set; }
        public int idTipoArchivo { get; set; }
        public int id { get; set; }
    }
}