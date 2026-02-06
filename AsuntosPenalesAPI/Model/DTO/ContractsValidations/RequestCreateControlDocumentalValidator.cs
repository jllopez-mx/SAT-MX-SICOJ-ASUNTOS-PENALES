
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateControlDocumentalValidator : AbstractValidator<RequestCreateControlDocumental>
    {
        public RequestCreateControlDocumentalValidator(){
            RuleFor(c => c.oficio_solicitud)
                .NotNull()
                .NotEmpty()
                .WithMessage("Oficio de solicitud es obligatorio.")
                .MaximumLength(40)
                .WithMessage("Oficio de solicitud requiere maximo 40 caracteres")
                .Matches(@"^[A-Za-z0-9_\-\(\)\/\\ ]*$")
                .WithMessage("Oficio de solicitud no válido");

            
            RuleFor(c => c.id_unidad_realiza_solicitud)
                .GreaterThanOrEqualTo(0)
                .NotNull()
                .WithMessage("Unidad que realiza solicitud es requerida.");


            RuleFor(c => c.fecha_recepcion_solicitud)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha recepción es requerida.")
                .Must(BeValidateDateFormar)
                .WithMessage("Fecha recepción no tiene un formato válido.");
        }

        private bool BeValidateDateFormar(string dateString)
        {
            
            return DateTime.TryParse(dateString, out _);
        }
    }
}