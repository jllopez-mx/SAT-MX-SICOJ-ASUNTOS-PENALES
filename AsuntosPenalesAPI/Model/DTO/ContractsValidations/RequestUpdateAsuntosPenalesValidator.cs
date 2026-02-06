using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{

     public class RequestUpdateAsuntosPenalesValidator : AbstractValidator<RequestUpdateAsuntosPenales>
     {
          public RequestUpdateAsuntosPenalesValidator()
        {

         RuleFor(c => c.oficio_solicitud)
                .NotNull()
                .NotEmpty()
                .WithMessage("Oficio de solicitud es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Oficio de solicitud requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Oficio de solicitud no válido");

            RuleFor(c => c.numero_expediente_cadido)
                .NotNull()
                .NotEmpty()
                .WithMessage("Número de expediente es obligatorio.")
                .MaximumLength(30)
                .WithMessage("Número de expediente requiere maximo 30 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Número de expediente no válido");
            
            RuleFor(c => c.id_unidad_realiza_solicitud)
                .GreaterThan(0)
                .When(c => c.interno)       
                .WithMessage("Unidad que realiza solicitud es requerida.");

            RuleFor(c => c.fecha_recepcion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha recepción es requerida.")
                .Must(BeValidateDateFormar)
                .WithMessage("Fecha recepción no tiene un formato válido.");
            
            RuleFor(c => c.fecha_vencimiento)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha vencimiento es requerida.")
                .Must(BeValidateDateFormar)
                .WithMessage("Fecha vencimiento no tiene un formato válido.");
            
            RuleFor(c => c.id_admin_controla)
                .NotNull()
                .WithMessage("La administracion que controla es requerido para Turnar.")
                .NotEmpty()
                .WithMessage("La administracion que controla es requerido para Turnar.");

            RuleFor(c => c.interno)
                .NotNull()
                .WithMessage("Interno es requerida.");
        }
        
        private bool BeValidateDateFormar(string dateString)
        {
            
            return DateTime.TryParse(dateString, out _);
        }
     }
}

