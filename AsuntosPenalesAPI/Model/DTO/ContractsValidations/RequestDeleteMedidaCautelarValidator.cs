using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDeleteMedidaCautelarValidator : AbstractValidator<RequestDeleteMedidaCautelar>
    {
        public RequestDeleteMedidaCautelarValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El id de la medida cautelar a eliminar es requerido.")
            .NotEmpty()
            .WithMessage("El id de la medida cautelar a eliminar es requerido.");
        }
    }
}