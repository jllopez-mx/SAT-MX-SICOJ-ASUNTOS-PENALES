namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesImputados
    {
        public int id { get; set; }
        public int id_asunto_penal { get; set; }
        public string rfc { get; set; } = null!;
        public bool contribuyente { get; set; }
        public string nombre { get; set; } = null!;
        public string usuario_creacion { get; set; } = null!;
        public DateTime fecha_creacion { get; set; }
        public int id_estado_procesal { get; set; }
        public int? id_estado_tarea { get; set; }
        public bool? concluye_investigacion { get; set; }
        public int? id_terminacion_investigacion { get; set; }
        public DateTime? fecha_terminacion_investigacion { get; set; }
        public bool? solucion_alterna { get; set; }
        public string? condiciones { get; set; }
        public DateTime? fecha_autorizacion_acuerdo_reparatorio { get; set; }
        public decimal? reparacion_daño { get; set; }
        public DateTime? fecha_celebracion_acuerdo_reparatorio { get; set; }
        public bool? conclusion_asunto { get; set; }
        public DateTime? fecha_conclusion { get; set; }
        public DateTime? fecha_criterio_oportunidad { get; set; }
        public int? id_tipo_solucion_alterna { get; set; }
        public DateTime? fecha_solicitud_audiencia_inicial { get; set; }
        public int? id_centro_justicia { get; set; }
        public string? causa_penal { get; set; }
        public DateTime? fecha_audiencia_inicial { get; set; }
        public bool? auto_vinculacion_proceso { get; set; }
        public DateTime? fecha_auto_vinculacion { get; set; }
        public bool? orden_aprehension { get; set; }
        public DateTime? fecha_orden_aprehension { get; set; }

        #region Etapa Complementaria
        public DateTime? fecha_plazo_investigacion_complementaria { get; set; }
        public bool? procedimiento_abreviado_cp { get; set; }
        public bool? solucion_alterna_cp { get; set; }
        public int? id_tipo_solucion_alterna_cp { get; set; }
        public bool? escrito_acusacion_cp { get; set; }
        public DateTime? fecha_escrito_acusacion_cp { get; set; }

        #endregion

        #region Etapa Intermedia

        public DateTime? fecha_audiencia_intermedia { get; set; }
        public bool? auto_apertura_juicio_oral { get; set; }
        public bool? solucion_alterna_in { get; set; }
        public int? id_tipo_solucion_alterna_in { get; set; }
        public bool? procedimiento_abreviado_in { get; set; }
        public DateTime? fecha_apertura_juicio_oral { get; set; }
        #endregion

        #region Etapa Juicio
        public DateTime? fecha_inicial_audiencia_juicio { get; set; }
        public DateTime? fecha_final_audiencia_juicio { get; set; }
        public DateTime? fecha_incial_audiencia_juicio { get; set; }

        #endregion

        public DateTime? fecha_modificacion { get; set; }
        public string? usuario_modificacion { get; set; }
        public bool activo { get; set; }
    }
}
