using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IBusinessPartnerGroupsRepository
    {
        Task<ResultadoTransaccionEntity<BusinessPartnerGroupsEntity>> GetList(BusinessPartnerGroupsEntity value);
    }
}
