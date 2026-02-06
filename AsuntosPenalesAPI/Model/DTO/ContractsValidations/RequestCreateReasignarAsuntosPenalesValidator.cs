using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateReasignarAsuntosPenalesValidator :AbstractValidator<RequestReasignarAsuntosPenales>
    {
        public RequestCreateReasignarAsuntosPenalesValidator()
        {
            
        }
    }
}