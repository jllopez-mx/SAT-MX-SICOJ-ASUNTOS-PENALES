using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateSolicitudTransparenciaValidator: AbstractValidator<RequestCreateSolicitudTransparencia>
    {
        public RequestCreateSolicitudTransparenciaValidator()
        {
           RuleFor(c => c.idAsunto)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("El valor indicado para el id debe ser un valor mayor a cero.");

             RuleFor(c => c.noSolicitud)
                .NotEmpty()
                .WithMessage("No ha indicado el parámetro de número de  solicitud.")
                .Matches(@"^[A-Za-z0-9/()-]*$")
                .WithMessage("El número de oficio contiene caracteres no permitidos. Solo se permiten letras, números, diagonal (/), guion medio (-) y paréntesis ().")
                .Must(x => !x.Contains(" "))
                .WithMessage("El número de  solicitud no debe contener espacios en blanco.");

            
            RuleFor(c => c.fechaSolicitud)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha de solicitud es requerida.")
                .Must(BeValidateDateFormar)
                .WithMessage("El formato de la fecha de solicitud no es válido");
           

        }
        private bool BeValidateDateFormar(string dateString)
        {
            return DateTime.TryParse(dateString, out _);
        }
    }
}