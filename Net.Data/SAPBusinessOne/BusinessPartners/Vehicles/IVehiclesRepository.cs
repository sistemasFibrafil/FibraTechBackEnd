using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IVehiclesRepository
    {
        Task<ResultadoTransaccionEntity<VehiclesEntity>> GetListByFilter(VehiclesFilterEntity value);
        Task<ResultadoTransaccionEntity<VehiclesEntity>> SetCreate(VehiclesCreateEntity value);
    }
}
