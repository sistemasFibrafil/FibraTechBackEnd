using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Interfaces.SAPBusinessOne.Purchasing
{
    public interface IPurchaseRequestService
    {
        Task<ResultadoTransaccionResponse<object>> SetValidateLinesItemsExcel(List<PurchaseRequestLinesItemsValidateRequestDto> dto);
        Task<ResultadoTransaccionResponse<object>> SetValidateLinesServicesExcel(List<PurchaseRequestLinesServicesValidateRequestDto> dto);
        Task<ResultadoTransaccionResponse<object>> SetCreate(PurchaseRequestCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(PurchaseRequestUpdateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetClose(PurchaseRequestCloseRequestDto dto);
    }
}
