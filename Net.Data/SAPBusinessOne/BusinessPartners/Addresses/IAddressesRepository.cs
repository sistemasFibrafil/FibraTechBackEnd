using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IAddressesRepository
    {
        Task<ResultadoTransaccionResponse<AddressesEntity>> GetListByCode(AddressesEntity value);
        Task<ResultadoTransaccionResponse<AddressesQueryEntity>> GetByCode(AddressesEntity value);
    }
}
