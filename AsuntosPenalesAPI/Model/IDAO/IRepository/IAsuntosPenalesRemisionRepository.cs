using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DTO;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesRemisionRepository
    {
        Task<List<ResponseRemision>> GetRemisionesDisconnected(int idAsuntosPenales);
    }
}