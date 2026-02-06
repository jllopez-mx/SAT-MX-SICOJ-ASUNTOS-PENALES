using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequesEtapaInvestigacionInicial
    {
        public int id { get; set; }
        public int idImputado  { get; set; }
	    public int idAsuntoPenal { get; set; }
	    // public string rfc { get; set; } = null!;
	    // public bool contribuyente { get; set; } 
	    // public string nombre { get; set; } = null!;
	    public bool concluyeInvestigacion { get; set; }
        public int idTerminacionInvestigacion {get; set;}
	    public string fechaTerminacionInvestigacion {get; set;} = null!;
	    public bool solucionAlterna {get; set;} 
	    public string condiciones {get; set;} = null!; 
	    public string fechaAutorizacionAcuerdoReparatorio {get; set;} = null!;
	    public decimal reparacionDaño {get; set;}
        public string fechaCelebracionAcuerdoReparatorio {get; set;} = null!;
	    public bool conclusionAsunto {get; set;} 
 	    public string fechaConclusion {get; set;}= null!;
	    public string fechaCriterioOportunidad {get; set;} = null!;
	    public int idTipoSolucionAlterna  {get; set;} 
	    public string fechaSolicitudAudienciaInicial {get; set;}  = null!;
	    public int idCentroJusticia {get; set;} 
	    public string causaPenal {get; set;}  = null!;
	    public string fechaAudienciaInicial  {get; set;} = null!;
	    public bool autoVinculacionProceso  {get; set;} 
	    public string fechaAutoVinculacion  {get; set;} = null!;
        public bool ordenAprehension  {get; set;} 
	    public string fechaOrdenAprehension  {get; set;}= null!;

        public IFormFile? documento { get; set; }
        public int? idTipoArchivo { get; set; }
        public int? idSeccion { get; set; }
    }
}