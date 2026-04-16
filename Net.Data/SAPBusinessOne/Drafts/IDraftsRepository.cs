using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDraftsRepository
    {
        Task<ResultadoTransaccionEntity<DraftsQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<DraftsStatusQueryEntity>> GetStatusByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<DraftsEntity>> SetCreate(DraftsCreateEntity value);
        Task<ResultadoTransaccionEntity<DraftsEntity>> SetResend(DraftsResendEntity value);
    }
}
