using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITakeInventorySparePartsRepository
    {
        Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> GetListByFilter(TakeInventorySparePartsFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetExcelByFilter(TakeInventorySparePartsFilterEntity value);
        Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> GetListCurrentDate(TakeInventorySparePartsFindEntity value);
        Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetCreate(TakeInventorySparePartsCreateEntity value);
        Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetUpdate(TakeInventorySparePartsUpdateEntity value);
        Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetDelete(TakeInventorySparePartsDeleteEntity value);
    }
}
