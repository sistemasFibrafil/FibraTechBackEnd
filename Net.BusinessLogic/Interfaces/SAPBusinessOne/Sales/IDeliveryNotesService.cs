using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales
{
    public interface IDeliveryNotesService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(DeliveryNotesCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(DeliveryNotesUpdateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetCancel(DeliveryNotesCancelRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetClose(DeliveryNotesCloseRequestDto dto);
    }
}
