using Sicoj.Utils.ViewModels;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class ResponseImputados
    {
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool? contribuyente { get; set; } 
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public bool? concluyeInvestigacion { get; set; }
        public string? terminacionInvestigacion { get; set; }
        public int? idTerminacionInvestigacion {get; set;}
        public DateTime? fechaTerminacionInvestigacion { get; set; }
        public bool? solucionAlterna { get; set; } 
        public string? tipoSolucionAlterna { get; set; }
        public int? idTipoSolucionAlterna {get; set;}
        public DateTime? fechaSolicitudAudienciaInicial { get; set; }  
        public string? centroJusticia { get; set; }
        public int? idCentroJusticia {get; set;}
        public string causaPenal { get; set; } = null!;
        public DateTime? fechaAudienciaInicial { get; set; }  
        public bool? autoVinculacionProceso { get; set; } 
        public DateTime? fechaAutoVinculacion { get; set; }  
        public bool? ordenAprehension { get; set; } 
        public DateTime? fechaOrdenAprehension { get; set; }  
        public bool? procedimientoAbreviadoCp { get; set; } 
        public bool? solucionAlternaCp { get; set; } 
        public string? tipoSolucionAlternaCp  { get; set; }
        public int? idTipoSolucionAlternaCp {get; set;}
        public bool? escritoAcusacionCp { get; set; } 
        public string? fechaEscritoAcusacionCp { get; set; } 
        public ResponseAcuerdoReparatorio? acuerdoReparatorio { get; set; }  
        public ResponseCriterioOportunidad? citerioOportunidad { get; set; }
        public ResponseSentencia? sentencia {get; set; } 
        public ResponseSobreseimiento? sobreseimiento {get; set; } 
        public ResponseSuspencionCondicional? suspencionCondicional {get; set; } 


    }

    public class ResponseImputadosEtapaInicial
    {
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool? contribuyente { get; set; } 
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public bool? concluyeInvestigacion { get; set; }
        public string? terminacionInvestigacion { get; set; }
        public int? idTerminacionInvestigacion {get; set;}
        public DateTime? fechaTerminacionInvestigacion { get; set; }
        public bool? solucionAlterna { get; set; } 
        public string? tipoSolucionAlterna { get; set; }
        public int? idSolucionAlterna {get; set;}
        public DateTime? fechaSolicitudAudienciaInicial { get; set; }  
        public string? centroJusticia { get; set; }
        public int? idCentroJusiticia {get; set;}
        public string causaPenal { get; set; } = null!;
        public DateTime? fechaAudienciaInicial { get; set; }  
        public bool? autoVinculacionProceso { get; set; } 
        public DateTime? fechaAutoVinculacion { get; set; }  
        public bool? ordenAprehension { get; set; } 
        public DateTime? fechaOrdenAprehension { get; set; }  
        public ResponseAcuerdoReparatorio? acuerdoReparatorio { get; set; }  
        public ResponseCriterioOportunidad? citerioOportunidad { get; set; }

    }

    public class ResponseImputadosEtapaComplementaria
    {
        public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool? contribuyente { get; set; } 
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public bool? procedimientoAbreviadoCp { get; set; } 
        public bool? solucionAlternaCp { get; set; } 
        public string? tipoSolucionAlternaCp { get; set; }
        public int? idTipoSolucionAlternaCp {get; set;}
        public bool? escritoAcusacionCp { get; set; } 
        public string? fechaEscritoAcusacionCp { get; set; } 
        public string? fechaPlazoInvestigacionCp { get; set; } 
        public string? tipoSentencia { get; set; }
        public int? idTipoSentencia {get; set;}
        public ResponseAcuerdoReparatorio? acuerdoReparatorio { get; set; }  
        public ResponseSentencia? sentencia {get; set; } 
        public ResponseSobreseimiento? sobreseimiento {get; set; } 
        public ResponseSuspencionCondicional? suspencionCondicional {get; set; } 
        

    }
    
    public class ResponseImputadosEtapaIntermedia
    {
         public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool? contribuyente { get; set; } 
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public string? fechaAudienciaIntermedia { get; set; } 
        public bool? autoAperturaJuicioOral {get; set;}   
        public string? fechaAperturaJuicioOral { get; set; } 
        public bool? procedimientoAbreviadoIn {get; set;} 
        public bool? solucionAlternaIn {get; set;}   
        public string? tipoSolucionAlternaIn { get; set; }
        public int? idTipoSolucionAlternaIn {get; set;}
        public ResponseAcuerdoReparatorio? acuerdoReparatorio { get; set; }       
        public ResponseSentencia? sentencia {get; set; } 
        public ResponseSobreseimiento? sobreseimiento {get; set; } 
        public ResponseSuspencionCondicional? suspencionCondicional {get; set; } 

    }

    public class ResponseImputadosEtapaJuicio
    {
         public int Id { get; set; }
        public int idAsuntoPenal { get; set; }
        public string rfc { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public bool? contribuyente { get; set; } 
        public string? estadoProcesal { get; set; }
        public int? idEstadoProcesal {get; set;}
        public string? estadoTarea { get; set; }
        public int? idEstadoTarea {get; set;}
        public string? fechaIncialAudienciaJuicio { get; set; } 
        public string? fechaFinalAudienciaJuicio { get; set; } 
        public ResponseSentencia? sentencia {get; set; } 

    }

    public class ResponseSentencia 
    {
                public int Id { get; set; }
                public int idImputado { get; set; }
                public string? tipoSentencia { get; set; }
                public int? idTipoSentencia {get; set;} 
                public string? fechaEmisionSentencia { get; set; }
                public decimal reparacionDanioSentencia { get; set; }
                public bool? cumplimientoPrivadaLibertad { get; set; }
                public int? anios { get; set; }
                public int? meses { get; set; }
                public int? dias { get; set; }
                public string? otorgamientoBeneficios { get; set; }
                public string? accionesEjecucion { get; set; }
                public string? fechaEjecucion { get; set; }
                public bool? conclusionAsuntoSentencia { get; set; }
                public string? fechaConclusionSentencia { get; set; }
    
    }

    public class ResponseAcuerdoReparatorio
    {
        public int id { get; set; }
        public int idImputado { get; set; }
        public string condicionesAcuerdoReparatorio { get; set; } = null!;
        public string fechaAutorizacionAcuerdoReparatorio { get; set; } = null!;
        public decimal? reparacionDañoAcuerdoReparatorio { get; set; }
        public string fechaCelebracionAcuerdoReparatorio { get; set; } = null!;
        public bool? conclusionAsuntoAcuerdoReparatorio { get; set; }
        public string? fechaConclusionAcuerdoReparatorio { get; set; }
    }

    public class ResponseSobreseimiento
    {
        public int id { get; set; }
        public int idImputado { get; set; }
        public bool? solicitudSobreseimiento { get; set; }
        public string? fechaDeterminacionSobreseimiento { get; set; }
        public bool? conclusionAsuntoSobreseimiento { get; set; }
        public string? fechaConclusionSobreseimiento { get; set; }
    }

    public class ResponseSuspencionCondicional 
    {

        public int id { get; set; }
        public int idImputado { get; set; }             
        public string? condicionesSuspencion { get; set; }
        public string? fechaCelebracionSuspencion { get; set; }
        public decimal? reparacionDañoSuspencion { get; set; }
        public string? fechaPlazoSuspencion { get; set; }
        public string? fechaCumplimientoSuspencion { get; set; }
        public bool? conclusionAsuntoSuspencion { get; set; }
        public string? fechaConclusionSuspencion { get; set; }

    }

    public class ResponseCriterioOportunidad
    {
        public int id { get; set; }
        public int idImputado { get; set; }
        public string condicionesCriterioOportunidad { get; set; } = null!;
        public string fechaCriterioOportunidad { get; set; } = null!;
        public bool? conclusionAsuntoCriterioOportunidad{ get; set; }
        public string? fechaConclusionCriterioOportunidad { get; set; }
    }
}



