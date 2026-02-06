using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDeleteAmparoValidator : AbstractValidator<RequestDeleteAmparo>
    {
        public RequestDeleteAmparoValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El id de la apelacion a eliminar es requerido.")
            .NotEmpty()
            .WithMessage("El id de la apelacion a eliminar es requerido.");
        }
    }
}