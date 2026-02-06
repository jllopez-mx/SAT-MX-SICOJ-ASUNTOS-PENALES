using AmparoDirectoAPI.Model.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{    public class ResponseArchivosAsuntosPenales
    {
        public int Id { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; } = null!;
        public int? IdAsuntoPenal { get; set; }
        public int? IdSeccion { get; set; }
        public string Seccion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string TamanoDocumento { get; set; } = null!;
        public int? IdRenglonSeccion { get; set; }
        public bool Permanente { get; set; }  
        
        //public string content_type { get; set;} = null!;
        //public string? owner_name { get; set;}
        //public DateTime fecha_creacion { get; set;}
        //public DateTime? fecha_modificacion { get; set;}
        //public bool activo { get; set; }
        //public string? size { get; set; }



    }
}

