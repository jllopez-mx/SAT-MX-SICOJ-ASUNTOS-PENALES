using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;


namespace AsuntosPenalesAPI.Model.DTO
{

    public class RequestUpdateAsuntosPenales
    {
        [Required]
        public int id { get; set; }
        public string numero_asunto_penal { get; set; } = null!;
        public string oficio_solicitud { get; set;} = null!;

        public string fecha_recepcion { get; set; } = null!;

        public string numero_expediente_cadido { get; set; } = null!;

        public string fecha_vencimiento { get; set; } = null!;

        public int id_unidad_realiza_solicitud { get; set; }
    
        public int id_admin_controla {get; set; }

        public bool interno { get; set; }
 
    }
}
