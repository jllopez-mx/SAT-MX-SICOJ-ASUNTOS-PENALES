using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestUpdateEtapaInvestigacionInicial
    {
        public int id { get; set; }
        public int idAsuntoPenal { get; set; }
        public bool concluyeInvestigacion { get; set; }
        public int? idTerminacionInvestigacion { get; set; }
        public string? fechaTerminacionInvestigacion { get; set; }

        public bool solucionAlterna { get; set; }
        public int? idTipoSolucionAlterna { get; set; }

        public string? condicionesAcuerdoReparatorio { get; set; }
        public string? fechaAutorizacionAcuerdoReparatorio { get; set; }
        public decimal? reparacionDañoAcuerdoReparatorio { get; set; }
        public string? fechaCelebracionAcuerdoReparatorio { get; set; }
        public bool? conclusionAsuntoAcuerdoReparatorio { get; set; }
        public string? fechaConclusionAcuerdoReparatorio { get; set; }
        
        public string? condicionesCriterioOportunidad { get; set; }
        public string? fechaCriterioOportunidad { get; set; }
        public bool? conclusionAsuntoCriterioOportunidad{ get; set; }
        public string? fechaConclusionCriterioOportunidad { get; set; }
        
        public string fechaSolicitudAudienciaInicial { get; set; } = null!;
        public int idCentroJusticia { get; set; }
        public string causaPenal { get; set; } = null!;
        public string fechaAudienciaInicial { get; set; } = null!;
        public bool autoVinculacionProceso { get; set; }
        public string? fechaAutoVinculacion { get; set; }
        public bool ordenAprehension { get; set; }
        public string? fechaOrdenAprehension { get; set; }

        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
}
