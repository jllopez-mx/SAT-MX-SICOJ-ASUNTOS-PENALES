namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesSobreseimiento
    {
        public int id { get; set; }
        public int id_imputado { get; set; }
            public int id_tipo_etapa_investigacion { get; set; }  

        public bool solicitud_sobreseimiento { get; set; }
        public DateTime fecha_determinacion_sobreseimiento { get; set; }
        public bool conclusion_asunto { get; set; }
        public DateTime? fecha_conclusion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;        
        public string? usuario_modificacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
    }
}
