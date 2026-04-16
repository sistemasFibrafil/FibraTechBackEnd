using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IExchangeRatesRepository
    {
        Task<ResultadoTransaccionEntity<ExchangeRatesQueryEntity>> GetByDocDateAndCurrency(ExchangeRatesFindEntity value);
    }
}
