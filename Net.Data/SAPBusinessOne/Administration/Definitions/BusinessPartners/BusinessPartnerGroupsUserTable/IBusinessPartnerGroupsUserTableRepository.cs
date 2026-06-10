using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroupsUserTable;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroupsUserTable
{
    public interface IBusinessPartnerGroupsUserTableRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnerGroupsUserTableEntity>> GetByCode(string code);
    }
}
