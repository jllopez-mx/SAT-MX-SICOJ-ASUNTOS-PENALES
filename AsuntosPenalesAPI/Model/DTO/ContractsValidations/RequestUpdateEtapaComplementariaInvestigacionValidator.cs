using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.ViewModels.Enums;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateEtapaComplementariaInvestigacionValidator : AbstractValidator<RequestUpdateEtapaInvestigacionComplementaria>
    {
        public RequestUpdateEtapaComplementariaInvestigacionValidator()
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

            #region Procedimiento abreviado Sentencia
            RuleFor(c => c.procedimientoAbreviado)
                .NotNull()
                .WithMessage("Procedimiento Abreviado es obligatorio");

            RuleFor(c => c.idTipoSentencia)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para tipo de sentencia debe ser un valor mayor a cero."
                )
                .When(c => c.procedimientoAbreviado);
            
            RuleFor(c => c.fechaEmisionSentencia)
                .NotNull()
                .When(c => c.procedimientoAbreviado)
                .NotEmpty()
                .WithMessage("La fecha de terminación de investigación es requerido")
                .When(c => c.procedimientoAbreviado)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.procedimientoAbreviado)
                .WithMessage("El formato de la fecha de terminación de investigación no es válido");
            
            RuleFor(x => x.reparacionDañoSentencia)
                .Must(repDaño => repDaño == null || Regex.IsMatch(repDaño.Value.ToString("F2"), @"^\d{1,9}(\.\d{1,2})?$"))
                .WithMessage("El valor debe ser un número válido con hasta 9 dígitos enteros y 2 decimales.")
                .When(c => c.procedimientoAbreviado);
            
            RuleFor(c => c.cumplimientoPrivadaLibertadSentencia)
                .NotNull()
                .WithMessage("Cumplimiento de pena privada de la libertad es obligatorio") 
                .When(c => c.procedimientoAbreviado); 

            RuleFor(c => c.otorgamientoBeneficiosSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Otorgamiento de beneficios es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Otorgamiento de beneficios requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Otorgamiento de beneficios no válido")
                .When(c => c.procedimientoAbreviado);  

            RuleFor(c => c.accionesEjecucionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Acciones de ejecución es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Acciones de ejecución requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Acciones de ejecución no válido")
                .When(c => c.procedimientoAbreviado);   

            RuleFor(c => c.fechaEjecucionSentencia)
                .NotNull()
                .When(c => c.procedimientoAbreviado)
                .NotEmpty()
                .WithMessage("La fecha de ejecución es requerido")
                .When(c => c.procedimientoAbreviado)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.procedimientoAbreviado)
                .WithMessage("El formato de la fecha de ejecución no es válido");

            RuleFor(c => c.conclusionAsuntoSentencia)
                .NotNull()
                .WithMessage("Conclusion del asunto es obligatorio")
                .When(c => c.procedimientoAbreviado);
            
            RuleFor(c => c.fechaConclusionSentencia)
                .NotNull()
                .When(c => c.procedimientoAbreviado && c.conclusionAsuntoSentencia.Equals(true))
                .NotEmpty()
                .WithMessage("La fecha de conclusión es requerido")
                .When(c => c.procedimientoAbreviado && c.conclusionAsuntoSentencia == true)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.procedimientoAbreviado && c.conclusionAsuntoSentencia == true)
                .WithMessage("El formato de la fecha de conclusión no es válido");

            #region Solución alterna

            RuleFor(c => c.idTipoSolucionAlterna)
                .Empty()
                .WithMessage("Tipo de solución alterna no es requerida")
                .When(c => c.solucionAlterna == null);
            #endregion

            #region Acuerdo Reparatorio

            RuleFor(c => c.condicionesAcuerdoReparatorio)
            .NotNull()
            .WithMessage("Las condiciones son requeridas.")
            .NotEmpty()
            .WithMessage("Las condiciones son requeridas.")
            .When(c => c.solucionAlterna== true && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.AcuerdoReparatorio.GetHashCode());

            #endregion

            #region Sobreseimineto
            #endregion

            #region Suspension

            RuleFor(c => c.condicionesSuspension)
            .NotNull()
            .WithMessage("Las condiciones son requeridas.")
            .NotEmpty()
            .WithMessage("Las condiciones son requeridas.")
            .When(c => c.solucionAlterna == true && c.idTipoSolucionAlterna == EnumTipoSolucionAlternaEtapaComplementaria.Suspension.GetHashCode());

            #endregion




            #region Escrito de acusación

            RuleFor(c => c.escritoAcusacion)
                .NotNull()
                .WithMessage("Escrito de acusación es obligatorio")
                .When(c => !c.procedimientoAbreviado && c.solucionAlterna == null);

            RuleFor(c => c.fechaEscritoAcusacion)
                .NotNull()
                .When(c => !c.procedimientoAbreviado && c.solucionAlterna == null && c.escritoAcusacion)
                .NotEmpty()
                .WithMessage("La fecha de escrito de acusación es requerido")
                .When(c => !c.procedimientoAbreviado && c.solucionAlterna == null && c.escritoAcusacion)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => !c.procedimientoAbreviado && c.solucionAlterna == null && c.escritoAcusacion)
                .WithMessage("El formato de la fecha de escrito de acusación no es válido"); 

            #endregion

            
            #endregion
        }
    }
}