using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.BusinessPartners
{
    public interface IVehiclesService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(VehiclesCreateRequestDto dto);
    }
}
