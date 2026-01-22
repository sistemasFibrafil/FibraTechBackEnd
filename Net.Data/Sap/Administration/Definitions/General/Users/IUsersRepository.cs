using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IUsersRepository
    {
        Task<ResultadoTransaccionEntity<UsersEntity>> GetList();
        Task<ResultadoTransaccionEntity<UsersEntity>> GetByCode(UsersEntity value);
    }
}
