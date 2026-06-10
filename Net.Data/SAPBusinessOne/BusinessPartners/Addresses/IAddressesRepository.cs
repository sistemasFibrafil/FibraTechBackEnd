using System.Threading.Tasks;
    using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.CrossCotting;
namespace Net.Data.SAPBusinessOne
{
    public interface IAddressesRepository
    {
        Task<ResultadoTransaccionResponse<AddressesEntity>> GetListByCode(AddressesEntity value);
        Task<ResultadoTransaccionResponse<AddressesQueryEntity>> GetByCode(AddressesEntity value);
        Task<ResultadoTransaccionResponse<AddressesEntity>> SetCreate(AddressesEntity value);
        Task<ResultadoTransaccionResponse<AddressesEntity>> SetUpdate(AddressesEntity value);
        Task<ResultadoTransaccionResponse<AddressesEntity>> SetDelete(string cardCode, string address);
    }
}
