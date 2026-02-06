using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateDocumentoValidator : AbstractValidator<RequestCreateFileAsuntosPenales>
    {
        public RequestCreateDocumentoValidator() {
            RuleFor(c => c.FileAsuntosPenales)
                .NotNull().WithMessage("Documento es requerido.")
                .Must(c => FluentValidationGuard.ConvertBytesToMegaBytes(c.Length) <= 10).WithMessage("El tamaño del documento no puede ser mayor a 10MB.");


            RuleFor(c => c.idTipoDocumento)
                .GreaterThan(0).WithMessage("Tipo de documento no válido.");

            RuleFor(c => c.idAsuntosPenales)
                .GreaterThan(0).WithMessage("El identificador del asunto no es válido.");

            RuleFor(c => c.idSeccion)
                .GreaterThan(0).WithMessage("La sección no es válida.");

        }
    }
}