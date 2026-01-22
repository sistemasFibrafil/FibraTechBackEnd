using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ILogisticUserRepository
    {
        Task<ResultadoTransaccionEntity<LogisticUserQueryEntity>> GetById(LogisticUserEntity value);
        Task<ResultadoTransaccionEntity<LogisticUserQueryEntity>> GetValidateByUser(LogisticUserValidatedFindEntity value);
        Task<ResultadoTransaccionEntity<LogisticUserEntity>> SetCreate(LogisticUserCreateEntity value);
    }
}
