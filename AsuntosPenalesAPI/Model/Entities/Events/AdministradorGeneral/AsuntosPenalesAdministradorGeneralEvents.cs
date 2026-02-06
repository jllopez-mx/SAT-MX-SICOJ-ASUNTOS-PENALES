using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;
using Sicoj.Utils;



namespace AsuntosPenalesAPI.Model.Entities.Events.AdministradorGeneral
{
    public class AsuntosPenalesAdministradorGeneralEvents
    {
        public static void Reactivar(ref AsuntosPenales entity,
        AsuntosPenalesImputados entityImputado,
        List<AsuntosPenalesHistoricoImputados> listaHistorico,
        string usuarioMidificacion
        )
        {
            int idEstadoProcesal = entity.id_estado_procesal.GetValueOrDefault();
            List<int> listEstadosProcesales = new()
            {
                EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_ARCHIVO_TEMPORAL.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_CRITERIO_DE_OPORTUNIDAD.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_FACULTAD_DE_ABSTENERSE_DE_INVESTIGAR.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_NO_EJERCICIO_DE_LA_ACCION_PENAL.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_ABSOLUTORIA.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA_EN_PROCEDIMIENTO_ABREVIADO.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_SOBRESEIMIENTO.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_SUSPENSIÓN_CONDICIONAL_DEL_PROCESO.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_REMITIDO.GetHashCode(),

            };
            if (!listEstadosProcesales.Any(c => c.Equals(idEstadoProcesal)))
            {
                throw new Exception("El asunto penal no se puede reactivar debido a que no tiene en el estado procesal: Concluido por ...");
            }
            if (entityImputado is not null)
            {
                if (listaHistorico is not null && listaHistorico.Any())
                {
                    var listFiltrada = listaHistorico.Where(c => !listEstadosProcesales.Any(v => v == c.id_estado_procesal)).ToList();

                    if (listFiltrada is not null && listFiltrada.Any())
                    {
                        var historico = listFiltrada.LastOrDefault();

                        entity.id_estado_procesal = historico!.id_estado_procesal;
                        entity.id_estado_tarea = EnumEstadoTarea.ASIGNADO.GetHashCode();
                        entity.usuario_modificacion = usuarioMidificacion;
                    }
                }
            }
            else
            {
                entity.id_estado_procesal = EnumEstadoProcesal.EN_ESTUDIO.GetHashCode();
                entity.id_estado_tarea = EnumEstadoTarea.ASIGNADO.GetHashCode();
                entity.usuario_modificacion = usuarioMidificacion;
            }
        }
        public static AsuntosPenalesDescartar CreateDescartar(AsuntosPenales entityAsuntosPenales,
            int? id_seccion,
            string? usuario_modificacion
        )
        {
            if (entityAsuntosPenales.id_estado_procesal == EnumEstadoProcesal.ACTIVO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que su estado se encuentra en ACTIVO");
            }
            if (entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_REMITIDO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que se encuentra concluido");
            }
            AsuntosPenalesDescartar entity = new()
            {
                p_id_asunto_penal = entityAsuntosPenales.id,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_estado_procesal = entityAsuntosPenales.id_estado_procesal.GetValueOrDefault(),
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }

        public static AsuntosPenalesDescartar CreateDescartarEtapa(AsuntosPenales entityAsuntosPenales,
            int id_imputado,
            int? id_seccion,
            string? usuario_modificacion
        )
        {
            if (entityAsuntosPenales.id_estado_procesal == EnumEstadoProcesal.ACTIVO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que su estado se encuentra en ACTIVO");
            }
            if (entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_POR_IMPROCEDENCIA.GetHashCode() || entityAsuntosPenales.id_estado_tarea == EnumEstadoTarea.CONCLUIDO_REMITIDO.GetHashCode())
            {
                throw new Exception("No se puede descartar la información del asunto penal debido a que se encuentra concluido");
            }
            AsuntosPenalesDescartar entity = new()
            {
                p_id_asunto_penal = entityAsuntosPenales.id,
                id_imputado = id_imputado,
                id_seccion = id_seccion.GetValueOrDefault(),
                id_estado_procesal = entityAsuntosPenales.id_estado_procesal.GetValueOrDefault(),
                usuario_creacion = usuario_modificacion,
            };
            return entity;
        }


        public static void Descartar(ref AsuntosPenales entityAsuntosPenales,
            string? usuario_modificacion)
        {
            entityAsuntosPenales.id_estado_procesal = EnumEstadoProcesal.EN_ESTUDIO.GetHashCode();
            entityAsuntosPenales.usuario_modificacion = usuario_modificacion!;
            entityAsuntosPenales.fecha_control_solicitudes = DateTime.Now;

        }

        public static void DescartarSeccion(ref AsuntosPenales entityAsuntosPenales,
        int? id_seccion,
        string? usuario_modificacion)
        {
            //ValidacionEstadosAdminAbogadoEvents.ValidateRequerimientoDescartarSeccion(true, entityAsuntosPenales.id_estado_procesal, true, entityAsuntosPenales.id_estado_tarea);
            //Guard.GenerateDate(entityAsuntosPenales.fecha_asignacion, "Fecha de Vencimiento", out var fechaVencimiento, 90, true);
            //entityAsuntosPenales.fecha_vencimiento = fechaVencimiento;
            switch (id_seccion)
            {
                case 1:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.INVESTIGACION_INICIAL.GetHashCode();
                    break;
                case 2:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.INVESTIGACION_COMPLEMENTARIA.GetHashCode();
                    break;
                case 3:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.ETAPA_INTERMEDIA.GetHashCode();
                    break;
                case 4:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.ETAPA_JUICIO.GetHashCode();
                    break;
                case 8:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.EN_ESTUDIO.GetHashCode();
                    break;
                case 9:
                    entityAsuntosPenales.id_estado_procesal =
                        EnumEstadoProcesal.PROCEDENTE.GetHashCode();
                    break;
            }
            //entityAsuntosPenales.id_estado_procesal = EnumEstadoProcesal.EN_ESTUDIO.GetHashCode();
            entityAsuntosPenales.usuario_modificacion = usuario_modificacion!;
        }

        public static void UpdateDatosGeneralesAG(ref AsuntosPenales entity,
            string? noExpedienteCadido,
            string? oficioSolicitud,
            DateTime fechaRecepcion,
            DateTime fechaVencimiento,
            int? idUnidadRealizaSolicitud,
            int? id_admin_controla,
            bool interno
        )
        {
            Guard.ValidateStringEmpty(ref noExpedienteCadido!, "No. Expediente Candido");
            Guard.ValidateStringEmpty(ref oficioSolicitud!, "Oficio Solicitud");
            Guard.CatalogValue(ref id_admin_controla, "id admin controla");
            Guard.CatalogValue(ref idUnidadRealizaSolicitud!, "Unidad Realiza Solicitud", false);
            Guard.BooleanValue(interno!, "Tipo de Unidad");


            entity.numero_expediente_cadido = noExpedienteCadido;
            entity.oficio_solicitud = oficioSolicitud;
            entity.id_unidad_realiza_solicitud = idUnidadRealizaSolicitud;
            entity.interno = interno;
            entity.fecha_recepcion = fechaRecepcion;
            entity.fecha_vencimiento = fechaVencimiento;
            entity.id_admin_controla = id_admin_controla.GetValueOrDefault();
        }

        public static void UpdateAnalisisAG(ref AsuntosPenalesAnalisis entityAnalisis,

                            ref AsuntosPenales entity,
                            bool requerimiento,
                            string oficio_requerimiento,
                            string? fecha_requerimiento,
                            bool? requerimiento_atendido,
                            string oficio_atencion,
                            string? fecha_atencion,
                            int? id_determinacion_asunto_penal,
                            DateTime fecha_determinacion,
                            string usuarioModificacion
            )

        {

            entityAnalisis.requerimiento = requerimiento;
            entityAnalisis.oficio_requerimiento = oficio_requerimiento;
            entityAnalisis.fecha_requerimiento = string.IsNullOrWhiteSpace(fecha_requerimiento)
            ? (DateTime?)null
            : DateTime.Parse(fecha_requerimiento);

            entityAnalisis.requerimiento_atendido = requerimiento_atendido;
            entityAnalisis.oficio_atencion = oficio_atencion;
            entityAnalisis.fecha_atencion = string.IsNullOrWhiteSpace(fecha_atencion)
            ? (DateTime?)null
            : DateTime.Parse(fecha_atencion);

            entityAnalisis.id_determinacion_asunto_penal = id_determinacion_asunto_penal.GetValueOrDefault();
            entityAnalisis.fecha_determinacion = fecha_determinacion;
            entityAnalisis.usuario_modificacion = usuarioModificacion;
        }

        public static void UpdateFechaVencimiento(
            ref AsuntosPenales entityExists,
            DateTime? fechaVencimiento,
            string usuarioModificacion
        )
        {
            entityExists.fecha_vencimiento = fechaVencimiento.GetValueOrDefault();
            entityExists.usuario_modificacion = usuarioModificacion;

        }
        
        public static XLWorkbook GenerarExcelClosedXmlGeneral(List<ResponseReporteGeneral> datos)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte");

            // Crear encabezados agrupados (filas 1 y 2)
            ws.Cell("A1").Value = "Datos Generales";
            ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromName("PowderBlue");
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Range("A1:G1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("H1").Value = "Análisis del asunto";
            ws.Cell("H1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("H1").Style.Font.Bold = true;
            ws.Range("H1:I1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("J1").Value = "Requisito de Procedibilidad";
            ws.Cell("J1").Style.Fill.BackgroundColor = XLColor.FromName("PowderBlue");
            ws.Cell("J1").Style.Font.Bold = true;
            ws.Range("J1:U1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("V1").Value = "Etapa de Investigación/Fase Inicial";
            ws.Cell("V1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("V1").Style.Font.Bold = true;
            ws.Range("V1:AL1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("AM1").Value = "Etapa de Investigación/Fase Complementaria";
            ws.Cell("AM1").Style.Fill.BackgroundColor = XLColor.FromName("PowderBlue");
            ws.Cell("AM1").Style.Font.Bold = true;
            ws.Range("AM1:BN1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("BO1").Value = "Etapa Intermedia";
            ws.Cell("BO1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("BO1").Style.Font.Bold = true;
            ws.Range("BO1:CO1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("CP1").Value = "Etapa de Juicio";
            ws.Cell("CP1").Style.Fill.BackgroundColor = XLColor.FromName("PowderBlue");
            ws.Cell("CP1").Style.Font.Bold = true;
            ws.Range("CP1:DC1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("DD1").Value = "Apelación";
            ws.Cell("DD1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("DD1").Style.Font.Bold = true;
            ws.Range("DD1:DH1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("DI1").Value = "Amparo";
            ws.Cell("DI1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            ws.Cell("DI1").Style.Font.Bold = true;
            ws.Range("DI1:DP1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Encabezados individuales (fila 2)
            string[] headers = new string[]
            {
            "Administración",
            "Subadministración",
            "Unidad que realiza la solicitud",
            "Estado procesal",
            "Número de asunto penal",
            "Fecha de recepcion de la Solicitud",
            "Abogado asignado",

            "Determinación del asunto penal",
            "Fecha de determinación",

            "Requisito de Procedibilidad",
            "Fecha de presentación del Requisito de Procedibilidad",
            "Persona Moral",
            "¿La persona moral es contribuyente?",
            "RFC persona moral",
            "Delito",
            "Cuantía",
            "Número de Carpeta de Investigación",
            "Agente del Ministerio Público",
            "Nombre del Imputado",
            "¿El imputado es contribuyente?",
            "RFC imputado",

            "Formas de Terminación de la investigación",
            "Fecha de terminación de la investigación",
            "Tipo de Solución Alterna",
            "Condiciones",
            "Fecha de Autorización del Acuerdo Reparatorio",
            "Reparacion del daño",
            "Fecha de la celebración del Acuerdo Reparatorio",
            "Fecha de conclusión",
            "Condiciones",
            "Fecha de criterio de oportunidad",
            "Fecha de conclusión",
            "Fecha de solicitud de audiencia inicial",
            "Centro de Justicia",
            "Causa Penal",
            "Fecha de audiencia Inicial",
            "Fecha del auto de vinculación",
            "Fecha de la orden de aprehensión",

            "Tipo de sentencia",
            "Fecha de emisión de la Sentencia",
            "Reparacion del daño",
            "Cumplimiento de pena privada de la libertad",
            "Pena de Prisión - Años",
            "Pena de Prision - Meses",
            "Pena de Prisión - Días",
            "Otorgamiento de beneficios",
            "Acciones de ejecución",
            "Fecha de ejecución",
            "Conclusión del asunto",
            "Fecha de conclusión",
            "Tipo de Solución Alterna",
            "Condiciones del Acuerdo Reparatorio",
            "Fecha de autorización del Acuerdo Reparatorio",
            "Reparación del daño",
            "Fecha de celebración del Acuerdo Reparatorio",
            "Fecha de conclusión",
            "Fecha determinación de sobreseimiento",
            "Solicitud de sobreseimiento por parte de MP",
            "Fecha de conclusión",
            "Condiciones de la Suspensión Condicional del Proceso",
            "Fecha de celebración de la Suspensión Condicional del Proceso",
            "Reparacion del daño",
            "Plazo de Suspensión Condicional del Proceso",
            "Fecha de cumplimiento de la Suspensión Condicional del Proceso",
            "Fecha de conclusión",
            "Fecha de escrito de acusación",

            "Fecha de audiencia",
            "Fecha de Auto de apertura a juicio oral",
            "Tipo de Sentencia",
            "Fecha de emisión de la Sentencia",
            "Reparación del daño",
            "Cumplimiento de pena privada de la libertad",
            "Pena de Prisión - Años",
            "Pena de Prisión - Meses",
            "Pena de Prisión - Días",
            "Otorgamiento de beneficios",
            "Acciones de ejecución",
            "Fecha de ejecución",
            "Conclusión del asunto",
            "Fecha de conclusión",
            "Tipo de Solución Alterna",
            "Condiciones del Acuerdo Reparatorio",
            "Fecha de autorización de Acuerdo Reparatorio",
            "Reparación del daño",
            "Fecha de celebración del Acuerdo Reparatorio",
            "Fecha de conclusión",
            "Fecha de determinación de sobreseimiento",
            "Fecha de conclusión",
            "Condiciones de la Suspensión Condicional del Procesao",
            "Fecha de celebración de la Suspensión Condicional del Proceso",
            "Reparación del daño",
            "Plazo de Suspensión Condicional del Proceso",
            "Fecha de cumplimiento de la Suspensión Condicional del Proceso",
            "Fecha de conclusión",

            "Fecha inicial de audiencia de juicio",
            "Fecha final de audiencia de juicio",
            "Tipo de sentencia",
            "Fecha de emisión de la Sentencia",
            "Reparación del daño",
            "Cumplimiento de pena privada de la libertad",
            "Pena de Prisión - Años",
            "Pena de Prisión - Meses",
            "Pena de Prisión - Días",
            "Otorgamiento de beneficions",
            "Acciones de ejecución",
            "Fecha de ejecución",
            "Conclusión del asunto",
            "Fecha de conclusión",

            "Fecha de presentación",
            "Número de toca penal",
            "Órgano Jurisdiccional",
            "Resolución",
            "Fecha de resolución",

            "Tipo de Amparo",
            "Fecha de presentación",
            "Número de juicio de amparo",
            "Órgano Jurisdiccional",
            "Juzgado",
            "Resolución",
            "Fecha de resolución",
            "Fecha de notificación"



            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(2, i + 1).Value = headers[i];
                ws.Cell(2, i + 1).Style.Font.Bold = true;
            }

            // Cargar datos (desde fila 3)
            int row = 3;
            foreach (var item in datos)
            {
                ws.Cell(row, 1).Value = item.UnidadAdministrativa;
                ws.Cell(row, 2).Value = item.Subadministracion;
                ws.Cell(row, 3).Value = item.UnidadRealizaSolicitud;
                ws.Cell(row, 4).Value = item.EstadoProcesal;
                ws.Cell(row, 5).Value = item.NumeroAsunto;
                ws.Cell(row, 6).Value = item.FechaRecepcion;
                ws.Cell(row, 7).Value = item.NombreAbogado;
                ws.Cell(row, 8).Value = item.DeterminacionAsunto;
                ws.Cell(row, 9).Value = item.FechaDeterminacion;
                ws.Cell(row, 10).Value = item.RequisitoProcedibilidad;
                ws.Cell(row, 11).Value = item.FechaPresentacionRequisito;
                ws.Cell(row, 12).Value = item.PersonaMoral;
                ws.Cell(row, 13).Value = item.ContribuyentePersona;
                ws.Cell(row, 14).Value = item.RfcPersonaMoral;
                ws.Cell(row, 15).Value = item.Delito;
                ws.Cell(row, 16).Value = item.Cuantia;
                ws.Cell(row, 17).Value = item.NumeroCarpetaInvestigacion;
                ws.Cell(row, 18).Value = item.AgenteMinisterioPublico;
                ws.Cell(row, 19).Value = item.NombreImputado;
                ws.Cell(row, 20).Value = item.ContribuyenteImputado;
                ws.Cell(row, 21).Value = item.RfcImputado;
                ws.Cell(row, 22).Value = item.FormaTerminacionInvestigacion;
                ws.Cell(row, 23).Value = item.FechaTerminacionInvestigacion;
                ws.Cell(row, 24).Value = item.SolucionAlternaIn;
                ws.Cell(row, 25).Value = item.CondicionesArInicial;
                ws.Cell(row, 26).Value = item.FechaAutorizacionArInicial;
                ws.Cell(row, 27).Value = item.ReparacionDanioArInicial;
                ws.Cell(row, 28).Value = item.FechaReparacionArInicial;
                ws.Cell(row, 29).Value = item.FechaConclusionArInicial;
                ws.Cell(row, 30).Value = item.CondicionesCo;
                ws.Cell(row, 31).Value = item.FechaCriterioOportunidad;
                ws.Cell(row, 32).Value = item.FechaConclusionCo;
                ws.Cell(row, 33).Value = item.FechaSolicitudAudienciaInicial;
                ws.Cell(row, 34).Value = item.CentroJusticia;
                ws.Cell(row, 35).Value = item.CausaPenal;
                ws.Cell(row, 36).Value = item.FechaAudienciaInicial;
                ws.Cell(row, 37).Value = item.FechaAutoVinculacion;
                ws.Cell(row, 38).Value = item.FechaOrdenAprehension;
                ws.Cell(row, 39).Value = item.TipoSentenciaComplementaria;
                ws.Cell(row, 40).Value = item.FechaEmisionSentenciaComplementaria;
                ws.Cell(row, 41).Value = item.ReparacionDanioSenComplementaria;
                ws.Cell(row, 42).Value = item.CumplimientoLibertadComplementaria;
                ws.Cell(row, 43).Value = item.AniosSenComplementaria;
                ws.Cell(row, 44).Value = item.MesesSenComplementaria;
                ws.Cell(row, 45).Value = item.DiasSenComplementaria;
                ws.Cell(row, 46).Value = item.OtorgamientoBeneficiosComplementaria;
                ws.Cell(row, 47).Value = item.AccionesEjecucionComplementaria;
                ws.Cell(row, 48).Value = item.FechaEjecucionComplementaria;
                ws.Cell(row, 49).Value = item.ConclusionAsuntoComplementaria;
                ws.Cell(row, 50).Value = item.FechaConclusionSenComplementaria;
                ws.Cell(row, 51).Value = item.SolucionAlternaCom;
                ws.Cell(row, 52).Value = item.CondicionesArComplementaria;
                ws.Cell(row, 53).Value = item.FechaAutorizacionArComplementaria;
                ws.Cell(row, 54).Value = item.ReparacionDanioArComplementaria;
                ws.Cell(row, 55).Value = item.FechaReparacionArComplementaria;
                ws.Cell(row, 56).Value = item.FechaConclusionArComplementaria;
                ws.Cell(row, 57).Value = item.FechaDeterminacionSobreseimientoCom;
                ws.Cell(row, 58).Value = item.SolicitudSobreseimientoCom;
                ws.Cell(row, 59).Value = item.FechaConclusionSobreseimientoCom;
                ws.Cell(row, 60).Value = item.CondicionesSuspensionComplementariaCom;
                ws.Cell(row, 61).Value = item.FechaCelebracionSuspensionComplementariaCom;
                ws.Cell(row, 62).Value = item.ReparacionDanioSuspensionComplementariaCom;
                ws.Cell(row, 63).Value = item.PlazoSuspensionComplementariaCom;
                ws.Cell(row, 64).Value = item.FechaCumplimientoSuspensionComplementariaCom;
                ws.Cell(row, 65).Value = item.FechaConclusionSuspensionComplementariaCom;
                ws.Cell(row, 66).Value = item.FechaEscritoAcusacionCp;
                ws.Cell(row, 67).Value = item.FechaAudienciaIntermedia;
                ws.Cell(row, 68).Value = item.FechaAperturaJuicioOral;
                ws.Cell(row, 69).Value = item.TipoSentenciaIntermedia;
                ws.Cell(row, 70).Value = item.FechaEmisionSentenciaIntermedia;
                ws.Cell(row, 71).Value = item.ReparacionDanioSenIntermedia;
                ws.Cell(row, 72).Value = item.CumplimientoLibertadIntermedia;
                ws.Cell(row, 73).Value = item.AniosSenIntermedia;
                ws.Cell(row, 74).Value = item.MesesSenIntermedia;
                ws.Cell(row, 75).Value = item.DiasSenIntermedia;
                ws.Cell(row, 76).Value = item.OtorgamientoBeneficiosIntermedia;
                ws.Cell(row, 77).Value = item.AccionesEjecucionIntermedia;
                ws.Cell(row, 78).Value = item.FechaEjecucionIntermedia;
                ws.Cell(row, 79).Value = item.ConclusionAsuntoIntermedia;
                ws.Cell(row, 80).Value = item.FechaConclusionSenIntermedia;
                ws.Cell(row, 81).Value = item.SolucionAlternaInt;
                ws.Cell(row, 82).Value = item.CondicionesArIntermedia;
                ws.Cell(row, 83).Value = item.FechaAutorizacionArIntermedia;
                ws.Cell(row, 84).Value = item.ReparacionDanioArIntermedia;
                ws.Cell(row, 85).Value = item.FechaReparacionArIntermedia;
                ws.Cell(row, 86).Value = item.FechaConclusionArIntermedia;
                ws.Cell(row, 87).Value = item.FechaDeterminacionSobreseimientoInt;
                ws.Cell(row, 88).Value = item.FechaConclusionSobreseimientoInt;
                ws.Cell(row, 89).Value = item.CondicionesSuspensionComplementariaInt;
                ws.Cell(row, 90).Value = item.FechaCelebracionSuspensionComplementariaInt;
                ws.Cell(row, 91).Value = item.ReparacionDanioSuspensionComplementariaInt;
                ws.Cell(row, 92).Value = item.PlazoSuspensionComplementariaInt;
                ws.Cell(row, 93).Value = item.FechaCumplimientoSuspensionComplementariaInt;
                ws.Cell(row, 94).Value = item.FechaConclusionSuspensionComplementariaInt;
                ws.Cell(row, 95).Value = item.FechaInicialAudienciaJuicio;
                ws.Cell(row, 96).Value = item.FechaFinalAudienciaJuicio;
                ws.Cell(row, 97).Value = item.TipoSentenciaJuicio;
                ws.Cell(row, 98).Value = item.FechaEmisionSentenciaJuicio;
                ws.Cell(row, 99).Value = item.ReparacionDanioSenJuicio;
                ws.Cell(row, 100).Value = item.CumplimientoLibertadJuicio;
                ws.Cell(row, 101).Value = item.AniosSenJuicio;
                ws.Cell(row, 102).Value = item.MesesSenJuicio;
                ws.Cell(row, 103).Value = item.DiasSenJuicio;
                ws.Cell(row, 104).Value = item.OtorgamientoBeneficiosJuicio;
                ws.Cell(row, 105).Value = item.AccionesEjecucionJuicio;
                ws.Cell(row, 106).Value = item.FechaEjecucionJuicio;
                ws.Cell(row, 107).Value = item.ConclusionAsuntoJuicio;
                ws.Cell(row, 108).Value = item.FechaConclusionSenJuicio;
                ws.Cell(row, 109).Value = item.FechaPresentacionApelacion;
                ws.Cell(row, 110).Value = item.NumeroTocaPenal;
                ws.Cell(row, 111).Value = item.TipoOrganoJurisdiccionalApelacion;
                ws.Cell(row, 112).Value = item.ResolucionApelacion;
                ws.Cell(row, 113).Value = item.FechaResolucionApelacion;
                ws.Cell(row, 114).Value = item.TipoAmparo;
                ws.Cell(row, 115).Value = item.FechaPresentacionAmparo;
                ws.Cell(row, 116).Value = item.NumeroJuicioAmparo;
                ws.Cell(row, 117).Value = item.TipoOrganoJurisdiccionalAmparo;
                ws.Cell(row, 118).Value = item.Juzgado;
                ws.Cell(row, 119).Value = item.ResolucionAmparo;
                ws.Cell(row, 120).Value = item.FechaResolucionAmparo;
                ws.Cell(row, 121).Value = item.FechaNotificacionAmparo;
            
                row++;
            }

            ws.Columns().AdjustToContents();

            return wb;
        }

    }
}