using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using System.Threading.Tasks;

namespace Net.Data.SAPBusinessOne
{
    public interface IStatesRepository
    {
        Task<ResultadoTransaccionEntity<StatesEntity>> GetList(string countryCode);
    }
}
