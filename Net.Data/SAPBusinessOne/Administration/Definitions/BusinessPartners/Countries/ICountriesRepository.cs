using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.Countries;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.Countries
{
    public interface ICountriesRepository
    {
        Task<ResultadoTransaccionResponse<CountriesEntity>> GetList();
    }
}
