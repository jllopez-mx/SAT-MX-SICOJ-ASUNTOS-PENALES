using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
  public class RequestRemisionAsuntosPenalesValidator : AbstractValidator<RequestRemitir>
  {
    public RequestRemisionAsuntosPenalesValidator()
    {
      RuleFor(c => c.idAsuntoPenal)
           .NotEmpty()
           .GreaterThan(0)
           .WithMessage("El valor indicado para el id debe ser un valor mayor a cero.");

      RuleFor(c => c.idUnidadAdministrativaRecibe)
         .NotEmpty()
         .WithMessage("No ha indicado el parametro de la Unidad Administrativa")
         .GreaterThan(0)
         .When(c => c.idTipoAutoridad == 1)
         .WithMessage("El valor indicado para la unidad administrativa que recibe debe ser un valor mayor a cero.");


      RuleFor(c => c.idUnidadAdministrativaExterna)
         .NotEmpty()
         .WithMessage("No ha indicado el parametro de la Unidad Administrativa")
         .GreaterThan(0)
         .When(c => c.idTipoAutoridad == 2)
         .WithMessage("El valor indicado para la unidad administrativa Externa  debe ser un valor mayor a cero.");

      RuleFor(c => c.idTipoAutoridad)
        .NotEmpty()
        .GreaterThan(0)
        .WithMessage("El valor indicado para el tipo de autoridad debe ser un valor mayor a cero.");
      
      RuleFor(c => c.numeroOficio)
          .NotEmpty()
          .WithMessage("No ha indicado el parametro de numero de oficio");
      RuleFor(c => c.fechaOficio)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha de oficio es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de oficio no es válido");
    }
     private bool BeValidateDateFormar(string dateString)
        {
            return DateTime.TryParse(dateString, out _);
        }
  }
}