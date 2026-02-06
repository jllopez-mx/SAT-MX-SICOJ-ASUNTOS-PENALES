using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateEtapaInicialInvestigacionValidator
        : AbstractValidator<RequestUpdateEtapaInvestigacionInicial>
    {
        public RequestUpdateEtapaInicialInvestigacionValidator()
        {
            RuleFor(c => c.id)
                .NotNull()
                .WithMessage("El id a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id a editar es requerido.");

            RuleFor(c => c.idAsuntoPenal)
                .NotNull()
                .WithMessage("El id del asunto penal a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id del asunto penal a editar es requerido.");

            #region Validaciones de requerimiento

            RuleFor(c => c.concluyeInvestigacion)
                .NotNull()
                .WithMessage("Se concluye investigación es obligatorio");

            RuleFor(c => c.idTerminacionInvestigacion)
                .Empty()
                .WithMessage("Forma de terminación de la investigación no es requerida")
                .When(c => !c.concluyeInvestigacion);

            RuleFor(c => c.idTerminacionInvestigacion)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para terminación del asunto penal debe ser un valor mayor a cero."
                )
                .When(c => c.concluyeInvestigacion);

            RuleFor(c => c.fechaTerminacionInvestigacion)
                .Empty()
                .WithMessage("Fecha de terminación de la investigación no es requerida")
                .When(c => !c.concluyeInvestigacion);

            RuleFor(c => c.fechaTerminacionInvestigacion)
                .NotNull()
                .When(c => c.concluyeInvestigacion)
                .NotEmpty()
                .WithMessage("La fecha de terminación de investigación es requerido")
                .When(c => c.concluyeInvestigacion)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.concluyeInvestigacion)
                .WithMessage("El formato de la fecha de terminación de investigación no es válido");

            RuleFor(c => c.idTipoSolucionAlterna)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para tipo de solución alterna debe ser un valor mayor a cero."
                )
                .When(c => c.solucionAlterna);

            RuleFor(c => c.idTipoSolucionAlterna)
                .Empty()
                .WithMessage("Tipo de solución alterna no es requerida")
                .When(c => !c.solucionAlterna);

            // RuleFor(c => c.condiciones)
            // .NotNull()
            // .WithMessage("Las condiciones son requeridas.")
            // .NotEmpty()
            // .WithMessage("Las condiciones son requeridas.")
            // .When(c => c.solucionAlterna);

            // RuleFor(c => c.fechaAutorizacionAcuerdoReparatorio)
            //     .NotNull()
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
            //     .NotEmpty()
            //     .WithMessage("La fecha de autorización de acuerdo reparatorio es requerido")
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
            //     .Must(FluentValidationGuard.BeValidateDateFormat!)
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
            //     .WithMessage("El formato de la fecha de autorización de acuerdo reparatorio no es válido");

            // RuleFor(c => c.reparacionDaño)
            // .NotEmpty()
            //     .GreaterThan(0)
            //     .WithMessage(
            //         "El valor indicado para tipo de solución alterna debe ser un valor mayor a cero."
            //     )
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1);

            RuleFor(c => c.fechaCelebracionAcuerdoReparatorio)
                .NotNull()
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
                .NotEmpty()
                .WithMessage("La fecha de celebración de acuerdo reparatorio es requerido")
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 1)
                .WithMessage(
                    "El formato de la fecha de celebración de acuerdo reparatorio no es válido"
                );

            RuleFor(c => c.fechaCriterioOportunidad)
                .NotNull()
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2)
                .NotEmpty()
                .WithMessage("La fecha de celebración de acuerdo reparatorio es requerido")
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2)
                .WithMessage(
                    "El formato de la fecha de celebración de acuerdo reparatorio no es válido"
                );

            // RuleFor(c => c.conclusionAsunto)
            //     .NotNull()
            //     .WithMessage("Conclusion del asunto es obligatorio")
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2);

            // RuleFor(c => c.fechaConclusion)
            //     .NotNull()
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2 && c.conclusionAsunto.Equals(true))
            //     .NotEmpty()
            //     .WithMessage("La fecha de conclusión es requerido")
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2 && c.conclusionAsunto.Equals(true))
            //     .Must(FluentValidationGuard.BeValidateDateFormat!)
            //     .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == 2 && c.conclusionAsunto.Equals(true))
            //     .WithMessage("El formato de la fecha de conclusión no es válido");

            RuleFor(c => c.fechaSolicitudAudienciaInicial)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de solicitud de audiencia inicial es requerido")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage(
                    "El formato de la fecha de solicitud de audiencia inicial no es válido"
                );

            RuleFor(c => c.idCentroJusticia)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para centro de justicia debe ser un valor mayor a cero."
                );

            RuleFor(c => c.causaPenal)
                .NotNull()
                .NotEmpty()
                .WithMessage("La causa penal  es requerida")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Causa penal no válida");

            RuleFor(c => c.fechaAudienciaInicial)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de audiencia inicial es requerido")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de audiencia inicial no es válido");

            RuleFor(c => c.autoVinculacionProceso)
                .NotNull()
                .WithMessage("Auto de vinculación a proceso es un obligatorio");

            RuleFor(c => c.fechaAutoVinculacion)
                .Empty()
                .WithMessage("Fecha de auto vinculación a proceso no es requerida")
                .When(c => !c.autoVinculacionProceso);

            RuleFor(c => c.fechaAutoVinculacion)
                .NotNull()
                .When(c => c.autoVinculacionProceso)
                .NotEmpty()
                .WithMessage("La fecha de auto vinculación a proceso es requerido")
                .When(c => c.autoVinculacionProceso)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.autoVinculacionProceso)
                .WithMessage("El formato de la fecha de auto vinculación a proceso no es válido");

            #endregion
        }
    }
}
