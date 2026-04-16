using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IWarehousesRepository
    {
        Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByInactive(WarehousesEntity value);
        Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListProduccion();
        Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByItem(WarehousesByItemFilterEntity value);
        Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByWhsCodeAndItemCode(WarehousesByItemFindEntity value);
    }
}
