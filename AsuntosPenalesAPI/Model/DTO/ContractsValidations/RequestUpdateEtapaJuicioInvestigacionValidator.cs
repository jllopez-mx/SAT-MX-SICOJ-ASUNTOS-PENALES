using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateEtapaJuicioInvestigacionValidator : AbstractValidator<RequestUpdateEtapaInvestigacionJuicio>
    {
        public RequestUpdateEtapaJuicioInvestigacionValidator()
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

            RuleFor(c => c.fechaInicialAudienciaJuicio)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha inicial de audiencia de juicio es requerido")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha inicial de audiencia de juicio no es válido");
            
            RuleFor(c => c.fechaFinalAudienciaJuicio)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha final de audiencia de juicio es requerido")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha final de audiencia de juicio no es válido");

            RuleFor(c => c.idSentencia)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para tipo de sentencia debe ser un valor mayor a cero."
                );
            
            RuleFor(c => c.fechaEmisionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de emisión de es requerida")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de emisión no es válido");
            #region CONDENATORIA
            RuleFor(c => c.reparacionDañoSentencia)
                .Must(repDaño => repDaño == null || Regex.IsMatch(repDaño.Value.ToString("F2"), @"^\d{1,9}(\.\d{1,2})?$"))
                .WithMessage("El valor debe ser un número válido con hasta 9 dígitos enteros y 2 decimales.")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());   
            
            RuleFor(c => c.cumplimientoPrivadaLibertadSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Cumplimiento de pena privada de la libertad es obligatorio") 
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());
            
            RuleFor(c => c.idAnioSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Los años de pena de prisión son obligatorio")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());
            
            RuleFor(c => c.idMesSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Los meses de pena de prisión son obligatorio")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());

            RuleFor(c => c.idDiaSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Los diás de pena de prisión son obligatorio")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());

            RuleFor(c => c.otorgamientoBeneficiosSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Otorgamiento de beneficios es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Otorgamiento de beneficios requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Otorgamiento de beneficios no válido")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());
            RuleFor(c => c.accionesEjecucionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Acciones de ejecución es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Acciones de ejecución requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Acciones de ejecución no válido")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode()); 
            RuleFor(c => c.fechaEjecucionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de ejecución es requerida")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de ejecución no es válido")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());
            RuleFor(c => c.conclusionAsuntoSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Conclusión del asunto es obligatorio") 
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode());
            RuleFor(c => c.fechaConclusionSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de conclusión es requerida")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de ejecución no es válido")
                .When(c => c.idSentencia == EnumTipoSentencia.CONDENATORIA.GetHashCode() && (bool)c.conclusionAsuntoSentencia!);
            #endregion

            #region ABSOLUTORIA
            RuleFor(c => c.reparacionDañoSentencia)
                .Null()
                .Empty()
                .WithMessage("Reparacion de daño no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());   
            
            RuleFor(c => c.cumplimientoPrivadaLibertadSentencia)
                .Null()
                .Empty()
                .WithMessage("Cumplimiento de pena privada de la libertad no es requerido.") 
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());
            
            RuleFor(c => c.idAnioSentencia)
                .Null()
                .Empty()
                .WithMessage("Los años de pena de presión no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());
            
            RuleFor(c => c.idMesSentencia)
                .Null()
                .Empty()
                .WithMessage("Los meses de pena de presión no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());

            RuleFor(c => c.idDiaSentencia)
                .Null()
                .Empty()
                .WithMessage("Los diás de pena de presión no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());

            RuleFor(c => c.otorgamientoBeneficiosSentencia)
                .Null()
                .Empty()
                .WithMessage("Otorgamiento de beneficios no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());
            RuleFor(c => c.accionesEjecucionSentencia)
                .Null()
                .Empty()
                .WithMessage("Acciones de ejecución no es requerido.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode()); 
            RuleFor(c => c.fechaEjecucionSentencia)
                .Null()
                .Empty()
                .WithMessage("La fecha de ejecución no es requerida.")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());
            RuleFor(c => c.conclusionAsuntoSentencia)
                .NotNull()
                .NotEmpty()
                .WithMessage("Conclusión del asunto es obligatorio")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode());
            RuleFor(c => c.fechaConclusionSentencia)
                .Null()
                .Empty()
                .WithMessage("La fecha de conclusión no es requerida")
                .When(c => c.idSentencia == EnumTipoSentencia.ABSOLUTORIA.GetHashCode() && (bool)c.conclusionAsuntoSentencia!);
            #endregion
        }
        
    }
}