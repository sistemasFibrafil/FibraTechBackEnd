using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnersRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetListByFilter(BusinessPartnersFilterEntity value);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetListModalByFilter(BusinessPartnersModalFilterEntity value);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetByCode(string cardCode);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetVehicleByCode(string cardCode);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetDriverByCode(string cardCode);
        Task<ResultadoTransaccionResponse<BusinessPartnersViewEntity>> GetListClienteBySectorStatus(BusinessPartnersSectorStatusFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetClienteBySectorStatusExcel(BusinessPartnersSectorStatusFilterEntity value);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetCreate(BusinessPartnersCreateEntity value);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetUpdate(BusinessPartnersUpdateEntity value);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetDelete(string cardCode);
        Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetByRUC(string ruc);
    }
}
