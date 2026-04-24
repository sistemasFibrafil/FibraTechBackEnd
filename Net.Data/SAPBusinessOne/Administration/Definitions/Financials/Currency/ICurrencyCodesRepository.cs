using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ICurrencyCodesRepository
    {
        Task<ResultadoTransaccionResponse<CurrencyCodesEntity>> GetList();
        Task<ResultadoTransaccionResponse<CurrencyCodesEntity>> GetListByCode(string currCode);
    }
}
