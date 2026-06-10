using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.CrossCotting;

namespace Net.Data.SAPBusinessOne
{
    public interface IPriceListRepository
    {
        Task<ResultadoTransaccionResponse<PriceListEntity>> GetList();
    }
}
