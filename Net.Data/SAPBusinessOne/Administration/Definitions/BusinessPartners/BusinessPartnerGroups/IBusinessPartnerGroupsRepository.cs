using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnerGroupsRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnerGroupsEntity>> GetList(BusinessPartnerGroupsEntity value);
    }
}
