using System.IO;
using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
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
