using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IWarehousesRepository
    {
        Task<ResultadoTransaccionResponse<WarehousesQueryEntity>> GetListByInactive(WarehousesEntity value);
        Task<ResultadoTransaccionResponse<WarehousesQueryEntity>> GetListProduccion();
        Task<ResultadoTransaccionResponse<WarehousesQueryEntity>> GetListByItem(WarehousesByItemFilterEntity value);
        Task<ResultadoTransaccionResponse<WarehousesQueryEntity>> GetListByWhsCodeAndItemCode(WarehousesByItemFindEntity value);
    }
}
