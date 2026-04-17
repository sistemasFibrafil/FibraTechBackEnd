using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using System.Threading.Tasks;

namespace Net.Data.SAPBusinessOne
{
    public interface ICountriesRepository
    {
        Task<ResultadoTransaccionEntity<CountryEntity>> GetList();
    }
}
