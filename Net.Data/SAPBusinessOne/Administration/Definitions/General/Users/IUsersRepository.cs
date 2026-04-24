using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Query;
namespace Net.Data.SAPBusinessOne
{
    public interface IUsersRepository
    {
        Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetList();
        Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetListByFilter(UsersFilterEntity value);
        Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetByCode(UsersFindEntity value);
    }
}
