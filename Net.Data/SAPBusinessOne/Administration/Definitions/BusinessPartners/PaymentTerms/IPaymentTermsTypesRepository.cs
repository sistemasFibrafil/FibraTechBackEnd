using System;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPaymentTermsTypesRepository
    {
        Task<ResultadoTransaccionEntity<PaymentTermsTypesEntity>> GetList();
        Task<ResultadoTransaccionEntity<PaymentTermsTypesEntity>> GetByCode(short groupNum);
    }
}
