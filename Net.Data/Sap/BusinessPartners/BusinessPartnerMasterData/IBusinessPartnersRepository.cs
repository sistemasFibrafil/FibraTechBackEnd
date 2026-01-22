using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IBusinessPartnersRepository
    {
        Task<ResultadoTransaccionEntity<BusinessPartnersEntity>> GetListByFilter(BusinessPartnersFilterEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetByCode(string cardCode);
        Task<ResultadoTransaccionEntity<BusinessPartnersViewEntity>> GetListClienteBySectorStatus(BusinessPartnersSectorStatusFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteBySectorStatusExcel(BusinessPartnersSectorStatusFilterEntity value);
    }
}
