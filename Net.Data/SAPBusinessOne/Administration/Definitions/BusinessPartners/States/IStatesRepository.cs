using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.States;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.States
{
    public interface IStatesRepository
    {
        Task<ResultadoTransaccionResponse<StatesEntity>> GetListByCountryCode(string countryCode);
    }
}
