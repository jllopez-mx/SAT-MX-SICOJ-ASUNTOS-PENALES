using AsuntosPenalesAPI.Model.Entities;
using Sicoj.Utils.Models;

namespace AsuntosPenalesAPI.Model.IDAO.IRepository
{
    public interface IAsuntosPenalesDescartarRepository
    {
        Task<ResultTransaction> DescartarAsync(AsuntosPenales entityAsuntosPenales, AsuntosPenalesDescartar entity, List<int> listaSecciones);
        Task<ResultTransaction> DescartarRequerimientoAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultTransaction> DescartarRequisitosAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultTransaction> DescartarEtapaInicialAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultTransaction> DescartarEtapaComplementariaAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones);
        Task<ResultTransaction> DescartarEtapaIntermediaAsync(AsuntosPenales entityAsuntosPenales,  AsuntosPenalesDescartar entity, List<int>? listaSecciones);

        
    
    }
}