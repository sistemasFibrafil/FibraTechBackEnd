using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDeliveryNotesRepository
    {
        Task<ResultadoTransaccionEntity<DeliveryNotesQueryEntity>> GetListByFilter(DeliveryNotesFilterEntity value);
        Task<ResultadoTransaccionEntity<DeliveryNotesQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<DeliveryNotesEntity>> SetCreate(DeliveryNotesCreateEntity value);
        Task<ResultadoTransaccionEntity<DeliveryNotesEntity>> SetUpdate(DeliveryNotesUpdateEntity value);
        Task<ResultadoTransaccionEntity<DeliveryNotesEntity>> SetClose(DeliveryNotesCloseEntity value);
        Task<ResultadoTransaccionEntity<DeliveryNotesEntity>> SetCancel(DeliveryNotesCancelEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPrintNationalDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPrintExportDocEntry(int docEntry);
    }
}
