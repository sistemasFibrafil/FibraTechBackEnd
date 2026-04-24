using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.Data.SAPBusinessOne
{
    public interface IDeliveryNotesRepository
    {
        Task<ResultadoTransaccionResponse<DeliveryNotesQueryEntity>> GetListByFilter(DeliveryNotesFilterEntity value);
        Task<ResultadoTransaccionResponse<DeliveryNotesQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetCreate(DeliveryNotesCreateEntity value);
        Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetUpdate(DeliveryNotesUpdateEntity value);
        Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetCancel(DeliveryNotesCancelEntity value);
        Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetClose(DeliveryNotesCloseEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintNationalDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportDocEntry(int docEntry);
    }
}
