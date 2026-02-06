using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateImputadosAmparoAsuntosPenalesValidator: AbstractValidator<RequestUpdateAmparo>
    {
        public RequestUpdateImputadosAmparoAsuntosPenalesValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El nombre del imputado es requerido.")
            .NotEmpty()
            .WithMessage("El nombre del imputado es requerido.");          
        }   

    }
}
