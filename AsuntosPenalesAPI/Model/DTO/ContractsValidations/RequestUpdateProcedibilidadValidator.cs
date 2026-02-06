using FluentValidation;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateProcedibilidadValidator : AbstractValidator<RequestUpdateProcedibilidad>
    {         
        public RequestUpdateProcedibilidadValidator()
        {
                RuleFor(c => c.id)
                .NotNull()
                .WithMessage("El id a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id a editar es requerido.");
        }
    }
}
