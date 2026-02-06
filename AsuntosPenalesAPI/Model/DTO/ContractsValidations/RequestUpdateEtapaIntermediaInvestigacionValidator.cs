using System.Text.RegularExpressions;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateEtapaIntermediaInvestigacionValidator : AbstractValidator<RequestUpdateEtapaInvestigacionIntermedia>
    {
        public RequestUpdateEtapaIntermediaInvestigacionValidator()
        {
            RuleFor(c => c.id)
                    .NotNull()
                    .WithMessage("El id de imputado a editar es requerido.")
                    .NotEmpty()
                    .WithMessage("El id imputado a editar es requerido.");

            RuleFor(c => c.idAsuntoPenal)
                .NotNull()
                .WithMessage("El id del asunto penal a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id del asunto penal a editar es requerido.");

            // RuleFor(c => c.fechaEmisionSentencia)
            //     .NotNull()
            //     .NotEmpty()
            //     .WithMessage("La fecha de terminación de investigación es requerido")
            //     .Must(FluentValidationGuard.BeValidateDateFormat!)
            //     .WithMessage("El formato de la fecha de terminación de investigación no es válido");
            
            RuleFor(c => c.autoAperturaJuicioOral)
            .NotNull()
            .WithMessage("Procedimiento Abreviado es obligatorio");

            RuleFor(c => c.fechaAperturaJuicioOral)
            .NotNull()
            .When(c => c.autoAperturaJuicioOral)
            .NotEmpty()
            .WithMessage("La fecha de terminación de investigación es requerido")
            .When(c => c.autoAperturaJuicioOral)
            .Must(FluentValidationGuard.BeValidateDateFormat!)
            .When(c => c.autoAperturaJuicioOral)
            .WithMessage("El formato de la fecha de terminación de investigación no es válido");

            #region Procedimiento abreviado Sentencia

            RuleFor(c => c.idSentencia)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para tipo de sentencia debe ser un valor mayor a cero."
                )
                .When(c => (bool)c.procedimientoAbreviadoIn!);
            
            RuleFor(c => c.fechaEmisionSentencia)
                .NotNull()
                .When(c => c.procedimientoAbreviadoIn == true)
                .NotEmpty()
                .WithMessage("La fecha de emisión sentencia es requerido")
                .When(c => c.procedimientoAbreviadoIn == true)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.procedimientoAbreviadoIn == true)
                .WithMessage("El formato de la fecha de emisión sentencia no es válido");
            
            RuleFor(x => x.reparacionDañoSentencia)
                .Must(repDaño => repDaño == null || Regex.IsMatch(repDaño.Value.ToString("F2"), @"^\d{1,9}(\.\d{1,2})?$"))
                .WithMessage("El valor debe ser un número válido con hasta 9 dígitos enteros y 2 decimales.")
                .When(c => (bool)c.procedimientoAbreviadoIn!);
            
            RuleFor(c => c.cumplimientoPrivadaLibertadSentencia)
                .NotNull()
                .WithMessage("Cumplimiento de pena privada de la libertad es obligatorio") 
                .When(c => (bool)c.procedimientoAbreviadoIn!); 

            RuleFor(c => c.otorgamientoBeneficiosSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Otorgamiento de beneficios es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Otorgamiento de beneficios requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Otorgamiento de beneficios no válido")
                .When(c => (bool)c.procedimientoAbreviadoIn!);  

            RuleFor(c => c.accionesEjecucionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Acciones de ejecución es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Acciones de ejecución requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Acciones de ejecución no válido")
                .When(c => (bool)c.procedimientoAbreviadoIn!);   

            RuleFor(c => c.fechaEjecucionSentencia)
                .NotNull()
                .When(c => (bool)c.procedimientoAbreviadoIn!)
                .NotEmpty()
                .WithMessage("La fecha de ejecución es requerido")
                .When(c => (bool)c.procedimientoAbreviadoIn!)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => (bool)c.procedimientoAbreviadoIn!)
                .WithMessage("El formato de la fecha de ejecución no es válido");

            RuleFor(c => c.conclusionAsuntoSentencia)
                .NotNull()
                .WithMessage("Conclusion del asunto es obligatorio")
                .When(c => (bool)c.procedimientoAbreviadoIn!);
            
            RuleFor(c => c.fechaConclusionSentencia)
                .NotNull()
                .When(c => (bool)c.procedimientoAbreviadoIn! && c.conclusionAsuntoSentencia.Equals(true))
                .NotEmpty()
                .WithMessage("La fecha de conclusión es requerido")
                .When(c => (bool)c.procedimientoAbreviadoIn! && c.conclusionAsuntoSentencia == true)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => (bool)c.procedimientoAbreviadoIn! && c.conclusionAsuntoSentencia == true)
                .WithMessage("El formato de la fecha de conclusión no es válido");
            #endregion

            #region Solución alterna
            RuleFor(c => c.solucionAlterna)
                .NotNull()
                .WithMessage("Se concluye investigación es obligatorio")
                .When(c => (bool)!c.procedimientoAbreviadoIn!);

            RuleFor(c => c.idTipoSolucionAlterna)
                .Empty()
                .WithMessage("Tipo de solución alterna no es requerida")
                .When(c => !c.solucionAlterna);
            #endregion

            #region Acuerdo Reparatorio

            RuleFor(c => c.condicionesAcuerdoReparatorio)
            .NotNull()
            .WithMessage("Las condiciones son requeridas.")
            .NotEmpty()
            .WithMessage("Las condiciones son requeridas.")
            .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.AcuerdoReparatorio.GetHashCode());

            #endregion

            #region Sobreseimineto

            RuleFor(c => c.solicitudSobreseimiento)
                .NotNull()
                .WithMessage("Solicitud de sobreseimiento es obligatorio")
                .When(c => c.solucionAlterna! && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.Sobreseimineto.GetHashCode());

            RuleFor(c => c.fechaDeterminacionSobreseimiento)
                .NotNull()
                .When(c =>c.solucionAlterna! && c.solicitudSobreseimiento.Equals(true))
                .NotEmpty()
                .WithMessage("La fecha de determinación es requerido")
                .When(c => c.solucionAlterna! && c.solicitudSobreseimiento == true)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.solucionAlterna! && c.solicitudSobreseimiento == true)
                .WithMessage("El formato de la fecha de determinación no es válido");

            RuleFor(c => c.conclusionAsuntoSobreseimiento)
                .NotNull()
                .WithMessage("Conclusion del asunto sobreseimiento es obligatorio")
                .When(c => c.solucionAlterna! && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.Sobreseimineto.GetHashCode());
            
            RuleFor(c => c.fechaConclusionSobreseimiento)
                .NotNull()
                .When(c =>c.solucionAlterna! && c.conclusionAsuntoSobreseimiento.Equals(true))
                .NotEmpty()
                .WithMessage("La fecha de conclusión sobreseimiento es requerido")
                .When(c => c.solucionAlterna! && c.conclusionAsuntoSobreseimiento == true)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.solucionAlterna! && c.conclusionAsuntoSobreseimiento == true)
                .WithMessage("El formato de la fecha de conclusión sobreseimiento no es válido");

            #endregion

            #region Suspension

            RuleFor(c => c.condicionesSuspension)
            .NotNull()
            .WithMessage("Las condiciones son requeridas.")
            .NotEmpty()
            .WithMessage("Las condiciones son requeridas.")
            .When(c => c.solucionAlterna && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.Suspension.GetHashCode());

            #endregion
        
        }
        
    }
}