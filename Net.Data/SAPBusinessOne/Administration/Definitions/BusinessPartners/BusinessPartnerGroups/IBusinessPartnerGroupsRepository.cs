using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroups;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnerGroupsRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnerGroupsEntity>> GetListByGroupType(BusinessPartnerGroupsEntity value);
    }
}
