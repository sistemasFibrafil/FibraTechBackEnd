using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IUsersRepository
    {
        Task<ResultadoTransaccionEntity<UsersQueryEntity>> GetList();
        Task<ResultadoTransaccionEntity<UsersQueryEntity>> GetListByFilter(UsersFilterEntity value);
        Task<ResultadoTransaccionEntity<UsersQueryEntity>> GetByCode(UsersFindEntity value);
    }
}
