using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using Polly.CircuitBreaker;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.DTO;

namespace AsuntosPenalesAPI.Model.Entities.Events.Abogado
{
    public class AsuntosPenalesImputadosEvents
    {
        public static AsuntosPenalesImputados Create(
            ref AsuntosPenales entity,
            string? rfc,
            bool contribuyente,
            string? nombre,
            string Rfc
        )
        {
            Guard.ValidateStringRfc(ref rfc, "RFC contribuyente");
            Guard.ValidateStringAlphanumeric(ref nombre, "Contribuyente");

            AsuntosPenalesImputados entityImputados =
                new()
                {
                    id_asunto_penal = entity.id,
                    rfc = rfc!,
                    contribuyente = contribuyente,
                    nombre = nombre!,
                    usuario_creacion = Rfc,
                    id_estado_procesal = EnumEstadoProcesal.INVESTIGACION_INICIAL.GetHashCode(),
                    id_estado_tarea = entity.id_estado_tarea,
                };

            return entityImputados;
        }

        // public static void UpdateImputados(
        //             ref AsuntosPenalesImputados entityImputados,
        //             ref AsuntosPenales entity,
        //             string rfc,
        //             string nombre,
        //             bool contribuyente,
        //             string usuarioModificacion

        // )
        // {
        //     entityImputados.rfc = rfc;
        //     entityImputados.nombre = nombre;
        //     entityImputados.contribuyente = contribuyente;
        //     entityImputados.usuario_modificacion = usuarioModificacion;

        // }


        public static void UpdateImputadosEtapaInicial(
            int? estadoProcesalMod,
            ref AsuntosPenalesImputados entity,
            ref AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            ref AsuntosPenalesCriterioOportunidad entityCriterioOportunidad,
            bool concluye_investigacion,
            int? id_terminacion_investigacion,
            DateTime? fecha_terminacion_investigacion,
            bool solucion_alterna,
            int? id_tipo_solucion_alterna,
            DateTime? fecha_solicitud_audiencia_inicial,
            int id_centro_justicia,
            string causa_penal,
            DateTime? fecha_audiencia_inicial,
            bool auto_vinculacion_proceso,
            DateTime? fecha_auto_vinculacion,
            bool orden_aprehension,
            DateTime? fecha_orden_aprehension,
            string usuario_modificacion,
            string? condicionesAcuerdoReparatorio,
            DateTime? fechaAutorizacionAcuerdoReparatorio,
            decimal? reparacionDañoAcuerdoReparatorio,
            DateTime? fechaCelebracionAcuerdoReparatorio,
            bool? conclusionAsuntoAcuerdoReparatorio,
            DateTime? fechaConclusionAcuerdoReparatorio,
            string? condicionesCriterioOportunidad,
            DateTime? fechaCriterioOportunidad,
            bool? conclusionAsuntoCriterioOportunidad,
            DateTime? fechaConclusionCriterioOportunidad
        )
        {
            entity.concluye_investigacion = concluye_investigacion;
            entity.id_terminacion_investigacion = id_terminacion_investigacion;
            entity.fecha_terminacion_investigacion = fecha_terminacion_investigacion;

            entity.solucion_alterna = solucion_alterna;
            entity.id_tipo_solucion_alterna = id_tipo_solucion_alterna;

            entity.fecha_solicitud_audiencia_inicial = fecha_solicitud_audiencia_inicial;
            entity.id_centro_justicia = id_centro_justicia;
            entity.causa_penal = causa_penal;
            entity.fecha_audiencia_inicial = fecha_audiencia_inicial;
            entity.auto_vinculacion_proceso = auto_vinculacion_proceso;
            entity.fecha_auto_vinculacion = fecha_auto_vinculacion;
            entity.orden_aprehension = orden_aprehension;
            entity.fecha_orden_aprehension = fecha_orden_aprehension;
            entity.usuario_modificacion = usuario_modificacion;
            if (estadoProcesalMod != EnumEstadoProcesal.EN_REPARACION.GetHashCode())
            {
                if (concluye_investigacion == true && auto_vinculacion_proceso == false)
                {
                    switch (id_terminacion_investigacion)
                    {
                        case 1:
                            entity.id_estado_procesal =
                                EnumEstadoProcesal.CONCLUIDO_POR_FACULTAD_DE_ABSTENERSE_DE_INVESTIGAR.GetHashCode();
                            break;
                        case 2:
                            entity.id_estado_procesal =
                                EnumEstadoProcesal.CONCLUIDO_POR_ARCHIVO_TEMPORAL.GetHashCode();
                            break;
                        case 3:
                            entity.id_estado_procesal =
                                EnumEstadoProcesal.CONCLUIDO_POR_NO_EJERCICIO_DE_LA_ACCION_PENAL.GetHashCode();
                            break;
                    }
                }
                if (auto_vinculacion_proceso == true)
                {
                    entity.id_estado_procesal =
                        EnumEstadoProcesal.INVESTIGACION_COMPLEMENTARIA.GetHashCode();
                }
                if (solucion_alterna == true && auto_vinculacion_proceso == false)
                {
                    if (id_tipo_solucion_alterna == 1)
                        if (conclusionAsuntoAcuerdoReparatorio == true)
                        {
                            entity.id_estado_procesal =
                                EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode();
                        }
                        else
                        {
                            entity.id_estado_procesal = entity.id_estado_procesal;
                        }
                    else
                    {
                        if (conclusionAsuntoCriterioOportunidad == true)
                        {
                            entity.id_estado_procesal =
                                EnumEstadoProcesal.CONCLUIDO_POR_CRITERIO_DE_OPORTUNIDAD.GetHashCode();
                        }
                        else
                        {
                            entity.id_estado_procesal = entity.id_estado_procesal;
                        }
                    }
                }
                if (
                    auto_vinculacion_proceso != true
                    && solucion_alterna != true
                    && concluye_investigacion != true
                )
                {
                    entity.id_estado_procesal = entity.id_estado_procesal;
                }
            }


            if (solucion_alterna)
            {
                switch (id_tipo_solucion_alterna)
                {
                    case 1:
                        if (entityAcuerdoReparatorio is null)
                        {
                            entityAcuerdoReparatorio = CreateAcuerdoReparatorio(
                                ref entity,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.INICIAL
                            );
                        }
                        else
                        {
                            UpdateAcuerdoReparatorio(
                                ref entity,
                                ref entityAcuerdoReparatorio,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion
                            );
                        }
                        break;

                    case 2:
                        if (entityCriterioOportunidad is null)
                        {
                            entityCriterioOportunidad = CreateCriterioOportunidad(
                                ref entity,
                                condicionesCriterioOportunidad,
                                fechaCriterioOportunidad,
                                conclusionAsuntoCriterioOportunidad,
                                fechaConclusionCriterioOportunidad,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.INICIAL
                            );
                        }
                        else
                        {
                            UpdateCriterioOportunidad(
                                ref entity,
                                ref entityCriterioOportunidad,
                                condicionesCriterioOportunidad,
                                fechaCriterioOportunidad,
                                conclusionAsuntoCriterioOportunidad,
                                fechaConclusionCriterioOportunidad,
                                usuario_modificacion
                            );
                        }

                        break;
                }
            }
        }

        public static AsuntosPenalesAcuerdoReparatorio CreateAcuerdoReparatorio(
            ref AsuntosPenalesImputados entityImputado,
            string? condicionesAcuerdoReparatorio,
            DateTime? fechaAutorizacionAcuerdoReparatorio,
            decimal? reparacionDañoAcuerdoReparatorio,
            DateTime? fechaCelebracionAcuerdoReparatorio,
            bool? conclusionAsuntoAcuerdoReparatorio,
            DateTime? fechaConclusionAcuerdoReparatorio,
            string? usuario_creacion,
            EnumTipoEtapaInvestigacion etapaInvestigacion
        )
        {
            AsuntosPenalesAcuerdoReparatorio entity =
                new()
                {
                    id_imputado = entityImputado.id,
                    id_tipo_etapa_investigacion = etapaInvestigacion.GetHashCode(),
                    condiciones = condicionesAcuerdoReparatorio!,
                    fecha_autorizacion_acuerdo_reparatorio = fechaAutorizacionAcuerdoReparatorio.GetValueOrDefault(),
                    reparacion_daño = reparacionDañoAcuerdoReparatorio.GetValueOrDefault(),
                    fecha_celebracion_acuerdo_reparatorio = fechaCelebracionAcuerdoReparatorio.GetValueOrDefault(),
                    conclusion_asunto = conclusionAsuntoAcuerdoReparatorio.GetValueOrDefault(),
                    fecha_conclusion = fechaConclusionAcuerdoReparatorio,
                    usuario_creacion = usuario_creacion!,
                };

            return entity;
        }

        public static void UpdateAcuerdoReparatorio(
            ref AsuntosPenalesImputados entityImputado,
            ref AsuntosPenalesAcuerdoReparatorio entity,
            string? condicionesAcuerdoReparatorio,
            DateTime? fechaAutorizacionAcuerdoReparatorio,
            decimal? reparacionDañoAcuerdoReparatorio,
            DateTime? fechaCelebracionAcuerdoReparatorio,
            bool? conclusionAsuntoAcuerdoReparatorio,
            DateTime? fechaConclusionAcuerdoReparatorio,
            string? usuario_creacion
        )
        {
            entity.condiciones = condicionesAcuerdoReparatorio!;
            entity.fecha_autorizacion_acuerdo_reparatorio =
                fechaAutorizacionAcuerdoReparatorio.GetValueOrDefault();
            entity.reparacion_daño = reparacionDañoAcuerdoReparatorio.GetValueOrDefault();
            entity.fecha_celebracion_acuerdo_reparatorio =
                fechaCelebracionAcuerdoReparatorio.GetValueOrDefault();
            entity.conclusion_asunto = conclusionAsuntoAcuerdoReparatorio.GetValueOrDefault();
            entity.fecha_conclusion = fechaConclusionAcuerdoReparatorio;
            entity.usuario_modificacion = usuario_creacion;
        }

        public static AsuntosPenalesCriterioOportunidad CreateCriterioOportunidad(
            ref AsuntosPenalesImputados entityImputado,
            string? condicionesCriterioOportunidad,
            DateTime? fechaCriterioOportunidad,
            bool? conclusionAsuntoCriterioOportunidad,
            DateTime? fechaConclusionCriterioOportunidad,
            string? usuario_creacion,
            EnumTipoEtapaInvestigacion etapaInvestigacion
        )
        {
            AsuntosPenalesCriterioOportunidad entity =
                new()
                {
                    id_imputado = entityImputado.id,
                    condiciones = condicionesCriterioOportunidad!,
                    fecha_criterio_oportunidad = fechaCriterioOportunidad.GetValueOrDefault(),
                    conclusion_asunto = conclusionAsuntoCriterioOportunidad.GetValueOrDefault(),
                    fecha_conclusion = fechaConclusionCriterioOportunidad,
                    usuario_creacion = usuario_creacion!,
                    id_tipo_etapa_investigacion = etapaInvestigacion.GetHashCode(),
                };

            return entity;
        }

        public static void UpdateCriterioOportunidad(
            ref AsuntosPenalesImputados entityImputado,
            ref AsuntosPenalesCriterioOportunidad entity,
            string? condicionesCriterioOportunidad,
            DateTime? fechaCriterioOportunidad,
            bool? conclusionAsuntoCriterioOportunidad,
            DateTime? fechaConclusionCriterioOportunidad,
            string? usuario_creacion
        )
        {
            entity.condiciones = condicionesCriterioOportunidad!;
            entity.fecha_criterio_oportunidad = fechaCriterioOportunidad.GetValueOrDefault();
            entity.conclusion_asunto = conclusionAsuntoCriterioOportunidad.GetValueOrDefault();
            entity.fecha_conclusion = fechaConclusionCriterioOportunidad;
            entity.usuario_modificacion = usuario_creacion;
        }

        public static void UpdateImputadosEtapaComplementariaFechaPlazoInvestigacion(
            ref AsuntosPenales entityExist,
            ref AsuntosPenalesImputados entity,
            DateTime? fechaPlazoInvestigacionComplementaria,
            string usuario_modificacion
        )
        {
            entity.fecha_plazo_investigacion_complementaria = fechaPlazoInvestigacionComplementaria;
            entity.usuario_modificacion = usuario_modificacion;
        }



        public static void UpdateImputadosEtapaComplementaria(
            ref AsuntosPenales entityExist,
            ref AsuntosPenalesImputados entity,
            ref AsuntosPenalesSentencia entitySentencia,
            ref AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            ref AsuntosPenalesSobreseimiento entitySobreseimiento,
            ref AsuntosPenalesSuspensionCondicional entitySuspension,
            DateTime? fechaPlazoInvestigacionComplementaria,
            bool? procedimientoAbreviado,
            bool? escritoAcusacion,
            DateTime? fechaEscritoAcusacion,
            int? sentencia,
            DateTime? fechaEmisionSentencia,
            decimal? reparacionDañoSentencia,
            bool? cumplimientoPrivadaLibertadSentencia,
            int? idAnioSentencia,
            int? idMesSentencia,
            int? idDiaSentencia,
            string? otorgamientoBeneficiosSentencia,
            string? accionesEjecucionSentencia,
            DateTime? fechaEjecucionSentencia,
            bool? conclusionAsuntoSentencia,
            DateTime? fechaConclusionSentencia,
            bool? solucionAlterna,
            int? idTipoSolucionAlterna,
            string? condicionesAcuerdoReparatorio,
            DateTime? fechaAutorizacionAcuerdoReparatorio,
            decimal? reparacionDañoAcuerdoReparatorio,
            DateTime? fechaCelebracionAcuerdoReparatorio,
            bool? conclusionAsuntoAcuerdoReparatorio,
            DateTime? fechaConclusionAcuerdoReparatorio,
            bool? solicitudSobreseimiento,
            DateTime? fechaDeterminacionSobreseimiento,
            bool? conclusionAsuntoSobreseimiento,
            DateTime? fechaConclusionSobreseimiento,
            string condicionesSuspension,
            DateTime? fechaCelebracionSuspension,
            decimal? reparacionDañoSuspension,
            DateTime? plazoSuspension,
            DateTime? fechaCumplimientoSuspension,
            bool? conclusionAsuntoSuspension,
            DateTime? fechaConclusionSuspension,
            string usuario_modificacion
        )
        {
            entity.fecha_plazo_investigacion_complementaria = fechaPlazoInvestigacionComplementaria;
            entity.procedimiento_abreviado_cp = procedimientoAbreviado;
            entity.escrito_acusacion_cp = escritoAcusacion;
            entity.fecha_escrito_acusacion_cp = fechaEscritoAcusacion;
            entity.usuario_modificacion = usuario_modificacion;
            entity.solucion_alterna_cp = solucionAlterna;
            entity.id_tipo_solucion_alterna_cp = idTipoSolucionAlterna;

            if (escritoAcusacion == true)
            {
                entity.id_estado_procesal = EnumEstadoProcesal.ETAPA_INTERMEDIA.GetHashCode();
            }

            if (conclusionAsuntoSentencia == true)
            {
                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA_EN_PROCEDIMIENTO_ABREVIADO.GetHashCode();
            }

            if ((bool)solucionAlterna!)
            {
                switch (idTipoSolucionAlterna)
                {
                    case 1:
                        if (conclusionAsuntoAcuerdoReparatorio == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode();
                        break;
                    case 2:
                        if (conclusionAsuntoSobreseimiento == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SOBRESEIMIENTO.GetHashCode();
                        break;
                    case 3:
                        if (conclusionAsuntoSuspension == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SUSPENSIÓN_CONDICIONAL_DEL_PROCESO.GetHashCode();
                        break;
                }
            }
            if (procedimientoAbreviado.GetValueOrDefault())
            {
                if (entitySentencia is null)
                {
                    entitySentencia = CreateSentencia(
                        ref entity,
                        sentencia,
                        fechaEmisionSentencia,
                        reparacionDañoSentencia,
                        cumplimientoPrivadaLibertadSentencia,
                        idAnioSentencia,
                        idMesSentencia,
                        idDiaSentencia,
                        otorgamientoBeneficiosSentencia,
                        accionesEjecucionSentencia,
                        fechaEjecucionSentencia,
                        conclusionAsuntoSentencia,
                        fechaConclusionSentencia,
                        usuario_modificacion,
                        EnumTipoEtapaInvestigacion.COMPLEMENTARIA
                    );
                }
                else
                {
                    UpdateSentencia(
                        ref entity,
                        ref entitySentencia,
                        sentencia,
                        fechaEmisionSentencia,
                        reparacionDañoSentencia,
                        cumplimientoPrivadaLibertadSentencia,
                        idAnioSentencia,
                        idMesSentencia,
                        idDiaSentencia,
                        otorgamientoBeneficiosSentencia,
                        accionesEjecucionSentencia,
                        fechaEjecucionSentencia,
                        conclusionAsuntoSentencia,
                        fechaConclusionSentencia,
                        usuario_modificacion
                    );
                }
            }

            if ((bool)solucionAlterna)
            {
                switch (idTipoSolucionAlterna)
                {
                    case 1:
                        if (entityAcuerdoReparatorio is null)
                        {
                            entityAcuerdoReparatorio = CreateAcuerdoReparatorio(
                                ref entity,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.COMPLEMENTARIA
                            );
                        }
                        else
                        {
                            UpdateAcuerdoReparatorio(
                                ref entity,
                                ref entityAcuerdoReparatorio,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion
                            );
                        }
                        break;

                    case 2:
                        if (entitySobreseimiento is null)
                        {
                            entitySobreseimiento = CreateSobreisimiento(
                                ref entity,
                                solicitudSobreseimiento,
                                fechaDeterminacionSobreseimiento,
                                conclusionAsuntoSobreseimiento,
                                fechaConclusionSobreseimiento,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.COMPLEMENTARIA
                            );
                        }
                        else
                        {
                            UpdateSobreisimiento(
                                ref entity,
                                ref entitySobreseimiento,
                                solicitudSobreseimiento,
                                fechaDeterminacionSobreseimiento,
                                conclusionAsuntoSobreseimiento,
                                fechaConclusionSobreseimiento,
                                usuario_modificacion
                            );
                        }
                        break;
                    case 3:
                        if (entitySuspension is null)
                        {
                            entitySuspension = CreateSuspension(
                                ref entity,
                                condicionesSuspension,
                                fechaCelebracionSuspension,
                                reparacionDañoSuspension,
                                plazoSuspension,
                                fechaCumplimientoSuspension,
                                conclusionAsuntoSuspension,
                                fechaConclusionSuspension,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.COMPLEMENTARIA
                            );
                        }
                        else
                        {
                            UpdateSuspension(
                                ref entity,
                                ref entitySuspension,
                                condicionesSuspension,
                                fechaCelebracionSuspension,
                                reparacionDañoSuspension,
                                plazoSuspension,
                                fechaCumplimientoSuspension,
                                conclusionAsuntoSuspension,
                                fechaConclusionSuspension,
                                usuario_modificacion
                            );
                        }
                        break;
                }
            }
        }

        public static AsuntosPenalesSentencia CreateSentencia(
            ref AsuntosPenalesImputados entityImputado,
            int? sentencia,
            DateTime? fechaEmisionSentencia,
            decimal? reparacionDañoSentencia,
            bool? cumplimientoPrivadaLibertadSentencia,
            int? idAnioSentencia,
            int? idMesSentencia,
            int? idDiaSentencia,
            string? otorgamientoBeneficiosSentencia,
            string? accionesEjecucionSentencia,
            DateTime? fechaEjecucionSentencia,
            bool? conclusionAsuntoSentencia,
            DateTime? fechaConclusionSentencia,
            string? usuario_creacion,
            EnumTipoEtapaInvestigacion etapaInvestigacion
        )
        {
            AsuntosPenalesSentencia entity =
                new()
                {
                    id_imputado = entityImputado.id,
                    id_tipo_sentencia = sentencia.GetValueOrDefault(),
                    fecha_emision_sentencia = fechaEmisionSentencia.GetValueOrDefault(),
                    reparacion_daño = reparacionDañoSentencia.GetValueOrDefault()!,
                    cumplimiento_privada_libertad =
                        cumplimientoPrivadaLibertadSentencia.GetValueOrDefault(),
                    id_anio = idAnioSentencia.GetValueOrDefault(),
                    id_mes = idMesSentencia.GetValueOrDefault(),
                    id_dia = idDiaSentencia.GetValueOrDefault(),
                    otorgamiento_beneficios = otorgamientoBeneficiosSentencia!,
                    acciones_ejecucion = accionesEjecucionSentencia!,
                    fecha_ejecucion = fechaEjecucionSentencia.GetValueOrDefault(),
                    conclusion_asunto = conclusionAsuntoSentencia.GetValueOrDefault(),
                    fecha_conclusion = fechaConclusionSentencia,
                    usuario_creacion = usuario_creacion!,
                    id_tipo_etapa_investigacion = etapaInvestigacion.GetHashCode(),
                };

            return entity;
        }

        public static void UpdateSentencia(
            ref AsuntosPenalesImputados entityImputado,
            ref AsuntosPenalesSentencia entity,
            int? sentencia,
            DateTime? fechaEmisionSentencia,
            decimal? reparacionDañoSentencia,
            bool? cumplimientoPrivadaLibertadSentencia,
            int? idAnioSentencia,
            int? idMesSentencia,
            int? idDiaSentencia,
            string? otorgamientoBeneficiosSentencia,
            string? accionesEjecucionSentencia,
            DateTime? fechaEjecucionSentencia,
            bool? conclusionAsuntoSentencia,
            DateTime? fechaConclusionSentencia,
            string? usuario_creacion
        )
        {
            entity.id_tipo_sentencia = sentencia.GetValueOrDefault();
            entity.fecha_emision_sentencia = fechaEmisionSentencia.GetValueOrDefault();
            entity.reparacion_daño = reparacionDañoSentencia.GetValueOrDefault()!;
            entity.cumplimiento_privada_libertad =
                cumplimientoPrivadaLibertadSentencia.GetValueOrDefault();
            entity.id_anio = idAnioSentencia.GetValueOrDefault();
            entity.id_mes = idMesSentencia.GetValueOrDefault();
            entity.id_dia = idDiaSentencia.GetValueOrDefault();
            entity.otorgamiento_beneficios = otorgamientoBeneficiosSentencia!;
            entity.acciones_ejecucion = accionesEjecucionSentencia!;
            entity.fecha_ejecucion = fechaEjecucionSentencia.GetValueOrDefault();
            entity.conclusion_asunto = conclusionAsuntoSentencia.GetValueOrDefault();
            entity.fecha_conclusion = fechaConclusionSentencia;
            entity.usuario_modificacion = usuario_creacion!;
        }

        public static AsuntosPenalesSobreseimiento CreateSobreisimiento(
            ref AsuntosPenalesImputados entityImputado,
            bool? solicitudSobreseimiento,
            DateTime? fechaDeterminacionSobreseimiento,
            bool? conclusionAsuntoSobreseimiento,
            DateTime? fechaConclusionSobreseimiento,
            string? usuario_creacion,
            EnumTipoEtapaInvestigacion etapaInvestigacion
        )
        {
            AsuntosPenalesSobreseimiento entity =
                new()
                {
                    id_imputado = entityImputado.id,
                    solicitud_sobreseimiento = solicitudSobreseimiento.GetValueOrDefault(),
                    fecha_determinacion_sobreseimiento =
                        fechaDeterminacionSobreseimiento.GetValueOrDefault(),
                    conclusion_asunto = conclusionAsuntoSobreseimiento.GetValueOrDefault(),
                    fecha_conclusion = fechaConclusionSobreseimiento,
                    usuario_creacion = usuario_creacion!,
                    id_tipo_etapa_investigacion = etapaInvestigacion.GetHashCode(),
                };

            return entity;
        }

        public static void UpdateSobreisimiento(
            ref AsuntosPenalesImputados entityImputado,
            ref AsuntosPenalesSobreseimiento entity,
            bool? solicitudSobreseimiento,
            DateTime? fechaDeterminacionSobreseimiento,
            bool? conclusionAsuntoSobreseimiento,
            DateTime? fechaConclusionSobreseimiento,
            string? usuario_creacion
        )
        {
            entity.solicitud_sobreseimiento = solicitudSobreseimiento.GetValueOrDefault();
            entity.fecha_determinacion_sobreseimiento =
                fechaDeterminacionSobreseimiento.GetValueOrDefault();
            entity.conclusion_asunto = conclusionAsuntoSobreseimiento.GetValueOrDefault();
            entity.fecha_conclusion = fechaConclusionSobreseimiento;
            entity.usuario_modificacion = usuario_creacion!;
        }

        public static AsuntosPenalesSuspensionCondicional CreateSuspension(
            ref AsuntosPenalesImputados entityImputado,
            string condicionesSuspension,
            DateTime? fechaCelebracionSuspension,
            decimal? reparacionDañoSuspension,
            DateTime? plazoSuspension,
            DateTime? fechaCumplimientoSuspension,
            bool? conclusionAsuntoSuspension,
            DateTime? fechaConclusionSuspension,
            string? usuario_creacion,
            EnumTipoEtapaInvestigacion etapaInvestigacion
        )
        {
            AsuntosPenalesSuspensionCondicional entity =
                new()
                {
                    id_imputado = entityImputado.id,
                    condiciones = condicionesSuspension,
                    fecha_celebracion = fechaCelebracionSuspension.GetValueOrDefault(),
                    reparacion_daño = reparacionDañoSuspension.GetValueOrDefault(),
                    fecha_plazo = plazoSuspension.GetValueOrDefault(),
                    fecha_cumplimiento = fechaCumplimientoSuspension.GetValueOrDefault(),
                    conclusion_asunto = conclusionAsuntoSuspension.GetValueOrDefault(),
                    fecha_conclusion = fechaConclusionSuspension,
                    usuario_creacion = usuario_creacion!,
                    id_tipo_etapa_investigacion = etapaInvestigacion.GetHashCode(),
                };

            return entity;
        }

        public static void UpdateSuspension(
            ref AsuntosPenalesImputados entityImputado,
            ref AsuntosPenalesSuspensionCondicional entity,
            string condicionesSuspension,
            DateTime? fechaCelebracionSuspension,
            decimal? reparacionDañoSuspension,
            DateTime? plazoSuspension,
            DateTime? fechaCumplimientoSuspension,
            bool? conclusionAsuntoSuspension,
            DateTime? fechaConclusionSuspension,
            string? usuario_creacion
        )
        {
            entity.condiciones = condicionesSuspension;
            entity.fecha_celebracion = fechaCelebracionSuspension.GetValueOrDefault();
            entity.reparacion_daño = reparacionDañoSuspension.GetValueOrDefault();
            entity.fecha_plazo = plazoSuspension.GetValueOrDefault();
            entity.fecha_cumplimiento = fechaCumplimientoSuspension.GetValueOrDefault();
            entity.conclusion_asunto = conclusionAsuntoSuspension.GetValueOrDefault();
            entity.fecha_conclusion = fechaConclusionSuspension;
            entity.usuario_modificacion = usuario_creacion!;
        }

        public static void UpdateImputadosEtapaIntermedia(
            ref AsuntosPenalesImputados entity,
            ref AsuntosPenalesSentencia entitySentencia,
            ref AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            ref AsuntosPenalesSobreseimiento entitySobreseimiento,
            ref AsuntosPenalesSuspensionCondicional entitySuspension,
            DateTime? fechaAudienciaIntermedia,
            bool? autoAperturaJuicioOral,
            bool? procedimientoAbreviadoIn,
            DateTime? fechaAperturaJuicioOral,
            int? sentencia,
            DateTime? fechaEmisionSentencia,
            decimal? reparacionDañoSentencia,
            bool? cumplimientoPrivadaLibertadSentencia,
            int? idAnioSentencia,
            int? idMesSentencia,
            int? idDiaSentencia,
            string? otorgamientoBeneficiosSentencia,
            string? accionesEjecucionSentencia,
            DateTime? fechaEjecucionSentencia,
            bool? conclusionAsuntoSentencia,
            DateTime? fechaConclusionSentencia,
            bool solucionAlterna,
            int? idTipoSolucionAlterna,
            string? condicionesAcuerdoReparatorio,
            DateTime? fechaAutorizacionAcuerdoReparatorio,
            decimal? reparacionDañoAcuerdoReparatorio,
            DateTime? fechaCelebracionAcuerdoReparatorio,
            bool? conclusionAsuntoAcuerdoReparatorio,
            DateTime? fechaConclusionAcuerdoReparatorio,
            bool? solicitudSobreseimiento,
            DateTime? fechaDeterminacionSobreseimiento,
            bool? conclusionAsuntoSobreseimiento,
            DateTime? fechaConclusionSobreseimiento,
            string condicionesSuspension,
            DateTime? fechaCelebracionSuspension,
            decimal? reparacionDañoSuspension,
            DateTime? plazoSuspension,
            DateTime? fechaCumplimientoSuspension,
            bool? conclusionAsuntoSuspension,
            DateTime? fechaConclusionSuspension,
            string usuario_modificacion
        )
        {
            entity.fecha_audiencia_intermedia = fechaAudienciaIntermedia;
            entity.auto_apertura_juicio_oral = autoAperturaJuicioOral;
            entity.procedimiento_abreviado_in = procedimientoAbreviadoIn;
            entity.fecha_apertura_juicio_oral = fechaAperturaJuicioOral;
            entity.usuario_modificacion = usuario_modificacion;
            entity.solucion_alterna_in = solucionAlterna;
            entity.id_tipo_solucion_alterna_in = idTipoSolucionAlterna;
            if (autoAperturaJuicioOral == true)
            {
                entity.id_estado_procesal = EnumEstadoProcesal.ETAPA_JUICIO.GetHashCode();
            }
            if (conclusionAsuntoSentencia == true)
            {
                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA_EN_PROCEDIMIENTO_ABREVIADO.GetHashCode();
            }

            if (solucionAlterna)
            {
                switch (idTipoSolucionAlterna)
                {
                    case 1:
                        if (conclusionAsuntoAcuerdoReparatorio == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode();
                        break;
                    case 2:
                        if (conclusionAsuntoSobreseimiento == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SOBRESEIMIENTO.GetHashCode();
                        break;
                    case 3:
                        if (conclusionAsuntoSuspension == true)
                            entity.id_estado_procesal =
                                entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SUSPENSIÓN_CONDICIONAL_DEL_PROCESO.GetHashCode();
                        break;
                }
            }

            if (procedimientoAbreviadoIn.GetValueOrDefault())
            {
                if (entitySentencia is null)
                {
                    entitySentencia = CreateSentencia(
                        ref entity,
                        sentencia,
                        fechaEmisionSentencia,
                        reparacionDañoSentencia,
                        cumplimientoPrivadaLibertadSentencia,
                        idAnioSentencia,
                        idMesSentencia,
                        idDiaSentencia,
                        otorgamientoBeneficiosSentencia,
                        accionesEjecucionSentencia,
                        fechaEjecucionSentencia,
                        conclusionAsuntoSentencia,
                        fechaConclusionSentencia,
                        usuario_modificacion,
                        EnumTipoEtapaInvestigacion.INTERMEDIA
                    );
                }
                else
                {
                    UpdateSentencia(
                        ref entity,
                        ref entitySentencia,
                        sentencia,
                        fechaEmisionSentencia,
                        reparacionDañoSentencia,
                        cumplimientoPrivadaLibertadSentencia,
                        idAnioSentencia,
                        idMesSentencia,
                        idDiaSentencia,
                        otorgamientoBeneficiosSentencia,
                        accionesEjecucionSentencia,
                        fechaEjecucionSentencia,
                        conclusionAsuntoSentencia,
                        fechaConclusionSentencia,
                        usuario_modificacion
                    );
                }
            }

            if (solucionAlterna)
            {
                switch (idTipoSolucionAlterna)
                {

                    case 1:
                        if (entityAcuerdoReparatorio is null)
                        {
                            entityAcuerdoReparatorio = CreateAcuerdoReparatorio(
                                ref entity,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.INTERMEDIA
                            );
                        }
                        else
                        {
                            UpdateAcuerdoReparatorio(
                                ref entity,
                                ref entityAcuerdoReparatorio,
                                condicionesAcuerdoReparatorio,
                                fechaAutorizacionAcuerdoReparatorio,
                                reparacionDañoAcuerdoReparatorio,
                                fechaCelebracionAcuerdoReparatorio,
                                conclusionAsuntoAcuerdoReparatorio,
                                fechaConclusionAcuerdoReparatorio,
                                usuario_modificacion
                            );
                        }
                        break;

                    case 2:
                        if (entitySobreseimiento is null)
                        {
                            entitySobreseimiento = CreateSobreisimiento(
                                ref entity,
                                solicitudSobreseimiento,
                                fechaDeterminacionSobreseimiento,
                                conclusionAsuntoSobreseimiento,
                                fechaConclusionSobreseimiento,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.INTERMEDIA
                            );
                        }
                        else
                        {
                            UpdateSobreisimiento(
                                ref entity,
                                ref entitySobreseimiento,
                                solicitudSobreseimiento,
                                fechaDeterminacionSobreseimiento,
                                conclusionAsuntoSobreseimiento,
                                fechaConclusionSobreseimiento,
                                usuario_modificacion
                            );
                        }
                        break;
                    case 3:
                        if (entitySuspension is null)
                        {
                            entitySuspension = CreateSuspension(
                                ref entity,
                                condicionesSuspension,
                                fechaCelebracionSuspension,
                                reparacionDañoSuspension,
                                plazoSuspension,
                                fechaCumplimientoSuspension,
                                conclusionAsuntoSuspension,
                                fechaConclusionSuspension,
                                usuario_modificacion,
                                EnumTipoEtapaInvestigacion.INTERMEDIA
                            );
                        }
                        else
                        {
                            UpdateSuspension(
                                ref entity,
                                ref entitySuspension,
                                condicionesSuspension,
                                fechaCelebracionSuspension,
                                reparacionDañoSuspension,
                                plazoSuspension,
                                fechaCumplimientoSuspension,
                                conclusionAsuntoSuspension,
                                fechaConclusionSuspension,
                                usuario_modificacion
                            );
                        }
                        break;
                }
            }
        }

        public static void UpdateImputadosEtapaJuicio(
            ref AsuntosPenalesImputados entity,
            ref AsuntosPenalesSentencia entitySentencia,
            ref AsuntosPenalesAcuerdoReparatorio entityAcuerdoReparatorio,
            ref AsuntosPenalesSobreseimiento entitySobreseimiento,
            ref AsuntosPenalesSuspensionCondicional entitySuspension,
            DateTime? fechaInicialAudienciaJuicio,
            DateTime? fechaFinalAudienciaJuicio,
            int? sentencia,
            DateTime? fechaEmisionSentencia,
            decimal? reparacionDañoSentencia,
            bool? cumplimientoPrivadaLibertadSentencia,
            int? idAnioSentencia,
            int? idMesSentencia,
            int? idDiaSentencia,
            string? otorgamientoBeneficiosSentencia,
            string? accionesEjecucionSentencia,
            DateTime? fechaEjecucionSentencia,
            bool? conclusionAsuntoSentencia,
            DateTime? fechaConclusionSentencia,
            string usuario_modificacion
        )
        {
            entity.fecha_inicial_audiencia_juicio = fechaInicialAudienciaJuicio;
            entity.fecha_final_audiencia_juicio = fechaFinalAudienciaJuicio;
            entity.usuario_modificacion = usuario_modificacion;
            if (sentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode())
            {
                if ((bool)conclusionAsuntoSentencia!)
                {
                    entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA.GetHashCode();
                }
                else
                {
                    entity.id_estado_procesal = EnumEstadoProcesal.SENTENCIA.GetHashCode();
                }
            }
            else
            {
                // if((bool)conclusionAsuntoSentencia!){
                //     entity.id_estado_procesal = EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_ABSOLUTORIA.GetHashCode();
                // }else{
                //     entity.id_estado_procesal = EnumEstadoProcesal.SENTENCIA.GetHashCode();
                // }
                entity.id_estado_procesal = EnumEstadoProcesal.SENTENCIA.GetHashCode();
            }
            if (entitySentencia is null)
            {
                entitySentencia = CreateSentencia(
                    ref entity,
                    sentencia,
                    fechaEmisionSentencia,
                    reparacionDañoSentencia,
                    cumplimientoPrivadaLibertadSentencia,
                    idAnioSentencia,
                    idMesSentencia,
                    idDiaSentencia,
                    otorgamientoBeneficiosSentencia,
                    accionesEjecucionSentencia,
                    fechaEjecucionSentencia,
                    conclusionAsuntoSentencia,
                    fechaConclusionSentencia,
                    usuario_modificacion,
                    EnumTipoEtapaInvestigacion.JUCIO
                );
            }
            else
            {
                UpdateSentencia(
                    ref entity,
                    ref entitySentencia,
                    sentencia,
                    fechaEmisionSentencia,
                    reparacionDañoSentencia,
                    cumplimientoPrivadaLibertadSentencia,
                    idAnioSentencia,
                    idMesSentencia,
                    idDiaSentencia,
                    otorgamientoBeneficiosSentencia,
                    accionesEjecucionSentencia,
                    fechaEjecucionSentencia,
                    conclusionAsuntoSentencia,
                    fechaConclusionSentencia,
                    usuario_modificacion
                );
            }
        }

        public static AsuntosPenalesImputados VerificarEstados(
            ref AsuntosPenales entityAsuntosPenales,
            List<AsuntosPenalesImputados> imputadosList,
            AsuntosPenalesImputados entityImputado
        )
        {
            if (imputadosList is null || !imputadosList.Any())
                return null!;

            int idAsuntoPenal = entityAsuntosPenales.id;
            List<int> estadoProcesalArray =
                new()
                {
                    EnumEstadoProcesal.CONCLUIDO_POR_FACULTAD_DE_ABSTENERSE_DE_INVESTIGAR.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_ARCHIVO_TEMPORAL.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_NO_EJERCICIO_DE_LA_ACCION_PENAL.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_CRITERIO_DE_OPORTUNIDAD.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA_EN_PROCEDIMIENTO_ABREVIADO.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_SOBRESEIMIENTO.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_SUSPENSIÓN_CONDICIONAL_DEL_PROCESO.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_ABSOLUTORIA.GetHashCode(),
                    EnumEstadoProcesal.CONCLUIDO_POR_SENTENCIA_CONDENATORIA.GetHashCode()
                };

            var entityListTemp = imputadosList.Where(c => c.id != entityImputado.id).ToList();
            entityListTemp.Add(entityImputado);

            var listEstado = entityListTemp
                .Where(e =>
                    e.id_asunto_penal == idAsuntoPenal
                    && !estadoProcesalArray.Any(x => x == e.id_estado_procesal)
                )
                .ToList();

            if (!listEstado.Any())
            {
                entityAsuntosPenales.id_estado_tarea = EnumEstadoTarea.CONCLUIDO.GetHashCode();
                entityAsuntosPenales.id_estado_procesal = entityListTemp
                    .FirstOrDefault(e => e.id == entityImputado.id)!
                    .id_estado_procesal;
                return entityListTemp.FirstOrDefault(e => e.id == entityImputado.id)!;
            }

            int estadoMasAlto = listEstado.Max(e => e.id_estado_procesal);
            entityAsuntosPenales.id_estado_procesal = estadoMasAlto;
            return listEstado.FirstOrDefault(e => e.id_estado_procesal == estadoMasAlto)!;
        }

        public static AsuntosPenalesImputados VerificarEstadosCreate(
            ref AsuntosPenales entityAsuntosPenales,
            List<AsuntosPenalesImputados> imputadosList,
            AsuntosPenalesImputados entityImputado
        )
        {
            if (imputadosList is null || !imputadosList.Any())
            {
                entityAsuntosPenales.id_estado_procesal = entityImputado.id_estado_procesal;
                return null!;
            }
            int idAsuntoPenal = entityAsuntosPenales.id;
            List<int> estadoProcesalArray = new()
            {
                EnumEstadoProcesal.CONCLUIDO_POR_FACULTAD_DE_ABSTENERSE_DE_INVESTIGAR.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_ARCHIVO_TEMPORAL.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_NO_EJERCICIO_DE_LA_ACCION_PENAL.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_ACUERDO_REPARATORIO.GetHashCode(),
                EnumEstadoProcesal.CONCLUIDO_POR_CRITERIO_DE_OPORTUNIDAD.GetHashCode(),
            };


            var listEstado = imputadosList!.Where(e =>
                e.id_asunto_penal == idAsuntoPenal &&
                !estadoProcesalArray.Any(x => x == e.id_estado_procesal)).ToList();

            int estadoMasAlto = listEstado.Max(e => e.id_estado_procesal);
            if (estadoMasAlto != entityImputado.id_estado_procesal)
            {
                entityAsuntosPenales.id_estado_procesal = estadoMasAlto;
            }
            return listEstado.FirstOrDefault(e => e.id_estado_procesal == estadoMasAlto)!;

        }


        public static AsuntosPenalesApelacion CreateApelacion(
          ref AsuntosPenalesImputados entityImputado,
          DateTime? fechaPresentacion,
          string? numeroTocaPenal,
          string? TipoOrganoJurisdiccional,
          int? idTipoResolucion,
          DateTime? fechaResolucion,
          string? descripcionResolucion,
          string? usuarioCreacion

      )
        {
            Guard.CatalogValue(ref TipoOrganoJurisdiccional, "Organo Jurisdiccional");
            Guard.CatalogValue(ref idTipoResolucion, "Tipo Resolucion");

            AsuntosPenalesApelacion entityApelacion =
                new()
                {
                    id_imputado = entityImputado.id,
                    fecha_presentacion = fechaPresentacion.GetValueOrDefault(),
                    numero_toca_penal = numeroTocaPenal!,
                    tipo_organo_jurisdiccional = TipoOrganoJurisdiccional,
                    id_tipo_resolucion = idTipoResolucion.GetValueOrDefault(),
                    fecha_resolucion = fechaResolucion.GetValueOrDefault(),
                    descripcion_resolucion = descripcionResolucion,
                    usuario_creacion = usuarioCreacion!,
                };

            return entityApelacion;
        }

        public static void UpdateApelacion(
           ref AsuntosPenalesApelacion entityApelacion,
           DateTime? fechaPresentacion,
           string? numeroTocaPenal,
           string? TipoOrganoJurisdiccional,
           int? idTipoResolucion,
           DateTime? fechaResolucion,
           string? descripcionResolucion,
           string? usuarioCreacion

       )
        {

            entityApelacion.fecha_presentacion = fechaPresentacion.GetValueOrDefault();
            entityApelacion.numero_toca_penal = numeroTocaPenal!;
            entityApelacion.tipo_organo_jurisdiccional = TipoOrganoJurisdiccional;
            entityApelacion.id_tipo_resolucion = idTipoResolucion.GetValueOrDefault();
            entityApelacion.fecha_resolucion = fechaResolucion.GetValueOrDefault();
            entityApelacion.descripcion_resolucion = descripcionResolucion;
            entityApelacion.usuario_modificacion = usuarioCreacion!;


        }
        public static void DeleteApelacion(ref AsuntosPenalesApelacion entity

       )
        {

        }

        public static AsuntosPenalesAmparo CreateAmparo(
           ref AsuntosPenalesImputados entityImputado,
           int? idTipoAmparo,
           int? idEstadoProcesal,
           DateTime? fechaPresentacion,
           string? numeroJuicioAmparo,
           string? TipoOrganoJurisdiccional,
           string? juzgado,
           int? idTipoResolucion,
           DateTime? fechaResolucion,
           string? descripcionResolucion,
           DateTime? fechaNotificacion,
           bool alegatos,
           string? numeroOficio,
           DateTime? fechaOficio,
           string? usuarioCreacion
       )
        {
            Guard.CatalogValue(ref TipoOrganoJurisdiccional, "Organo Jurisdiccional", false);
            Guard.CatalogValue(ref idTipoResolucion, "Tipo Resolucion");
            Guard.CatalogValue(ref idTipoAmparo, "Tipo Amparo");

            AsuntosPenalesAmparo entityAmparo =
                new()
                {
                    id_imputado = entityImputado.id,
                    id_tipo_amparo = idTipoAmparo.GetValueOrDefault(),
                    id_estado_procesal = idEstadoProcesal.GetValueOrDefault(),
                    fecha_presentacion = fechaPresentacion.GetValueOrDefault(),
                    numeroJuicioAmparo = numeroJuicioAmparo,
                    tipo_organo_jurisdiccional = TipoOrganoJurisdiccional,
                    juzgado = juzgado,
                    id_tipo_resolucion = idTipoResolucion.GetValueOrDefault(),
                    fecha_resolucion = fechaResolucion.GetValueOrDefault(),
                    descripcion_resolucion = descripcionResolucion,
                    fecha_notificacion = fechaNotificacion.GetValueOrDefault(),
                    alegatos = alegatos,
                    numero_oficio = numeroOficio,
                    fecha_oficio = fechaOficio.GetValueOrDefault(),
                    usuario_creacion = usuarioCreacion!,
                };

            return entityAmparo;
        }


        public static void UpdateAmparo(
            ref AsuntosPenalesAmparo entityAmparo,
            int? idTipoAmparo,
            int? idEstadoProcesal,
            DateTime? fechaPresentacion,
            string? numeroJuicioAmparo,
            string? TipoOrganoJurisdiccional,
            string? juzgado,
            int? idTipoResolucion,
            DateTime? fechaResolucion,
            string? descripcionResolucion,
            DateTime? fechaNotificacion,
            bool alegatos,
            string? numeroOficio,
            DateTime? fechaOficio,
            string? usuarioModificacion

        )
        {
            entityAmparo.id_tipo_amparo = idTipoAmparo.GetValueOrDefault();
            entityAmparo.id_estado_procesal = idEstadoProcesal.GetValueOrDefault();
            entityAmparo.fecha_presentacion = fechaPresentacion.GetValueOrDefault();
            entityAmparo.numeroJuicioAmparo = numeroJuicioAmparo;
            entityAmparo.tipo_organo_jurisdiccional = TipoOrganoJurisdiccional;
            entityAmparo.juzgado = juzgado;
            entityAmparo.id_tipo_resolucion = idTipoResolucion.GetValueOrDefault();
            entityAmparo.fecha_resolucion = fechaResolucion.GetValueOrDefault();
            entityAmparo.descripcion_resolucion = descripcionResolucion;
            entityAmparo.fecha_notificacion = fechaNotificacion.GetValueOrDefault();
            entityAmparo.alegatos = alegatos;
            entityAmparo.numero_oficio = numeroOficio;
            entityAmparo.fecha_oficio = fechaOficio.GetValueOrDefault();
            entityAmparo.usuario_modificacion = usuarioModificacion;

        }


        public static void DeleteAmparo(ref AsuntosPenalesAmparo entity
       )
        { }


        public static byte[] GenerarPDF(List<ResponseAcuseConclusion> data)
        {
            using var stream = new MemoryStream();
            using var document = new PdfDocument();

            var font = new XFont("Arial", 12, XFontStyle.Regular);
            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

            const int marginLeft = 40;
            const int marginTop = 110;
            const int pageHeightLimit = 800;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DibujarEncabezado(gfx, page, boldFont);

            int y = marginTop;

            // Función para agregar nueva página cuando sea necesario
            void CheckPageBreak()
            {
                if (y >= pageHeightLimit)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    DibujarEncabezado(gfx, page, boldFont);
                    y = marginTop;
                }
            }

            // Función para dibujar etiqueta y valor en una línea
            void DrawLabelValue(string label, string value)
            {
                CheckPageBreak();

                // Reemplaza null por texto vacío o un marcador
                value ??= "N/A"; // o simplemente: value = value ?? "";

                gfx.DrawString(label, boldFont, XBrushes.Black, new XPoint(marginLeft, y));
                double labelWidth = gfx.MeasureString(label, boldFont).Width;
                gfx.DrawString(value, font, XBrushes.Black, new XPoint(marginLeft + labelWidth, y));
                y += 20;
            }

            gfx.DrawString("SICOJ", boldFont, XBrushes.Black, new XPoint(250, y));
            y += 20;
            gfx.DrawString("ACUSE DE CONCLUSIÓN DE CAPTURA", boldFont, XBrushes.Black, new XPoint(180, y));
            y += 40;

            foreach (var item in data)
            {
                DrawLabelValue("Número de asunto: ", item.no_asunto!);
                DrawLabelValue("Número de expediente: ", item.numero_expediente_cadido!);
                y += 10;

                // CheckPageBreak();
                // gfx.DrawString("Motivo de conclusión:", boldFont, XBrushes.Black, new XPoint(marginLeft, y));
                // y += 20;

                // Si se desea mostrar motivos:
                // foreach (var motivo in item.MotivosConclusion)
                // {
                //     CheckPageBreak();
                //     gfx.DrawString("• " + motivo, font, XBrushes.Black, new XPoint(marginLeft + 20, y));
                //     y += 20;
                // }

                //y += 20;

                DrawLabelValue("Motivo de Conclusion: ", item.estadoProcesal!);
                DrawLabelValue("Fecha de generación: ", DateTime.Now.ToString("dd/MM/yyyy"));
                DrawLabelValue("Unidad Administrativa: ", item.adminControla!);
                DrawLabelValue("Administración Central: ", item.unidadRealizaSolicitud!);
                DrawLabelValue("Sub Administración: ", item.subAdministracion!);
                DrawLabelValue("Abogado: ", item.abogado);

                y += 20; // Espacio adicional entre registros
            }

            document.Save(stream, false);
            return stream.ToArray();

        }

        static void DibujarEncabezado(XGraphics gfx, PdfPage page, XFont boldFont)
        {
            var imgLeft = XImage.FromFile("Resources/logo_hacienda.png");
            var imgCenter = XImage.FromFile("Resources/logo_sat.png");

            double pageWidth = page.Width.Point;
            double imgHeight = 50;
            double imgWidth = 50;

            // Dibuja imagen izquierda
            gfx.DrawImage(imgLeft, 40, 20, imgWidth, imgHeight);

            // Dibuja imagen centrada
            gfx.DrawImage(imgCenter, (pageWidth - imgWidth) / 2, 20, imgWidth, imgHeight);

            // Texto largo alineado a la derecha
            string fullText = "Administración General Jurídica" +
                     "Administración Central de Asuntos Penales" +
                     "y Especiales";

            double maxWidth = 200; // Máximo ancho permitido para el texto a la derecha
            double startX = pageWidth - 40; // Margen derecho
            double startY = 20; // Parte superior del encabezado



            // Divide el texto en líneas
            List<string> lineas = DividirTextoEnLineas(fullText, boldFont, gfx, maxWidth);

            // Dibuja cada línea alineada a la derecha
            foreach (var linea in lineas)
            {
                XSize size = gfx.MeasureString(linea, boldFont);
                double x = startX - size.Width;
                gfx.DrawString(linea, boldFont, XBrushes.Black, new XPoint(x, startY + 15));
                startY += 15; // Espacio entre líneas
            }
        }

        static List<string> DividirTextoEnLineas(string texto, XFont font, XGraphics gfx, double maxWidth)
        {
            var palabras = texto.Split(' ');
            var lineas = new List<string>();
            string lineaActual = "";

            foreach (var palabra in palabras)
            {
                string prueba = (lineaActual.Length == 0) ? palabra : lineaActual + " " + palabra;
                XSize size = gfx.MeasureString(prueba, font);

                if (size.Width <= maxWidth)
                {
                    lineaActual = prueba;
                }
                else
                {
                    if (!string.IsNullOrEmpty(lineaActual))
                        lineas.Add(lineaActual);
                    lineaActual = palabra;
                }
            }

            if (!string.IsNullOrEmpty(lineaActual))
                lineas.Add(lineaActual);

            return lineas;
        }





    }
}
