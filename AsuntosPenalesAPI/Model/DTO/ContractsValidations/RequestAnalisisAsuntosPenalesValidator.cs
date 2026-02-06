using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestAnalisisAsuntosPenalesValidator : AbstractValidator<RequestAnalisis>
    {
        public RequestAnalisisAsuntosPenalesValidator()
        {
            RuleFor(c => c.idAsuntoPenal)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("El valor indicado para el id debe ser un valor mayor a cero.");

            #region Validaciones de requerimiento
            RuleFor(c => c.requerimiento).NotNull().WithMessage("Requerimiento es un obligatorio");

            RuleFor(c => c.oficioRequerimiento)
                .Empty()
                .WithMessage("El Oficio requerimiento no es requerido")
                .When(c => !c.requerimiento);

            RuleFor(c => c.oficioRequerimiento)
                .NotNull()
                .When(c => c.requerimiento)
                .NotEmpty()
                .WithMessage("El Oficio de requerimiento es requerido")
                .When(c => c.requerimiento)
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Oficio de requerimiento no válido");

            RuleFor(c => c.fechaRequerimiento)
                .Empty()
                .WithMessage("La fecha requerimiento no es requerido")
                .When(c => !c.requerimiento);

            RuleFor(c => c.fechaRequerimiento)
                .NotNull()
                .When(c => c.requerimiento)
                .NotEmpty()
                .WithMessage("La fecha requerimiento no es requerido")
                .When(c => c.requerimiento)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.requerimiento)
                .WithMessage("El formato de la fecha de oficio no es válido");

            #endregion

            #region Validaciones requerimiento atendido
            // Si requerimiento es false no se debe de asignar valor a requerimiento atendido
            RuleFor(c => c.requerimientoAtendido)
                .Null()
                .When(c => !c.requerimiento && c.requerimientoAtendido.Equals(true))
                .WithMessage(
                    "se atendio el requirimiento no es requerido ya que no se tiene requerimiento."
                );

            RuleFor(c => c.requerimientoAtendido)
                .NotNull()
                .When(c => c.requerimiento)
                .WithMessage("Requerimiento es un obligatorio");

            RuleFor(c => c.oficioAtencion)
                .Empty()
                .WithMessage("El Oficio atencion no es requerido")
                .When(c => !c.requerimientoAtendido.GetValueOrDefault());

            RuleFor(c => c.oficioAtencion)
                .NotNull()
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .NotEmpty()
                .WithMessage("El Oficio de atencion es requerido")
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Oficio de atención no válido");
                
            RuleFor(c => c.fechaAtencion)
                .Empty()
                .WithMessage("La fecha atencion no es requerido")
                .When(c => !c.requerimientoAtendido.GetValueOrDefault());

            RuleFor(c => c.fechaAtencion)
                .NotNull()
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .NotEmpty()
                .WithMessage("La fecha atencion no es requerido")
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .WithMessage("El formato de la fecha de oficio no es válido");

            #endregion

            RuleFor(c => c.idDeterminacionAsuntoPenal)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para determiar el asunto penal debe ser un valor mayor a cero."
                );

            RuleFor(c => c.fechaDeterminacion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha de determinacion es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de oficio no es válido");
        }
    }
}
