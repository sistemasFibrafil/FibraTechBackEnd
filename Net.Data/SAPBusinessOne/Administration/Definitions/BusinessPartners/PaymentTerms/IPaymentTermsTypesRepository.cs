using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPaymentTermsTypesRepository
    {
        Task<ResultadoTransaccionResponse<PaymentTermsTypesEntity>> GetList();
        Task<ResultadoTransaccionResponse<PaymentTermsTypesEntity>> GetByCode(short groupNum);
    }
}
