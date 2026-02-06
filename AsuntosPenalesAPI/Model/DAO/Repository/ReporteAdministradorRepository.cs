using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using NpgsqlTypes;
using Sicoj.Utils.Postgres;

namespace AsuntosPenalesAPI.Model.DAO.Repository
{
    public class ReporteAdministradorRepository : IReporteAdministradorRepository
    {
        #region Variables
        private readonly ISqlTools _database;
        #endregion

        #region Constructor
        public ReporteAdministradorRepository(ISqlTools database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        #endregion

        public async Task<List<ResponseReporteGeneral>> ExportarReporteGeneralAsyncRepository(
        string? noAsunto,
        DateTime? fechaRecepcionDesde,
        DateTime? fechaRecepcionHasta,
        DateTime? fechaVencimientoDesde,
        DateTime? fechaVencimientoHasta,
        string? oficioSolicitud,
        string? NoExpedienteCadido,
        List<int>? UnidadRealizaSolicitud,
        int? idAdministracion,
        int? idSubadministracion,
        string? idAbogadoAsigno,
        List<int>? EstadoTarea,
        List<int>? EstadoProcesal,
        List<int>? TipoConclusion,
        DateTime? fechaConclusionDesde,
        DateTime? fechaConclusionHasta,
        List<int>? DeterminacionAsunto,
        List<int>? RequisitosProcedibilidad,
        List<int>? Delito,
        int? TipoSolucionAlterna,
        DateTime? FechaPresentacionRequisito,
        DateTime? FechaDelAutoVinculacion,
        DateTime? FechaEmisionSentencia
        )
        {


            ParameterPGsql[] parameters =
            {

                new ParameterPGsql("p_no_asunto", NpgsqlDbType.Text, noAsunto),
                new ParameterPGsql("p_fecha_recepcion_desde", NpgsqlDbType.Date, fechaRecepcionDesde),
                new ParameterPGsql("p_fecha_recepcion_hasta", NpgsqlDbType.Date, fechaRecepcionHasta),
                new ParameterPGsql("p_fecha_vencimiento_desde", NpgsqlDbType.Date, fechaVencimientoDesde),
                new ParameterPGsql("p_fecha_vencimiento_hasta", NpgsqlDbType.Date, fechaVencimientoHasta),
                new ParameterPGsql("p_oficio_solicitud", NpgsqlDbType.Varchar, oficioSolicitud),
                new ParameterPGsql("p_no_expediente_cadido", NpgsqlDbType.Varchar, NoExpedienteCadido),
                new ParameterPGsql("p_id_unidad_realiza_solicitud", NpgsqlDbType.Array | NpgsqlDbType.Integer, (UnidadRealizaSolicitud is null || !UnidadRealizaSolicitud.Any()) ? DBNull.Value :  UnidadRealizaSolicitud.ToArray()),
                new ParameterPGsql("p_id_administracion", NpgsqlDbType.Integer, idAdministracion),
                new ParameterPGsql("p_id_subadministracion", NpgsqlDbType.Integer, idSubadministracion),
                new ParameterPGsql("p_id_abogado_asigno", NpgsqlDbType.Text, idAbogadoAsigno),
                new ParameterPGsql("p_id_estado_tarea", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoTarea is null || !EstadoTarea.Any()) ? DBNull.Value :  EstadoTarea.ToArray()),
                new ParameterPGsql("p_id_estado_procesal", NpgsqlDbType.Array | NpgsqlDbType.Integer, (EstadoProcesal is null || !EstadoProcesal.Any()) ? DBNull.Value :  EstadoProcesal.ToArray()),
                new ParameterPGsql("p_id_tipo_conclusion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (TipoConclusion is null || !TipoConclusion.Any()) ? DBNull.Value :  TipoConclusion.ToArray()),
                new ParameterPGsql("p_fecha_conclusion_desde", NpgsqlDbType.Date, fechaConclusionDesde),
                new ParameterPGsql("p_fecha_conclusion_hasta", NpgsqlDbType.Date, fechaConclusionHasta),
                new ParameterPGsql("p_id_determinacion", NpgsqlDbType.Array | NpgsqlDbType.Integer, (DeterminacionAsunto is null || !DeterminacionAsunto.Any()) ? DBNull.Value :  DeterminacionAsunto.ToArray()),
                new ParameterPGsql("p_id_requisitos_procedibilidad", NpgsqlDbType.Array | NpgsqlDbType.Integer, (RequisitosProcedibilidad is null || !RequisitosProcedibilidad.Any()) ? DBNull.Value :  RequisitosProcedibilidad.ToArray()),
                new ParameterPGsql("p_id_delito", NpgsqlDbType.Array | NpgsqlDbType.Integer, (Delito is null || !Delito.Any()) ? DBNull.Value :  Delito.ToArray()),
                new ParameterPGsql("p_id_tipo_solucion_altera", NpgsqlDbType.Integer, TipoSolucionAlterna),
                new ParameterPGsql("p_fecha_presentacion_requisitos", NpgsqlDbType.Date, FechaPresentacionRequisito),
                new ParameterPGsql("p_fecha_auto_vinculacion", NpgsqlDbType.Date, FechaDelAutoVinculacion),
                new ParameterPGsql("p_fecha_emision_sentencia", NpgsqlDbType.Date, FechaEmisionSentencia),
            };

            var response = await _database.ExecuteFunctionAsync(
                EnumFunctions.GetReporteGeneral,
                     parameters!
             );
            if (response.ExisteError)
            {
                throw new Exception(response.Mensaje);
            }

            if (response.Data.Tables.Count <= 0 || response.Data.Tables[0].Rows.Count <= 0)
            {
                return null!;
            }

            List<ResponseReporteGeneral> resultList = new();

            foreach (DataRow item in response.Data.Tables[0].Rows)
            {
                resultList.Add(
                    new()
                    {
                        UnidadAdministrativa = item.IsNull(0) ? null! : item.Field<string>(0)!,
                        Subadministracion = item.IsNull(1) ? null! : item.Field<string>(1)!,
                        UnidadRealizaSolicitud = item.IsNull(2) ? null! : item.Field<string>(2)!,
                        EstadoProcesal = item.IsNull(3) ? null! : item.Field<string>(3)!,
                        NumeroAsunto = item.IsNull(4) ? null! : item.Field<string>(4)!,
                        FechaRecepcion = item.IsNull(5) ? null! : item.Field<DateTime>(5).ToString("yyyy-MM-dd")!,
                        NombreAbogado = item.IsNull(6) ? null! : item.Field<string>(6)!,
                        DeterminacionAsunto = item.IsNull(7) ? null! : item.Field<string>(7)!,
                        FechaDeterminacion = item.IsNull(8) ? null! : item.Field<DateTime>(8).ToString("yyyy-MM-dd")!,
                        RequisitoProcedibilidad = item.IsNull(9) ? null! : item.Field<string>(9)!,
                        FechaPresentacionRequisito = item.IsNull(10) ? null! : item.Field<DateTime>(10).ToString("yyyy-MM-dd")!,
                        PersonaMoral = item.IsNull(11) ? null! : item.Field<string>(11)!,
                        ContribuyentePersona = item.IsNull(12) ? null! : item.Field<string>(12)!,
                        RfcPersonaMoral = item.IsNull(13) ? null! : item.Field<string>(13)!,
                        Delito = item.IsNull(14) ? null! : item.Field<string>(14)!,
                        Cuantia = item.IsNull(15) ? null : (decimal?)item.Field<decimal>(15),
                        NumeroCarpetaInvestigacion = item.IsNull(16) ? null! : item.Field<string>(16)!,
                        AgenteMinisterioPublico = item.IsNull(17) ? null! : item.Field<string>(17)!,
                        NombreImputado = item.IsNull(18) ? null! : item.Field<string>(18)!,
                        ContribuyenteImputado = item.IsNull(19) ? null! : item.Field<string>(19)!,
                        RfcImputado = item.IsNull(20) ? null! : item.Field<string>(20)!,
                        FormaTerminacionInvestigacion = item.IsNull(21) ? null! : item.Field<string>(21)!,
                        FechaTerminacionInvestigacion = item.IsNull(22) ? null! : item.Field<DateTime>(22).ToString("yyyy-MM-dd")!,
                        SolucionAlternaIn = item.IsNull(23) ? null! : item.Field<string>(23)!,
                        CondicionesArInicial = item.IsNull(24) ? null! : item.Field<string>(24)!,
                        FechaAutorizacionArInicial = item.IsNull(25) ? null! : item.Field<DateTime>(25).ToString("yyyy-MM-dd")!,
                        ReparacionDanioArInicial = item.IsNull(26) ? null! : item.Field<string>(26)!,
                        FechaReparacionArInicial = item.IsNull(27) ? null! : item.Field<DateTime>(27).ToString("yyyy-MM-dd")!,
                        FechaConclusionArInicial = item.IsNull(28) ? null! : item.Field<DateTime>(28).ToString("yyyy-MM-dd")!,
                        CondicionesCo = item.IsNull(29) ? null! : item.Field<string>(29)!,
                        FechaCriterioOportunidad = item.IsNull(30) ? null! : item.Field<DateTime>(30).ToString("yyyy-MM-dd")!,
                        FechaConclusionCo = item.IsNull(31) ? null! : item.Field<DateTime>(31).ToString("yyyy-MM-dd")!,
                        FechaSolicitudAudienciaInicial = item.IsNull(32) ? null! : item.Field<DateTime>(32).ToString("yyyy-MM-dd")!,
                        CentroJusticia = item.IsNull(33) ? null! : item.Field<string>(33)!,
                        CausaPenal = item.IsNull(34) ? null! : item.Field<string>(34)!,
                        FechaAudienciaInicial = item.IsNull(35) ? null! : item.Field<DateTime>(35).ToString("yyyy-MM-dd")!,
                        FechaAutoVinculacion = item.IsNull(36) ? null! : item.Field<DateTime>(36).ToString("yyyy-MM-dd")!,
                        FechaOrdenAprehension = item.IsNull(37) ? null! : item.Field<DateTime>(37).ToString("yyyy-MM-dd")!,
                        TipoSentenciaComplementaria = item.IsNull(38) ? null! : item.Field<string>(38)!,
                        FechaEmisionSentenciaComplementaria = item.IsNull(39) ? null! : item.Field<DateTime>(39).ToString("yyyy-MM-dd")!,
                        ReparacionDanioSenComplementaria = item.IsNull(40) ? null! : item.Field<string>(40)!,
                        CumplimientoLibertadComplementaria = item.IsNull(41) ? null! : item.Field<string>(41)!,
                        AniosSenComplementaria = item.IsNull(42) ? null : item.Field<int?>(42),
                        MesesSenComplementaria = item.IsNull(43) ? null : item.Field<int?>(43),
                        DiasSenComplementaria = item.IsNull(44) ? null : item.Field<int?>(44),
                        OtorgamientoBeneficiosComplementaria = item.IsNull(45) ? null! : item.Field<string>(45)!,
                        AccionesEjecucionComplementaria = item.IsNull(46) ? null! : item.Field<string>(46)!,
                        FechaEjecucionComplementaria = item.IsNull(47) ? null! : item.Field<DateTime>(47).ToString("yyyy-MM-dd")!,
                        ConclusionAsuntoComplementaria = item.IsNull(48) ? null! : item.Field<string>(48)!,
                        FechaConclusionSenComplementaria = item.IsNull(49) ? null! : item.Field<DateTime>(49).ToString("yyyy-MM-dd")!,
                        SolucionAlternaCom = item.IsNull(50) ? null! : item.Field<string>(50)!,
                        CondicionesArComplementaria = item.IsNull(51) ? null! : item.Field<string>(51)!,
                        FechaAutorizacionArComplementaria = item.IsNull(52) ? null! : item.Field<DateTime>(52).ToString("yyyy-MM-dd")!,
                        ReparacionDanioArComplementaria = item.IsNull(53) ? null! : item.Field<string>(53)!,
                        FechaReparacionArComplementaria = item.IsNull(54) ? null! : item.Field<DateTime>(54).ToString("yyyy-MM-dd")!,
                        FechaConclusionArComplementaria = item.IsNull(55) ? null! : item.Field<DateTime>(55).ToString("yyyy-MM-dd")!,
                        FechaDeterminacionSobreseimientoCom = item.IsNull(56) ? null! : item.Field<DateTime>(56).ToString("yyyy-MM-dd")!,
                        SolicitudSobreseimientoCom = item.IsNull(57) ? null! : item.Field<string>(57)!,
                        FechaConclusionSobreseimientoCom = item.IsNull(58) ? null! : item.Field<DateTime>(58).ToString("yyyy-MM-dd")!,
                        CondicionesSuspensionComplementariaCom = item.IsNull(59) ? null! : item.Field<string>(59)!,
                        FechaCelebracionSuspensionComplementariaCom = item.IsNull(60) ? null! : item.Field<DateTime>(60).ToString("yyyy-MM-dd")!,
                        ReparacionDanioSuspensionComplementariaCom = item.IsNull(61) ? null! : item.Field<string>(61)!,
                        PlazoSuspensionComplementariaCom = item.IsNull(62) ? null : item.Field<int?>(62),
                        FechaCumplimientoSuspensionComplementariaCom = item.IsNull(63) ? null! : item.Field<DateTime>(63).ToString("yyyy-MM-dd")!,
                        FechaConclusionSuspensionComplementariaCom = item.IsNull(64) ? null! : item.Field<DateTime>(64).ToString("yyyy-MM-dd")!,
                        FechaEscritoAcusacionCp = item.IsNull(65) ? null! : item.Field<DateTime>(65).ToString("yyyy-MM-dd")!,
                        FechaAudienciaIntermedia = item.IsNull(66) ? null! : item.Field<DateTime>(66).ToString("yyyy-MM-dd")!,
                        FechaAperturaJuicioOral = item.IsNull(67) ? null! : item.Field<DateTime>(67).ToString("yyyy-MM-dd")!,
                        TipoSentenciaIntermedia = item.IsNull(68) ? null! : item.Field<string>(68)!,
                        FechaEmisionSentenciaIntermedia = item.IsNull(69) ? null! : item.Field<DateTime>(69).ToString("yyyy-MM-dd")!,
                        ReparacionDanioSenIntermedia = item.IsNull(70) ? null! : item.Field<string>(70)!,
                        CumplimientoLibertadIntermedia = item.IsNull(71) ? null! : item.Field<string>(71)!,
                        AniosSenIntermedia = item.IsNull(72) ? null : item.Field<int?>(72),
                        MesesSenIntermedia = item.IsNull(73) ? null : item.Field<int?>(73),
                        DiasSenIntermedia = item.IsNull(74) ? null : item.Field<int?>(74),
                        OtorgamientoBeneficiosIntermedia = item.IsNull(75) ? null! : item.Field<string>(75)!,
                        AccionesEjecucionIntermedia = item.IsNull(76) ? null! : item.Field<string>(76)!,
                        FechaEjecucionIntermedia = item.IsNull(77) ? null! : item.Field<DateTime>(77).ToString("yyyy-MM-dd")!,
                        ConclusionAsuntoIntermedia = item.IsNull(78) ? null! : item.Field<string>(78)!,
                        FechaConclusionSenIntermedia = item.IsNull(79) ? null! : item.Field<DateTime>(79).ToString("yyyy-MM-dd")!,
                        SolucionAlternaInt = item.IsNull(80) ? null! : item.Field<string>(80)!,
                        CondicionesArIntermedia = item.IsNull(81) ? null! : item.Field<string>(81)!,
                        FechaAutorizacionArIntermedia = item.IsNull(82) ? null! : item.Field<DateTime>(82).ToString("yyyy-MM-dd")!,
                        ReparacionDanioArIntermedia = item.IsNull(83) ? null! : item.Field<string>(83)!,
                        FechaReparacionArIntermedia = item.IsNull(84) ? null! : item.Field<DateTime>(84).ToString("yyyy-MM-dd")!,
                        FechaConclusionArIntermedia = item.IsNull(85) ? null! : item.Field<DateTime>(85).ToString("yyyy-MM-dd")!,
                        FechaDeterminacionSobreseimientoInt = item.IsNull(86) ? null! : item.Field<DateTime>(86).ToString("yyyy-MM-dd")!,
                        FechaConclusionSobreseimientoInt = item.IsNull(87) ? null! : item.Field<DateTime>(87).ToString("yyyy-MM-dd")!,
                        CondicionesSuspensionComplementariaInt = item.IsNull(88) ? null! : item.Field<string>(88)!,
                        FechaCelebracionSuspensionComplementariaInt = item.IsNull(89) ? null! : item.Field<DateTime>(89).ToString("yyyy-MM-dd")!,
                        ReparacionDanioSuspensionComplementariaInt = item.IsNull(90) ? null! : item.Field<string>(90)!,
                        PlazoSuspensionComplementariaInt = item.IsNull(91) ? null : item.Field<int?>(91),
                        FechaCumplimientoSuspensionComplementariaInt = item.IsNull(92) ? null! : item.Field<DateTime>(92).ToString("yyyy-MM-dd")!,
                        FechaConclusionSuspensionComplementariaInt = item.IsNull(93) ? null! : item.Field<DateTime>(93).ToString("yyyy-MM-dd")!,
                        FechaInicialAudienciaJuicio = item.IsNull(94) ? null! : item.Field<DateTime>(94).ToString("yyyy-MM-dd")!,
                        FechaFinalAudienciaJuicio = item.IsNull(95) ? null! : item.Field<DateTime>(95).ToString("yyyy-MM-dd")!,
                        TipoSentenciaJuicio = item.IsNull(96) ? null! : item.Field<string>(96)!,
                        FechaEmisionSentenciaJuicio = item.IsNull(97) ? null! : item.Field<DateTime>(97).ToString("yyyy-MM-dd")!,
                        ReparacionDanioSenJuicio = item.IsNull(98) ? null! : item.Field<string>(98)!,
                        CumplimientoLibertadJuicio = item.IsNull(99) ? null! : item.Field<string>(99)!,
                        AniosSenJuicio = item.IsNull(100) ? null : item.Field<int?>(100),
                        MesesSenJuicio = item.IsNull(101) ? null : item.Field<int?>(101),
                        DiasSenJuicio = item.IsNull(102) ? null : item.Field<int?>(102),
                        OtorgamientoBeneficiosJuicio = item.IsNull(103) ? null! : item.Field<string>(103)!,
                        AccionesEjecucionJuicio = item.IsNull(104) ? null! : item.Field<string>(104)!,
                        FechaEjecucionJuicio = item.IsNull(105) ? null! : item.Field<DateTime>(105).ToString("yyyy-MM-dd")!,
                        ConclusionAsuntoJuicio = item.IsNull(106) ? null! : item.Field<string>(106)!,
                        FechaConclusionSenJuicio = item.IsNull(107) ? null! : item.Field<DateTime>(107).ToString("yyyy-MM-dd")!,
                        FechaPresentacionApelacion = item.IsNull(108) ? null! : item.Field<DateTime>(108).ToString("yyyy-MM-dd")!,
                        NumeroTocaPenal = item.IsNull(109) ? null! : item.Field<string>(109)!,
                        TipoOrganoJurisdiccionalApelacion = item.IsNull(110) ? null! : item.Field<string>(110)!,
                        ResolucionApelacion = item.IsNull(111) ? null! : item.Field<string>(111)!,
                        FechaResolucionApelacion = item.IsNull(112) ? null! : item.Field<DateTime>(112).ToString("yyyy-MM-dd")!,
                        TipoAmparo = item.IsNull(113) ? null! : item.Field<string>(113)!,
                        FechaPresentacionAmparo = item.IsNull(114) ? null! : item.Field<DateTime>(114).ToString("yyyy-MM-dd")!,
                        NumeroJuicioAmparo = item.IsNull(115) ? null! : item.Field<string>(115)!,
                        TipoOrganoJurisdiccionalAmparo = item.IsNull(116) ? null! : item.Field<string>(116)!,
                        Juzgado = item.IsNull(117) ? null! : item.Field<string>(117)!,
                        ResolucionAmparo = item.IsNull(118) ? null! : item.Field<string>(118)!,
                        FechaResolucionAmparo = item.IsNull(119) ? null! : item.Field<DateTime>(119).ToString("yyyy-MM-dd")!,
                        FechaNotificacionAmparo = item.IsNull(120) ? null! : item.Field<DateTime>(120).ToString("yyyy-MM-dd")!


                    }
                );
            }

            return resultList;

        }
    }
}