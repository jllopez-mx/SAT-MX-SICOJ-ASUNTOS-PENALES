using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;
using Sicoj.Utils;
using AsuntosPenalesAPI.Model.ViewModels.Enums;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateFechaPlazoEtapaComplementariaInvestigacionValidator : AbstractValidator<RequestUpdateFechaPlazoEtapaInvestigacionComplementaria>
    {
        public RequestUpdateFechaPlazoEtapaComplementariaInvestigacionValidator()
        {
            RuleFor(c => c.id)
                .NotNull()
                .WithMessage("El id de imputado a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id imputado a editar es requerido.");

            RuleFor(c => c.idAsuntoPenal)
                .NotNull()
                .WithMessage("El id del asunto penal a editar es requerido.")
                .NotEmpty()
                .WithMessage("El id del asunto penal a editar es requerido.");

        }
    }
}