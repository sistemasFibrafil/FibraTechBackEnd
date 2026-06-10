using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
namespace Net.Business.Logic.Interfaces.SAPBusinessOne.BusinessPartners
{
    public interface IUbigeoService
    {
        Task<ResultadoTransaccionResponse<object>> GetListByFilter(UbigeoFilterRequestDto dto);
    }
}
