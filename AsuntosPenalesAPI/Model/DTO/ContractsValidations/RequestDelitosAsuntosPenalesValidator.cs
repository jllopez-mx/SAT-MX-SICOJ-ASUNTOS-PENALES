using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDelitosAsuntosPenalesValidator : AbstractValidator<RequestDelitos>
    {
        public RequestDelitosAsuntosPenalesValidator()
        {   
        RuleFor(c => c.id_delito)
        .NotEmpty()
        .Must(c=>c.All(a => a >0))
        .WithMessage("En la lista de delitos solo debe contener valores mayores a cero.");
    }
    
    }
}