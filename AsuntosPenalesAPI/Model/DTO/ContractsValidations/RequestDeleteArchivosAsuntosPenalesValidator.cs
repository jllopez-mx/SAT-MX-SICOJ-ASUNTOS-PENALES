using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDeleteArchivosAsuntosPenalesValidator : AbstractValidator<RequestDeleteArchivosAsuntosPenales>
    {
        public RequestDeleteArchivosAsuntosPenalesValidator()
        {
             RuleFor(c => c.id)  
                .NotEmpty()
                .WithMessage("Alguno de los datos no tiene el formato correcto");
        }
    }
}