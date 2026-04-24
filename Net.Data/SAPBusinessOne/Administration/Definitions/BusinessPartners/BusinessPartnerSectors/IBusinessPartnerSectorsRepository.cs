using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IBusinessPartnerSectorsRepository
    {
        Task<ResultadoTransaccionResponse<BusinessPartnerSectorsEntity>> GetList();
    }
}
