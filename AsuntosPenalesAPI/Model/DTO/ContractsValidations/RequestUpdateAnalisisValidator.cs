using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateAnalisisValidator : AbstractValidator<RequestUpdateAnalisis>
    {         
        public RequestUpdateAnalisisValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El id a editar es requerido.")
            .NotEmpty()
            .WithMessage("El id a editar es requerido.");

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
                .WithMessage("El Oficio requerimiento es requerido")
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
                .WithMessage("La fecha requerimiento es requerido")
                .When(c => c.requerimiento)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.requerimiento)
                .WithMessage("El formato de la fecha de oficio no es válido");

            #endregion

            #region Validaciones requerimiento atendido
            RuleFor(c => c.requerimientoAtendido)
                .Null()
                .When(c => !c.requerimiento && c.requerimientoAtendido.Equals(true))
                .WithMessage(
                    "se atendio el requirimiento no es requerido ya que no se tiene requerimiento."
                );

            RuleFor(c => c.requerimientoAtendido)
                .NotNull()
                .When(c => c.requerimiento)
                .WithMessage("Requerimiento es un campo obligatorio");

            RuleFor(c => c.oficioAtencion)
                .Empty()
                .WithMessage("El Oficio atención no es requerido")
                .When(c => !c.requerimientoAtendido.GetValueOrDefault() && c.requerimiento);

            RuleFor(c => c.oficioAtencion)
                .Empty()
                .WithMessage("El Oficio atención no es requerido")
                .When(c => !c.requerimiento);

            RuleFor(c => c.oficioAtencion)
                .NotNull()
                .When(c => c.requerimientoAtendido.GetValueOrDefault())
                .NotEmpty()
                .WithMessage("El Oficio atencion  es requerido")
                .When(c => c.requerimientoAtendido.GetValueOrDefault() && c.requerimiento)
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Oficio de atención no válido");

            RuleFor(c => c.fechaAtencion)
                .Empty()
                .WithMessage("La fecha atencion no es requerido")
                .When(c => !c.requerimientoAtendido.GetValueOrDefault() && c.requerimiento);
            
            RuleFor(c => c.fechaAtencion)
                .Empty()
                .WithMessage("La fecha atencion no es requerido")
                .When(c =>  !c.requerimiento);

            RuleFor(c => c.fechaAtencion)
                .NotNull()
                .When(c => c.requerimientoAtendido.GetValueOrDefault() && c.requerimiento)
                .NotEmpty()
                .WithMessage("La fecha atencion es requerido")
                .When(c => c.requerimientoAtendido.GetValueOrDefault()  && c.requerimiento)
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.requerimientoAtendido.GetValueOrDefault()  && c.requerimiento)
                .WithMessage("El formato de la fecha de atención no es válido");

            #endregion

            RuleFor(c => c.idDeterminacionAsuntoPenal)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para determinar el asunto penal debe ser un valor mayor a cero."
                );

            RuleFor(c => c.fechaDeterminacion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha de determinacion es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de determinación no es válido");

        }
    }
}
