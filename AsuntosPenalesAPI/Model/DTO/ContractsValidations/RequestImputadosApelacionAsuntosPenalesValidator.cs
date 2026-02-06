using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestImputadosApelacionAsuntosPenalesValidator: AbstractValidator<RequestApelacion>
    {
        public RequestImputadosApelacionAsuntosPenalesValidator()
        {

            RuleFor(c => c.idImputado)
            .NotNull()
            .WithMessage("El imputado es requerido.")
            .NotEmpty()
            .WithMessage("El imputado es requerido.");  

            RuleFor(c => c.fechaPresentacion)
                .NotNull()
                .NotEmpty()
                .WithMessage("La fecha de presentación es requerida")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .WithMessage("El formato de la fecha de presentación no es válido");       

            RuleFor(c => c.numeroTocaPenal)
                .NotNull()
                .NotEmpty()
                .WithMessage("Número de toca penal es obligatorio.")
                .MaximumLength(10)
                .WithMessage("Número de toca penal requiere maximo 10 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Número de toca penal no válido");

            RuleFor(c => c.TipoOrganoJurisdiccional)
                .NotEmpty()
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
            .WithMessage("Tipo de organo jurisdiccional no válido");
            
            RuleFor(c => c.idTipoResolucion)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(
                    "El valor indicado para Resolución debe ser un valor mayor a cero."
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
                .WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000)
                .WithMessage("Se excedio del maximo de caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Descripción de resolución no valida");        
        }   

    }
}
