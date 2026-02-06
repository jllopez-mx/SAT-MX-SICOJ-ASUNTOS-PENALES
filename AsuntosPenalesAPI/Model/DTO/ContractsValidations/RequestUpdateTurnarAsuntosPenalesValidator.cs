using FluentValidation;


namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateTurnarAsuntosPenalesValidator : AbstractValidator<RequestUpdateTurnarAsuntosPenales>
    {
        public RequestUpdateTurnarAsuntosPenalesValidator()
        {
        RuleFor(c => c.id)
                .NotNull()
                .WithMessage("El id a turnar es requerido.")
                .NotEmpty()
                .WithMessage("El id a turnar es requerido.");
        
        RuleFor(c => c.numero_empleado)
                .NotNull()
                .WithMessage("El Numero del empleado es requerido para Turnar.")
                .NotEmpty()
                .WithMessage("El Numero del empleado es requerido para Turnar.")
                .Length(12, 13)
                .WithMessage("RFC requiere entre 12 y 13 caracteres.");

         RuleFor(c => c.id_admin_controla)
                .NotNull()
                .WithMessage("La administracion que controla es requerido para Turnar.")
                .NotEmpty()
                .WithMessage("La administracion que controla es requerido para Turnar.");
        

        }
      
    }


}