namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesSuspensionCondicional
    {
        public int id { get; set; }
        public int id_imputado { get; set; }
        public int id_tipo_etapa_investigacion { get; set; }  
        public string condiciones { get; set; } = null!;
        public DateTime fecha_celebracion { get; set; }
        public decimal? reparacion_daño { get; set; }
        public DateTime fecha_plazo { get; set; }
        public DateTime fecha_cumplimiento { get; set; }        
        public bool conclusion_asunto { get; set; }
        public DateTime? fecha_conclusion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_creacion { get; set; } = null!;        
        public string? usuario_modificacion { get; set; } = null!;
        public DateTime? fecha_modificacion { get; set; }
    }
}