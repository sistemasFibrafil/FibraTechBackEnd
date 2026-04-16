using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITakeInventorySparePartsRepository
    {
        Task<ResultadoTransaccionEntity<TakeInventorySparePartsEntity>> GetListByFilter(TakeInventorySparePartsFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetExcelByFilter(TakeInventorySparePartsFilterEntity value);
        Task<ResultadoTransaccionEntity<TakeInventorySparePartsEntity>> GetListCurrentDate(TakeInventorySparePartsFindEntity value);
        Task<ResultadoTransaccionEntity<TakeInventorySparePartsEntity>> SetCreate(TakeInventorySparePartsCreateEntity value);
        Task<ResultadoTransaccionEntity<TakeInventorySparePartsEntity>> SetUpdate(TakeInventorySparePartsUpdateEntity value);
        Task<ResultadoTransaccionEntity<TakeInventorySparePartsEntity>> SetDelete(TakeInventorySparePartsDeleteEntity value);
    }
}
