using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Purchasing
{
    public interface IPurchaseRequestService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(PurchaseRequestCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(PurchaseRequestUpdateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetClose(PurchaseRequestCloseRequestDto dto);
    }
}
