using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestImputadosAmparoAsuntosPenalesValidator: AbstractValidator<RequestAmparo>
    {
        public RequestImputadosAmparoAsuntosPenalesValidator()
        {
            RuleFor(c => c.idImputado)
            .NotNull()
            .WithMessage("El nombre del imputado es requerido.")
            .NotEmpty()
            .WithMessage("El nombre del imputado es requerido.");

            RuleFor(c => c.idTipoAmparo)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(
                "El valor indicado para tipo de amparo debe ser un valor mayor a cero."
            );

            RuleFor(c => c.fechaPresentacion)
            .NotNull()
            .NotEmpty()
            .WithMessage("La fecha de presentación es requerida")
            .Must(FluentValidationGuard.BeValidateDateFormat!)
            .WithMessage("El formato de la fecha de presentación no es válido");

            RuleFor(c => c.numeroJuicioAmparo)
            .NotNull()
            .NotEmpty()
            .WithMessage("Número de juicio de amparo es obligatorio.")
            .MaximumLength(10)
            .WithMessage("Número de juicio de amparo requiere maximo 10 caracteres")
            .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
            .WithMessage("Número de juicio de amparo no válido");

            RuleFor(c => c.TipoOrganoJurisdiccional)
            .NotEmpty()
            .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
            .WithMessage("Tipo de organo jurisdiccional no válido")
            .When( c=> c.idTipoAmparo == 2 );

            RuleFor(c => c.TipoOrganoJurisdiccional )
            .Empty()
            .WithMessage("Tipo de organo jurisdiccional no es requerido")
            .When(c => c.idTipoAmparo == 1 );

            RuleFor(c => c.juzgado)
            .NotNull()
            .NotEmpty()
            .WithMessage("Juzgado es obligatorio.")
            .MaximumLength(200)
            .WithMessage("Juzgado requiere maximo 200 caracteres")
            .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
            .WithMessage("Juzgado no válido")
            .When(c => c.idTipoAmparo == 1);

            RuleFor(c => c.juzgado)
            .Empty()
            .WithMessage("Juzgado no es requerido")
            .When(c => c.idTipoAmparo == 2);

            RuleFor(c => c.idTipoResolucion)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(
                "El valor indicado para resolución debe ser un valor mayor a cero."
            );

            RuleFor(c => c.fechaResolucion)
            .NotNull()
            .NotEmpty()
            .WithMessage("La fecha de resolución es requerida")
            .Must(FluentValidationGuard.BeValidateDateFormat!)
            .WithMessage("El formato de la fecha de resolución no es válido");

            RuleFor(c => c.descripcionResolucion)
            .NotNull()
            .NotEmpty()
            .WithMessage("La descripción de resolución es obligatoria.")
            .MaximumLength(1000)
            .WithMessage("La descripción de resolución requiere maximo 1000 caracteres")
            .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
            .WithMessage("Descripción de resolución no válida");

            RuleFor(c => c.fechaResolucion)
            .NotNull()
            .NotEmpty()
            .WithMessage("La fecha de resolución es requerida")
            .Must(FluentValidationGuard.BeValidateDateFormat!)
            .WithMessage("El formato de la fecha de resolución no es válido");
        
        }   

    }
}
