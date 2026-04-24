using Net.CrossCotting;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Update;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales
{
    public interface IOrdersService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(OrdersCreateRequestDto dto, IList<IFormFile> files);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(OrdersUpdateRequestDto dto, IList<IFormFile> files);
        Task<ResultadoTransaccionResponse<object>> SetClose(OrdersCloseRequestDto dto);
    }
}
