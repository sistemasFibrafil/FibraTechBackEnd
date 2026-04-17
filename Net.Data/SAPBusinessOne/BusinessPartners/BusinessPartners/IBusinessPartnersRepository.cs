using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnersRepository
    {
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetListByFilter(BusinessPartnersFilterEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetListModalByFilter(BusinessPartnersModalFilterEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetByCode(string cardCode);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetVehicleByCode(string cardCode);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetDriverByCode(string cardCode);
        Task<ResultadoTransaccionEntity<BusinessPartnersViewEntity>> GetListClienteBySectorStatus(BusinessPartnersSectorStatusFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteBySectorStatusExcel(BusinessPartnersSectorStatusFilterEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetCreate(BusinessPartnersCreateEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetUpdate(BusinessPartnersUpdateEntity value);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetDelete(string cardCode);
        Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetByRUC(string ruc);
    }
}
