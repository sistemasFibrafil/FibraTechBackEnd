using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ICurrencyCodesRepository
    {
        Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetList();
        Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetListByCode(string currCode);
    }
}
