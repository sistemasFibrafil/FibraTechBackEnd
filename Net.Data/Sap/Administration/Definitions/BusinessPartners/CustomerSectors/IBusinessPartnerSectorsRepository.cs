using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IBusinessPartnerSectorsRepository
    {
        Task<ResultadoTransaccionEntity<BusinessPartnerSectorsEntity>> GetList();
    }
}
