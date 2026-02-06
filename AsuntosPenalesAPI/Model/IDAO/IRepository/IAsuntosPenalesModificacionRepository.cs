using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesModificacionRepository
    {   
        Task<ResultTransaction> AddAsync(AsuntosPenales entityAsuntoPenal, AsuntosPenalesModificacion entity);
        Task<AsuntosPenalesModificacion> GetByIdAsuntoPenalAsync(int idAsuntoPenal);
    }
}