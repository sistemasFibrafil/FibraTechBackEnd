using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IBranchesRepository
    {
        Task<ResultadoTransaccionResponse<BranchesEntity>> GetList();
    }
}
