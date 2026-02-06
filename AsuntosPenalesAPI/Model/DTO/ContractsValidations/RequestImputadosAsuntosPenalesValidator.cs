using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestImputadosAsuntosPenalesValidator : AbstractValidator<RequestImputados>
    {
        public RequestImputadosAsuntosPenalesValidator()
        {
            RuleFor(c => c.nombre)
            .NotNull()
            .WithMessage("El nombre del contribuyente es requerido.")
            .NotEmpty()
            .WithMessage("El nombre del contribuyente es requerido.");          
        }   

    }
}


