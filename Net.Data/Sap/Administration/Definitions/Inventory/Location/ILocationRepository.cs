using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ILocationRepository
    {
        Task<ResultadoTransaccionEntity<LocationEntity>> GetList();
    }
}
