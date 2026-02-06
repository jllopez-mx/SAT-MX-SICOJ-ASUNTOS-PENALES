using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class FiltersAsuntosPenalesRequest
    {
        public List<string> ByNoAsunto { get; set; } = new()!;
        public List<string> ByFechaRecepcionDesde { get; set; } = new()!;
        public List<string> ByFechaRecepcionHasta { get; set; } = new()!;
        public List<string> ByFechaVencimientoDesde { get; set; } = new()!;
        public List<string> ByFechaVencimientoHasta { get; set; } = new()!;
        public List<string> ByOficioSolicitud { get; set; } = new()!;
        public List<string> ByNoExpediente{ get; set; } = new()!;
        public List<string> ByIdUnidadRealizaSolicitud { get; set; } = new()!;
        public List<string> ByIdAdministracion { get; set; } = new()!;
        public List<string> ByIdSubadministracion { get; set; } = new()!;
        public List<string> ByNombreAbogado { get; set; } = new()!;
        public List<string> ByIdEstadoTarea { get; set; } = new()!;
        public List<string> ByIdEstadoProcesal { get; set; } = new()!;
    }
}