using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDeleteAsuntosPenalesValidator : AbstractValidator<RequestDeleteAsuntosPenales>
    {
        public RequestDeleteAsuntosPenalesValidator()
        {
            RuleFor(c => c.id)
            .NotNull()
            .WithMessage("El id del asunto penal a eliminar es requerido.")
            .NotEmpty()
            .WithMessage("El id del asunto penal a eliminar es requerido.");
        }
    }
}