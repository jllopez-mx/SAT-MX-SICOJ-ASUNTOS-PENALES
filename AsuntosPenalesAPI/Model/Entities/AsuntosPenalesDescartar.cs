namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesDescartar
    {
        public int id { get; set; }
        public int p_id_asunto_penal { get; set; }
        public int id_imputado { get; set; }
        public int id_seccion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string? usuario_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
        public int id_estado_procesal { get; set; }
    }
}
