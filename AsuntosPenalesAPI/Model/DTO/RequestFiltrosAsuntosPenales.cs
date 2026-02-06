namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestFiltrosAsuntosPenales
    {
        public List<string> ByNoAsuntoPenal { get; set; } = new()!;
        public List<string> ByFechaRecepcionDesde { get; set; } = new()!;
        public List<string> ByFechaRecepcionHasta { get; set; } = new()!;
        public List<string> ByFechaVencimientoDesde { get; set; } = new()!;
        public List<string> ByFechaVencimientoHasta { get; set; } = new()!;
        public List<string> ByOficioSolicitud { get; set; } = new()!;
        public List<string> ByNoExpedienteCadido { get; set; } = new()!;
        public List<string> ByPromovente { get; set; } = new()!;
        public List<string> ByIdTipoAsunto { get; set; } = new()!;
        public List<string> ByIdEstadoTarea { get; set; } = new()!;
        public List<string> ByIdEstadoProcesal { get; set; } = new()!;
        public List<string> ByIdUnidadRealizoSolicitud { get; set; } = new()!;
        public List<string> ByAdminControl { get; set; } = new()!;
        public List<string> BySubadministracion { get; set; } = new()!;
        public List<string> ByNombreAbogado { get; set; } = new()!;
        public List<string> ByTipoUnidad { get; set; } = new()!;
        public List<string> ByIdAsuntoPenal { get; set; } = new()!;
        public List<string> ByIdTipoConclusión { get; set; } = new()!;
        public List<string> ByFechaConclusionDesde { get; set; } = new()!;
        public List<string> ByFechaConclusionHasta { get; set; } = new()!;
        public List<string> ByDeterminacionAsunto { get; set; } = new()!;
        public List<string> ByRequisitosProcedibilidad { get; set; } = new()!;
        public List<string> ByDelito { get; set; } = new()!;
        public List<string> ByTipoSolucionAlterna { get; set; } = new()!;
        public List<string> ByFechaPresentaciónRequisito { get; set; } = new()!;
        public List<string> ByFechaDelAutoVinculacion { get; set; } = new()!;
        public List<string> ByFechaEmisionSentencia{ get; set; } = new()!;

    }
}