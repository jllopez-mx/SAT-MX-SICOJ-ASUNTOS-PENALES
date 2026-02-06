using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FluentValidation;

namespace AsuntosPenalesAPI.Model.DTO
{
    public class RequestDeleteArchivosAsuntosPenales
    {
        public int idAsuntoPenal {get; set;}
        public List<int> id { get; set; } = null!;

    }
    
}