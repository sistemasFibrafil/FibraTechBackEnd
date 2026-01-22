using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ICurrencyCodesRepository
    {
        Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetList();
        Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetListByCode(string currCode);
    }
}
