    using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IAddressesRepository
    {
        Task<ResultadoTransaccionEntity<AddressesEntity>> GetListByCode(AddressesEntity value);
        Task<ResultadoTransaccionEntity<AddressesQueryEntity>> GetByCode(AddressesEntity value);
        Task<ResultadoTransaccionEntity<AddressesEntity>> SetCreate(AddressesEntity value);
        Task<ResultadoTransaccionEntity<AddressesEntity>> SetUpdate(AddressesEntity value);
        Task<ResultadoTransaccionEntity<AddressesEntity>> SetDelete(string cardCode, string address);
    }
}
