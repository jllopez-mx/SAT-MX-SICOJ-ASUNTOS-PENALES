using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateAsignarAsuntosPenalesValidator : AbstractValidator<RequestAsignarAsuntosPenales>
    {
        public RequestUpdateAsignarAsuntosPenalesValidator(){
            
            RuleFor(c => c.rfc_abogado)
                .NotNull()
                .WithMessage("RFC es obligatorio.")
                .NotEmpty()
                .WithMessage("RFC es obligatorio.")
                .Length(12, 13)
                .WithMessage("RFC requiere entre 12 y 13 caracteres.");

        }
    }
}