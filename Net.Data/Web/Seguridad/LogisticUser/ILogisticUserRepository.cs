using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ILogisticUserRepository
    {
        Task<ResultadoTransaccionResponse<LogisticUserQueryEntity>> GetById(LogisticUserEntity value);
        Task<ResultadoTransaccionResponse<LogisticUserQueryEntity>> GetValidateByUser(LogisticUserValidatedFindEntity value);
        Task<ResultadoTransaccionResponse<LogisticUserEntity>> SetCreate(LogisticUserCreateEntity value);
    }
}
