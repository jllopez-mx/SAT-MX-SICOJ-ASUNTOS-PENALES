using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesRepository
    {
            Task<AsuntosPenales> GetByIdAsync(int id);
            Task<List<AsuntosPenales>> GetListByIdsAsync(int[] ids);
            Task<ResultTransaction> ReasignarAsync(
            List<AsuntosPenalesReasignar> listReasignacion
        );
        Task<AsuntosPenalesAbogado> GetAbogadoAsignadoAsync(int idAsuntoPenal);
        Task<List<AsuntosPenalesImputados>> GetImputados(int idAsuntosPenales); 

    }
}