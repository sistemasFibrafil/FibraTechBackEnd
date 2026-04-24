using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicle.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicles.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicle.Entities;
namespace Net.Data.SAPBusinessOne
{
    public interface IVehiclesRepository
    {
        Task<ResultadoTransaccionResponse<VehiclesEntity>> GetListByFilter(VehiclesFilterEntity value);
        Task<ResultadoTransaccionResponse<VehiclesEntity>> SetCreate(VehiclesCreateEntity value);
    }
}
