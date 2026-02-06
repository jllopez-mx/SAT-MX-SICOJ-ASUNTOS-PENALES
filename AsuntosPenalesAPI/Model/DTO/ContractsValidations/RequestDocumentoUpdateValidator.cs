using FluentValidation;
using Sicoj.Utils;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestDocumentoUpdateValidator : AbstractValidator<RequestDocumentoUpdate>
    {
        public RequestDocumentoUpdateValidator()
        {
            RuleFor(c => c.documento)
                    .NotNull().WithMessage("Documento es requerido.")
                    .Must(c => FluentValidationGuard.ConvertBytesToMegaBytes(c.Length) <= 10).WithMessage("El tamaño del documento no puede ser mayor a 10MB.");


            RuleFor(c => c.idTipoArchivo)
                .GreaterThan(0).WithMessage("Tipo asunto no es válido.");

            RuleFor(c => c.idAsuntoPenal)
                .GreaterThan(0).WithMessage("El identificador del asunto penal no es válido.");


        }
    }
}
