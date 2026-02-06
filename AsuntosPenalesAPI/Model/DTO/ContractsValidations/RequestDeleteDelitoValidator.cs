using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDeleteDelitoValidator : AbstractValidator<RequestDeleteDelito>
    {
        public RequestDeleteDelitoValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El id del delito a eliminar es requerido.")
            .NotEmpty()
            .WithMessage("El id del delito a eliminar es requerido.");
        }
    }
}