using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerDivisions.Entities;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnerDivisionsRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnerDivisionsEntity>> GetList();
    }
}
