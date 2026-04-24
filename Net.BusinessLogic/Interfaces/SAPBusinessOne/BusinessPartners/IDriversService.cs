using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.BusinessPartners
{
    public interface IDriversService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(DriversCreateRequestDto dto);
    }
}
